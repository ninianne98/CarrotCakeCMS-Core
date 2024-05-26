﻿using Carrotware.CMS.Data.Models;
using Carrotware.CMS.Interface;
using Carrotware.Web.UI.Components;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Security;
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

	[SecuritySafeCritical]
	public partial class SiteData {

		public static string DefaultPageTitlePattern {
			get {
				return "[[CARROT_SITE_NAME]] - [[CARROT_PAGE_TITLEBAR]]";
			}
		}

		public static string CurrentTitlePattern {
			get {
				string pattern = "{0} - {1}";
				SiteData s = CurrentSite;
				if (!string.IsNullOrEmpty(s.SiteTitlebarPattern)) {
					StringBuilder sb = new StringBuilder(s.SiteTitlebarPattern);
					sb.Replace("[[CARROT_SITENAME]]", "{0}");
					sb.Replace("[[CARROT_SITE_NAME]]", "{0}");
					sb.Replace("[[CARROT_SITE_SLOGAN]]", "{1}");
					sb.Replace("[[CARROT_PAGE_TITLEBAR]]", "{2}");
					sb.Replace("[[CARROT_PAGE_PAGEHEAD]]", "{3}");
					sb.Replace("[[CARROT_PAGE_NAVMENUTEXT]]", "{4}");
					sb.Replace("[[CARROT_PAGE_DATE_GOLIVE]]", "{5}");
					sb.Replace("[[CARROT_PAGE_DATE_EDIT]]", "{6}");

					// [[CARROT_SITE_NAME]]: [[CARROT_PAGE_TITLEBAR]] ([[CARROT_PAGE_DATE_GOLIVE:MMMM d, yyyy]])
					var p5 = ParsePlaceholder(s.SiteTitlebarPattern, "[[CARROT_PAGE_DATE_GOLIVE:*]]", 5);
					if (!string.IsNullOrEmpty(p5.Key)) {
						sb.Replace(p5.Key, p5.Value);
					}

					// [[CARROT_SITE_NAME]]: [[CARROT_PAGE_TITLEBAR]] ([[CARROT_PAGE_DATE_EDIT:MMMM d, yyyy]])
					var p6 = ParsePlaceholder(s.SiteTitlebarPattern, "[[CARROT_PAGE_DATE_EDIT:*]]", 6);
					if (!string.IsNullOrEmpty(p6.Key)) {
						sb.Replace(p6.Key, p6.Value);
					}

					pattern = sb.ToString();
				}

				return pattern;
			}
		}

		private static KeyValuePair<string, string> ParsePlaceholder(string titleString, string placeHolder, int posNum) {
			var pair = new KeyValuePair<string, string>(string.Empty, string.Empty);

			if (placeHolder.Contains(":")) {
				string fragTest = placeHolder.Substring(0, placeHolder.IndexOf(":") + 1);

				string formatPattern = string.Format("{{{0}}}", posNum);

				if (titleString.Contains(fragTest)) {
					int idx1 = titleString.IndexOf(fragTest);
					int idx2 = titleString.IndexOf("]]", idx1 + 4);
					int len = idx2 - idx1 - fragTest.Length;

					if (idx1 >= 0 && idx2 > 0 && titleString.Contains(fragTest)) {
						string format = "M/d/yyyy"; // default date format

						if (len > 0) {
							format = titleString.Substring(idx1 + fragTest.Length, len);
						}
						placeHolder = placeHolder.Replace("*", format);

						formatPattern = string.Format("{{{0}:{1}}}", posNum, format);
						pair = new KeyValuePair<string, string>(placeHolder, formatPattern);
					}
				}
			}

			return pair;
		}

		public static List<SiteData> GetSiteList() {
			using (var db = CarrotCakeContext.Create()) {
				return (from l in db.CarrotSites orderby l.SiteName select new SiteData(l)).ToList();
			}
		}

		public static SiteData GetSiteByID(Guid siteID) {
			using (var db = CarrotCakeContext.Create()) {
				var s = CompiledQueries.cqGetSiteByID(db, siteID);

				if (s != null) {
#if DEBUG
					Debug.WriteLine(" ================ " + DateTime.UtcNow.ToString() + " ================");
					Debug.WriteLine("Grabbed site : GetSiteByID(Guid siteID) " + siteID.ToString());
#endif
					return new SiteData(s);
				} else {
					return null;
				}
			}
		}

		public static int BlogSortOrderNumber { get { return 10; } }

		public static bool IsCurrentLikelyHomePage {
			get {
				return IsLikelyHomePage(CurrentScriptName);
			}
		}

		public static bool IsLikelyHomePage(string filePath) {
			if (filePath == null) {
				return false;
			}

			return string.Format("{0}", filePath).Length <= 1
				|| filePath.ToLowerInvariant() == SiteData.DefaultDirectoryFilename;
		}

		public static bool IsLikelyFakeSearch() {
			if (CurrentSite == null) {
				return false;
			}
			// no blog index is set, but the URL looks like a search is happening
			return !CurrentSite.Blog_Root_ContentID.HasValue
						&& (CurrentSite.IsBlogDateFolderPath
								|| CurrentSite.IsBlogCategoryPath
								|| CurrentSite.IsBlogTagPath
								|| CurrentSite.IsBlogEditorFolderPath
								|| CurrentSite.IsSiteSearchPath);
		}

		public static bool IsLikelySearch() {
			if (CurrentSite == null) {
				return false;
			}

			return CurrentSite.Blog_Root_ContentID.HasValue
						&& (CurrentSite.IsBlogDateFolderPath
								|| CurrentSite.IsBlogCategoryPath
								|| CurrentSite.IsBlogTagPath
								|| CurrentSite.IsBlogEditorFolderPath
								|| CurrentSite.IsSiteSearchPath);
		}

		private static string SiteKeyPrefix = "cms_SiteData_";

		public static void RemoveSiteFromCache(Guid siteID) {
			string ContentKey = SiteKeyPrefix + siteID.ToString();
			try {
				CarrotHttpHelper.CacheRemove(ContentKey);
			} catch { }
		}

		public static SiteData GetSiteFromCache(Guid siteID) {
			string ContentKey = SiteKeyPrefix + siteID.ToString();
			SiteData currentSite = null;
			try { currentSite = (SiteData)CarrotHttpHelper.CacheGet(ContentKey); } catch { }
			if (currentSite == null) {
				currentSite = GetSiteByID(siteID);
				if (currentSite != null) {
					CarrotHttpHelper.CacheInsert(ContentKey, currentSite, 5);
				} else {
					CarrotHttpHelper.CacheRemove(ContentKey);
				}
			}
			return currentSite;
		}

		public static SiteData CurrentSite {
			get {
				return GetSiteFromCache(CurrentSiteID);
			}
			set {
				string ContentKey = SiteKeyPrefix + CurrentSiteID.ToString();
				if (value == null) {
					CarrotHttpHelper.CacheRemove(ContentKey);
				} else {
					CarrotHttpHelper.CacheInsert(ContentKey, value, 5);
				}
			}
		}

		public static bool CurrentSiteExists {
			get {
				return (CarrotHttpHelper.HttpContext != null && CurrentSite != null) ? true : false;
			}
		}

		public static bool IsUniqueFilename(string theFileName, Guid pageId) {
			try {
				if (theFileName.Length < 2) {
					return false;
				}

				theFileName = ContentPageHelper.ScrubFilename(pageId, theFileName);
				theFileName = theFileName.ToLowerInvariant();

				if (SiteData.IsPageSpecial(theFileName) || SiteData.IsLikelyHomePage(theFileName)) {
					return false;
				}

				if (SiteData.CurrentSite.GetSpecialFilePathPrefixes().Where(x => theFileName.StartsWith(x.ToLowerInvariant())).Any()
							|| theFileName.StartsWith(SiteData.CurrentSite.BlogFolderPath.ToLowerInvariant())) {
					return false;
				}

				using (var pageHelper = new ContentPageHelper()) {
					ContentPage fn = pageHelper.FindByFilename(SiteData.CurrentSite.SiteID, theFileName);
					ContentPage cp = pageHelper.FindContentByID(SiteData.CurrentSite.SiteID, pageId);

					if (cp == null && pageId != Guid.Empty) {
						cp = pageHelper.GetVersion(SiteData.CurrentSite.SiteID, pageId);
					}

					if (fn == null || (fn != null && cp != null && fn.Root_ContentID == cp.Root_ContentID)) {
						return true;
					} else {
						return false;
					}
				}
			} catch (Exception ex) {
				SiteData.WriteDebugException("isuniquefilename", ex);

				throw;
			}
		}

		public static bool IsUniqueBlogFilename(string pageSlug, DateTime dateGoLive, Guid pageId) {
			try {
				if (pageSlug.Length < 2) {
					return false;
				}

				DateTime dateOrigGoLive = DateTime.MinValue;

				pageSlug = ContentPageHelper.ScrubFilename(pageId, pageSlug);
				pageSlug = pageSlug.ToLowerInvariant();

				string theFileName = pageSlug;

				using (var pageHelper = new ContentPageHelper()) {
					ContentPage cp = pageHelper.FindContentByID(SiteData.CurrentSite.SiteID, pageId);

					if (cp != null) {
						dateOrigGoLive = cp.GoLiveDate;
					}
					if (cp == null && pageId != Guid.Empty) {
						ContentPageExport cpe = ContentImportExportUtils.GetSerializedContentPageExport(pageId);
						if (cpe != null) {
							dateOrigGoLive = cpe.ThePage.GoLiveDate;
						}
					}

					theFileName = ContentPageHelper.CreateFileNameFromSlug(SiteData.CurrentSite, dateGoLive, pageSlug);

					if (SiteData.IsPageSpecial(theFileName) || SiteData.IsLikelyHomePage(theFileName)) {
						return false;
					}

					ContentPage fn1 = pageHelper.FindByFilename(SiteData.CurrentSite.SiteID, theFileName);

					if (cp == null && pageId != Guid.Empty) {
						cp = pageHelper.GetVersion(SiteData.CurrentSite.SiteID, pageId);
					}

					if (fn1 == null || (fn1 != null && cp != null && fn1.Root_ContentID == cp.Root_ContentID)) {
						return true;
					} else {
						return false;
					}
				}
			} catch (Exception ex) {
				SiteData.WriteDebugException("isuniqueblogfilename", ex);

				throw;
			}
		}

		public static string GenerateNewFilename(Guid pageId, string pageTitle, DateTime goLiveDate,
					ContentPageType.PageType pageType) {
			try {
				if (string.IsNullOrEmpty(pageTitle)) {
					pageTitle = pageId.ToString();
				}
				pageTitle = pageTitle.Replace("/", "-");
				string theFileName = ContentPageHelper.ScrubFilename(pageId, pageTitle);
				string testFile = string.Empty;

				if (pageType == ContentPageType.PageType.ContentEntry) {
					var resp = IsUniqueFilename(theFileName, pageId);
					if (resp == false) {
						for (int i = 1; i < 2500; i++) {
							testFile = string.Format("{0}-{1}", pageTitle, i);
							resp = IsUniqueFilename(testFile, pageId);
							if (resp) {
								theFileName = testFile;
								break;
							} else {
								theFileName = string.Empty;
							}
						}
					}
				} else {
					var resp = IsUniqueBlogFilename(theFileName, goLiveDate, pageId);
					if (resp == false) {
						for (int i = 1; i < 2500; i++) {
							testFile = string.Format("{0}-{1}", pageTitle, i);
							resp = IsUniqueBlogFilename(testFile, goLiveDate, pageId);
							if (resp) {
								theFileName = testFile;
								break;
							} else {
								theFileName = string.Empty;
							}
						}
					}
				}

				return ContentPageHelper.ScrubFilename(pageId, theFileName).ToLowerInvariant();
			} catch (Exception ex) {
				SiteData.WriteDebugException("generatenewfilename", ex);
				throw;
			}
		}

		public static ContentPage GetCurrentPage() {
			ContentPage pageContents = null;

			using (var cmsHelper = new CMSConfigHelper()) {
				if (SecurityData.AdvancedEditMode) {
					if (cmsHelper.cmsAdminContent == null) {
						pageContents = GetCurrentLivePage();
						pageContents.LoadAttributes();
						cmsHelper.cmsAdminContent = pageContents;
					} else {
						pageContents = cmsHelper.cmsAdminContent;
					}
				} else {
					pageContents = GetCurrentLivePage();

					if (pageContents == null && !SiteData.CurrentSiteExists) {
						pageContents = ContentPageHelper.GetEmptyHome();
					}
					if (SecurityData.CurrentUserGuid != Guid.Empty) {
						cmsHelper.cmsAdminContent = null;
					}
				}
			}

			return pageContents;
		}

		public static ContentPage GetPage(string currentPage) {
			ContentPage pageContents = null;

			using (var pageHelper = new ContentPageHelper()) {
				var requireActivePage = !(SecurityData.IsAdmin || SecurityData.IsSiteEditor);

				if (SiteData.IsLikelyHomePage(currentPage)) {
					pageContents = pageHelper.FindHome(SiteData.CurrentSiteID, requireActivePage);
				} else {
					pageContents = pageHelper.GetLatestContentByURL(SiteData.CurrentSiteID, requireActivePage, currentPage);
				}
			}

			return pageContents;
		}

		public static ContentPage GetPage(Guid guidContentID) {
			ContentPage pageContents = null;
			using (ContentPageHelper pageHelper = new ContentPageHelper()) {
				pageContents = pageHelper.FindContentByID(CurrentSiteID, guidContentID);
			}
			return pageContents;
		}

		public static List<Widget> GetCurrentPageWidgets(Guid guidContentID) {
			List<Widget> pageWidgets = new List<Widget>();

			using (CMSConfigHelper cmsHelper = new CMSConfigHelper()) {
				if (SecurityData.AdvancedEditMode) {
					if (cmsHelper.cmsAdminWidget == null) {
						pageWidgets = GetCurrentPageLiveWidgets(guidContentID);
						cmsHelper.cmsAdminWidget = (from w in pageWidgets
													orderby w.WidgetOrder, w.EditDate
													select w).ToList();
					} else {
						pageWidgets = (from w in cmsHelper.cmsAdminWidget
									   orderby w.WidgetOrder, w.EditDate
									   select w).ToList();
					}
				} else {
					pageWidgets = GetCurrentPageLiveWidgets(guidContentID);
					if (SecurityData.CurrentUserGuid != Guid.Empty) {
						cmsHelper.cmsAdminWidget = null;
					}
				}
			}

			return pageWidgets;
		}

		public static ContentPage GetCurrentLivePage() {
			ContentPage pageContents = null;

			using (var pageHelper = new ContentPageHelper()) {
				bool isPageTemplate = false;
				string currentPage = SiteData.CurrentScriptName;
				string scrubbedURL = SiteData.AlternateCurrentScriptName;

				if (scrubbedURL.ToLowerInvariant() != currentPage.ToLowerInvariant()) {
					currentPage = scrubbedURL;
				}

				var requireActivePage = !(SecurityData.IsAdmin || SecurityData.IsSiteEditor);

				// if the route has found the page ID just use it
				if (SiteData.CurrentRoutePageID.HasValue) {
					pageContents = pageHelper.FindContentByID(SiteData.CurrentSiteID, requireActivePage, SiteData.CurrentRoutePageID.Value);
				} else {
					if (SiteData.IsLikelyHomePage(currentPage)) {
						pageContents = pageHelper.FindHome(SiteData.CurrentSiteID, requireActivePage);
					} else {
						pageContents = pageHelper.GetLatestContentByURL(SiteData.CurrentSiteID, requireActivePage, currentPage);
					}
				}

				if (pageContents == null && SiteData.IsPageReal) {
					isPageTemplate = true;
				}

				if ((SiteData.IsPageSampler || isPageTemplate) && pageContents == null) {
					pageContents = ContentPageHelper.GetSamplerView();
				}

				if (pageContents == null && SiteData.IsLikelyFakeSearch()) {
					pageContents = ContentPageHelper.GetEmptySearch();
				}

				if (isPageTemplate) {
					pageContents.TemplateFile = currentPage;
				}
			}

			return pageContents;
		}

		public static List<Widget> GetCurrentPageLiveWidgets(Guid guidContentID) {
			List<Widget> pageWidgets = new List<Widget>();

			using (WidgetHelper widgetHelper = new WidgetHelper()) {
				pageWidgets = widgetHelper.GetWidgets(guidContentID, !SecurityData.AdvancedEditMode);
			}

			return pageWidgets;
		}

		public static Guid CurrentSiteID {
			get {
				Guid _site = CMSConfigHelper.PrimarySiteID;
				if (_site == Guid.Empty) {
					try {
						DynamicSite s = CMSConfigHelper.DynSite;
						if (s != null) {
							_site = s.SiteID;
						}
					} catch { }
				}
				return _site;
			}
		}

		public static SiteData InitNewSite(Guid siteID) {
			SiteData site = new SiteData();
			site.SiteID = siteID;
			site.BlockIndex = true;

			site.MainURL = "http://" + CMSConfigHelper.DomainName;
			site.SiteName = CMSConfigHelper.DomainName;

			site.SiteTitlebarPattern = SiteData.DefaultPageTitlePattern;

			site.Blog_FolderPath = "archive";
			site.Blog_CategoryPath = "category";
			site.Blog_TagPath = "tag";
			site.Blog_DatePath = "date";
			site.Blog_EditorPath = "author";
			site.Blog_DatePattern = "yyyy/MM/dd";

			site.TimeZoneIdentifier = TimeZoneInfo.Local.Id;

			return site;
		}

		public static string SiteSearchPageName {
			get { return "/search"; }
		}

		public static void ManuallyWriteDefaultFile(HttpContext context, Exception objErr) {
			var sbBody = new StringBuilder();
			sbBody.Append(CoreHelper.ReadEmbededScript("Carrotware.CMS.Core.SiteContent.Default.htm"));

			try {
				if (CurrentSiteExists) {
					sbBody.Replace("{TIME_STAMP}", CurrentSite.Now.ToString());
				}
			} catch { }
			sbBody.Replace("{TIME_STAMP}", DateTime.Now.ToString());

			if (objErr != null) {
				sbBody.Replace("{LONG_NAME}", FormatToHTML(" [" + objErr.GetType().ToString() + "] " + objErr.Message));

				if (objErr.StackTrace != null) {
					sbBody.Replace("{STACK_TRACE}", FormatToHTML(objErr.StackTrace));
				}
				if (objErr.InnerException != null) {
					sbBody.Replace("{CONTENT_DETAIL}", FormatToHTML(objErr.InnerException.Message));
				}
			}

			sbBody.Replace("{STACK_TRACE}", "");
			sbBody.Replace("{CONTENT_DETAIL}", "");

			sbBody.Replace("{SITE_ROOT_PATH}", SiteData.AdminFolderPath);

			context.Response.ContentType = "text/html";
			context.Response.Clear();
			context.Response.WriteAsync(sbBody.ToString());
		}

		private static string FormatToHTML(string inputString) {
			string outputString = string.Empty;
			if (!string.IsNullOrEmpty(inputString)) {
				StringBuilder sb = new StringBuilder(inputString);
				sb.Replace("\r\n", " <br \\> \r\n");
				sb.Replace("   ", "&nbsp;&nbsp;&nbsp;");
				sb.Replace("  ", "&nbsp;&nbsp;");
				sb.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
				outputString = sb.ToString();
			}
			return outputString;
		}

		public static string FormatErrorOutput(Exception objErr) {
			var sbBody = new StringBuilder();
			sbBody.Append(CoreHelper.ReadEmbededScript("Carrotware.CMS.Core.SiteContent.ErrorFormat.htm"));

			sbBody.Replace("{PAGE_TITLE}", objErr.Message);
			sbBody.Replace("{SHORT_NAME}", objErr.Message);
			sbBody.Replace("{LONG_NAME}", FormatToHTML(" [" + objErr.GetType().ToString() + "] " + objErr.Message));

			if (objErr.StackTrace != null) {
				sbBody.Replace("{STACK_TRACE}", FormatToHTML(objErr.StackTrace));
			}

			if (objErr.InnerException != null) {
				sbBody.Replace("{CONTENT_DETAIL}", FormatToHTML(objErr.InnerException.Message));
			}

			if (CurrentSiteExists) {
				sbBody.Replace("{TIME_STAMP}", CurrentSite.Now.ToString());
			}
			sbBody.Replace("{TIME_STAMP}", DateTime.Now.ToString());

			sbBody.Replace("{CONTENT_DETAIL}", "");
			sbBody.Replace("{STACK_TRACE}", "");

			return sbBody.ToString();
		}

		public static void Show404MessageFull() {
			HttpContext context = CarrotHttpHelper.HttpContext;
			context.Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;

			Exception errInner = new Exception("The resource you are looking for (or one of its dependencies) could have been removed, had its name changed, or is temporarily unavailable. Please review the following URL and make sure that it is spelled correctly.");
			Exception err = new Exception("File or directory not found, 404.", errInner);

			context.Response.WriteAsync(FormatErrorOutput(err));
		}

		public static void Show404MessageShort() {
			HttpContext context = CarrotHttpHelper.HttpContext;
			context.Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
		}

		public static void Show301Message(string sFileRequested) {
			HttpContext context = CarrotHttpHelper.HttpContext;
			context.Response.StatusCode = (int)System.Net.HttpStatusCode.MovedPermanently;
		}

		public static void WriteDebugException(string sSrc, Exception objErr) {
			bool bWriteError = false;

			CarrotCakeConfig config = CarrotCakeConfig.GetConfig();

			if (config.ExtraOptions != null && config.ExtraOptions.WriteErrorLog) {
				bWriteError = config.ExtraOptions.WriteErrorLog;
			}
#if DEBUG
			bWriteError = true; // always write errors when debug build
#endif
		}

		public static void Perform404Redirect(string sReqURL) {
			PerformRedirectToErrorPage(404, sReqURL);
		}

		public static void PerformRedirectToErrorPage(int ErrorKey, string sReqURL) {
			PerformRedirectToErrorPage(ErrorKey.ToString(), sReqURL);
		}

		public static void PerformRedirectToErrorPage(string sErrorKey, string sReqURL) {
			HttpContext context = CarrotHttpHelper.HttpContext;

			var errors = CustomErrorConfig.GetConfig();

			if (errors != null) {
				string defaultRedirectPage = errors.DefaultRedirect;
				var errorCodes = errors.ErrorCodes;

				if (errorCodes != null && errorCodes.Any()) {
					int errorCode = int.Parse(sErrorKey);
					var customErr = errorCodes.Where(x => x.StatusCode == errorCode).FirstOrDefault();

					if (customErr != null) {
						var redirectPage = customErr.Uri.Length > 0 ? customErr.Uri : defaultRedirectPage;

						string sQS = string.Empty;
						if (context.Request.QueryString != null) {
							if (!string.IsNullOrEmpty(context.Request.QueryString.ToString())) {
								sQS = HttpUtility.UrlEncode(string.Format("?{0}", context.Request.QueryString));
							}
						}

						if (!string.IsNullOrEmpty(redirectPage) && !sQS.ToLowerInvariant().Contains("errorpath")) {
							context.Response.Redirect(string.Format("{0}?errorpath={1}{2}", redirectPage, sReqURL, sQS));
						}
					}
				}
			}
		}

		public static bool IsFilenameCurrentPage(string sCurrentFile) {
			if (string.IsNullOrEmpty(sCurrentFile)) {
				return false;
			}

			if (sCurrentFile.Contains("?")) {
				sCurrentFile = sCurrentFile.Substring(0, sCurrentFile.IndexOf("?"));
			}

			if (sCurrentFile.ToLowerInvariant() == SiteData.CurrentScriptName.ToLowerInvariant()
				|| sCurrentFile.ToLowerInvariant() == SiteData.AlternateCurrentScriptName.ToLowerInvariant()) {
				return true;
			}
			return false;
		}

		public static string StarterHomePageSample {
			get {
				return CoreHelper.ReadEmbededScript("Carrotware.CMS.Core.SiteContent.FirstPage.txt");
			}
		}

		public static SiteNav SiteBlogPage {
			get {
				if (CurrentSite == null) {
					return null;
				}
				if (CurrentSite.Blog_Root_ContentID.HasValue) {
					using (ISiteNavHelper navHelper = SiteNavFactory.GetSiteNavHelper()) {
						return navHelper.GetLatestVersion(CurrentSite.SiteID, CurrentSite.Blog_Root_ContentID.Value);
					}
				} else {
					// fake / mockup of a search page
					return SiteNavHelper.GetEmptySearch();
				}
				return null;
			}
		}

		public static string SearchQueryParameter {
			get { return "query".ToLowerInvariant(); }
		}

		public static string AdvancedEditParameter {
			get { return "carrotedit".ToLowerInvariant(); }
		}

		public static string TemplatePreviewParameter {
			get { return "c3pv".ToLowerInvariant(); }
		}

		public static string DefaultDirectoryFilename {
			get { return "/"; }
		}

		[Display(Name = "Default - Plain L-R-C Content")]
		public static string DefaultTemplateFilename {
			get { return "/Views/CmsContent/PlainPage/PlainPageView.cshtml".ToLowerInvariant(); }
		}

		[Display(Name = "Black 'n White - Plain L-R-C Content")]
		public static string DefaultTemplateBWFilename {
			get { return "/Views/CmsContent/PlainPage/PlainPageBWView.cshtml".ToLowerInvariant(); }
		}

		public static List<string> DefaultTemplates {
			get {
				string[] _defaultTemplates = new string[] { DefaultTemplateFilename, DefaultTemplateBWFilename };

				return _defaultTemplates.ToList();
			}
		}

		public static string PreviewTemplateFilePage {
			get { return SiteFilename.TemplatePreviewURL; }
		}

		public static bool IsPageSampler {
			get {
				return CurrentScriptName.ToLowerInvariant().StartsWith(PreviewTemplateFilePage.ToLowerInvariant())
						&& CarrotHttpHelper.QueryString(TemplatePreviewParameter) != null;
			}
		}

		public static bool IsPageReal {
			get {
				if (CurrentScriptName.ToLowerInvariant() != DefaultDirectoryFilename.ToLowerInvariant()
							&& File.Exists(CarrotHttpHelper.MapPath(CurrentScriptName))) {
					return true;
				} else {
					return false;
				}
			}
		}

		private static List<string> _specialFiles = null;

		public static List<string> SpecialFiles {
			get {
				if (_specialFiles == null) {
					_specialFiles = new List<string>();
					//_specialFiles.Add(DefaultTemplateFilename);
					//_specialFiles.Add(DefaultDirectoryFilename);
					//_specialFiles.Add("/feed/rss.ashx");
					//_specialFiles.Add("/feed/sitemap.ashx");
					//_specialFiles.Add("/feed/xmlrpc.ashx");
				}

				return _specialFiles;
			}
		}

		public static bool IsCurrentPageSpecial {
			get {
				return SiteData.SpecialFiles.Contains(CurrentScriptName.ToLowerInvariant());
			}
		}

		public static bool IsPageSpecial(string sPageName) {
			return SiteData.SpecialFiles.Contains(sPageName.ToLowerInvariant()) || sPageName.ToLowerInvariant().StartsWith(AdminFolderPath);
		}

		public static string PreviewTemplateFile {
			get {
				string _preview = DefaultTemplateFilename;

				if (CarrotHttpHelper.QueryString(TemplatePreviewParameter) != null) {
					_preview = CarrotHttpHelper.QueryString(TemplatePreviewParameter).ToString();
					_preview = CMSConfigHelper.DecodeBase64(_preview);
				}

				return _preview;
			}
		}

		private static Version _version = null;

		private static Version CurrentVersion {
			get {
				if (_version == null) {
					_version = Assembly.GetExecutingAssembly().GetName().Version;
				}
				return _version;
			}
		}

		private static bool? _debug = null;
		private static FileVersionInfo _fileversion = null;

		private static void LoadFileInfo() {
			if (_fileversion == null) {
#if DEBUG
				_debug = true;
#else
				_debug = false;
#endif
				var assembly = Assembly.GetExecutingAssembly();
				_fileversion = FileVersionInfo.GetVersionInfo(assembly.Location);
			}
		}

		public static string CurrentDLLVersion {
			get {
				LoadFileInfo();
				return _fileversion != null ? _fileversion.FileVersion : "1.1.0.0";
			}
		}

		public static string CarrotCakeCMSVersionShort {
			get {
				LoadFileInfo();
				var releaseMask = _debug.Value ? "MVC Core {0} (debug)" : "MVC Core {0}";

				return string.Format(releaseMask, CurrentDLLVersion);
			}
		}

		public static string CurrentDLLMajorMinorVersion {
			get {
				Version v = CurrentVersion;
				return v.Major.ToString() + "." + v.Minor.ToString();
			}
		}

		public static string CarrotCakeCMSVersion {
			get {
				LoadFileInfo();
				var releaseMask = _debug.Value ? "CarrotCake CMS MVC {0} DEBUG MODE" : "CarrotCake CMS MVC {0}";

				return string.Format(releaseMask, CurrentDLLVersion);
			}
		}

		public static string CarrotCakeCMSVersionMM {
			get {
				LoadFileInfo();
				var releaseMask = _debug.Value ? "CarrotCake CMS MVC {0} (debug)" : "CarrotCake CMS MVC {0}";

				return string.Format(releaseMask, CurrentDLLMajorMinorVersion);
			}
		}

		public static string CurrentScriptName {
			get {
				string sPath = "/";
				try { sPath = CarrotHttpHelper.Request.Path; } catch { }
				return sPath;
			}
		}

		public static Guid? CurrentRoutePageID {
			get {
				Guid? id = null;
				try {
					var data = CarrotHttpHelper.HttpContext.GetRouteData();
					if (data != null && data.Values != null
								&& data.Values[CmsRouting.PageIdKey] != null) {
						var val = data.Values[CmsRouting.PageIdKey].ToString() ?? "";
						if (val.Length > 27) {
							id = new Guid(val);
						}
					}
				} catch { }

				return id;
			}
		}

		public static string AdminDefaultFile {
			get {
				return (AdminFolderPath + DefaultDirectoryFilename).Replace("//", "/");
			}
		}

		private static string _adminFolderPath = null;

		public static string AdminFolderPath {
			get {
				if (_adminFolderPath == null) {
					string _defPath = "/c3-admin/";
					try {
						CarrotCakeConfig config = CarrotCakeConfig.GetConfig();
						if (config.MainConfig != null && !string.IsNullOrEmpty(config.MainConfig.AdminFolderPath)) {
							_adminFolderPath = config.MainConfig.AdminFolderPath;
							_adminFolderPath = string.Format("/{0}/", _adminFolderPath).Replace(@"\", "/").Replace("///", "/").Replace("//", "/").Replace("//", "/").Trim();
						} else {
							_adminFolderPath = _defPath;
						}
						if (string.IsNullOrEmpty(_adminFolderPath) || _adminFolderPath.Length < 2) {
							_adminFolderPath = _defPath;
						}
					} catch (Exception ex) {
						SiteData.WriteDebugException("adminfolderpath", ex);
						return _defPath;
					}
				}
				return _adminFolderPath;
			}
		}

		public static string AlternateCurrentScriptName {
			get {
				string currentPage = CurrentScriptName;

				if (!CurrentScriptName.ToLowerInvariant().StartsWith(AdminFolderPath)) {
					string scrubbedURL = CheckForSpecialURL(CurrentSite);

					if (currentPage.EndsWith("/")) {
						currentPage = currentPage.Substring(0, currentPage.Length - 1);
					}

					if (!scrubbedURL.ToLowerInvariant().StartsWith(currentPage.ToLowerInvariant())
						&& !currentPage.ToLowerInvariant().EndsWith(DefaultDirectoryFilename)) {
						if (scrubbedURL.ToLowerInvariant() != currentPage.ToLowerInvariant()) {
							currentPage = scrubbedURL;
						}
					}
				}

				return currentPage;
			}
		}

		public static string CheckForSpecialURL(SiteData site) {
			string sRequestedURL = "/";

			sRequestedURL = CurrentScriptName;
			string sFileRequested = sRequestedURL;

			if (!sRequestedURL.ToLowerInvariant().StartsWith(AdminFolderPath) && site != null) {
				if (sFileRequested.ToLowerInvariant().StartsWith(site.BlogFolderPath.ToLowerInvariant())) {
					if (site.GetSpecialFilePathPrefixes().Where(x => sFileRequested.ToLowerInvariant().StartsWith(x)).Count() > 0) {
						if (site.Blog_Root_ContentID.HasValue) {
							using (ISiteNavHelper navHelper = SiteNavFactory.GetSiteNavHelper()) {
								SiteNav blogNavPage = navHelper.GetLatestVersion(site.SiteID, site.Blog_Root_ContentID.Value);
								if (blogNavPage == null) {
									blogNavPage = SiteNavHelper.GetEmptySearch();
								}
								if (blogNavPage != null) {
									sRequestedURL = blogNavPage.FileName;
								}
							}
						}
					}
				}
			}

			return sRequestedURL;
		}

		public static string RssDocType { get { return "text/xml"; } }

		public static string RawMode { get { return "raw"; } }
		public static string HtmlMode { get { return "html"; } }

		public static string EditMode(string mode) {
			return (string.IsNullOrEmpty(mode) || mode.Trim().ToLowerInvariant() != RawMode) ? HtmlMode.ToLowerInvariant() : RawMode.ToLowerInvariant();
		}
	}
}