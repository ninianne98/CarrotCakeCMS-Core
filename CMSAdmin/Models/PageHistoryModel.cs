using Carrotware.CMS.Core;
using Carrotware.Web.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.CoreMVC.UI.Admin.Models {

	public class PageHistoryModel {

		public PageHistoryModel() {
			this.History = new PagedData<ContentPage>();
			this.History.PageSize = 10;
			this.History.InitOrderBy(x => x.EditDate, false);
		}

		public PageHistoryModel(Guid siteID)
			: this() {
			this.SiteID = siteID;
		}

		public void SetCurrent(Guid id) {
			this.Root_ContentID = id;

			using (ContentPageHelper pageHelper = new ContentPageHelper()) {
				this.ContentPage = pageHelper.FindContentByID(this.SiteID, id);

				this.History.DataSource = pageHelper.GetVersionHistory(this.SiteID, id);
				this.History.TotalRecords = this.History.DataSource.Count();
				this.History.PageSize = this.History.TotalRecords * 2;
			}
		}

		public void SetVersion(Guid id) {
			this.VersionID = id;

			using (ContentPageHelper pageHelper = new ContentPageHelper()) {
				this.ContentPage = pageHelper.GetVersion(this.SiteID, id);
				this.Root_ContentID = this.ContentPage.Root_ContentID;
			}
		}

		public Guid SiteID { get; set; }
		public Guid Root_ContentID { get; set; }

		public Guid? VersionID { get; set; }

		public ContentPage ContentPage { get; set; }

		public PagedData<ContentPage> History { get; set; }
	}
}