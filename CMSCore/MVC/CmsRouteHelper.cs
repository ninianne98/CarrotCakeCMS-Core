using Carrotware.Web.UI.Components;

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

	public static class CmsRouteConstants {

		public static class CmsController {
			public static string Admin { get { return "CmsAdmin"; } }
			public static string AdminApi { get { return "CmsAdminApi"; } }
			public static string Home { get { return "Home"; } }
			public static string Content { get { return "CmsContent"; } }
			public static string AjaxForms { get { return "CmsAjaxForms"; } }
		}

		public static string IndexAction { get { return "Index"; } }
		public static string DefaultAction { get { return "Default"; } }
		public static string NotFoundAction { get { return "PageNotFound"; } }
		public static string RssAction { get { return "RSSFeed"; } }
		public static string SiteMapAction { get { return "SiteMap"; } }
	}

	//=====================

	public static class CmsRouteHelper {

		public static SiteNav? ManipulateRoutes(this RouteValueDictionary routeData) {
			var site = SiteData.CurrentSite;
			SiteNav? navData = null;

			string requestedUri = @"/";

			if (routeData[CmsRouting.RouteKey] != null) {
				requestedUri = routeData[CmsRouting.RouteKey].ToString();
			}

			requestedUri = string.IsNullOrEmpty(requestedUri) ? @"/" : requestedUri.ToLowerInvariant();
			requestedUri = requestedUri.FixPathSlashes();

			string adminFolder = SiteData.AdminFolderPath.TrimPathSlashes();

			var routes = requestedUri.Split('/').Where(x => x.Length > 0).ToArray();

			if (routes.Length >= 1) {
				if (routes[0].ToLowerInvariant() == adminFolder.ToLowerInvariant()) {
					routeData.Add(CmsRouting.PageIdKey, adminFolder);
					routeData[CmsRouting.SpecialKey] = true;
					routeData["controller"] = CmsRouteConstants.CmsController.Admin;

					return navData;
				}
				if (routes.Length >= 2 && routes[0].ToLowerInvariant() == "api"
							&& routes[1].ToLowerInvariant() == adminFolder.ToLowerInvariant()) {
					routeData.Add(CmsRouting.PageIdKey, adminFolder);
					routeData[CmsRouting.SpecialKey] = true;
					routeData["controller"] = CmsRouteConstants.CmsController.AdminApi;

					return navData;
				}

				if (routes.Length >= 2 && routes[0].ToLowerInvariant() == CmsRouteConstants.CmsController.AjaxForms.ToLowerInvariant()) {
					var formaction = routes[1].ToString();
					if (formaction.Length > 4 && formaction.ToLowerInvariant().EndsWith(".ashx")) {
						routeData.Add(CmsRouting.PageIdKey, CmsRouteConstants.CmsController.AjaxForms);
						routeData[CmsRouting.SpecialKey] = true;
						routeData["controller"] = CmsRouteConstants.CmsController.Content;
						routeData["action"] = Path.GetFileNameWithoutExtension(formaction);
						routeData["id"] = routes.Length > 2 ? routes[2] : null;
						routeData["area"] = null;

						return navData;
					}
				}
			}

			requestedUri = requestedUri.ToLowerInvariant();

			if (requestedUri.Contains(".") && requestedUri.Length > 3) {
				// use ashx hack because a long querystring fails to reach the route otherwise
				if (requestedUri == SiteFilename.TemplatePreviewAltUrl.ToLowerInvariant()) {
					routeData.Add(CmsRouting.PageIdKey, SiteActions.TemplatePreview);
					routeData[CmsRouting.SpecialKey] = true;
					routeData["controller"] = CmsRouteConstants.CmsController.Admin;
					routeData["action"] = SiteActions.TemplatePreview;
					routeData["id"] = null;
					routeData["area"] = null;

					return navData;
				}

				if (requestedUri == SiteFilename.RssFeedUri.ToLowerInvariant()
					|| requestedUri == SiteFilename.RssFeedUri.ToLowerInvariant().Replace(".ashx", ".axd")
					|| requestedUri == SiteFilename.RssFeedUri.ToLowerInvariant().Replace(".ashx", ".xml")) {
					routeData.Add(CmsRouting.PageIdKey, CmsRouteConstants.RssAction);
					routeData[CmsRouting.SpecialKey] = true;
					routeData["controller"] = CmsRouteConstants.CmsController.Content;
					routeData["action"] = CmsRouteConstants.RssAction;
					routeData["id"] = null;
					routeData["area"] = null;

					return navData;
				}

				if (requestedUri == SiteFilename.SiteMapUri.ToLowerInvariant()
					|| requestedUri == SiteFilename.SiteMapUri.ToLowerInvariant().Replace(".ashx", ".axd")
					|| requestedUri == SiteFilename.SiteMapUri.ToLowerInvariant().Replace(".ashx", ".xml")) {
					routeData.Add(CmsRouting.PageIdKey, CmsRouteConstants.SiteMapAction);
					routeData[CmsRouting.SpecialKey] = true;
					routeData["controller"] = CmsRouteConstants.CmsController.Content;
					routeData["action"] = CmsRouteConstants.SiteMapAction;
					routeData["id"] = null;
					routeData["area"] = null;

					return navData;
				}

				SiteData.WriteDebugException("cmsroutehelper ashx not matched", new Exception(string.Format("RequestedUri: {0}", requestedUri)));

				return navData;
			} else {
				// find page even if not live, controller will enforce visibility

				//cms pages can't have a . in them, short circuit
				if (SiteData.CurrentScriptName.Contains(".")) {
					return navData;
				}

				string sCurrentPage = SiteData.CurrentScriptName;

				try {
					string sScrubbedURL = SiteData.AlternateCurrentScriptName;

					if (site == null || SiteData.CurrentSiteExists == false) {
						navData = SiteNavHelper.GetEmptyHome();
					}

					if (sScrubbedURL.ToLowerInvariant() != sCurrentPage.ToLowerInvariant()) {
						requestedUri = sScrubbedURL;
					}

					using (ISiteNavHelper navHelper = SiteNavFactory.GetSiteNavHelper()) {
						if (SiteData.IsLikelyHomePage(requestedUri) && navData == null) {
							navData = navHelper.FindHome(site.SiteID);
							if (navData != null) {
								requestedUri = navData.FileName;
							}
						}

						if (!string.IsNullOrEmpty(requestedUri) && navData == null) {
							navData = navHelper.GetLatestVersion(site.SiteID, false, requestedUri);
						}

						if (SiteData.IsLikelyHomePage(requestedUri) && navData == null) {
							navData = SiteNavHelper.GetEmptyHome();
						}

						if (SiteData.IsLikelySearch() && site.Blog_Root_ContentID.HasValue && navData == null) {
							navData = navHelper.GetLatestVersion(site.SiteID, site.Blog_Root_ContentID.Value);
						}

						// use a fake search page when needed, but don't allow editing
						if (!SecurityData.AdvancedEditMode && SiteData.IsLikelyFakeSearch() && navData == null) {
							navData = SiteNavHelper.GetEmptySearch();
						}

						if (navData != null) {
							SiteData.WriteDebugException("cmsroutehelper != null", new Exception(string.Format("Default: {0}", navData.FileName)));

							routeData[CmsRouting.SpecialKey] = false;
							routeData[CmsRouting.PageIdKey] = navData.Root_ContentID;
							routeData[CmsRouting.RouteKey] = !string.IsNullOrEmpty(requestedUri) ? requestedUri : @"/";
							routeData["controller"] = CmsRouteConstants.CmsController.Content;
							routeData["action"] = CmsRouteConstants.DefaultAction;
							routeData["id"] = null;
							routeData["area"] = null;
						} else {
							SiteData.WriteDebugException("cmsroutehelper == null", new Exception(string.Format("_PageNotFound: {0}", sCurrentPage)));
							// routeData["action"] = CmsRouteConstants.NotFoundAction;
						}
					}
				} catch (Exception ex) {
					SiteData.WriteDebugException("cmsroutehelper_exception_uri", new Exception(string.Format("Exception: {0}", sCurrentPage)));
					throw;
				}
			}

			return navData;
		}
	}
}