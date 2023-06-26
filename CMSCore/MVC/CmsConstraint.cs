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

	public class CmsConstraint : IRouteConstraint {
		private readonly IConfiguration _configuration;

		public static string RouteKey { get { return "requesteduri"; } }
		public static string PageIdKey { get { return "cmspageid"; } }
		public static string SpecialKey { get { return "specialpage"; } }

		public CmsConstraint(IConfiguration configuration) {
			_configuration = configuration; ;
		}

		public bool Match(HttpContext httpContext, IRouter route, string routeKey,
								RouteValueDictionary routeData, RouteDirection routeDirection) {
			var cmspageid = routeData[PageIdKey];

			if (cmspageid == null) {
				var nav = routeData.ManipulateRoutes();

				if (nav != null) {
					return true;
				}

				cmspageid = routeData[PageIdKey];
			}

			if (cmspageid != null) {
				return true;
			}

			return false;
		}
	}
}