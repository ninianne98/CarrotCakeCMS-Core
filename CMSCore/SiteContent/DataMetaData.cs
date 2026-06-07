using Microsoft.AspNetCore.Html;

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
	//======================================

	public class ContentDateTally : IMetaDataLinks {

		public ContentDateTally() { }

		public ContentDateTally(SiteData site, DateTime tallyDate) {
			this.TheSite = site;
			this.TallyDate = tallyDate;
		}

		public SiteData TheSite { get; set; } = new SiteData();
		public DateTime TallyDate { get; set; }
		public string DateCaption { get; set; } = string.Empty;
		public string DateSlug { get; set; } = string.Empty;

		public string DateURL {
			get { return (this.TheSite.BuildMonthSearchLink(this.TallyDate)); }
		}

		public int? UseCount { get; set; }

		public IHtmlContent Text { get { return new HtmlString(this.DateCaption); } }
		public string Uri { get { return this.DateURL; } }
		public int Count { get { return this.UseCount ?? 0; } }
	}

	//======================================

	public class ContentDateLinks : IMetaDataLinks {

		public ContentDateLinks() {
			this.PostDate = DateTime.MinValue;
		}

		public SiteData TheSite { get; set; } = new SiteData();

		public DateTime PostDate { get; set; }

		public string DateCaption {
			get { return this.PostDate.ToString("MMMM d, yyyy"); }
		}

		public string DateURL {
			get { return (this.TheSite.BuildDateSearchLink(this.PostDate)); }
		}

		public int UseCount { get; set; }

		public IHtmlContent Text { get { return new HtmlString(this.DateCaption); } }
		public string Uri { get { return this.Uri; } }
		public int Count { get { return this.UseCount; } }
	}
}