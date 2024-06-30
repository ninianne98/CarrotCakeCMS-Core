using Carrotware.CMS.Data.Models;
using Carrotware.CMS.Interface;

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

	public class ContentPageType : IDisposable {
		private CarrotCakeContext _db = CarrotCakeContext.Create();

		public enum PageType {
			Unknown,
			BlogEntry,
			ContentEntry
		}

		public Guid ContentTypeID { get; set; }
		public string ContentTypeValue { get; set; }

		private static string keyContentPageType = "cms_ContentPageTypeList";

		public static List<ContentPageType> ContentPageTypeList {
			get {
				var types = new List<ContentPageType>();

				bool bCached = false;

				try {
					types = (List<ContentPageType>)CarrotHttpHelper.CacheGet(keyContentPageType);
					if (types != null) {
						bCached = true;
					}
				} catch {
					bCached = false;
				}

				if (!bCached) {
					using (var db = CarrotCakeContext.Create()) {
						var query = CompiledQueries.cqGetContentTypes(db);

						types = (from d in query.ToList()
								 select new ContentPageType {
									 ContentTypeID = d.ContentTypeId,
									 ContentTypeValue = d.ContentTypeValue
								 }).ToList();
					}

					types.Add(new ContentPageType {
						ContentTypeID = Guid.Empty,
						ContentTypeValue = "Unknown"
					});

					CarrotHttpHelper.CacheInsert(keyContentPageType, types, 5);
				}

				return types;
			}
		}

		public static PageType GetTypeByID(Guid contentTypeID) {
			var type = ContentPageTypeList.Where(t => t.ContentTypeID == contentTypeID).FirstOrDefault();

			return GetTypeByName(type.ContentTypeValue);
		}

		public static PageType GetTypeByName(string contentTypeValue) {
			PageType pt = PageType.ContentEntry;

			if (!string.IsNullOrEmpty(contentTypeValue)) {
				try {
					pt = (PageType)Enum.Parse(typeof(PageType), contentTypeValue, true);
				} catch (Exception ex) { }
			}

			return pt;
		}

		public static Guid GetIDByType(PageType contentType) {
			var type = ContentPageTypeList.Where(t => t.ContentTypeValue.ToLowerInvariant() == contentType.ToString().ToLowerInvariant()).FirstOrDefault();

			return type.ContentTypeID;
		}

		#region IDisposable Members

		public void Dispose() {
			if (_db != null) {
				_db.Dispose();
			}
		}

		#endregion IDisposable Members
	}
}