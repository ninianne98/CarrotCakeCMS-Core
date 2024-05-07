using Carrotware.CMS.Data.Models;
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

namespace Carrotware.CMS.Core {

	public class UserRole {

		public UserRole() {
			this.RoleId = Guid.Empty.ToString();
		}

		public UserRole(string roleName) {
			this.RoleName = roleName;
			this.RoleId = Guid.Empty.ToString();
		}

		public UserRole(string roleName, string roleID) {
			this.RoleName = roleName;
			this.RoleId = roleID;
		}

		internal UserRole(AspNetRole role) {
			if (role != null) {
				this.RoleId = role.Id;
				this.RoleName = role.Name;
			}
		}

		[Display(Name = "ID")]
		[Required]
		public string RoleId { get; set; }

		[Display(Name = "Name")]
		[Required]
		public string RoleName { get; set; }

		[Display(Name = "Lowercase Name")]
		public string LoweredRoleName { get { return (this.RoleName ?? string.Empty).ToLowerInvariant(); } }

		public void Save() {
			using (var db = CarrotCakeContext.Create()) {
				AspNetRole role = (from r in db.AspNetRoles
								   where r.Name == this.RoleName || r.Id == this.RoleId
								   select r).FirstOrDefault();

				if (role == null) {
					role = new AspNetRole();
					role.Id = Guid.NewGuid().ToString().ToLowerInvariant();
					db.AspNetRoles.Add(role);
				}

				role.Name = this.RoleName.Trim();

				db.SaveChanges();

				this.RoleName = role.Name;
				this.RoleId = role.Id;
			}
		}

		public List<ExtendedUserData> GetMembers() {
			using (var db = CarrotCakeContext.Create()) {
				return (from ur in db.AspNetUserRoles
						join r in db.AspNetRoles on ur.RoleId equals r.Id
						join ud in db.vwCarrotUserData on ur.UserId equals ud.UserKey
						where r.Id == this.RoleId
						orderby ud.UserName
						select new ExtendedUserData(ud)).ToList();
			}
		}
	}
}