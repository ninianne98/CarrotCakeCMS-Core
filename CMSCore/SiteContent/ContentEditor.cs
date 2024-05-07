using Carrotware.CMS.Data.Models;
using System;

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

	public class ContentEditor {

		public ContentEditor() { }

		public Guid UserId { get; set; }
		public Guid SiteID { get; set; }
		public string LoweredEmail { get; set; }
		public string UserName { get; set; }
		public string UserUrl { get; set; }
		public int? UseCount { get; set; }
		public int? PublicUseCount { get; set; }
		public bool IsPublic { get; set; }
		public DateTime? EditDate { get; set; }

		public override bool Equals(object? obj) {
			//Check for null and compare run-time types.
			if (obj == null || this.GetType() != obj.GetType()) return false;

			if (obj is ContentEditor) {
				ContentEditor p = (ContentEditor)obj;
				return (this.SiteID == p.SiteID
						&& this.UserName.ToLowerInvariant() == p.UserName.ToLowerInvariant());
			} else {
				return false;
			}
		}

		public override int GetHashCode() {
			return this.UserName.GetHashCode() ^ this.SiteID.GetHashCode();
		}

		internal ContentEditor(vwCarrotEditorUrl c) {
			if (c != null) {
				SiteData site = SiteData.GetSiteFromCache(c.SiteId);

				this.UserId = c.UserId.Value;
				this.SiteID = c.SiteId;
				this.UserUrl = c.UserUrl;
				this.LoweredEmail = c.LoweredEmail;
				this.UseCount = c.UseCount;
				this.PublicUseCount = c.PublicUseCount;
				this.IsPublic = true;

				if (c.EditDate.HasValue) {
					this.EditDate = site.ConvertUTCToSiteTime(c.EditDate.Value);
				}
			}
		}

		public static ContentEditor Get(Guid SiteID, Guid UserID) {
			ContentEditor item = null;
			using (var db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetContentEditorByID(db, SiteID, UserID);
				if (query != null) {
					item = new ContentEditor(query);
				}
			}

			return item;
		}

		public static ContentEditor GetByURL(Guid SiteID, string requestedURL) {
			ContentEditor item = null;
			using (var db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetContentEditorURL(db, SiteID, requestedURL);
				if (query != null) {
					item = new ContentEditor(query);
				}
			}

			return item;
		}

		#region IContentMetaInfo Members

		public void SetValue(Guid ContentMetaInfoID) {
			this.UserId = ContentMetaInfoID;
		}

		public Guid ContentMetaInfoID {
			get { return this.UserId; }
		}

		public string MetaInfoText {
			get { return this.LoweredEmail; }
		}

		public string MetaInfoURL {
			get { return this.UserUrl; }
		}

		public bool MetaIsPublic {
			get { return this.IsPublic; }
		}

		public DateTime? MetaDataDate {
			get { return this.EditDate; }
		}

		public int MetaInfoCount {
			get { return this.UseCount == null ? 0 : Convert.ToInt32(this.UseCount); }
		}

		public int MetaPublicInfoCount {
			get { return this.PublicUseCount == null ? 0 : Convert.ToInt32(this.PublicUseCount); }
		}

		#endregion IContentMetaInfo Members
	}
}