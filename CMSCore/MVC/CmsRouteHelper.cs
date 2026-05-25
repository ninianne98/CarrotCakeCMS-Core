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

			if (routeData[CmsRouting.Keys.Route] != null) {
				requestedUri = routeData[CmsRouting.Keys.Route].ToString();
			}

			requestedUri = string.IsNullOrEmpty(requestedUri) ? @"/" : requestedUri.ToLowerInvariant();
			requestedUri = requestedUri.FixPathSlashes();

			string adminFolder = SiteData.AdminFolderPath.TrimPathSlashes();

			var routes = requestedUri.Split('/').Where(x => x.Length > 0).ToArray();

			if (routes.Length >= 1) {
				if (routes[0].ToLowerInvariant() == adminFolder.ToLowerInvariant()) {
					routeData.MarkSpecial(adminFolder);

					routeData[RouteInfo.Keys.Controller] = CmsRouteConstants.CmsController.Admin;

					return navData;
				}
				if (routes.Length >= 2 && routes[0].ToLowerInvariant() == "api"
							&& routes[1].ToLowerInvariant() == adminFolder.ToLowerInvariant()) {
					routeData.MarkSpecial(adminFolder);

					routeData[RouteInfo.Keys.Controller] = CmsRouteConstants.CmsController.AdminApi;

					return navData;
				}

				if (routes.Length >= 2 && routes[0].ToLowerInvariant() == CmsRouteConstants.CmsController.AjaxForms.ToLowerInvariant()) {
					var formAction = routes[1].ToString();
					if (formAction.Length > 4 && formAction.ToLowerInvariant().EndsWith(".ashx")) {
						routeData.MarkSpecial(CmsRouteConstants.CmsController.AjaxForms);

						var idValue = routes.Length > 2 ? routes[2] : null;
						var formId = Path.GetFileNameWithoutExtension(formAction);

						routeData.SetRouteValues(CmsRouteConstants.CmsController.Content, formId, idValue);

						return navData;
					}
				}
			}

			requestedUri = requestedUri.ToLowerInvariant();

			if (requestedUri.Contains(".") && requestedUri.Length > 3) {
				// use ashx hack because a long querystring fails to reach the route otherwise
				if (requestedUri == SiteFilename.TemplatePreviewAltUrl.ToLowerInvariant()) {
					routeData.MarkSpecial(SiteActions.TemplatePreview);

					routeData.SetRouteValues(CmsRouteConstants.CmsController.Admin, SiteActions.TemplatePreview);

					return navData;
				}

				if (UseDynamicFeed(SiteFilename.RssFeedUri, requestedUri)) {
					routeData.MarkSpecial(CmsRouteConstants.RssAction);

					routeData.SetRouteValues(CmsRouteConstants.CmsController.Content, CmsRouteConstants.RssAction);

					return navData;
				}

				if (UseDynamicFeed(SiteFilename.SiteMapUri, requestedUri)) {
					routeData.MarkSpecial(CmsRouteConstants.SiteMapAction);

					routeData.SetRouteValues(CmsRouteConstants.CmsController.Content, CmsRouteConstants.SiteMapAction);

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

							routeData[CmsRouting.Keys.Special] = false;
							routeData[CmsRouting.Keys.PageId] = navData.Root_ContentID;
							routeData[CmsRouting.Keys.Route] = !string.IsNullOrWhiteSpace(requestedUri) ? requestedUri : @"/";

							routeData.SetRouteValues(CmsRouteConstants.CmsController.Content, CmsRouteConstants.DefaultAction);
						} else {
							SiteData.WriteDebugException("cmsroutehelper == null", new Exception(string.Format("_PageNotFound: {0}", sCurrentPage)));
						}
					}
				} catch (Exception ex) {
					SiteData.WriteDebugException("cmsroutehelper_exception_uri", new Exception(string.Format("Exception: {0}", sCurrentPage)));
					throw;
				}
			}

			return navData;
		}

		private static bool UseDynamicFeed(string feedUri, string requestedUri) {
			var uri = feedUri.ToLowerInvariant();
			var reqUri = requestedUri.ToLowerInvariant();

			var pathMatch = reqUri == uri.ToLowerInvariant()
								|| reqUri == uri.Replace(".ashx", ".axd")
								|| reqUri == uri.Replace(".ashx", ".xml");

			// give precidence to actual xml
			if (pathMatch && reqUri.EndsWith(".xml")) {
				return File.Exists(CarrotWebHelper.MapWebPath(reqUri)) == false;
			}

			return pathMatch;
		}
	}
}