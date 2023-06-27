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

	public class RoleModel {

		public RoleModel() {
			this.Role = new UserRole();
			this.Role.RoleId = Guid.Empty.ToString().ToLowerInvariant();
			this.Users = new List<UserModel>();
		}

		public RoleModel(string roleId) {
			this.Role = SecurityData.GetUserRole(roleId);
			LoadUsers();
		}

		public void LoadUsers() {
			this.Users = this.Role.GetMembers().OrderBy(x => x.UserName).Select(x => new UserModel(x)).ToList();
		}

		public UserRole Role { get; set; }
		public List<UserModel> Users { get; set; }

		[Display(Name = "New User")]
		public string? NewUserId { get; set; }

		public bool CanEditRoleName {
			get {
				return !(SecurityData.CMSGroup_Admins.ToLowerInvariant() == this.Role.LoweredRoleName
							|| SecurityData.CMSGroup_Editors.ToLowerInvariant() == this.Role.LoweredRoleName
							|| SecurityData.CMSGroup_Users.ToLowerInvariant() == this.Role.LoweredRoleName);
			}
		}
	}
}