using Carrotware.CMS.Data.Models;
using Microsoft.EntityFrameworkCore;
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

	public class ContentSnippet : IValidatableObject {

		public ContentSnippet() { }

		public ContentSnippet(Guid siteID) {
			this.Root_ContentSnippetID = Guid.NewGuid();
			this.ContentSnippetID = Guid.NewGuid();
			this.SiteID = siteID;
			this.CreateDate = SiteData.GetSiteByID(siteID).Now;
			this.EditDate = this.CreateDate;

			this.GoLiveDate = this.CreateDate.AddHours(-1);
			this.RetireDate = this.CreateDate.AddYears(2);
		}

		public Guid Root_ContentSnippetID { get; set; }
		public Guid SiteID { get; set; }

		[Display(Name = "Name")]
		[StringLength(256)]
		[Required]
		public string ContentSnippetName { get; set; }

		[Display(Name = "Slug")]
		[StringLength(128)]
		[Required]
		public string ContentSnippetSlug { get; set; }

		public Guid CreateUserId { get; set; }

		[Display(Name = "Date Created")]
		public DateTime CreateDate { get; set; }

		[Display(Name = "Active")]
		public bool ContentSnippetActive { get; set; }

		public Guid ContentSnippetID { get; set; }

		[Display(Name = "Latest")]
		public bool IsLatestVersion { get; set; }

		public Guid EditUserId { get; set; }

		[Display(Name = "Edit Date")]
		public DateTime EditDate { get; set; }

		public string ContentBody { get; set; }

		[Display(Name = "Go Live Date")]
		public DateTime GoLiveDate { get; set; }

		[Display(Name = "Retire Date")]
		public DateTime RetireDate { get; set; }

		public int? VersionCount { get; set; }

		public Guid? Heartbeat_UserId { get; set; }
		public DateTime? EditHeartbeat { get; set; }

		[Display(Name = "Selected Item")]
		public bool Selected { get; set; }

		[Display(Name = "Retired")]
		public bool IsRetired {
			get {
				if (this.RetireDate < SiteData.CurrentSite.Now) {
					return true;
				} else {
					return false;
				}
			}
		}

		[Display(Name = "Unreleased")]
		public bool IsUnReleased {
			get {
				if (this.GoLiveDate > SiteData.CurrentSite.Now) {
					return true;
				} else {
					return false;
				}
			}
		}

		internal ContentSnippet(vwCarrotContentSnippet c) {
			if (c != null) {
				SiteData site = SiteData.GetSiteFromCache(c.SiteId);

				this.Root_ContentSnippetID = c.RootContentSnippetId;
				this.SiteID = c.SiteId;
				this.ContentSnippetID = c.ContentSnippetId;
				this.ContentSnippetName = c.ContentSnippetName;
				this.ContentSnippetSlug = c.ContentSnippetSlug;
				this.ContentSnippetActive = c.ContentSnippetActive;
				this.IsLatestVersion = c.IsLatestVersion;
				this.ContentBody = c.ContentBody;

				this.CreateUserId = c.CreateUserId;
				this.CreateDate = site.ConvertUTCToSiteTime(c.CreateDate);
				this.EditUserId = c.EditUserId;
				this.EditDate = site.ConvertUTCToSiteTime(c.EditDate);
				this.GoLiveDate = site.ConvertUTCToSiteTime(c.GoLiveDate);
				this.RetireDate = site.ConvertUTCToSiteTime(c.RetireDate);

				this.Heartbeat_UserId = c.HeartbeatUserId;
				this.EditHeartbeat = c.EditHeartbeat;

				this.VersionCount = c.VersionCount;
			}
		}

		public void ResetHeartbeatLock() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var rc = CompiledQueries.cqGetSnippetDataTbl(_db, this.SiteID, this.Root_ContentSnippetID);

				rc.EditHeartbeat = DateTime.UtcNow.AddHours(-2);
				rc.HeartbeatUserId = null;
				_db.SaveChanges();
			}
		}

		public void RecordHeartbeatLock(Guid currentUserID) {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var rc = CompiledQueries.cqGetSnippetDataTbl(_db, this.SiteID, this.Root_ContentSnippetID);

				rc.HeartbeatUserId = currentUserID;
				rc.EditHeartbeat = DateTime.UtcNow;

				_db.SaveChanges();
			}
		}

		public bool IsLocked {
			get {
				bool bLock = false;
				if (this.Heartbeat_UserId != null) {
					if (this.Heartbeat_UserId != SecurityData.CurrentUserGuid
							&& this.EditHeartbeat.Value > DateTime.UtcNow.AddMinutes(-2)) {
						bLock = true;
					}
					if (this.Heartbeat_UserId == SecurityData.CurrentUserGuid
						|| this.Heartbeat_UserId == null) {
						bLock = false;
					}
				}
				return bLock;
			}
		}

		public bool IsSnippetLocked(Guid currentUserID) {
			bool bLock = false;
			if (this.Heartbeat_UserId != null) {
				if (this.Heartbeat_UserId != currentUserID
						&& this.EditHeartbeat.Value > DateTime.UtcNow.AddMinutes(-2)) {
					bLock = true;
				}
				if (this.Heartbeat_UserId == currentUserID
					|| this.Heartbeat_UserId == null) {
					bLock = false;
				}
			}
			return bLock;
		}

		public bool RecordSnippetLock(Guid currentUserID) {
			bool bLock = this.IsLocked;
			bool bRet = false;

			if (!bLock) {
				ExtendedUserData usr = new ExtendedUserData(currentUserID);

				//only allow admin/editors to record a lock
				if (usr.IsAdmin || usr.IsEditor) {
					bRet = true;
					RecordHeartbeatLock(currentUserID);
				}
			}

			return bRet;
		}

		public Guid GetCurrentEditUser() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var rc = CompiledQueries.cqGetSnippetDataTbl(_db, this.SiteID, this.Root_ContentSnippetID);

				if (rc != null) {
					return rc.HeartbeatUserId.Value;
				} else {
					return Guid.Empty;
				}
			}
		}

		public ExtendedUserData GetCurrentEditUserData() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var rc = CompiledQueries.cqGetSnippetDataTbl(_db, this.SiteID, this.Root_ContentSnippetID);

				if (rc != null) {
					return new ExtendedUserData(rc.HeartbeatUserId.Value);
				} else {
					return null;
				}
			}
		}

		public List<ContentSnippet> GetHistory() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				List<ContentSnippet> _types = (from d in CompiledQueries.cqGetSnippetVersionHistory(_db, this.Root_ContentSnippetID)
											   select new ContentSnippet(d)).ToList();

				return _types;
			}
		}

		public void Delete() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var s = CompiledQueries.cqGetSnippetDataTbl(_db, this.SiteID, this.Root_ContentSnippetID);

				if (s != null) {
					_db.CarrotContentSnippets.Where(x => x.RootContentSnippetId == this.Root_ContentSnippetID).ExecuteDelete();
					_db.CarrotRootContentSnippets.Remove(s);
					_db.SaveChanges();
				}
			}
		}

		public void DeleteThisVersion() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				CarrotRootContentSnippet s = CompiledQueries.cqGetSnippetDataTbl(_db, this.SiteID, this.Root_ContentSnippetID);

				if (s != null) {
					_db.CarrotContentSnippets.Where(m => m.ContentSnippetId == this.ContentSnippetID
												&& m.RootContentSnippetId == s.RootContentSnippetId
												&& m.IsLatestVersion != true).ExecuteDelete();
					_db.SaveChanges();
				}
			}
		}

		public void Save() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				SiteData site = SiteData.GetSiteFromCache(this.SiteID);

				CarrotRootContentSnippet rc = CompiledQueries.cqGetSnippetDataTbl(_db, this.SiteID, this.Root_ContentSnippetID);

				var oldC = CompiledQueries.cqGetLatestSnippetContentTbl(_db, this.SiteID, this.Root_ContentSnippetID);

				bool bNew = false;

				if (rc == null) {
					rc = new CarrotRootContentSnippet();
					rc.RootContentSnippetId = Guid.NewGuid();
					rc.SiteId = site.SiteID;

					rc.CreateDate = DateTime.UtcNow;
					if (this.CreateUserId != Guid.Empty) {
						rc.CreateUserId = this.CreateUserId;
					} else {
						rc.CreateUserId = SecurityData.CurrentUserGuid;
					}

					_db.CarrotRootContentSnippets.Add(rc);
					bNew = true;
				}

				this.ContentSnippetSlug = ContentPageHelper.ScrubSlug(this.ContentSnippetSlug);

				rc.ContentSnippetActive = this.ContentSnippetActive;
				rc.ContentSnippetName = this.ContentSnippetName;
				rc.ContentSnippetSlug = this.ContentSnippetSlug;

				rc.GoLiveDate = site.ConvertSiteTimeToUTC(this.GoLiveDate);
				rc.RetireDate = site.ConvertSiteTimeToUTC(this.RetireDate);

				CarrotContentSnippet c = new CarrotContentSnippet();
				c.ContentSnippetId = Guid.NewGuid();
				c.RootContentSnippetId = rc.RootContentSnippetId;
				c.IsLatestVersion = true;

				if (!bNew) {
					oldC.IsLatestVersion = false;
				}

				c.EditDate = DateTime.UtcNow;
				if (this.EditUserId != Guid.Empty) {
					c.EditUserId = this.EditUserId;
				} else {
					c.EditUserId = SecurityData.CurrentUserGuid;
				}

				c.ContentBody = this.ContentBody;

				rc.HeartbeatUserId = c.EditUserId;
				rc.EditHeartbeat = DateTime.UtcNow;

				_db.CarrotContentSnippets.Add(c);

				_db.SaveChanges();

				this.ContentSnippetID = c.ContentSnippetId;
				this.Root_ContentSnippetID = rc.RootContentSnippetId;
			}
		}

		public static int GetSimilar(Guid siteID, Guid rootSnippetID, string categorySlug) {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetContentSnippetNoMatch(_db, siteID, rootSnippetID, categorySlug);

				return query.Count();
			}
		}

		public static ContentSnippet Get(Guid rootSnippetID) {
			ContentSnippet _item = null;
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				vwCarrotContentSnippet query = CompiledQueries.cqGetLatestSnippetVersion(_db, rootSnippetID);
				if (query != null) {
					_item = new ContentSnippet(query);
				}
			}

			return _item;
		}

		public static ContentSnippet GetSnippetByID(Guid siteID, Guid rootSnippetID, bool bActiveOnly) {
			ContentSnippet _item = null;
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				vwCarrotContentSnippet query = CompiledQueries.GetLatestContentSnippetByID(_db, siteID, bActiveOnly, rootSnippetID);
				if (query != null) {
					_item = new ContentSnippet(query);
				}
			}

			return _item;
		}

		public static ContentSnippet GetSnippetBySlug(Guid siteID, string categorySlug, bool bActiveOnly) {
			ContentSnippet _item = null;
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				vwCarrotContentSnippet query = CompiledQueries.GetLatestContentSnippetBySlug(_db, siteID, bActiveOnly, categorySlug);
				if (query != null) {
					_item = new ContentSnippet(query);
				}
			}

			return _item;
		}

		public static ContentSnippet GetVersion(Guid snippetDataID) {
			ContentSnippet _item = null;
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				vwCarrotContentSnippet query = CompiledQueries.cqGetSnippetVersionByID(_db, snippetDataID);
				if (query != null) {
					_item = new ContentSnippet(query);
				}
			}

			return _item;
		}

		public static List<ContentSnippet> GetHistory(Guid rootSnippetID) {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				List<ContentSnippet> _types = (from d in CompiledQueries.cqGetSnippetVersionHistory(_db, rootSnippetID)
											   select new ContentSnippet(d)).ToList();

				return _types;
			}
		}

		private List<ValidationResult> _errors = null;

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
			if (_errors == null) {
				_errors = new List<ValidationResult>();
				List<string> lst = new List<string>();

				if (!IsUniqueSlug()) {
					ValidationResult err = new ValidationResult("Content slug not unique", new string[] { "ContentSnippetSlug" });
					_errors.Add(err);
				}
			}

			return _errors;
		}

		public bool IsUniqueSlug() {
			string theFileName = this.ContentSnippetSlug;

			int iCount = ContentSnippet.GetSimilar(this.SiteID, this.Root_ContentSnippetID, this.ContentSnippetSlug);

			if (iCount < 1) {
				return true;
			}

			return false;
		}
	}
}