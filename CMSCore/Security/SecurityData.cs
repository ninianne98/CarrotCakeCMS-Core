using Carrotware.CMS.Core.Security;
using Carrotware.CMS.Data.Models;
using Carrotware.CMS.Interface;
using Carrotware.CMS.Security;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Security.Principal;
using System.Text;
using System.Web;

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

	public class SecurityData {
		private CarrotSecurityConfig _config;

		public SecurityData() {
			LoadSettings();
		}

		protected void LoadSettings() {
			_config = CarrotSecurityConfig.GetConfig(CarrotHttpHelper.Configuration);
		}

		public static bool CheckUserExistance() {
			using (var db = CarrotCakeContext.Create()) {
				return (from u in db.AspNetUsers
						select u.UserName).Take(10).Any();
			}
		}

		public static List<SelectListItem> CheckMigrationHistory() {
			var lst = new List<SelectListItem>();
			string queryText = "SELECT [MigrationId], [ProductVersion] FROM [dbo].[__EFMigrationsHistory] ORDER BY [MigrationId] DESC, [ProductVersion]";

			var ds = CarrotCakeContext.Exec(queryText);

			if (ds.Tables.Count > 0) {
				foreach (DataRow row in ds.Tables[0].Rows) {
					lst.Add(new SelectListItem(row["MigrationId"].ToString(), row["ProductVersion"].ToString()));
				}
			}

			return lst;
		}

		public static IdentityRole FindRole(string roleName) {
			using (var db = CarrotCakeContext.Create()) {
				return (from r in db.AspNetRoles
						where r.Name == roleName
						select NewIdentityRole(r)).FirstOrDefault();
			}
		}

		public static IdentityRole FindRoleByID(string roleID) {
			using (var db = CarrotCakeContext.Create()) {
				return (from r in db.AspNetRoles
						where r.Id == roleID
						select NewIdentityRole(r)).FirstOrDefault();
			}
		}

		public static UserRole GetUserRole(string roleID) {
			using (var db = CarrotCakeContext.Create()) {
				return (from r in db.AspNetRoles
						where r.Id == roleID
						select new UserRole(r)).FirstOrDefault();
			}
		}

		public static List<UserRole> GetRoleList() {
			using (var db = CarrotCakeContext.Create()) {
				return (from r in db.AspNetRoles
						orderby r.Name
						select new UserRole(r)).ToList();
			}
		}

		public static List<UserRole> GetRoleListRestricted() {
			using (var db = CarrotCakeContext.Create()) {
				if (!SecurityData.IsAdmin) {
					return (from r in db.AspNetRoles
							where r.Name != SecurityData.CMSGroup_Users && r.Name != SecurityData.CMSGroup_Admins
							orderby r.Name
							select new UserRole(r)).ToList();
				} else {
					return (from r in db.AspNetRoles
							where r.Name != SecurityData.CMSGroup_Users
							orderby r.Name
							select new UserRole(r)).ToList();
				}
			}
		}

		public static List<IdentityUser> GetUserSearch(string searchTerm) {
			using (var db = CarrotCakeContext.Create()) {
				return (from u in db.AspNetUsers
						where u.UserName.Contains(searchTerm)
								|| u.Email.Contains(searchTerm)
						select NewIdentityUser(u)).Take(100).ToList();
			}
		}

		public static UserProfile GetProfileByUserID(Guid userId) {
			using (var db = CarrotCakeContext.Create()) {
				return (from u in db.AspNetUsers
						join ud1 in db.CarrotUserData on u.Id equals ud1.UserKey into ud2
						from ud in ud2.DefaultIfEmpty()
						where ud.UserId == userId
						select new UserProfile(u, ud)).FirstOrDefault();
			}
		}

		public static UserProfile GetProfileByUserName(string userName) {
			using (var db = CarrotCakeContext.Create()) {
				return (from u in db.AspNetUsers
						join ud1 in db.CarrotUserData on u.Id equals ud1.UserKey into ud2
						from ud in ud2.DefaultIfEmpty()
						where u.UserName == userName
						select new UserProfile(u, ud)).FirstOrDefault();
			}
		}

		public static List<UserProfile> GetUserProfileSearch(string searchTerm) {
			using (var db = CarrotCakeContext.Create()) {
				return (from u in db.AspNetUsers
						join ud1 in db.CarrotUserData on u.Id equals ud1.UserKey into ud2
						from ud in ud2.DefaultIfEmpty()
						where u.UserName.Contains(searchTerm)
								   || u.Email.Contains(searchTerm)
						select new UserProfile(u, ud)).Take(100).ToList();
			}
		}

		public static List<IdentityUser> GetCreditUserSearch(string searchTerm) {
			List<IdentityUser> usrs = null;
			List<string> admins = null;
			List<string> editors = null;

			using (var db = CarrotCakeContext.Create()) {
				admins = (from ur in db.AspNetUserRoles
						  join u in db.AspNetUsers on ur.UserId equals u.Id
						  join r in db.AspNetRoles on ur.RoleId equals r.Id
						  join ud in db.CarrotUserData on u.Id equals ud.UserKey
						  where r.Name == CMSGroup_Admins
						  select ud.UserKey).ToList();

				editors = (from sm in db.CarrotUserSiteMappings
						   join ud in db.CarrotUserData on sm.UserId equals ud.UserId
						   where sm.SiteId == SiteData.CurrentSiteID
						   select ud.UserKey).ToList();

				usrs = (from u in db.AspNetUsers
						where (u.UserName.Contains(searchTerm)
									|| u.Email.Contains(searchTerm))
								&& admins.Union(editors).Contains(u.Id)
						select NewIdentityUser(u)).Take(50).ToList();
			}

			return usrs;
		}

		public static List<UserProfile> GetCreditUserProfileSearch(string searchTerm) {
			List<UserProfile> usrs = null;
			List<string> admins = null;
			List<string> editors = null;

			using (var db = CarrotCakeContext.Create()) {
				admins = (from ur in db.AspNetUserRoles
						  join u in db.AspNetUsers on ur.UserId equals u.Id
						  join r in db.AspNetRoles on ur.RoleId equals r.Id
						  join ud in db.CarrotUserData on u.Id equals ud.UserKey
						  where r.Name == CMSGroup_Admins
						  select ud.UserKey).ToList();

				editors = (from sm in db.CarrotUserSiteMappings
						   join ud in db.CarrotUserData on sm.UserId equals ud.UserId
						   where sm.SiteId == SiteData.CurrentSiteID
						   select ud.UserKey).ToList();

				usrs = (from u in db.AspNetUsers
						join ud1 in db.CarrotUserData on u.Id equals ud1.UserKey into ud2
						from ud in ud2.DefaultIfEmpty()
						where (u.UserName.Contains(searchTerm)
									|| u.Email.Contains(searchTerm))
								&& admins.Union(editors).Contains(u.Id)
						select new UserProfile(u, ud)).Take(50).ToList();
			}

			return usrs;
		}

		public static List<IdentityUser> GetUserListByEmail(string email) {
			using (var db = CarrotCakeContext.Create()) {
				return (from u in db.AspNetUsers
						where u.Email.Contains(email)
						select NewIdentityUser(u)).Take(50).ToList();
			}
		}

		public static List<IdentityUser> GetUserListByName(string userName) {
			using (var db = CarrotCakeContext.Create()) {
				return (from u in db.AspNetUsers
						where (u.UserName.Contains(userName))
						select NewIdentityUser(u)).Take(50).ToList();
			}
		}

		public static List<IdentityUser> GetUserList() {
			using (var db = CarrotCakeContext.Create()) {
				return (from u in db.AspNetUsers
						select NewIdentityUser(u)).Take(1000).ToList();
			}
		}

		public static List<IdentityUser> GetUsersInRole(string groupName) {
			List<IdentityUser> usrs = new List<IdentityUser>();

			using (var db = CarrotCakeContext.Create()) {
				var role = (from r in db.AspNetRoles
							where r.Name == groupName
							select r).FirstOrDefault();

				if (role != null) {
					usrs = (from ur in db.AspNetUserRoles
							join u in db.AspNetUsers on ur.UserId equals u.Id
							select NewIdentityUser(u)).Take(2500).ToList();
				}
			}

			return usrs;
		}

		public static string CMSGroup_Admins {
			get {
				return "CarrotCMS Administrators";
			}
		}

		public static string CMSGroup_Editors {
			get {
				return "CarrotCMS Editors";
			}
		}

		public static string CMSGroup_Users {
			get {
				return "CarrotCMS Users";
			}
		}

		private static string keyIsAdmin = "cms_IsAdmin";

		private static string keyIsSiteEditor = "cms_IsSiteEditor";

		public static bool GetIsAdminFromCache() {
			bool keyVal = false;
			try {
				if (IsAuthenticated) {
					string key = string.Format("{0}_{1}", keyIsAdmin, SecurityData.CurrentUserIdentityName);
					if (CarrotHttpHelper.CacheGet(key) != null) {
						keyVal = Convert.ToBoolean(CarrotHttpHelper.CacheGet(key));
					} else {
						keyVal = IsUserInRole(SecurityData.CMSGroup_Admins);
						CarrotHttpHelper.CacheInsert(key, keyVal.ToString(), 1);
					}
				}
			} catch (Exception ex) {
				SiteData.WriteDebugException("getisadminfromcache", ex);
			}
			return keyVal;
		}

		public static bool GetIsSiteEditorFromCache() {
			bool keyVal = false;
			try {
				if (IsAuthenticated) {
					string key = string.Format("{0}_{1}_{2}", keyIsSiteEditor, SecurityData.CurrentUserIdentityName, SiteData.CurrentSiteID);
					if (CarrotHttpHelper.CacheGet(key) != null) {
						keyVal = Convert.ToBoolean(CarrotHttpHelper.CacheGet(key));
					} else {
						ExtendedUserData usrEx = SecurityData.CurrentExUser;

						keyVal = (IsEditor || usrEx.IsEditor) && usrEx.MemberSiteIDs.Contains(SiteData.CurrentSiteID);

						CarrotHttpHelper.CacheInsert(key, keyVal.ToString(), 1);
					}
				}
			} catch (Exception ex) {
				SiteData.WriteDebugException("getissiteeditorfromcache", ex);
			}
			return keyVal;
		}

		public static bool IsAdmin {
			get {
				return GetIsAdminFromCache();
			}
		}

		public static bool IsEditor {
			get {
				try {
					if (IsAuthenticated) {
						return IsUserInRole(SecurityData.CMSGroup_Editors);
					}
				} catch (Exception ex) {
					SiteData.WriteDebugException("iseditor", ex);
				}
				return false;
			}
		}

		public static bool IsUsers {
			get {
				try {
					if (IsAuthenticated) {
						return IsUserInRole(SecurityData.CMSGroup_Users);
					}
				} catch (Exception ex) {
					SiteData.WriteDebugException("isusers", ex);
				}
				return false;
			}
		}

		public static IPrincipal UserPrincipal {
			get {
				return CarrotHttpHelper.HttpContext.User;
			}
		}

		public static bool IsAuthenticated {
			get {
				if (CarrotHttpHelper.HttpContext != null && UserPrincipal.Identity.IsAuthenticated) {
					return true;
				}

				return false;
			}
		}

		public static string GetUserName() {
			if (IsAuthenticated) {
				return UserPrincipal.Identity.Name;
			}

			return string.Empty;
		}

		public static bool IsUserInRole(string groupName) {
			return IsUserInRole(SecurityData.CurrentUserIdentityName, groupName);
		}

		private static string keyIsUserInRole = "cms_IsUserInRole";

		public static bool IsUserInRole(string userName, string roleName) {
			bool keyVal = false;

			if (IsAuthenticated) {
				string key = string.Format("{0}_{1}_{2}", keyIsUserInRole, userName, roleName);

				if (CarrotHttpHelper.CacheGet(key) != null) {
					keyVal = Convert.ToBoolean(CarrotHttpHelper.CacheGet(key));
				} else {
					using (var db = CarrotCakeContext.Create()) {
						var usrRole = (from r in db.AspNetRoles
									   join ur in db.AspNetUserRoles on r.Id equals ur.RoleId
									   join u in db.AspNetUsers on ur.UserId equals u.Id
									   where r.Name == roleName
											   && u.UserName == userName
									   select ur).FirstOrDefault();

						keyVal = usrRole != null;
					}
					CarrotHttpHelper.CacheInsert(key, keyVal.ToString(), 1);
				}
			}

			return keyVal;
		}

		public static bool IsSiteEditor {
			get {
				return GetIsSiteEditorFromCache();
			}
		}

		public static bool IsAuthEditor {
			get {
				if (IsAuthenticated) {
					return AdvancedEditMode || IsAdmin || IsSiteEditor;
				} else {
					return false;
				}
			}
		}

		public static bool IsAuthUser {
			get {
				if (IsAuthenticated) {
					return IsAdmin || IsSiteEditor || IsUsers;
				} else {
					return false;
				}
			}
		}

		public static Guid CurrentUserGuid {
			get {
				Guid _currentUserGuid = Guid.Empty;
				if (CurrentUser != null) {
					_currentUserGuid = SecurityData.CurrentExUser.UserId;
				}
				return _currentUserGuid;
			}
		}

		public static UserProfile CurrentUser {
			get {
				UserProfile currentUser = null;
				if (IsAuthenticated) {
					string userName = SecurityData.CurrentUserIdentityName;
					string key = string.Format("cms_CurrentUserProfile_{0}", userName);

					if (CarrotHttpHelper.CacheGet(key) != null) {
						currentUser = (UserProfile)CarrotHttpHelper.CacheGet(key);
					} else {
						using (var db = CarrotCakeContext.Create()) {
							currentUser = (from u in db.AspNetUsers
										   join ud1 in db.CarrotUserData on u.Id equals ud1.UserKey into ud2
										   from ud in ud2.DefaultIfEmpty()
										   where u.UserName == userName
										   select new UserProfile(u, ud)).FirstOrDefault();
						}

						if (currentUser != null) {
							CarrotHttpHelper.CacheInsert(key, currentUser, 2);
						}
					}
				} else {
					currentUser = new UserProfile();
					currentUser.UserId = Guid.Empty;
					currentUser.UserKey = Guid.Empty.ToString();
					currentUser.UserName = "anonymous-user-" + Guid.Empty.ToString();
				}

				return currentUser;
			}
		}

		public static ExtendedUserData CurrentExUser {
			get {
				ExtendedUserData currentUser = null;

				if (IsAuthenticated) {
					string userName = SecurityData.CurrentUserIdentityName;
					string key = string.Format("cms_CurrentExUser_{0}", userName);

					if (CarrotHttpHelper.CacheGet(key) != null) {
						currentUser = (ExtendedUserData)CarrotHttpHelper.CacheGet(key);
					} else {
						using (var db = CarrotCakeContext.Create()) {
							currentUser = (from u in db.vwCarrotUserData
										   where u.UserName == userName
										   select new ExtendedUserData(u)).FirstOrDefault();
						}

						if (currentUser != null) {
							CarrotHttpHelper.CacheInsert(key, currentUser, 2);
						}
					}
				} else {
					currentUser = new ExtendedUserData();
					currentUser.UserId = Guid.Empty;
					currentUser.UserKey = Guid.Empty.ToString();
					currentUser.UserName = "anonymous-user-" + Guid.Empty.ToString();
				}

				return currentUser;
			}
		}

		public static IdentityUser NewIdentityUser(ExtendedUserData u) {
			return new IdentityUser(u.UserName) { Id = u.UserKey, Email = u.Email, UserName = u.UserName };
		}

		private static IdentityRole NewIdentityRole(AspNetRole r) {
			return new IdentityRole(r.Name) { Id = r.Id, Name = r.Name };
		}

		private static IdentityUser NewIdentityUser(AspNetUser u) {
			return new IdentityUser(u.UserName) { Id = u.Id, Email = u.Email, UserName = u.UserName };
		}

		public static IdentityUser CurrentIdentityUser {
			get {
				if (string.IsNullOrEmpty(CurrentUserIdentityName)) {
					return null;
				}

				var userTask = CarrotHttpHelper.UserManager.FindByNameAsync(CurrentUserIdentityName);
				var user = userTask.Result;

				return user;
			}
		}

		public static IdentityUser GetUserByID(string key) {
			using (var db = CarrotCakeContext.Create()) {
				return (from u in db.AspNetUsers
						where u.UserName == key
						select NewIdentityUser(u)).FirstOrDefault();
			}
		}

		public static IdentityUser GetUserByName(string username) {
			using (var db = CarrotCakeContext.Create()) {
				return (from u in db.AspNetUsers
						where u.UserName == username
						select NewIdentityUser(u)).FirstOrDefault();
			}
		}

		public static IdentityUser GetUserByEmail(string email) {
			using (var db = CarrotCakeContext.Create()) {
				return (from u in db.AspNetUsers
						where u.Email == email
						select NewIdentityUser(u)).FirstOrDefault();
			}
		}

		public static bool AdvancedEditMode {
			get {
				bool _Advanced = false;
				if (IsAuthenticated) {
					if (CarrotHttpHelper.QueryString(SiteData.AdvancedEditParameter) != null
								&& (SecurityData.IsAdmin || SecurityData.IsSiteEditor)) {
						_Advanced = true;
					} else {
						_Advanced = false;
					}
				}

				return _Advanced;
			}
		}

		private static string CurrentDLLVersion {
			get { return SiteData.CurrentDLLVersion; }
		}

		public static string CurrentUserIdentityName {
			get {
				if (IsAuthenticated) {
					return UserPrincipal.Identity.Name.ToLowerInvariant();
				}
				return string.Empty;
			}
		}

		public async Task<NewUser> CreateIdentityUser(IdentityUser user) {
			return await AttemptCreateIdentityUser(user, SecurityData.GenerateSimplePassword());
		}

		public async Task<NewUser> CreateIdentityUser(IdentityUser user, string password) {
			return await AttemptCreateIdentityUser(user, password);
		}

		private async Task<NewUser> AttemptCreateIdentityUser(IdentityUser user, string password) {
			var data = new NewUser();
			var result = new IdentityResult();

			if (user != null && !string.IsNullOrEmpty(user.Id)) {
				using (var db = CarrotCakeContext.Create()) {
					var mgr = new ManageSecurity();

					result = await mgr.UserManager.CreateAsync(user, password);
					data.IdentityResult = result;

					if (result.Succeeded) {
						var newusr = new ExtendedUserData();
						newusr.UserKey = user.Id;
						newusr.UserName = user.UserName;
						newusr.Save();

						newusr = ExtendedUserData.FindByUserID(newusr.UserId);

						data = new NewUser(newusr, result);
					}
				}
			}

			return data;
		}

		public async Task<IdentityResult> ResetPassword(IdentityUser user, string token, string password) {
			var result = new IdentityResult();

			if (user != null && !string.IsNullOrEmpty(user.Id)) {
				var mgr = new ManageSecurity();
				result = await mgr.UserManager.ResetPasswordAsync(user, token, password);
				return result;
			}

			return result;
		}

		public async Task<bool> ResetPassword(string email) {
			string adminFolder = SiteData.AdminFolderPath.TrimPathSlashes();

			string adminEmailPath = string.Format("{0}/{1}", adminFolder, SiteActions.ResetPassword);

			return await ResetPassword(adminEmailPath, email);
		}

		public async Task<bool> ResetPassword(string resetUri, string email) {
			HttpRequest request = CarrotHttpHelper.HttpContext.Request;
			IdentityUser user = null;
			string token = string.Empty;

			resetUri = resetUri.TrimPathSlashes();

			if (!string.IsNullOrEmpty(email)) {
				var mgr = new ManageSecurity();
				user = await mgr.FindByEmailAsync(email);

				if (user != null) {
					token = await mgr.UserManager.GeneratePasswordResetTokenAsync(user);

					var sb = new StringBuilder();
					sb.Append(CoreHelper.ReadEmbededScript("Carrotware.CMS.Core.Security.EmailForgotPassMsg.txt"));

					string httpHost;
					try { httpHost = request.Host.Value.Trim(); } catch { httpHost = string.Empty; }
					string hostName = httpHost.ToLowerInvariant();

					httpHost = CarrotWebHelper.BuildHttpHost().ToLowerInvariant();
					var baseRoute = _config.AdditionalSettings.ResetPath;

					var resetTokenUrl = $"{httpHost}{baseRoute}?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(user.Email)}";

					sb.Replace("{%%UserName%%}", user.UserName);
					sb.Replace("{%%SiteURL%%}", httpHost);
					sb.Replace("{%%Version%%}", CurrentDLLVersion);
					sb.Replace("{%%AdminFolderPath%%}", string.Format("{0}{1}", httpHost, SiteData.AdminFolderPath));

					sb.Replace("{%%ResetURL%%}", resetTokenUrl);

					if (SiteData.CurrentSiteExists) {
						sb.Replace("{%%Time%%}", SiteData.CurrentSite.Now.ToString());
					} else {
						sb.Replace("{%%Time%%}", DateTime.Now.ToString());
					}

					var body = sb.ToString();

					var message = MailRequest.Create();
					message.HtmlBody = false;
					message.ConfigureMessage(user.Email, string.Format("Reset Password {0}", hostName), body);

					await message.SendEmailAsync();
					return true;
				} else {
					return false;
				}
			}

			return false;
		}

		public static bool RemoveUserFromRole(string userName, string roleName) {
			using (var db = CarrotCakeContext.Create()) {
				var usrRole = (from r in db.AspNetRoles
							   join ur in db.AspNetUserRoles on r.Id equals ur.RoleId
							   join u in db.AspNetUsers on ur.UserId equals u.Id
							   where r.Name == roleName
									   && u.UserName == userName
							   select ur).FirstOrDefault();

				if (usrRole != null) {
					db.AspNetUserRoles.Remove(usrRole);
					db.SaveChanges();

					return true;
				}
				return false;
			}
		}

		public static bool AddUserToRole(string userName, string roleName) {
			using (var db = CarrotCakeContext.Create()) {
				AspNetRole role = (from r in db.AspNetRoles
								   where r.Name == roleName
								   select r).FirstOrDefault();

				var user = (from u in db.AspNetUsers
							where u.UserName == userName
							select u).FirstOrDefault();

				var usrRole = (from r in db.AspNetRoles
							   join ur in db.AspNetUserRoles on r.Id equals ur.RoleId
							   join u in db.AspNetUsers on ur.UserId equals u.Id
							   where r.Name == roleName
									   && u.UserName == userName
							   select ur).FirstOrDefault();

				if (usrRole == null && role != null && user != null) {
					usrRole = new AspNetUserRole();
					usrRole.UserId = user.Id;
					usrRole.RoleId = role.Id;
					db.AspNetUserRoles.Add(usrRole);
					db.SaveChanges();

					return true;
				}
				return false;
			}
		}

		public static bool AddUserToRole(Guid UserId, string roleName) {
			using (var db = CarrotCakeContext.Create()) {
				AspNetRole role = (from r in db.AspNetRoles
								   where r.Name == roleName
								   select r).FirstOrDefault();

				var user = (from u in db.AspNetUsers
							join ud in db.CarrotUserData on u.Id equals ud.UserKey
							where ud.UserId == UserId
							select u).FirstOrDefault();

				var usrRole = (from r in db.AspNetRoles
							   join ur in db.AspNetUserRoles on r.Id equals ur.RoleId
							   join u in db.AspNetUsers on ur.UserId equals u.Id
							   join ud in db.CarrotUserData on u.Id equals ud.UserKey
							   where r.Name == roleName
									   && ud.UserId == UserId
							   select ur).FirstOrDefault();

				if (usrRole == null && role != null && user != null) {
					usrRole = new AspNetUserRole();
					usrRole.UserId = user.Id;
					usrRole.RoleId = role.Id;
					db.AspNetUserRoles.Add(usrRole);
					db.SaveChanges();

					return true;
				}
				return false;
			}
		}

		private static string alphaUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private static string alphaLower = "abcdefghijklmnopqrstuvwxyz";
		private static string numericChars = "1234567890";
		private static string specialChars = "@!$}{";

		private static string allChars = alphaUpper + alphaLower + numericChars + specialChars;

		private static int _length = -1;

		private static int GeneratedPasswordLength {
			get {
				if (_length <= 3) {
					CarrotSecurityConfig config = CarrotSecurityConfig.GetConfig();
					_length = config.PasswordValidator.RequiredLength;
					if (_length <= 8) {
						_length = 12;
					}
				}

				return _length;
			}
		}

		public static string GenerateSimplePassword() {
			int length = GeneratedPasswordLength;

			string generatedPassword = SelectRandomString(allChars, 4);

			for (int i = 0; i < length; i++) {
				if (i == 0 || i == 7) {
					generatedPassword += SelectRandomChar(alphaUpper);
				} else if (i == 2 || i == 5) {
					generatedPassword += SelectRandomChar(alphaLower);
				} else if (i == 4 || i == 3) {
					generatedPassword += SelectRandomChar(numericChars);
				} else if (i == 6 || i == 1) {
					generatedPassword += SelectRandomChar(specialChars);
				} else {
					generatedPassword += SelectRandomString(allChars, 3);
				}
			}

			return generatedPassword;
		}

		private static string SelectRandomString(string sourceString, int take) {
			return new string(sourceString.OrderBy(x => Guid.NewGuid()).Take(take).ToArray());
		}

		private static char SelectRandomChar(string sourceString) {
			return SelectRandomString(sourceString, 1).FirstOrDefault();
		}
	}
}