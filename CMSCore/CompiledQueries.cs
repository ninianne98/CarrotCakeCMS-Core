using Carrotware.CMS.Data.Models;
using Microsoft.EntityFrameworkCore;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, 2024 Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023, June 2024
*/

namespace Carrotware.CMS.Core {

	public static class CompiledQueries {
		//internal static CarrotCakeContext dbConn = new CarrotCakeContext();

		internal static readonly Func<CarrotCakeContext, Guid, Guid, CarrotRootContent?> cqGetRootContentTbl =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID, Guid rootContentID) =>
					  (from r in ctx.CarrotRootContents
					   where r.SiteId == siteID
						   && r.RootContentId == rootContentID
					   select r).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, CarrotSite?> cqGetSiteFromRootContentID =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid rootContentID) =>
					  (from r in ctx.CarrotRootContents
					   join s in ctx.CarrotSites on r.SiteId equals s.SiteId
					   where r.RootContentId == rootContentID
					   select s).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, Guid, CarrotContent?> cqGetLatestContentTbl =
				 EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID, Guid rootContentID) =>
					  (from ct in ctx.CarrotContents
					   join r in ctx.CarrotRootContents on ct.RootContentId equals r.RootContentId
					   where r.SiteId == siteID
						   && r.RootContentId == rootContentID
						   && ct.IsLatestVersion == true
					   orderby ct.EditDate descending
					   select ct).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, Guid, IEnumerable<vwCarrotContent>> cqGetVersionHistory =
				EF.CompileQuery(
				(CarrotCakeContext ctx, Guid siteID, Guid rootContentID) =>
					(from ct in ctx.vwCarrotContents
					 orderby ct.EditDate descending
					 where ct.SiteId == siteID
						&& ct.RootContentId == rootContentID
					 select ct).AsNoTracking());

		internal static readonly Func<CarrotCakeContext, Guid, Guid, vwCarrotContent?> cqGetContentByContentID =
				EF.CompileQuery(
				(CarrotCakeContext ctx, Guid siteID, Guid contentID) =>
					(from ct in ctx.vwCarrotContents
					 orderby ct.EditDate descending
					 where ct.SiteId == siteID
						&& ct.ContentId == contentID
					 select ct).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, int> cqGetMaxOrderID =
				 EF.CompileQuery(
				(CarrotCakeContext ctx, Guid siteID) =>
					(from ct in ctx.vwCarrotContents.Where(c => c.SiteId == siteID && c.IsLatestVersion == true)
					 select ct.NavOrder).Max());

		internal static IEnumerable<vwCarrotContent> TopLevelPages(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				ContentTypeID = ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry),
				ContentType = ContentPageType.PageType.ContentEntry,
				ActiveOnly = bActiveOnly
			};

			return cqTopLevelPages(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqTopLevelPages =
				//EF.CompileQuery(
				(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from ct in ctx.vwCarrotContents
					 orderby ct.NavOrder, ct.NavMenuText
					 where ct.SiteId == sp.SiteID
						 && ct.ParentContentId == null
						 && ct.IsLatestVersion == true
						 && ct.ContentTypeId == sp.ContentTypeID
						 && (ct.PageActive == true || sp.ActiveOnly == false)
						 && (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
						 && (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct).AsNoTracking();// );

		internal static IEnumerable<vwCarrotContent> PostsByDateRange(CarrotCakeContext ctx, Guid siteID, DateTime dateBegin, DateTime dateEnd, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				ContentTypeID = ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry),
				ContentType = ContentPageType.PageType.BlogEntry,
				DateBegin = dateBegin,
				DateEnd = dateEnd,
				ActiveOnly = bActiveOnly
			};

			return cqPostsByDateRange(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqPostsByDateRange =
				// EF.CompileQuery(
				(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from ct in ctx.vwCarrotContents
					 orderby ct.NavOrder, ct.NavMenuText
					 where ct.SiteId == sp.SiteID
						&& ct.IsLatestVersion == true
						&& (ct.GoLiveDate >= sp.DateBegin && ct.GoLiveDate <= sp.DateEnd)
						&& ct.ContentTypeId == sp.ContentTypeID
						&& (ct.PageActive == true || sp.ActiveOnly == false)
						&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
						&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct).AsNoTracking();// );

		internal static vwCarrotContent GetLatestContentByURL(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, string sPage) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly,
				FileName = sPage
			};
			return cqGetLatestContentByURL(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, vwCarrotContent?> cqGetLatestContentByURL =
					// EF.CompileQuery(
					(CarrotCakeContext ctx, SearchParameterObject sp) =>
					  (from ct in ctx.vwCarrotContents
					   where ct.SiteId == sp.SiteID
							&& ct.FileName == sp.FileName
							&& ct.IsLatestVersion == true
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					   orderby ct.EditDate descending
					   select ct).AsNoTracking().FirstOrDefault();// );

		internal static readonly Func<CarrotCakeContext, Guid, DateTime, string, vwCarrotContent?> cqGetLatestContentBySlug =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID, DateTime datePublished, string sPageSlug) =>
					  (from ct in ctx.vwCarrotContents
					   where ct.SiteId == siteID
							&& ct.PageSlug == sPageSlug
							&& (ct.GoLiveDate.Date == datePublished.Date)
							&& ct.IsLatestVersion == true
					   orderby ct.EditDate descending
					   select ct).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, string, vwCarrotCategoryUrl?> cqGetCategoryByURL =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID, string sPage) =>
					  (from ct in ctx.vwCarrotCategoryUrls
					   where ct.SiteId == siteID
							&& ct.CategoryUrl == sPage
					   select ct).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, string, vwCarrotTagUrl?> cqGetTagByURL =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID, string sPage) =>
					  (from ct in ctx.vwCarrotTagUrls
					   where ct.SiteId == siteID
							&& ct.TagUrl == sPage
					   select ct).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, string, vwCarrotEditorUrl?> cqGetEditorByURL =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID, string sPage) =>
					  (from ct in ctx.vwCarrotEditorUrls
					   where ct.SiteId == siteID
							&& ct.UserUrl == sPage
					   select ct).AsNoTracking().FirstOrDefault());

		internal static vwCarrotContent GetLatestContentByID(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, Guid rootContentID) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				RootContentID = rootContentID,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly
			};

			return cqGetLatestContentByID(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, vwCarrotContent?> cqGetLatestContentByID =
					// EF.CompileQuery(
					(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from ct in ctx.vwCarrotContents
					 where ct.SiteId == sp.SiteID
							&& ct.RootContentId == sp.RootContentID
							&& ct.IsLatestVersion == true
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 orderby ct.EditDate descending
					 select ct).AsNoTracking().FirstOrDefault();// );

		internal static IEnumerable<vwCarrotContent> GetLatestContentByParent(CarrotCakeContext ctx, Guid siteID, Guid? parentContentID, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				ParentContentID = parentContentID,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly
			};

			return cqGetLatestContentByParent1(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqGetLatestContentByParent1 =
				// EF.CompileQuery(
				(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from ct in ctx.vwCarrotContents
					 orderby ct.NavOrder, ct.NavMenuText
					 where ct.SiteId == sp.SiteID
							&& ct.ParentContentId == sp.ParentContentID
							&& ct.IsLatestVersion == true
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct).AsNoTracking();// );

		internal static IEnumerable<vwCarrotContent> GetLatestContentByParent(CarrotCakeContext ctx, Guid siteID, string parentPage, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				ParentFileName = parentPage,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly
			};

			return cqGetLatestContentByParent2(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqGetLatestContentByParent2 =
				// EF.CompileQuery(
				(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from ct in ctx.vwCarrotContents
					 join cp in ctx.vwCarrotContentChildren on ct.RootContentId equals cp.RootContentId
					 orderby ct.NavOrder, ct.NavMenuText
					 where ct.SiteId == sp.SiteID
							&& cp.ParentFileName == sp.ParentFileName
							&& ct.IsLatestVersion == true
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct).AsNoTracking();// );

		internal static IEnumerable<vwCarrotContent> GetLatestContentBySibling(CarrotCakeContext ctx, Guid siteID, Guid rootContentID, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				RootContentID = rootContentID,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly
			};

			return cqGetLatestContentBySibling1(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqGetLatestContentBySibling1 =
				// EF.CompileQuery(
				(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from cc1 in ctx.vwCarrotContentChildren
					 join cc2 in ctx.vwCarrotContentChildren on cc1.ParentContentId equals cc2.ParentContentId
					 join ct in ctx.vwCarrotContents on cc2.RootContentId equals ct.RootContentId
					 orderby ct.NavOrder, ct.NavMenuText
					 where ct.SiteId == sp.SiteID
							&& cc1.RootContentId == sp.RootContentID
							&& cc1.SiteId == sp.SiteID
							&& cc2.SiteId == sp.SiteID
							&& ct.IsLatestVersion == true
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct).AsNoTracking();// );

		internal static IEnumerable<vwCarrotContent> GetLatestContentBySibling(CarrotCakeContext ctx, Guid siteID, string sPage, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				FileName = sPage,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly
			};

			return cqGetLatestContentBySibling2(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqGetLatestContentBySibling2 =
				// EF.CompileQuery(
				(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from cc1 in ctx.vwCarrotContentChildren
					 join cc2 in ctx.vwCarrotContentChildren on cc1.ParentContentId equals cc2.ParentContentId
					 join ct in ctx.vwCarrotContents on cc2.RootContentId equals ct.RootContentId
					 orderby ct.NavOrder, ct.NavMenuText
					 where ct.SiteId == sp.SiteID
							&& cc1.FileName == sp.FileName
							&& cc1.SiteId == sp.SiteID
							&& cc2.SiteId == sp.SiteID
							&& ct.IsLatestVersion == true
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct).AsNoTracking();// );

		internal static int GetContentCountByParent(CarrotCakeContext ctx, Guid siteID, Guid? parentContentID, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				ParentContentID = parentContentID,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly
			};

			return cqGetContentCountByParent1(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, int> cqGetContentCountByParent1 =
				// EF.CompileQuery(
				(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from ct in ctx.vwCarrotContents
					 orderby ct.NavOrder, ct.NavMenuText
					 where ct.SiteId == sp.SiteID
							&& ct.ParentContentId == sp.ParentContentID
							&& ct.IsLatestVersion == true
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct).AsNoTracking().Count();// );

		internal static int GetContentCountByParent(CarrotCakeContext ctx, Guid siteID, string parentPage, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				ParentFileName = parentPage,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly
			};

			return cqGetContentCountByParent2(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, int> cqGetContentCountByParent2 =
				// EF.CompileQuery(
				(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from ct in ctx.vwCarrotContents
					 join cp in ctx.vwCarrotContentChildren on ct.RootContentId equals cp.RootContentId
					 orderby ct.NavOrder, ct.NavMenuText
					 where ct.SiteId == sp.SiteID
							&& cp.ParentFileName == sp.ParentFileName
							&& ct.IsLatestVersion == true
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct).AsNoTracking().Count();// );

		internal static IEnumerable<vwCarrotContent> GetLatestContentWithParent(CarrotCakeContext ctx, Guid siteID, Guid? parentContentID, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				ParentContentID = parentContentID,
				ContentTypeID = ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry),
				ContentType = ContentPageType.PageType.ContentEntry,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly
			};

			return cqGetLatestContentWithParent(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqGetLatestContentWithParent =
				// EF.CompileQuery(
				(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from ct in ctx.vwCarrotContents
					 orderby ct.NavOrder, ct.NavMenuText
					 where ct.SiteId == sp.SiteID
							&& (ct.ParentContentId == sp.ParentContentID || ct.RootContentId == sp.ParentContentID)
							&& ct.IsLatestVersion == true
							&& ct.ContentTypeId == sp.ContentTypeID
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct).AsNoTracking();// );

		internal static IEnumerable<vwCarrotContent> ContentNavAll(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				ContentTypeID = ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry),
				ContentType = ContentPageType.PageType.ContentEntry,
				ActiveOnly = bActiveOnly
			};

			return cqContentNavAll(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqContentNavAll =
					// EF.CompileQuery(
					(CarrotCakeContext ctx, SearchParameterObject sp) =>
					  (from ct in ctx.vwCarrotContents
					   orderby ct.NavOrder, ct.NavMenuText
					   where ct.SiteId == sp.SiteID
							&& ct.IsLatestVersion == true
							&& ct.ContentTypeId == sp.ContentID
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					   select ct).AsNoTracking();// );

		internal static IEnumerable<vwCarrotContent> BlogNavAll(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				ContentTypeID = ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry),
				ContentType = ContentPageType.PageType.BlogEntry,
				ActiveOnly = bActiveOnly
			};

			return cqBlogNavAll(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqBlogNavAll =
					// EF.CompileQuery(
					(CarrotCakeContext ctx, SearchParameterObject sp) =>
					  (from ct in ctx.vwCarrotContents
					   orderby ct.NavOrder, ct.NavMenuText
					   where ct.SiteId == sp.SiteID
							&& ct.IsLatestVersion == true
							&& ct.ContentTypeId == sp.ContentTypeID
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					   select ct).AsNoTracking();// );

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<CarrotContent>> cqBlogAllContentTbl =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID) =>
					  (from ct in ctx.CarrotRootContents
					   join c in ctx.CarrotContents on ct.RootContentId equals c.RootContentId
					   where ct.SiteId == siteID
							&& c.IsLatestVersion == true
							&& ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
					   orderby c.EditDate descending
					   select c));

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<CarrotContent>> cqContentAllContentTbl =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID) =>
					  (from ct in ctx.CarrotRootContents
					   join c in ctx.CarrotContents on ct.RootContentId equals c.RootContentId
					   where ct.SiteId == siteID
							&& c.IsLatestVersion == true
							&& ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry)
					   orderby c.EditDate descending
					   select c));

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<CarrotRootContent>> cqBlogAllRootTbl =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID) =>
					  (from ct in ctx.CarrotRootContents
					   where ct.SiteId == siteID
							&& ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
					   select ct));

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<string>> cqBlogDupFileNames =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID) =>
					  (ctx.CarrotRootContents.Where(c => c.SiteId == siteID && c.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry))
						   .GroupBy(g => g.FileName)
						   .Where(g => g.Count() > 1)
						   .Select(g => g.Key)).AsNoTracking());

		internal static readonly Func<CarrotCakeContext, Guid, Guid, string, IEnumerable<CarrotRootContent>> cqGetRootContentListByURLTbl =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID, Guid entryType, string sPage) =>
					  (from r in ctx.CarrotRootContents
					   orderby r.GoLiveDate descending
					   where r.SiteId == siteID
							&& r.ContentTypeId == entryType
							&& r.FileName == sPage
					   select r));

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<vwCarrotContent>> cqGetAllContent =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID) =>
					  (from ct in ctx.vwCarrotContents
					   where ct.SiteId == siteID
							&& ct.IsLatestVersion == true
					   orderby ct.EditDate descending
					   select ct).AsNoTracking());

		internal static readonly Func<CarrotCakeContext, Guid, Guid, string, IEnumerable<vwCarrotContent>> cqGetRootContentListNoMatchByURL =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID, Guid rootContentID, string sPage) =>
					  (from ct in ctx.vwCarrotContents
					   where ct.SiteId == siteID
						   && ct.FileName == sPage
						   && ct.RootContentId != rootContentID
					   select ct).AsNoTracking());

		internal static vwCarrotContent FindHome(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly
			};

			return cqFindHome(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, vwCarrotContent?> cqFindHome =
				// EF.CompileQuery(
				(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from ct in ctx.vwCarrotContents
					 orderby ct.NavOrder ascending
					 where ct.SiteId == sp.SiteID
							&& ct.NavOrder < 1
							&& ct.IsLatestVersion == true
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct).FirstOrDefault();// );

		internal static IEnumerable<vwCarrotContent> GetLatestContentList(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				ContentTypeID = ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry),
				ContentType = ContentPageType.PageType.ContentEntry,
				ActiveOnly = bActiveOnly
			};

			return cqGetLatestContentList(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqGetLatestContentList =
					// EF.CompileQuery(
					(CarrotCakeContext ctx, SearchParameterObject sp) =>
					  (from ct in ctx.vwCarrotContents
					   orderby ct.NavOrder, ct.NavMenuText
					   where ct.SiteId == sp.SiteID
							&& ct.IsLatestVersion == true
							&& ct.ContentTypeId == sp.ContentTypeID
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					   select ct).AsNoTracking();// );

		internal static IEnumerable<vwCarrotContent> GetLatestBlogList(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				ContentTypeID = ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry),
				ContentType = ContentPageType.PageType.BlogEntry,
				ActiveOnly = bActiveOnly
			};

			return cqGetLatestBlogList(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqGetLatestBlogList =
					// EF.CompileQuery(
					(CarrotCakeContext ctx, SearchParameterObject sp) =>
					  (from ct in ctx.vwCarrotContents
					   orderby ct.NavOrder, ct.NavMenuText
					   where ct.SiteId == sp.SiteID
							&& ct.IsLatestVersion == true
							&& ct.ContentTypeId == sp.ContentTypeID
							&& (ct.PageActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					   select ct).AsNoTracking();// );

		//===============================

		internal static readonly Func<CarrotCakeContext, Guid, CarrotContentComment?> cqGetContentCommentsTblByID =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid contentCommentID) =>
					  (from r in ctx.CarrotContentComments
					   orderby r.CreateDate descending
					   where r.ContentCommentId == contentCommentID
					   select r).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, vwCarrotComment?> cqGetContentCommentByID =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid contentCommentID) =>
					  (from r in ctx.vwCarrotComments
					   orderby r.CreateDate descending
					   where r.ContentCommentId == contentCommentID
					   select r).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, Guid, CarrotContentComment?> cqGetContentCommentsTbl =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid rootContentID, Guid contentCommentID) =>
					  (from r in ctx.CarrotContentComments
					   orderby r.CreateDate descending
					   where r.ContentCommentId == contentCommentID
						   && r.RootContentId == rootContentID
					   select r).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<CarrotContentComment>> cqGetContentCommentsListTbl =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid rootContentID) =>
					  (from r in ctx.CarrotContentComments
					   orderby r.CreateDate descending
					   where r.RootContentId == rootContentID
					   select r));

		//===============================

		internal static readonly Func<CarrotCakeContext, Guid, CarrotWidget?> cqGetRootWidget =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid rootWidgetID) =>
				(from r in ctx.CarrotWidgets
				 where r.RootWidgetId == rootWidgetID
				 select r).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, vwCarrotWidget?> cqGetLatestWidget =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid rootWidgetID) =>
				(from r in ctx.vwCarrotWidgets
				 where r.RootWidgetId == rootWidgetID
					&& r.IsLatestVersion == true
				 orderby r.EditDate descending
				 select r).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, vwCarrotWidget?> cqGetWidgetDataByID_VW =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid widgetDataID) =>
				(from r in ctx.vwCarrotWidgets
				 where r.WidgetDataId == widgetDataID
				 select r).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, CarrotWidgetData?> cqGetWidgetDataByID_TBL =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid widgetDataID) =>
				(from r in ctx.CarrotWidgetData
				 where r.WidgetDataId == widgetDataID
				 select r).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, CarrotWidgetData?> cqGetWidgetDataByRootID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid rootWidgetID) =>
				(from r in ctx.CarrotWidgetData
				 where r.RootWidgetId == rootWidgetID
					&& r.IsLatestVersion == true
				 orderby r.EditDate descending
				 select r).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<CarrotWidgetData>> cqGetWidgetDataByRootAll =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid rootWidgetID) =>
				(from r in ctx.CarrotWidgetData
				 where r.RootWidgetId == rootWidgetID
				 orderby r.EditDate descending
				 select r));

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<vwCarrotWidget>> cqGetWidgetVersionHistory_VW =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid rootWidgetID) =>
					  (from r in ctx.vwCarrotWidgets
					   orderby r.EditDate descending
					   where r.RootWidgetId == rootWidgetID
					   select r).AsNoTracking());

		internal static readonly Func<CarrotCakeContext, Guid, bool, IEnumerable<vwCarrotWidget>> cqGetLatestWidgets =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid rootContentID, bool bActiveOnly) =>
					  (from r in ctx.vwCarrotWidgets
					   orderby r.WidgetOrder
					   where r.RootContentId == rootContentID
						  && r.IsLatestVersion == true
						  && (r.WidgetActive == true || bActiveOnly == false)
					   select r).AsNoTracking());

		internal static readonly Func<CarrotCakeContext, SearchParameterObject, CarrotSerialCache> cqGetSerialCacheTbl =
			// EF.CompileQuery(
			(CarrotCakeContext ctx, SearchParameterObject sp) =>
			(from c in ctx.CarrotSerialCache
			 where c.ItemId == sp.ItemID
					&& c.KeyType == sp.KeyType
					&& c.SiteId == sp.SiteID
					&& c.EditUserId == sp.UserId
			 select c).FirstOrDefault();// );

		internal static CarrotSerialCache SearchSeriaCache(CarrotCakeContext ctx, Guid siteID, Guid userID, Guid itemID, string keyType) {
			SearchParameterObject searchParm = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				UserId = userID,
				ItemID = itemID,
				KeyType = keyType
			};

			return cqGetSerialCacheTbl(ctx, searchParm);
		}

		internal static CarrotSerialCache SearchSeriaCache(CarrotCakeContext ctx, Guid itemID, string keyType) {
			return SearchSeriaCache(ctx, SiteData.CurrentSiteID, SecurityData.CurrentUserGuid, itemID, keyType);
		}

		//=====================

		internal static readonly Func<CarrotCakeContext, Guid, CarrotSite> cqGetSiteByID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID) =>
				(from r in ctx.CarrotSites
				 where r.SiteId == siteID
				 select r).FirstOrDefault());

		//=====================

		internal static readonly Func<CarrotCakeContext, string, CarrotContentType> cqGetContentTypeByName =
			EF.CompileQuery(
			(CarrotCakeContext ctx, string contentTypeValue) =>
				(from r in ctx.CarrotContentTypes
				 where r.ContentTypeValue == contentTypeValue
				 select r).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, CarrotContentType> cqGetContentTypeByID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid ContentTypeId) =>
				(from r in ctx.CarrotContentTypes
				 where r.ContentTypeId == ContentTypeId
				 select r).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, IEnumerable<CarrotContentType>> cqGetContentTypes =
			EF.CompileQuery(
			(CarrotCakeContext ctx) =>
				(from r in ctx.CarrotContentTypes
				 select r));

		//==========================

		internal static readonly Func<CarrotCakeContext, Guid, string, vwCarrotTagUrl?> cqGetContentTagByURL =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID, string slugURL) =>
				(from c in ctx.vwCarrotTagUrls
				 where c.SiteId == siteID
					 && c.TagUrl == slugURL
				 select c).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, string, vwCarrotCategoryUrl?> cqGetContentCategoryByURL =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID, string slugURL) =>
				(from c in ctx.vwCarrotCategoryUrls
				 where c.SiteId == siteID
					 && c.CategoryUrl == slugURL
				 select c).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, string, vwCarrotEditorUrl?> cqGetContentEditorURL =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID, string slugURL) =>
				(from c in ctx.vwCarrotEditorUrls
				 where c.SiteId == siteID
					 && c.UserUrl == slugURL
				 select c).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, CarrotContentTag?> cqGetContentTagByID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid ContentTagId) =>
				(from c in ctx.CarrotContentTags
				 where c.ContentTagId == ContentTagId
				 select c).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, CarrotContentCategory?> cqGetContentCategoryByID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid ContentCategoryId) =>
				(from c in ctx.CarrotContentCategories
				 where c.ContentCategoryId == ContentCategoryId
				 select c).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, Guid, vwCarrotEditorUrl?> cqGetContentEditorByID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID, Guid userId) =>
				(from c in ctx.vwCarrotEditorUrls
				 where c.SiteId == siteID
					 && c.UserId == userId
				 select c).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, int> cqGetContentTagCountBySiteID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID) =>
				(from c in ctx.CarrotContentTags
				 where c.SiteId == siteID
				 select c).AsNoTracking().Count());

		internal static readonly Func<CarrotCakeContext, Guid, int> cqGetContentCategoryCountBySiteID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID) =>
				(from c in ctx.CarrotContentCategories
				 where c.SiteId == siteID
				 select c).AsNoTracking().Count());

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<vwCarrotTagCounted>> cqGetContentTagBySiteID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID) =>
				(from c in ctx.vwCarrotTagCounts
				 where c.SiteId == siteID
				 select c).AsNoTracking());

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<vwCarrotCategoryCounted>> cqGetContentCategoryBySiteID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID) =>
				(from c in ctx.vwCarrotCategoryCounts
				 where c.SiteId == siteID
				 select c).AsNoTracking());

		internal static readonly Func<CarrotCakeContext, Guid, Guid, string, IEnumerable<CarrotContentTag>> cqGetContentTagNoMatch =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID, Guid ContentTagId, string slug) =>
				(from r in ctx.CarrotContentTags
				 where r.SiteId == siteID
					&& r.ContentTagId != ContentTagId
					&& r.TagSlug == slug
				 select r));

		internal static readonly Func<CarrotCakeContext, Guid, Guid, string, IEnumerable<CarrotContentCategory>> cqGetContentCategoryNoMatch =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID, Guid ContentCategoryId, string slug) =>
				(from r in ctx.CarrotContentCategories
				 where r.SiteId == siteID
					&& r.ContentCategoryId != ContentCategoryId
					&& r.CategorySlug == slug
				 select r));

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<CarrotContentTag>> cqGetContentTagByContentID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid rootContentID) =>
				(from r in ctx.CarrotContentTags
				 join c in ctx.CarrotTagContentMappings on r.ContentTagId equals c.ContentTagId
				 where c.RootContentId == rootContentID
				 select r));

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<CarrotContentCategory>> cqGetContentCategoryByContentID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid rootContentID) =>
				(from r in ctx.CarrotContentCategories
				 join c in ctx.CarrotCategoryContentMappings on r.ContentCategoryId equals c.ContentCategoryId
				 where c.RootContentId == rootContentID
				 select r));

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<CarrotTagContentMapping>> cqGetContentTagMapByContentID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid rootContentID) =>
				(from r in ctx.CarrotContentTags
				 join c in ctx.CarrotTagContentMappings on r.ContentTagId equals c.ContentTagId
				 where c.RootContentId == rootContentID
				 select c));

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<CarrotCategoryContentMapping>> cqGetContentCategoryMapByContentID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid rootContentID) =>
				(from r in ctx.CarrotContentCategories
				 join c in ctx.CarrotCategoryContentMappings on r.ContentCategoryId equals c.ContentCategoryId
				 where c.RootContentId == rootContentID
				 select c));

		internal static IEnumerable<vwCarrotContent> GetContentByTagURL(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, string sTagURL) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				FileName = sTagURL,
				ActiveOnly = bActiveOnly
			};

			return cqGetContentByTagURL(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqGetContentByTagURL =
			// EF.CompileQuery(
			(CarrotCakeContext ctx, SearchParameterObject sp) =>
				(from r in ctx.vwCarrotTagUrls
				 join m in ctx.CarrotTagContentMappings on r.ContentTagId equals m.ContentTagId
				 join ct in ctx.vwCarrotContents on m.RootContentId equals ct.RootContentId
				 where r.SiteId == sp.SiteID
						&& ct.SiteId == sp.SiteID
						&& r.TagUrl == sp.FileName
						&& (ct.PageActive == true || sp.ActiveOnly == false)
						&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
						&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
						&& ct.IsLatestVersion == true
				 select ct).AsNoTracking();// );

		internal static IEnumerable<vwCarrotContent> GetContentByCategoryURL(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, string sCatURL) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				FileName = sCatURL,
				ActiveOnly = bActiveOnly
			};

			return cqGetContentByCategoryURL(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqGetContentByCategoryURL =
			// EF.CompileQuery(
			(CarrotCakeContext ctx, SearchParameterObject sp) =>
				(from r in ctx.vwCarrotCategoryUrls
				 join m in ctx.CarrotCategoryContentMappings on r.ContentCategoryId equals m.ContentCategoryId
				 join ct in ctx.vwCarrotContents on m.RootContentId equals ct.RootContentId
				 where r.SiteId == sp.SiteID
						&& ct.SiteId == sp.SiteID
						&& r.CategoryUrl == sp.FileName
						&& (ct.PageActive == true || sp.ActiveOnly == false)
						&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
						&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
						&& ct.IsLatestVersion == true
				 select ct).AsNoTracking();// );

		//=====================

		internal static IEnumerable<vwCarrotContent> GetOtherNotPage(CarrotCakeContext ctx, Guid siteID, Guid rootContentID, Guid? parentContentID) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				RootContentID = rootContentID,
				ParentContentID = parentContentID,
				ContentTypeID = ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry),
				DateCompare = DateTime.UtcNow
			};

			return cqGetOtherNotPage(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, IEnumerable<vwCarrotContent>> cqGetOtherNotPage =
					// EF.CompileQuery(
					(CarrotCakeContext ctx, SearchParameterObject sp) =>
					  (from ct in ctx.vwCarrotContents
					   orderby ct.NavOrder, ct.NavMenuText
					   where ct.SiteId == sp.SiteID
							&& ct.IsLatestVersion == true
							&& ct.ContentTypeId == sp.ContentTypeID
							&& ct.RootContentId != sp.RootContentID
							&& (ct.ParentContentId == sp.ParentContentID
								 || (ct.ParentContentId == null && sp.ParentContentID == Guid.Empty))
					   select ct).AsNoTracking();// );

		internal static readonly Func<CarrotCakeContext, Guid, Guid?, IEnumerable<vwCarrotContent>> cqGetLatestContentPages =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID, Guid? rootContentID) =>
					  (from ct in ctx.vwCarrotContents
					   orderby ct.NavOrder, ct.NavMenuText
					   where ct.SiteId == siteID
						   && ct.IsLatestVersion == true
						   && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry)
						   && ct.RootContentId == rootContentID
					   select ct).AsNoTracking());

		internal static readonly Func<CarrotCakeContext, Guid, Guid?, IEnumerable<vwCarrotContent>> cqGetLatestBlogPages =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID, Guid? rootContentID) =>
					  (from ct in ctx.vwCarrotContents
					   orderby ct.NavOrder, ct.NavMenuText
					   where ct.SiteId == siteID
						   && ct.IsLatestVersion == true
						   && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
						   && ct.RootContentId == rootContentID
					   select ct).AsNoTracking());

		internal static vwCarrotContent GetPreviousPost(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, Guid rootContentID) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				RootContentID = rootContentID,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly
			};

			return cqGetPreviousPost(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, vwCarrotContent?> cqGetPreviousPost =
					// EF.CompileQuery(
					(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from ct1 in ctx.vwCarrotContents
					 join ct2 in ctx.vwCarrotContents on ct1.SiteId equals ct2.SiteId
					 orderby ct1.GoLiveDate descending
					 where ct1.SiteId == sp.SiteID
							&& ct2.GoLiveDate >= ct1.GoLiveDate
							&& ct2.ContentTypeId == ct1.ContentTypeId
							&& ct2.RootContentId != ct1.RootContentId
							&& ct2.RootContentId == sp.RootContentID
							&& ct1.IsLatestVersion == true
							&& ct2.IsLatestVersion == true
							&& (ct1.PageActive == true || sp.ActiveOnly == false)
							&& (ct1.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct1.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct1).AsNoTracking().FirstOrDefault();// );

		internal static vwCarrotContent GetNextPost(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, Guid rootContentID) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				RootContentID = rootContentID,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly
			};

			return cqGetNextPost(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, vwCarrotContent?> cqGetNextPost =
					// EF.CompileQuery(
					(CarrotCakeContext ctx, SearchParameterObject sp) =>
					(from ct1 in ctx.vwCarrotContents
					 join ct2 in ctx.vwCarrotContents on ct1.SiteId equals ct2.SiteId
					 orderby ct1.GoLiveDate ascending
					 where ct1.SiteId == sp.SiteID
							&& ct2.GoLiveDate <= ct1.GoLiveDate
							&& ct2.ContentTypeId == ct1.ContentTypeId
							&& ct2.RootContentId != ct1.RootContentId
							&& ct2.RootContentId == sp.RootContentID
							&& ct1.IsLatestVersion == true
							&& ct2.IsLatestVersion == true
							&& (ct1.PageActive == true || sp.ActiveOnly == false)
							&& (ct1.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct1.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					 select ct1).AsNoTracking().FirstOrDefault();// );

		//=========================================

		internal static readonly Func<CarrotCakeContext, Guid, CarrotUserData> cqFindUserTblByID =
				EF.CompileQuery(
				(CarrotCakeContext ctx, Guid UserId) =>
					(from ct in ctx.CarrotUserData
					 where ct.UserId == UserId
					 select ct).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, vwCarrotUserData> cqFindUserByID =
				EF.CompileQuery(
				(CarrotCakeContext ctx, Guid UserId) =>
					(from ct in ctx.vwCarrotUserData
					 where ct.UserId == UserId
					 select ct).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, string, vwCarrotUserData> cqFindUserByName =
				EF.CompileQuery(
				(CarrotCakeContext ctx, string userName) =>
					(from ct in ctx.vwCarrotUserData
					 where ct.UserName == userName
					 select ct).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, string, vwCarrotUserData> cqFindUserByEmail =
				EF.CompileQuery(
				(CarrotCakeContext ctx, string emailAddy) =>
					(from ct in ctx.vwCarrotUserData
					 where ct.Email == emailAddy
					 select ct).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, IEnumerable<vwCarrotUserData>> cqGetUserList =
				EF.CompileQuery(
				(CarrotCakeContext ctx) =>
					(from ct in ctx.vwCarrotUserData
					 select ct).AsNoTracking());

		//======================

		internal static readonly Func<CarrotCakeContext, Guid, CarrotTextWidget> cqTextWidgetByID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid textWidgetID) =>
				(from w in ctx.CarrotTextWidgets
				 where w.TextWidgetId == textWidgetID
				 select w).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<CarrotTextWidget>> cqTextWidgetBySiteID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID) =>
				(from w in ctx.CarrotTextWidgets
				 where w.SiteId == siteID
				 select w));

		//==============

		internal static readonly Func<CarrotCakeContext, Guid, Guid, CarrotRootContentSnippet> cqGetSnippetDataTbl =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID, Guid rootSnippetID) =>
			  (from r in ctx.CarrotRootContentSnippets
			   where r.SiteId == siteID
				   && r.RootContentSnippetId == rootSnippetID
			   select r).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, Guid, CarrotContentSnippet> cqGetLatestSnippetContentTbl =
					EF.CompileQuery(
					(CarrotCakeContext ctx, Guid siteID, Guid rootSnippetID) =>
					  (from ct in ctx.CarrotContentSnippets
					   join r in ctx.CarrotRootContentSnippets on ct.RootContentSnippetId equals r.RootContentSnippetId
					   where r.SiteId == siteID
						   && r.RootContentSnippetId == rootSnippetID
						   && ct.IsLatestVersion == true
					   select ct).FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<vwCarrotContentSnippet>> cqGetSnippetVersionHistory =
				EF.CompileQuery(
				(CarrotCakeContext ctx, Guid rootSnippetID) =>
					(from ct in ctx.vwCarrotContentSnippets
					 orderby ct.EditDate descending
					 where ct.RootContentSnippetId == rootSnippetID
					 select ct).AsNoTracking());

		internal static readonly Func<CarrotCakeContext, Guid, IEnumerable<vwCarrotContentSnippet>> cqGetSnippetsBySiteID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID) =>
				(from ct in ctx.vwCarrotContentSnippets
				 orderby ct.EditDate descending
				 where ct.SiteId == siteID
					&& ct.IsLatestVersion == true
				 select ct).AsNoTracking());

		internal static readonly Func<CarrotCakeContext, Guid, vwCarrotContentSnippet?> cqGetLatestSnippetVersion =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid rootSnippetID) =>
				(from ct in ctx.vwCarrotContentSnippets
				 orderby ct.EditDate descending
				 where ct.RootContentSnippetId == rootSnippetID
					&& ct.IsLatestVersion == true
				 select ct).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, vwCarrotContentSnippet?> cqGetSnippetVersionByID =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid snippetDataID) =>
				(from ct in ctx.vwCarrotContentSnippets
				 where ct.ContentSnippetId == snippetDataID
				 select ct).AsNoTracking().FirstOrDefault());

		internal static readonly Func<CarrotCakeContext, Guid, Guid, string, IEnumerable<vwCarrotContentSnippet>> cqGetContentSnippetNoMatch =
			EF.CompileQuery(
			(CarrotCakeContext ctx, Guid siteID, Guid rootSnippetID, string slug) =>
				(from r in ctx.vwCarrotContentSnippets
				 where r.SiteId == siteID
					&& r.RootContentSnippetId != rootSnippetID
					&& r.ContentSnippetSlug == slug
				 select r).AsNoTracking());

		internal static vwCarrotContentSnippet GetLatestContentSnippetBySlug(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, string sItemSlug) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly,
				ItemSlug = sItemSlug
			};
			return cqGetLatestContentSnippetBySlug(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, vwCarrotContentSnippet?> cqGetLatestContentSnippetBySlug =
					// EF.CompileQuery(
					(CarrotCakeContext ctx, SearchParameterObject sp) =>
					  (from ct in ctx.vwCarrotContentSnippets
					   where ct.SiteId == sp.SiteID
							&& ct.ContentSnippetSlug == sp.ItemSlug
							&& ct.IsLatestVersion == true
							&& (ct.ContentSnippetActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					   select ct).AsNoTracking().FirstOrDefault();// );

		internal static vwCarrotContentSnippet GetLatestContentSnippetByID(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, Guid itemID) {
			SearchParameterObject sp = new SearchParameterObject {
				SiteID = siteID,
				DateCompare = DateTime.UtcNow,
				ActiveOnly = bActiveOnly,
				ItemSlugID = itemID
			};
			return cqGetLatestContentSnippetByID(ctx, sp);
		}

		private static readonly Func<CarrotCakeContext, SearchParameterObject, vwCarrotContentSnippet?> cqGetLatestContentSnippetByID =
					// EF.CompileQuery(
					(CarrotCakeContext ctx, SearchParameterObject sp) =>
					  (from ct in ctx.vwCarrotContentSnippets
					   where ct.SiteId == sp.SiteID
							&& ct.RootContentSnippetId == sp.ItemSlugID
							&& ct.IsLatestVersion == true
							&& (ct.ContentSnippetActive == true || sp.ActiveOnly == false)
							&& (ct.GoLiveDate < sp.DateCompare || sp.ActiveOnly == false)
							&& (ct.RetireDate > sp.DateCompare || sp.ActiveOnly == false)
					   select ct).AsNoTracking().FirstOrDefault();// );
	}
}