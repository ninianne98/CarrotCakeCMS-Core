using Carrotware.CMS.Data.Models;
using Microsoft.AspNetCore.Html;
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

	public class ContentCategory : IValidatableObject, IMetaDataLinks {

		public ContentCategory() { }

		public Guid ContentCategoryID { get; set; }
		public Guid SiteID { get; set; }

		[Display(Name = "Category")]
		[StringLength(256)]
		public string CategoryText { get; set; }

		[Display(Name = "Slug")]
		[StringLength(256)]
		public string CategorySlug { get; set; }

		[Display(Name = "URL")]
		public string CategoryURL { get; set; }

		public int? UseCount { get; set; }
		public int? PublicUseCount { get; set; }

		[Display(Name = "Public")]
		public bool IsPublic { get; set; }

		public DateTime? EditDate { get; set; }

		public IHtmlContent Text { get { return new HtmlString(this.CategoryText); } }
		public string Uri { get { return this.CategoryURL; } }

		public int Count {
			get {
				if (SecurityData.IsAuthEditor) {
					return this.UseCount ?? 0;
				} else {
					return this.PublicUseCount ?? 0;
				}
			}
		}

		public override bool Equals(object obj) {
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType()) return false;

			if (obj is ContentCategory) {
				ContentCategory p = (ContentCategory)obj;
				return (this.SiteID == p.SiteID && this.ContentCategoryID == p.ContentCategoryID);
			} else {
				return false;
			}
		}

		public override int GetHashCode() {
			return this.ContentCategoryID.GetHashCode() ^ this.SiteID.GetHashCode();
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
			List<ValidationResult> errors = new List<ValidationResult>();
			List<string> lst = new List<string>();

			if (string.IsNullOrEmpty(this.CategorySlug)) {
				ValidationResult err = new ValidationResult("Slug is required", new string[] { "CategorySlug" });
				errors.Add(err);
			}
			if (string.IsNullOrEmpty(this.CategoryText)) {
				ValidationResult err = new ValidationResult("Text is required", new string[] { "CategoryText" });
				errors.Add(err);
			}

			if (ContentCategory.GetSimilar(SiteData.CurrentSite.SiteID, this.ContentCategoryID, this.CategorySlug) > 0) {
				ValidationResult err = new ValidationResult("Slug must be unique", new string[] { "CategorySlug" });
				errors.Add(err);
			}

			return errors;
		}

		internal ContentCategory(vwCarrotCategoryCounted c) {
			if (c != null) {
				this.ContentCategoryID = c.ContentCategoryId;
				this.SiteID = c.SiteId;
				this.CategorySlug = ContentPageHelper.ScrubSlug(c.CategorySlug);
				this.CategoryText = c.CategoryText;
				this.UseCount = c.UseCount;
				this.PublicUseCount = 1;
				this.IsPublic = c.IsPublic;

				SiteData site = SiteData.GetSiteFromCache(c.SiteId);
				if (site != null) {
					this.CategoryURL = ContentPageHelper.ScrubFilename(c.ContentCategoryId, string.Format("/{0}/{1}", site.BlogCategoryPath, c.CategorySlug));
				}
			}
		}

		internal ContentCategory(vwCarrotCategoryUrl c) {
			if (c != null) {
				SiteData site = SiteData.GetSiteFromCache(c.SiteId);

				this.ContentCategoryID = c.ContentCategoryId;
				this.SiteID = c.SiteId;
				this.CategoryURL = ContentPageHelper.ScrubFilename(c.ContentCategoryId, c.CategoryUrl);
				this.CategoryText = c.CategoryText;
				this.UseCount = c.UseCount;
				this.PublicUseCount = c.PublicUseCount;
				this.IsPublic = c.IsPublic;

				if (c.EditDate.HasValue) {
					this.EditDate = site.ConvertUTCToSiteTime(c.EditDate.Value);
				}
			}
		}

		internal ContentCategory(CarrotContentCategory c) {
			if (c != null) {
				this.ContentCategoryID = c.ContentCategoryId;
				this.SiteID = c.SiteId;
				this.CategorySlug = ContentPageHelper.ScrubSlug(c.CategorySlug);
				this.CategoryText = c.CategoryText;
				this.IsPublic = c.IsPublic;
				this.UseCount = 1;
				this.PublicUseCount = 1;

				SiteData site = SiteData.GetSiteFromCache(c.SiteId);
				if (site != null) {
					this.CategoryURL = ContentPageHelper.ScrubFilename(c.ContentCategoryId, string.Format("/{0}/{1}", site.BlogCategoryPath, c.CategorySlug));
				}
			}
		}

		public static ContentCategory Get(Guid CategoryID) {
			ContentCategory _item = null;
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetContentCategoryByID(_db, CategoryID);
				if (query != null) {
					_item = new ContentCategory(query);
				}
			}

			return _item;
		}

		public static ContentCategory GetByURL(Guid SiteID, string requestedURL) {
			ContentCategory _item = null;
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetContentCategoryByURL(_db, SiteID, requestedURL);
				if (query != null) {
					_item = new ContentCategory(query);
				}
			}

			return _item;
		}

		public static int GetSimilar(Guid SiteID, Guid CategoryID, string categorySlug) {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetContentCategoryNoMatch(_db, SiteID, CategoryID, categorySlug);

				return query.Count();
			}
		}

		public static int GetSiteCount(Guid siteID) {
			int iCt = -1;

			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				iCt = CompiledQueries.cqGetContentCategoryCountBySiteID(_db, siteID);
			}

			return iCt;
		}

		public static List<ContentCategory> BuildCategoryList(Guid rootContentID) {
			List<ContentCategory> _types = null;

			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var query = CompiledQueries.cqGetContentCategoryByContentID(_db, rootContentID);

				_types = (from d in query.ToList()
						  select new ContentCategory(d)).ToList();
			}

			return _types;
		}

		public void Save() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				bool bNew = false;
				var s = CompiledQueries.cqGetContentCategoryByID(_db, this.ContentCategoryID);

				if (s == null || (s != null && s.ContentCategoryId == Guid.Empty)) {
					s = new CarrotContentCategory();
					s.ContentCategoryId = Guid.NewGuid();
					s.SiteId = this.SiteID;
					bNew = true;
				}

				s.CategorySlug = ContentPageHelper.ScrubSlug(this.CategorySlug);
				s.CategoryText = this.CategoryText;
				s.IsPublic = this.IsPublic;

				if (bNew) {
					_db.CarrotContentCategories.Add(s);
				}

				_db.SaveChanges();

				this.ContentCategoryID = s.ContentCategoryId;
			}
		}

		public void Delete() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				var s = CompiledQueries.cqGetContentCategoryByID(_db, this.ContentCategoryID);

				if (s != null) {
					_db.CarrotCategoryContentMappings.Where(x => x.ContentCategoryId == this.ContentCategoryID).ExecuteDelete();
					_db.CarrotContentCategories.Remove(s);
					_db.SaveChanges();
				}
			}
		}
	}
}