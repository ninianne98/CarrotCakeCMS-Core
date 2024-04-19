/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Interface {

	public class SiteTestInfo : ICarrotSite {

		public SiteTestInfo() {
			//var dbg = CarrotHttpHelper.Configuration.GetDebugView;

			var siteid = CarrotHttpHelper.Configuration.GetValue<string>("TestSiteID") != null
					   ? CarrotHttpHelper.Configuration.GetValue<string>("TestSiteID").ToString()
					   : Guid.Empty.ToString();

			this.SiteID = new Guid(siteid);

			if (this.SiteID == Guid.Empty) {
				var cfgId = CarrotHttpHelper.Configuration.GetValue<string>("CarrotCakeCMS:Config:SiteID");
				siteid = cfgId != null ? cfgId : Guid.Empty.ToString();

				this.SiteID = new Guid(siteid);
			}

			this.SiteName = "This is a test site";
			this.SiteTagline = "This is a tagline string so there is something to see";
			this.TimeZoneIdentifier = TimeZoneInfo.Local.ToString();
			this.MainURL = "http://" + CarrotHttpHelper.HttpContext.Request.Host + "/";
		}

		public Guid SiteID { get; set; }
		public string? SiteName { get; set; }
		public string? SiteTagline { get; set; }
		public string? TimeZoneIdentifier { get; set; }
		public string? MainURL { get; set; }
		public Guid? Blog_Root_ContentID { get; set; }
	}
}