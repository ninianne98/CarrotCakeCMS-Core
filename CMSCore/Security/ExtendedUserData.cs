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

	public class ExtendedUserData {

		[Display(Name = "ID")]
		public string Id { get; set; }

		[Display(Name = "Email")]
		[StringLength(128)]
		[Required]
		public string Email { get; set; }

		public bool EmailConfirmed { get; set; }
		public string? PasswordHash { get; set; }
		public string? SecurityStamp { get; set; }

		[Display(Name = "Phone #")]
		public string? PhoneNumber { get; set; }

		public bool PhoneNumberConfirmed { get; set; }
		public bool TwoFactorEnabled { get; set; }

		[Display(Name = "Lockout End Date (UTC)")]
		public DateTime? LockoutEndDateUtc { get; set; }

		public bool LockoutEnabled { get; set; }

		[Display(Name = "No Lockout Date")]
		public bool LockoutEndDateBlank { get; set; }

		public int AccessFailedCount { get; set; }

		[Display(Name = "Username")]
		[Required]
		public string UserName { get; set; }

		[Display(Name = "User Id")]
		[Required]
		public Guid UserId { get; set; }

		public string UserKey { get; set; }

		[StringLength(128)]
		[Display(Name = "Nickname")]
		public string? UserNickName { get; set; }

		[StringLength(128)]
		[Display(Name = "First")]
		public string? FirstName { get; set; }

		[StringLength(128)]
		[Display(Name = "Last")]
		public string? LastName { get; set; }

		[Display(Name = "Bio")]
		public string? UserBio { get; set; }

		[Display(Name = "URL")]
		public string EditorURL { get { return ContentPageHelper.ScrubFilename(this.UserId, string.Format("/{0}/{1}", SiteData.CurrentSite.BlogEditorFolderPath, this.UserName)); } }

		public override string ToString() {
			return this.FullName_FirstLast;
		}

		[Display(Name = "First + Last")]
		public string FullName_FirstLast {
			get {
				if (!string.IsNullOrEmpty(this.LastName)) {
					return string.Format("{0} {1}", this.FirstName, this.LastName);
				} else {
					if (!string.IsNullOrEmpty(this.UserName)) {
						return this.UserName;
					} else {
						return "Unknown User";
					}
				}
			}
		}

		[Display(Name = "Last, First")]
		public string FullName_LastFirst {
			get {
				if (!string.IsNullOrEmpty(this.LastName)) {
					return string.Format("{0}, {1}", this.LastName, this.FirstName);
				} else {
					if (!string.IsNullOrEmpty(this.UserName)) {
						return this.UserName;
					} else {
						return "Unknown User";
					}
				}
			}
		}

		//==================================

		public ExtendedUserData() { }

		public ExtendedUserData(string UserName) {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				vwCarrotUserData rc = CompiledQueries.cqFindUserByName(_db, UserName);
				LoadUserData(rc);
			}
		}

		public ExtendedUserData(Guid UserID) {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				vwCarrotUserData rc = CompiledQueries.cqFindUserByID(_db, UserID);
				LoadUserData(rc);
			}
		}

		public static ExtendedUserData FindByUsername(string UserName) {
			ExtendedUserData usr = new ExtendedUserData();

			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				vwCarrotUserData rc = CompiledQueries.cqFindUserByName(_db, UserName);
				usr.LoadUserData(rc);
			}

			return usr;
		}

		public static ExtendedUserData FindByEmail(string Email) {
			ExtendedUserData usr = new ExtendedUserData();

			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				vwCarrotUserData rc = CompiledQueries.cqFindUserByEmail(_db, Email);
				usr.LoadUserData(rc);
			}

			return usr;
		}

		public static ExtendedUserData FindByUserID(Guid UserID) {
			ExtendedUserData usr = new ExtendedUserData();

			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				vwCarrotUserData rc = CompiledQueries.cqFindUserByID(_db, UserID);
				usr.LoadUserData(rc);
			}

			return usr;
		}

		internal void LoadUserData(vwCarrotUserData c) {
			this.UserId = Guid.Empty;
			this.Email = string.Empty;
			this.UserName = string.Empty;
			this.UserKey = string.Empty;

			if (c != null) {
				this.Id = c.Id;
				this.Email = c.Email;
				this.EmailConfirmed = c.EmailConfirmed;
				this.PasswordHash = c.PasswordHash;
				this.SecurityStamp = c.SecurityStamp;
				this.PhoneNumber = c.PhoneNumber;
				this.PhoneNumberConfirmed = c.PhoneNumberConfirmed;
				this.TwoFactorEnabled = c.TwoFactorEnabled;
				this.LockoutEndDateUtc = c.LockoutEndDateUtc;
				this.LockoutEndDateBlank = !c.LockoutEndDateUtc.HasValue;
				this.LockoutEnabled = c.LockoutEnabled;
				this.AccessFailedCount = c.AccessFailedCount;
				this.UserName = c.UserName;
				this.UserId = c.UserId.HasValue ? c.UserId.Value : Guid.Empty;
				this.UserKey = c.UserKey;
				this.UserNickName = c.UserNickName;
				this.FirstName = c.FirstName;
				this.LastName = c.LastName;
				this.UserBio = c.UserBio;
			}
		}

		private List<Guid> _siteIDs = null;

		public List<Guid> MemberSiteIDs {
			get {
				if (_siteIDs == null) {
					using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
						_siteIDs = (from m in _db.CarrotUserSiteMappings
									where m.UserId == this.UserId
									select m.SiteId).Distinct().ToList();
					}
				}
				return _siteIDs;
			}
		}

		public List<SiteData> GetSiteList() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				return (from m in _db.CarrotUserSiteMappings
						join s in _db.CarrotSites on m.SiteId equals s.SiteId
						where m.UserId == this.UserId
						select new SiteData(s)).ToList();
			}
		}

		public List<UserRole> GetRoles() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				return (from ur in _db.AspNetUserRoles
						join u in _db.AspNetUsers on ur.UserId equals u.Id
						join r in _db.AspNetRoles on ur.RoleId equals r.Id
						join ud in _db.CarrotUserData on u.Id equals ud.UserKey
						where u.UserName == this.UserName
						orderby r.Name
						select new UserRole(r)).ToList();
			}
		}

		public bool AddToRole(string roleName) {
			return SecurityData.AddUserToRole(this.UserName, roleName);
		}

		public bool RemoveFromRole(string roleName) {
			return SecurityData.RemoveUserFromRole(this.UserName, roleName);
		}

		public bool AddToSite(Guid siteID) {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var map = (from m in _db.CarrotUserSiteMappings
						   where m.UserId == this.UserId
							 && m.SiteId == siteID
						   select m).FirstOrDefault();

				if (map == null) {
					_siteIDs = null;

					map = new CarrotUserSiteMapping();
					map.UserSiteMappingId = Guid.NewGuid();
					map.SiteId = siteID;
					map.UserId = this.UserId;

					_db.CarrotUserSiteMappings.Add(map);
					_db.SaveChanges();

					return true;
				} else {
					return false;
				}
			}
		}

		public bool RemoveFromSite(Guid siteID) {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var map = (from m in _db.CarrotUserSiteMappings
						   where m.UserId == this.UserId
							 && m.SiteId == siteID
						   select m).FirstOrDefault();

				if (map != null) {
					_siteIDs = null;

					_db.CarrotUserSiteMappings.Remove(map);
					_db.SaveChanges();

					return true;
				} else {
					return false;
				}
			}
		}

		public void Save() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				bool bNew = false;
				var usr = CompiledQueries.cqFindUserTblByID(_db, this.UserId);

				if (usr == null) {
					usr = new CarrotUserData();
					usr.UserKey = this.UserKey;
					usr.UserId = Guid.NewGuid();
					bNew = true;
				}

				usr.UserNickName = this.UserNickName;
				usr.FirstName = this.FirstName;
				usr.LastName = this.LastName;
				usr.UserBio = this.UserBio;

				if (bNew) {
					_db.CarrotUserData.Add(usr);
				}

				_db.SaveChanges();

				this.UserId = usr.UserId;

				//grab fresh copy from DB
				vwCarrotUserData rc = CompiledQueries.cqFindUserByID(_db, usr.UserId);
				LoadUserData(rc);
			}
		}

		internal ExtendedUserData(vwCarrotUserData c) {
			LoadUserData(c);
		}

		[Display(Name = "Is Admin")]
		public bool IsAdmin {
			get {
				try {
					return SecurityData.IsUserInRole(this.UserName, SecurityData.CMSGroup_Admins);
				} catch (Exception ex) {
					return false;
				}
			}
		}

		[Display(Name = "Is Editor")]
		public bool IsEditor {
			get {
				try {
					return SecurityData.IsUserInRole(this.UserName, SecurityData.CMSGroup_Editors);
				} catch (Exception ex) {
					return false;
				}
			}
		}

		//================================================

		public static List<ExtendedUserData> GetUserList() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				List<ExtendedUserData> lstUsr = (from u in CompiledQueries.cqGetUserList(_db)
												 select new ExtendedUserData(u)).ToList();
				return lstUsr;
			}
		}

		public static IEnumerable<ExtendedUserData> GetUsers() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var lstUsr = (from u in CompiledQueries.cqGetUserList(_db)
							  select new ExtendedUserData(u));
				return lstUsr;
			}
		}

		public static ExtendedUserData GetEditorFromURL() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetEditorByURL(_db, SiteData.CurrentSiteID, SiteData.CurrentScriptName);
				if (query != null) {
					ExtendedUserData usr = new ExtendedUserData(query.UserId.Value);
					return usr;
				} else {
					return null;
				}
			}
		}
	}
}