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

namespace Carrotware.CMS.Interface {

	public static class CarrotLayout {

		public static string LayoutKey { get { return "UseStdWidgetLayout"; } }

		public static string ToggleLayout(IHtmlHelper helper, ViewDataDictionary viewData, string viewName) {
			bool useWidget = viewData[LayoutKey] != null ? ((bool)viewData[LayoutKey]) : false;

			if (useWidget) {
				viewName = Main;
			}

			return viewName;
		}

		public static string Main {
			get {
				return CarrotHttpHelper.Configuration.GetValue<string>("LayoutMain") != null
					? CarrotHttpHelper.Configuration.GetValue<string>("LayoutMain").ToString()
					: "/Views/Shared/_LayoutModule.cshtml";
			}
		}

		public static string Popup {
			get {
				return CarrotHttpHelper.Configuration.GetValue<string>("LayoutPopup") != null
					? CarrotHttpHelper.Configuration.GetValue<string>("LayoutPopup").ToString()
					: "/Views/CmsAdmin/_LayoutPopup.cshtml";
			}
		}

		public static string Public {
			get {
				return "/Views/CmsAdmin/_LayoutPublic.cshtml";
			}
		}

		public static string PopupFunction {
			get {
				return CarrotHttpHelper.Configuration.GetValue<string>("LayoutPopupOpenFunction") != null
					? CarrotHttpHelper.Configuration.GetValue<string>("LayoutPopupOpenFunction").ToString()
					: "ShowWindowNoRefresh";
			}
		}

		public static HtmlString WritePopupLink(string Uri) {
			return new HtmlString(string.Format("{0}('{1}')", PopupFunction, Uri));
		}

		public static string CurrentScriptName {
			get {
				string sPath = "/";
				try { sPath = CarrotHttpHelper.HttpContext.Request.Path; } catch { }
				return sPath;
			}
		}
	}
}