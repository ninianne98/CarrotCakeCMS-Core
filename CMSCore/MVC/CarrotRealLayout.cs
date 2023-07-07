using Carrotware.CMS.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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

	public static class CarrotRealLayout {

		public static string ToggleLayout(IHtmlHelper helper, ViewDataDictionary viewData, string viewName) {
			var routeData = helper.ViewContext.RouteData.Values;

			if (routeData[CmsRouting.PageIdKey] != null && routeData[CmsRouting.PageIdKey].ToString().Length > 30
						&& routeData["controller"] == CmsRouteConstants.CmsController.Content) {
				return viewName;
			}

			bool useWidget = viewData[CarrotLayout.LayoutKey] != null ? ((bool)viewData[CarrotLayout.LayoutKey]) : false;

			if (useWidget) {
				viewName = CarrotLayout.Main;
			}

			return viewName;
		}
	}
}