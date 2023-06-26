using Carrotware.CMS.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

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

	public class UserModel {

		public UserModel() { }

		public UserModel(Guid UserId) {
			this.User = new ExtendedUserData(UserId);
		}

		public UserModel(ExtendedUserData user) {
			this.User = user;
		}

		public ExtendedUserData User { get; set; }

		public bool Selected { get; set; }

		private List<SelectListItem> _sites = null;

		public List<SelectListItem> SiteOptions {
			get {
				if (_sites == null) {
					_sites = (from l in this.AllSites
							  select new SelectListItem {
								  Text = l.SiteName,
								  Selected = this.User.GetSiteList().Where(x => x.SiteID == l.SiteID).Any(),
								  Value = l.SiteID.ToString().ToLowerInvariant()
							  }).ToList();
				}

				return _sites;
			}
		}

		private List<SelectListItem> _roles = null;

		public List<SelectListItem> RoleOptions {
			get {
				if (_roles == null) {
					_roles = (from l in SecurityData.GetRoleList()
							  select new SelectListItem {
								  Text = l.RoleName,
								  Selected = this.User.GetRoles().Where(x => x.RoleId == l.RoleId).Any(),
								  Value = l.RoleId.ToLowerInvariant()
							  }).ToList();
				}

				return _roles;
			}
		}

		private List<SiteData> _allSites = null;

		public List<SiteData> AllSites {
			get {
				if (_allSites == null) {
					_allSites = SiteData.GetSiteList();
				}
				return _allSites;
			}
		}

		public void SaveOptions() {
			if (this.SiteOptions.Any()) {
				foreach (var s in this.SiteOptions) {
					if (s.Selected) {
						this.User.AddToSite(new Guid(s.Value));
					} else {
						this.User.RemoveFromSite(new Guid(s.Value));
					}
				}
			}

			if (this.RoleOptions.Any()) {
				foreach (var r in this.RoleOptions) {
					if (r.Selected) {
						this.User.AddToRole(r.Text);
					} else {
						this.User.RemoveFromRole(r.Text);
					}
				}
			}
		}
	}
}