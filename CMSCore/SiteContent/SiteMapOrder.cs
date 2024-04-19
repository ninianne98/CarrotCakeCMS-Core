using Carrotware.CMS.Data.Models;

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

	public class SiteMapOrderSub {

		public SiteMapOrderSub() {
			this.AllSiteMaps = new List<SiteMapOrder>();
		}

		public SiteMapOrderSub(Guid? pageid, List<SiteMapOrder> items) {
			this.Current_Root_ContentID = pageid;
			this.AllSiteMaps = items;
		}

		public List<SiteMapOrder> ChildItems {
			get {
				return this.AllSiteMaps.Where(x => x.Parent_ContentID == this.Current_Root_ContentID).OrderBy(x => x.NavOrder).ToList();
			}
		}

		public Guid? Current_Root_ContentID { get; set; }

		public List<SiteMapOrder> AllSiteMaps { get; set; }
	}

	//======================
	public class SiteMapOrder {

		public SiteMapOrder() { }

		public int NavOrder { get; set; }
		public Guid? Parent_ContentID { get; set; }
		public Guid Root_ContentID { get; set; }
		public Guid SiteID { get; set; }
		public string NavMenuText { get; set; }
		public string FileName { get; set; }
		public bool PageActive { get; set; }
		public bool ShowInSiteNav { get; set; }
		public int NavLevel { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime GoLiveDate { get; set; }
		public DateTime RetireDate { get; set; }
		public ContentPageType.PageType ContentType { get; set; }

		internal SiteMapOrder(vwCarrotContent c) {
			if (c != null) {
				SiteData site = SiteData.GetSiteFromCache(c.SiteId);

				this.SiteID = c.SiteId;
				this.Root_ContentID = c.RootContentId;
				this.PageActive = c.PageActive;
				this.ShowInSiteNav = c.ShowInSiteNav;
				this.Parent_ContentID = c.ParentContentId;
				this.NavMenuText = c.NavMenuText;
				this.FileName = c.FileName;
				this.NavOrder = c.NavOrder;

				if (this.Parent_ContentID.HasValue) {
					this.NavLevel = 0;
				} else {
					this.NavLevel = 10;
				}

				this.ContentType = ContentPageType.GetTypeByID(c.ContentTypeId);
				this.CreateDate = site.ConvertUTCToSiteTime(c.CreateDate);
				this.GoLiveDate = site.ConvertUTCToSiteTime(c.GoLiveDate);
				this.RetireDate = site.ConvertUTCToSiteTime(c.RetireDate);
			}
		}

		public override bool Equals(object? obj) {
			//Check for null and compare run-time types.
			if (obj == null || this.GetType() != obj.GetType()) return false;
			if (obj is SiteMapOrder) {
				SiteMapOrder p = (SiteMapOrder)obj;
				return (this.Root_ContentID == p.Root_ContentID);
			} else {
				return false;
			}
		}

		public override int GetHashCode() {
			return Root_ContentID.GetHashCode();
		}
	}
}