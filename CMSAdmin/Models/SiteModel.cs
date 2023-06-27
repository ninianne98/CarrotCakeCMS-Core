using Carrotware.CMS.Core;
using System.ComponentModel.DataAnnotations;

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

	public class SiteModel {

		public SiteModel() {
			this.Site = new SiteData();
			this.Users = new List<UserModel>();
		}

		public SiteModel(Guid siteId) {
			this.Site = SiteData.GetSiteByID(siteId);

			LoadUsers();
		}

		public void LoadUsers() {
			this.Users = this.Site.GetMappedUsers().OrderBy(x => x.UserName).Select(x => new UserModel(x)).ToList();
		}

		public SiteData Site { get; set; }
		public List<UserModel> Users { get; set; }

		[Display(Name = "New User")]
		public string? NewUserId { get; set; }

		[Display(Name = "Add to Editor Role ")]
		public bool NewUserAsEditor { get; set; }
	}
}