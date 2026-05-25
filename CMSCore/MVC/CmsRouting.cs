using Microsoft.AspNetCore.Mvc.Routing;

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

	public class CmsRouting : DynamicRouteValueTransformer {
		public static string RouteKey { get { return Keys.Route; } }
		public static string PageIdKey { get { return Keys.PageId; } }
		public static string WidgetIdKey { get { return Keys.WidgetId; } }
		public static string SpecialKey { get { return Keys.Special; } }
		public static string FormKey { get { return Keys.Form; } }

		public static class Keys {
			public static string Route { get { return "cmsRequestedUri"; } }
			public static string PageId { get { return "cmsPageId"; } }
			public static string WidgetId { get { return "cmsWidgetId"; } }
			public static string Special { get { return "cmsSpecialPage"; } }
			public static string Form { get { return "cmsFormAction"; } }
		}

		public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext,
				RouteValueDictionary routeData) {
			var cmspageid = routeData[Keys.PageId];

			if (cmspageid == null) {
				var nav = routeData.ManipulateRoutes();
				cmspageid = routeData[Keys.PageId];
			}

			return new ValueTask<RouteValueDictionary>(routeData);
		}
	}
}