using Carrotware.CMS.Data.Models;
using Carrotware.CMS.Interface;

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

	public class SiteBasicInfo : ICarrotSite {
		public SiteBasicInfo() {
			//var dbg = CarrotHttpHelper.Configuration.GetDebugView;

			this.SiteID = SiteData.CurrentSiteID;

			if (this.SiteID == Guid.Empty) {
				var siteid = CarrotHttpHelper.Configuration.GetValue<string>("TestSiteID") != null
					   ? CarrotHttpHelper.Configuration.GetValue<string>("TestSiteID").ToString()
					   : Guid.Empty.ToString();

				this.SiteID = new Guid(siteid);
			}

			string key = $"key_BasicSite_{this.SiteID}";
			bool bCached = false;
			var site = new CarrotSite();

			try {
				var val = CarrotHttpHelper.CacheGet(key);
				site = (CarrotSite)val;
				bCached = site != null && site.SiteId != Guid.Empty;
			} catch {
				bCached = false;
			}

			if (!bCached) {
				using (var db = new CarrotCakeContext()) {
					site = db.CarrotSites.Where(x => x.SiteId == this.SiteID).FirstOrDefault();
				}

				if (site != null) {
					this.SiteName = site.SiteName;
					this.SiteTagline = site.SiteTagline;
					this.MainURL = site.MainUrl;
					this.TimeZoneIdentifier = site.TimeZone;
					this.Blog_Root_ContentID = site.BlogRootContentId;

					CarrotHttpHelper.CacheInsert(key, site, 3);
				}

			} else {
				this.SiteName = site.SiteName;
				this.SiteTagline = site.SiteTagline;
				this.MainURL = site.MainUrl;
				this.TimeZoneIdentifier = site.TimeZone;
				this.Blog_Root_ContentID = site.BlogRootContentId;
			}
		}

		public Guid SiteID { get; set; } = Guid.Empty;
		public string? SiteName { get; set; }
		public string? SiteTagline { get; set; }
		public string? TimeZoneIdentifier { get; set; }
		public string? MainURL { get; set; }
		public Guid? Blog_Root_ContentID { get; set; }
	}
}