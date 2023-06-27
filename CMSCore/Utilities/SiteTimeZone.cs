using Carrotware.CMS.Data.Models;
using System.Data;
using System.Xml.Serialization;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Core {

	public class ContentLocalTime {
		public DateTime GoLiveDate { get; set; }

		public DateTime GoLiveDateLocal { get; set; }
	}

	//===============================
	public class BlogPostPageUrl {
		public string PostPrefix { get; set; }

		public DateTime GoLiveDate { get; set; }

		public DateTime GoLiveDateLocal { get; set; }
	}

	//===============================
	public class TimeZoneContent {
		public List<ContentLocalTime> ContentLocalDates { get; set; }

		public List<BlogPostPageUrl> BlogPostUrls { get; set; }

		public Guid SiteID { get; set; }

		public TimeZoneContent() {
			this.ContentLocalDates = new List<ContentLocalTime>();
			this.BlogPostUrls = new List<BlogPostPageUrl>();
		}

		public TimeZoneContent(Guid siteID) {
			// use C# libraries for timezones rather than pass in offset as some dates are +/- an hour off because of DST

			this.SiteID = siteID;
			SiteData site = SiteData.GetSiteFromCache(siteID);

			this.ContentLocalDates = new List<ContentLocalTime>();
			this.BlogPostUrls = new List<BlogPostPageUrl>();

			var allContentDates = new List<DateTime>();
			var blogDateList = new List<DateTime>();

			using (var db = CarrotCakeContext.Create()) {
				allContentDates = CannedQueries.GetAllDates(db, siteID).Distinct().ToList();
				blogDateList = CannedQueries.GetAllDatesByType(db, siteID, ContentPageType.PageType.BlogEntry).Distinct().ToList();
			}

			this.ContentLocalDates = (from d in allContentDates
									  select new ContentLocalTime() {
										  GoLiveDate = d,
										  GoLiveDateLocal = site.ConvertUTCToSiteTime(d)
									  }).ToList();

			this.BlogPostUrls = (from bd in blogDateList
								 join ld in this.ContentLocalDates on bd equals ld.GoLiveDate
								 select new BlogPostPageUrl() {
									 GoLiveDate = ld.GoLiveDate,
									 PostPrefix = ContentPageHelper.CreateFileNameFromSlug(siteID, ld.GoLiveDateLocal, string.Empty),
									 GoLiveDateLocal = ld.GoLiveDateLocal
								 }).ToList();
		}

		public void Save() {
			using (var db = CarrotCakeContext.Create()) {
				string xml = this.GetXml();

				db.SprocCarrotUpdateGoLiveLocal(this.SiteID, xml);
			}
		}

		public string GetXml() {
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TimeZoneContent));
			string sXML = string.Empty;
			using (StringWriter stringWriter = new StringWriter()) {
				xmlSerializer.Serialize(stringWriter, this);
				sXML = stringWriter.ToString();
			}
			return sXML;
		}
	}
}