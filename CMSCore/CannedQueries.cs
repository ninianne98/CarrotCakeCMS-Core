using Carrotware.CMS.Data.Models;

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

	static internal class CannedQueries {

		internal static IQueryable<vwCarrotContent> GetAllByTypeList(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, ContentPageType.PageType entryType) {
			return (from ct in ctx.vwCarrotContents
					orderby ct.NavOrder, ct.NavMenuText
					where ct.SiteId == siteID
					&& ct.IsLatestVersion == true
					&& (ct.PageActive == true || bActiveOnly == false)
					&& (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
					&& (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
					&& ct.IsLatestVersion == true
					&& ct.ContentTypeId == ContentPageType.GetIDByType(entryType)
					select ct);
		}

		internal static IQueryable<vwCarrotContentSnippet> GetSnippets(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly) {
			return (from ct in ctx.vwCarrotContentSnippets
					orderby ct.ContentSnippetName
					where ct.SiteId == siteID
					&& ct.IsLatestVersion == true
					&& (ct.ContentSnippetActive == true || bActiveOnly == false)
					&& (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
					&& (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
					&& ct.IsLatestVersion == true
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetAllContentList(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.vwCarrotContents
					orderby ct.NavOrder, ct.NavMenuText
					where ct.SiteId == siteID
					 && ct.IsLatestVersion == true
					 && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry)
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetAllBlogList(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.vwCarrotContents
					orderby ct.NavOrder, ct.NavMenuText
					where ct.SiteId == siteID
					 && ct.IsLatestVersion == true
					 && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
					select ct);
		}

		internal static IQueryable<vwCarrotContent> FindPageByTitleAndDate(CarrotCakeContext ctx, Guid siteID, string sTitle, string sFileNameFrag, DateTime dateCreate) {
			return (from ct in ctx.vwCarrotContents
					orderby ct.NavOrder, ct.NavMenuText
					where ct.SiteId == siteID
					 && ct.IsLatestVersion == true
					 && (ct.PageHead == sTitle || ct.TitleBar == sTitle)
					 && ct.FileName.Contains(sFileNameFrag)
					 && ct.CreateDate.Date == dateCreate.Date
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetLatestContentList(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly) {
			return (from ct in ctx.vwCarrotContents
					orderby ct.NavOrder, ct.NavMenuText
					where ct.SiteId == siteID
					 && ct.IsLatestVersion == true
					 && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry)
					 && (ct.PageActive == true || bActiveOnly == false)
					 && (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
					 && (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
					select ct);
		}

		internal static IQueryable<CarrotWidgetData> GetWidgetDataByRootAll(CarrotCakeContext ctx, Guid rootWidgetID) {
			return (from r in ctx.CarrotWidgetData
					where r.RootWidgetId == rootWidgetID
					select r);
		}

		internal static IQueryable<CarrotWidget> GetWidgetsByRootContent(CarrotCakeContext ctx, Guid rootContentID) {
			return (from r in ctx.CarrotWidgets
					where r.RootContentId == rootContentID
					select r);
		}

		internal static IQueryable<vwCarrotContent> GetContentByRoot(CarrotCakeContext ctx, Guid rootContentID) {
			return (from r in ctx.vwCarrotContents
					where r.RootContentId == rootContentID
					select r);
		}

		internal static IQueryable<vwCarrotContent> GetContentByStatusAndDateRange(CarrotCakeContext ctx, Guid siteID, ContentPageType.PageType pageType,
			DateTime dateBegin, DateTime dateEnd, bool? bActive, bool? bSiteMap, bool? bSiteNav, bool? bBlock) {
			Guid gContent = ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry);
			Guid gBlog = ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry);
			Guid contentTypeID = ContentPageType.GetIDByType(pageType);

			return (from ct in ctx.vwCarrotContents
					orderby ct.ContentTypeValue, ct.NavMenuText
					where ct.SiteId == siteID
						&& ct.IsLatestVersion == true
						&& ct.GoLiveDate >= dateBegin
						&& ct.GoLiveDate <= dateEnd
						&& (ct.ContentTypeId == contentTypeID || pageType == ContentPageType.PageType.Unknown)
						&& (ct.PageActive == Convert.ToBoolean(bActive) || bActive == null)
						&& (ct.BlockIndex == Convert.ToBoolean(bBlock) || bBlock == null)
						&& ((ct.ShowInSiteMap == Convert.ToBoolean(bSiteMap) && ct.ContentTypeId == gContent) || bSiteMap == null)
						&& ((ct.ShowInSiteNav == Convert.ToBoolean(bSiteNav) && ct.ContentTypeId == gContent) || bSiteNav == null)
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetLatestBlogListDateRange(CarrotCakeContext ctx, Guid siteID, DateTime dateBegin, DateTime dateEnd, bool bActiveOnly) {
			return (from ct in ctx.vwCarrotContents
					where ct.SiteId == siteID
					 && ct.IsLatestVersion == true
					 && ct.GoLiveDate >= dateBegin
					 && ct.GoLiveDate <= dateEnd
					 && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
					 && (ct.PageActive == true || bActiveOnly == false)
					 && (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
					 && (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetLatestBlogList(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly) {
			return (from ct in ctx.vwCarrotContents
					orderby ct.NavOrder, ct.NavMenuText
					where ct.SiteId == siteID
					 && ct.IsLatestVersion == true
					 && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
					 && (ct.PageActive == true || bActiveOnly == false)
					 && (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
					 && (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
					select ct);
		}

		internal static Dictionary<string, float> GetTemplateCounts(CarrotCakeContext ctx, Guid siteID, ContentPageType.PageType pageType) {
			Guid contentTypeID = ContentPageType.GetIDByType(pageType);

			return (from ct in ctx.vwCarrotContents.Where(c => c.SiteId == siteID && c.ContentTypeId == contentTypeID && c.IsLatestVersion == true)
					group ct by ct.TemplateFile into grp
					orderby grp.Count() descending
					select new KeyValuePair<string, float>(grp.Key, (float)grp.Count()))
					.ToDictionary(t => t.Key, t => t.Value);
		}

		internal static IQueryable<vwCarrotContent> GetContentByTagURL(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, string sTagURL) {
			return (from t in ctx.vwCarrotTagUrls
					join m in ctx.CarrotTagContentMappings on t.ContentTagId equals m.ContentTagId
					join ct in ctx.vwCarrotContents on m.RootContentId equals ct.RootContentId
					where t.SiteId == siteID
						&& ct.SiteId == siteID
						&& t.TagUrl == sTagURL
						&& ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
						&& (ct.PageActive == true || bActiveOnly == false)
						&& (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
						&& (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
						&& ct.IsLatestVersion == true
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetContentByCategoryURL(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, string sCatURL) {
			return (from c in ctx.vwCarrotCategoryUrls
					join m in ctx.CarrotCategoryContentMappings on c.ContentCategoryId equals m.ContentCategoryId
					join ct in ctx.vwCarrotContents on m.RootContentId equals ct.RootContentId
					where c.SiteId == siteID
						&& ct.SiteId == siteID
						&& c.CategoryUrl == sCatURL
						&& ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
						&& (ct.PageActive == true || bActiveOnly == false)
						&& (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
						&& (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
						&& ct.IsLatestVersion == true
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetContentByUserURL(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, string sUserURL) {
			return (from ed in ctx.vwCarrotEditorUrls
					join ct in ctx.vwCarrotContents on ed.SiteId equals ct.SiteId
					where ed.SiteId == siteID
						&& ct.SiteId == siteID
						&& ed.UserUrl == sUserURL
						&& ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
						&& (ct.PageActive == true || bActiveOnly == false)
						&& (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
						&& (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
						&& ct.IsLatestVersion == true
						&& ((ed.UserId == ct.EditUserId && ct.CreditUserId == null)
									|| (ed.UserId == ct.CreditUserId && ct.CreditUserId != null))
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetContentByCategoryIDs(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, List<Guid> lstCategories) {
			return GetContentByCategoryIDs(ctx, siteID, bActiveOnly, lstCategories, new List<string>());
		}

		internal static IQueryable<vwCarrotContent> GetContentByCategoryIDs(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, List<Guid> lstCategoryGUIDs, List<string> lstCategorySlugs) {
			if (lstCategoryGUIDs == null) {
				lstCategoryGUIDs = new List<Guid>();
			}
			if (lstCategorySlugs == null) {
				lstCategorySlugs = new List<string>();
			}

			return (from ct in ctx.vwCarrotContents
					where ct.SiteId == siteID
						&& ((from m in ctx.CarrotCategoryContentMappings
							 join cc in ctx.CarrotContentCategories on m.ContentCategoryId equals cc.ContentCategoryId
							 where cc.SiteId == siteID
									&& lstCategorySlugs.Contains(cc.CategorySlug)
							 select m.RootContentId).Contains(ct.RootContentId)
						|| (from m in ctx.CarrotCategoryContentMappings
							where lstCategoryGUIDs.Contains(m.ContentCategoryId)
							select m.RootContentId).Contains(ct.RootContentId))
						&& ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
						&& (ct.PageActive == true || bActiveOnly == false)
						&& (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
						&& (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
						&& ct.IsLatestVersion == true
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetContentSiteSearch(CarrotCakeContext ctx, Guid siteID, bool bActiveOnly, string searchTerm) {
			return (from ct in ctx.vwCarrotContents
					where ct.SiteId == siteID
						&& (ct.PageText.Contains(searchTerm)
								|| ct.LeftPageText.Contains(searchTerm)
								|| ct.RightPageText.Contains(searchTerm)
								|| ct.TitleBar.Contains(searchTerm)
								|| ct.MetaDescription.Contains(searchTerm)
								|| ct.MetaKeyword.Contains(searchTerm)
							)
						&& (ct.PageActive == true || bActiveOnly == false)
						&& (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
						&& (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
						&& ct.IsLatestVersion == true
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetContentByParent(CarrotCakeContext ctx, Guid siteID, Guid? parentContentID, bool bActiveOnly) {
			return (from ct in ctx.vwCarrotContents
					orderby ct.NavOrder, ct.NavMenuText
					where ct.SiteId == siteID
						   && ct.ParentContentId == parentContentID
						   && ct.IsLatestVersion == true
						   && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry)
						   && (ct.PageActive == true || bActiveOnly == false)
						   && (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
						   && (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetLatestContentByParent(CarrotCakeContext ctx, Guid siteID, Guid? parentContentID, bool bActiveOnly) {
			return (from ct in ctx.vwCarrotContents
					orderby ct.NavOrder, ct.NavMenuText
					where ct.SiteId == siteID
						   && ct.ParentContentId == parentContentID
						   && ct.ParentContentId != null
						   && ct.IsLatestVersion == true
						   && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry)
						   && (ct.PageActive == true || bActiveOnly == false)
						   && (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
						   && (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
					select ct);
		}

		internal static IQueryable<vwCarrotContent> GetLatestContentByParent(CarrotCakeContext ctx, Guid siteID, string parentPage, bool bActiveOnly) {
			return (from ct in ctx.vwCarrotContents
					join cp in ctx.vwCarrotContentChildren on ct.RootContentId equals cp.RootContentId
					orderby ct.NavOrder, ct.NavMenuText
					where ct.SiteId == siteID
						   && cp.ParentFileName == parentPage
						   && ct.IsLatestVersion == true
						   && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry)
						   && (ct.PageActive == true || bActiveOnly == false)
						   && (ct.GoLiveDate < DateTime.UtcNow || bActiveOnly == false)
						   && (ct.RetireDate > DateTime.UtcNow || bActiveOnly == false)
					select ct);
		}

		internal static IQueryable<CarrotCategoryContentMapping> GetContentCategoryMapByContentID(CarrotCakeContext ctx, Guid rootContentID) {
			return (from r in ctx.CarrotContentCategories
					join c in ctx.CarrotCategoryContentMappings on r.ContentCategoryId equals c.ContentCategoryId
					where c.RootContentId == rootContentID
					select c);
		}

		internal static IQueryable<CarrotTagContentMapping> GetContentTagMapByContentID(CarrotCakeContext ctx, Guid rootContentID) {
			return (from r in ctx.CarrotContentTags
					join c in ctx.CarrotTagContentMappings on r.ContentTagId equals c.ContentTagId
					where c.RootContentId == rootContentID
					select c);
		}

		internal static IQueryable<CarrotContent> GetBlogAllContentTbl(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.CarrotRootContents
					join c in ctx.CarrotContents on ct.RootContentId equals c.RootContentId
					where ct.SiteId == siteID
					where c.IsLatestVersion == true
						 && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
					select c);
		}

		internal static IQueryable<CarrotRootContent> GetBlogAllRootTbl(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.CarrotRootContents
					where ct.SiteId == siteID
					where ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
					select ct);
		}

		internal static IQueryable<CarrotRootContent> GetContentAllRootTbl(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.CarrotRootContents
					where ct.SiteId == siteID
					where ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry)
					select ct);
		}

		internal static IQueryable<CarrotRootContent> GetAllRootTbl(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.CarrotRootContents
					where ct.SiteId == siteID
					select ct);
		}

		internal static IQueryable<DateTime> GetAllDates(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.CarrotRootContents
					where ct.SiteId == siteID
					select ct.GoLiveDate).Distinct();
		}

		internal static IQueryable<DateTime> GetAllDatesByType(CarrotCakeContext ctx, Guid siteID, ContentPageType.PageType pageType) {
			var pageTypeId = ContentPageType.GetIDByType(pageType);

			return (from ct in ctx.CarrotRootContents
					where ct.SiteId == siteID && ct.ContentTypeId == pageTypeId
					select ct.GoLiveDate).Distinct();
		}

		internal static IQueryable<CarrotContent> GetContentAllContentTbl(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.CarrotRootContents
					join c in ctx.CarrotContents on ct.RootContentId equals c.RootContentId
					where ct.SiteId == siteID
					where c.IsLatestVersion == true
						 && ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry)
					select c);
		}

		internal static IQueryable<CarrotContent> GetContentTopContentTbl(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.CarrotRootContents
					join c in ctx.CarrotContents on ct.RootContentId equals c.RootContentId
					where ct.SiteId == siteID
					where c.IsLatestVersion == true
						&& c.ParentContentId == null
						&& ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry)
					select c);
		}

		internal static IQueryable<CarrotContent> GetContentSubContentTbl(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.CarrotRootContents
					join c in ctx.CarrotContents on ct.RootContentId equals c.RootContentId
					where ct.SiteId == siteID
					where c.IsLatestVersion == true
						&& c.ParentContentId != null
						&& ct.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry)
					select c);
		}

		internal static IQueryable<vwCarrotCategoryUrl> GetCategoryURLs(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.vwCarrotCategoryUrls
					where ct.SiteId == siteID
					select ct);
		}

		internal static IQueryable<vwCarrotTagUrl> GetTagURLs(CarrotCakeContext ctx, Guid siteID) {
			return (from ct in ctx.vwCarrotTagUrls
					where ct.SiteId == siteID
					select ct);
		}

		internal static IQueryable<vwCarrotCategoryUrl> GetPostCategoryURL(CarrotCakeContext ctx, Guid siteID, string urlFileName) {
			return (from ct in ctx.vwCarrotCategoryUrls
					join m in ctx.CarrotCategoryContentMappings on ct.ContentCategoryId equals m.ContentCategoryId
					join c in ctx.CarrotRootContents on m.RootContentId equals c.RootContentId
					where c.FileName == urlFileName
						&& ct.SiteId == siteID
					select ct);
		}

		internal static IQueryable<vwCarrotTagUrl> GetPostTagURLs(CarrotCakeContext ctx, Guid siteID, string urlFileName) {
			return (from ct in ctx.vwCarrotTagUrls
					join m in ctx.CarrotTagContentMappings on ct.ContentTagId equals m.ContentTagId
					join c in ctx.CarrotRootContents on m.RootContentId equals c.RootContentId
					where c.FileName == urlFileName
						&& ct.SiteId == siteID
					select ct);
		}

		internal static IQueryable<vwCarrotCategoryUrl> GetPostCategoryURL(CarrotCakeContext ctx, Guid siteID, Guid rootContentID) {
			return (from ct in ctx.vwCarrotCategoryUrls
					join m in ctx.CarrotCategoryContentMappings on ct.ContentCategoryId equals m.ContentCategoryId
					where m.RootContentId == rootContentID
						&& ct.SiteId == siteID
					select ct);
		}

		internal static IQueryable<vwCarrotTagUrl> GetPostTagURLs(CarrotCakeContext ctx, Guid siteID, Guid rootContentID) {
			return (from ct in ctx.vwCarrotTagUrls
					join m in ctx.CarrotTagContentMappings on ct.ContentTagId equals m.ContentTagId
					where m.RootContentId == rootContentID
						&& ct.SiteId == siteID
					select ct);
		}

		internal static IQueryable<vwCarrotComment> GetSiteContentComments(CarrotCakeContext ctx, Guid siteID) {
			return (from r in ctx.vwCarrotComments
					orderby r.CreateDate descending
					where r.SiteId == siteID
					select r);
		}

		internal static IQueryable<vwCarrotComment> GetContentPageComments(CarrotCakeContext ctx, Guid rootContentID, bool bActiveOnly) {
			return (from r in ctx.vwCarrotComments
					orderby r.CreateDate descending
					where r.RootContentId == rootContentID
						&& (r.IsApproved == true || bActiveOnly == false)
					select r);
		}

		internal static IQueryable<vwCarrotComment> GetContentPageComments(CarrotCakeContext ctx, Guid rootContentID, bool? approved, bool? spam) {
			return (from r in ctx.vwCarrotComments
					orderby r.CreateDate descending
					where r.RootContentId == rootContentID
						   && (spam == null || r.IsSpam == spam)
						   && (approved == null || r.IsApproved == approved)
					select r);
		}

		internal static IQueryable<vwCarrotComment> FindCommentsByDate(CarrotCakeContext ctx, Guid siteID, Guid rootContentID, DateTime postDate, string postIP, string sCommentText) {
			return (from r in ctx.vwCarrotComments
					orderby r.CreateDate descending
					where r.SiteId == siteID
						&& r.RootContentId == rootContentID
						&& r.CreateDate.Date == postDate.Date
						&& r.CommenterIp == postIP
						&& r.PostComment.Trim() == sCommentText.Trim()
					select r);
		}

		internal static IQueryable<vwCarrotComment> FindCommentsByDate(CarrotCakeContext ctx, Guid siteID, Guid rootContentID, DateTime postDate, string postIP) {
			return (from r in ctx.vwCarrotComments
					orderby r.CreateDate descending
					where r.SiteId == siteID
						&& r.RootContentId == rootContentID
						&& r.CreateDate.Date == postDate.Date
						&& r.CreateDate.Hour == postDate.Hour
						&& r.CreateDate.Minute == postDate.Minute
						&& r.CommenterIp == postIP
					select r);
		}

		internal static IQueryable<vwCarrotComment> GetSiteContentCommentsByPostType(CarrotCakeContext ctx, Guid siteID, ContentPageType.PageType contentEntry) {
			return (from r in ctx.vwCarrotComments
					orderby r.CreateDate descending
					where r.SiteId == siteID
						&& r.ContentTypeId == ContentPageType.GetIDByType(contentEntry)
					select r);
		}

		internal static IQueryable<vwCarrotComment> GetSiteContentCommentsByPostType(CarrotCakeContext ctx, Guid siteID, ContentPageType.PageType contentEntry, bool? approved, bool? spam) {
			return (from r in ctx.vwCarrotComments
					orderby r.CreateDate descending
					where r.SiteId == siteID
						   && (spam == null || r.IsSpam == spam)
						   && (approved == null || r.IsApproved == approved)
						&& r.ContentTypeId == ContentPageType.GetIDByType(contentEntry)
					select r);
		}
	}
}