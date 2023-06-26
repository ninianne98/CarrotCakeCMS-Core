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
		public static string RouteKey { get { return "requesteduri"; } }
		public static string PageIdKey { get { return "cmspageid"; } }
		public static string SpecialKey { get { return "specialpage"; } }
		public static string FormKey { get { return "formaction"; } }

		public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext,
				RouteValueDictionary routeData) {

			var cmspageid = routeData[PageIdKey];

			if (cmspageid == null) {
				var nav = routeData.ManipulateRoutes();
			}

			return new ValueTask<RouteValueDictionary>(routeData);
		}
	}
}
