using Carrotware.CMS.Data.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

	public class ContentTag : IValidatableObject, IMetaDataLinks {

		public ContentTag() { }

		public Guid ContentTagID { get; set; }
		public Guid SiteID { get; set; }

		[Display(Name = "Tag")]
		[StringLength(256)]
		public string TagText { get; set; }

		[Display(Name = "Slug")]
		[StringLength(256)]
		public string TagSlug { get; set; }

		[Display(Name = "URL")]
		public string TagURL { get; set; }

		public int? UseCount { get; set; }
		public int? PublicUseCount { get; set; }

		[Display(Name = "Public")]
		public bool IsPublic { get; set; }

		public DateTime? EditDate { get; set; }

		public IHtmlContent Text { get { return new HtmlString(this.TagText); } }
		public string Uri { get { return this.TagURL; } }

		public int Count {
			get {
				if (SecurityData.IsAuthEditor) {
					return this.UseCount ?? 0;
				} else {
					return this.PublicUseCount ?? 0;
				}
			}
		}

		public override bool Equals(object? obj) {
			//Check for null and compare run-time types.
			if (obj == null || this.GetType() != obj.GetType()) return false;

			if (obj is ContentTag) {
				ContentTag p = (ContentTag)obj;
				return (this.SiteID == p.SiteID && this.ContentTagID == p.ContentTagID);
			} else {
				return false;
			}
		}

		public override int GetHashCode() {
			return this.ContentTagID.GetHashCode() ^ this.SiteID.GetHashCode();
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
			List<ValidationResult> errors = new List<ValidationResult>();
			List<string> lst = new List<string>();

			if (string.IsNullOrEmpty(this.TagSlug)) {
				ValidationResult err = new ValidationResult("Slug is required", new string[] { "TagSlug" });
				errors.Add(err);
			}
			if (string.IsNullOrEmpty(this.TagText)) {
				ValidationResult err = new ValidationResult("Text is required", new string[] { "TagText" });
				errors.Add(err);
			}

			if (ContentTag.GetSimilar(SiteData.CurrentSite.SiteID, this.ContentTagID, this.TagSlug) > 0) {
				ValidationResult err = new ValidationResult("Slug must be unique", new string[] { "TagSlug" });
				errors.Add(err);
			}

			return errors;
		}

		public ModelStateDictionary ClearOptionalItems(ModelStateDictionary modelState) {
			// these child objects are for display only, and their validation is not needed
			foreach (var ms in modelState.ToArray()) {
				if (ms.Key.ToLowerInvariant().Contains("uri") || ms.Key.ToLowerInvariant().Contains("tagurl")) {
					modelState.Remove(ms.Key);
				}
			}

			return modelState;
		}

		internal ContentTag(vwCarrotTagCounted c) {
			if (c != null) {
				this.ContentTagID = c.ContentTagId;
				this.SiteID = c.SiteId;
				this.TagSlug = ContentPageHelper.ScrubSlug(c.TagSlug);
				this.TagText = c.TagText;
				this.UseCount = c.UseCount;
				this.PublicUseCount = 1;
				this.IsPublic = c.IsPublic;

				SiteData site = SiteData.GetSiteFromCache(c.SiteId);
				if (site != null) {
					this.TagURL = ContentPageHelper.ScrubFilename(c.ContentTagId, string.Format("/{0}/{1}", site.BlogTagPath, c.TagSlug));
				}
			}
		}

		internal ContentTag(vwCarrotTagUrl c) {
			if (c != null) {
				SiteData site = SiteData.GetSiteFromCache(c.SiteId);

				this.ContentTagID = c.ContentTagId;
				this.SiteID = c.SiteId;
				this.TagURL = ContentPageHelper.ScrubFilename(c.ContentTagId, c.TagUrl);
				this.TagText = c.TagText;
				this.UseCount = c.UseCount;
				this.PublicUseCount = c.PublicUseCount;
				this.IsPublic = c.IsPublic;

				if (c.EditDate.HasValue) {
					this.EditDate = site.ConvertUTCToSiteTime(c.EditDate.Value);
				}
			}
		}

		internal ContentTag(CarrotContentTag c) {
			if (c != null) {
				this.ContentTagID = c.ContentTagId;
				this.SiteID = c.SiteId;
				this.TagSlug = ContentPageHelper.ScrubSlug(c.TagSlug);
				this.TagText = c.TagText;
				this.IsPublic = c.IsPublic;
				this.UseCount = 1;
				this.PublicUseCount = 1;

				SiteData site = SiteData.GetSiteFromCache(c.SiteId);
				if (site != null) {
					this.TagURL = ContentPageHelper.ScrubFilename(c.ContentTagId, string.Format("/{0}/{1}", site.BlogTagPath, c.TagSlug));
				}
			}
		}

		public static ContentTag Get(Guid TagID) {
			ContentTag _item = null;
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetContentTagByID(_db, TagID);
				if (query != null) {
					_item = new ContentTag(query);
				}
			}

			return _item;
		}

		public static ContentTag GetByURL(Guid SiteID, string requestedURL) {
			ContentTag _item = null;
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetContentTagByURL(_db, SiteID, requestedURL);
				if (query != null) {
					_item = new ContentTag(query);
				}
			}

			return _item;
		}

		public static int GetSimilar(Guid SiteID, Guid TagID, string tagSlug) {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetContentTagNoMatch(_db, SiteID, TagID, tagSlug);

				return query.Count();
			}
		}

		public static int GetSiteCount(Guid siteID) {
			int iCt = -1;

			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				iCt = CompiledQueries.cqGetContentTagCountBySiteID(_db, siteID);
			}

			return iCt;
		}

		public static List<ContentTag> BuildTagList(Guid rootContentID) {
			List<ContentTag> _types = null;

			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetContentTagByContentID(_db, rootContentID);

				_types = (from d in query.ToList()
						  select new ContentTag(d)).ToList();
			}

			return _types;
		}

		public void Save() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				bool bNew = false;
				var s = CompiledQueries.cqGetContentTagByID(_db, this.ContentTagID);

				if (s == null || (s != null && s.ContentTagId == Guid.Empty)) {
					s = new CarrotContentTag();
					s.ContentTagId = Guid.NewGuid();
					s.SiteId = this.SiteID;
					bNew = true;
				}

				s.TagSlug = ContentPageHelper.ScrubSlug(this.TagSlug);
				s.TagText = this.TagText;
				s.IsPublic = this.IsPublic;

				if (bNew) {
					_db.CarrotContentTags.Add(s);
				}

				_db.SaveChanges();

				this.ContentTagID = s.ContentTagId;
			}
		}

		public void Delete() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var s = CompiledQueries.cqGetContentTagByID(_db, this.ContentTagID);

				if (s != null) {
					_db.CarrotContentTags.Where(x => x.ContentTagId == this.ContentTagID).ExecuteDelete();
					_db.CarrotContentTags.Remove(s);
				}
			}
		}
	}
}