using Carrotware.CMS.Interface;
using Microsoft.AspNetCore.Html;
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

		public static string LayoutKey { get { return CarrotLayout.LayoutKey; } }

		public static string ToggleLayout(IHtmlHelper helper, ViewDataDictionary viewData, string viewName) {
			var routeData = helper.ViewContext.RouteData.Values;

			if (routeData[CmsRouting.PageIdKey] != null && routeData[CmsRouting.PageIdKey].ToString().Length > 30
				&& routeData["controller"] == CmsRouteConstants.CmsController.Content) {
				return viewName;
			}

			bool useWidget = viewData[LayoutKey] != null ? ((bool)viewData[LayoutKey]) : false;

			if (useWidget) {
				viewName = Main;
			}

			return viewName;
		}

		public static string Main {
			get {
				return CarrotLayout.Main;
			}
		}

		public static string Popup {
			get {
				return CarrotLayout.Popup;
			}
		}

		public static string Public {
			get {
				return CarrotLayout.Public;
			}
		}

		public static string PopupFunction {
			get {
				return CarrotLayout.PopupFunction;
			}
		}

		public static HtmlString WritePopupLink(string Uri) {
			return CarrotLayout.WritePopupLink(Uri);
		}

		public static string CurrentScriptName {
			get {
				return CarrotLayout.CurrentScriptName;
			}
		}
	}
}