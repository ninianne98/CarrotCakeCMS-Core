using Carrotware.CMS.Interface.Controllers;
using Carrotware.Web.UI.Components;
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

namespace Carrotware.CMS.Interface {

	public class WidgetControllerAttribute : Attribute, IRouteValueProvider {

		public WidgetControllerAttribute(Type type) {
			this.RouteKey = RouteInfo.Keys.Area;

			if (!BaseWidgetController.WidgetStandaloneMode) {
				var areaName = type.Assembly.GetAssemblyName();

				this.RouteValue = areaName;
			} else {
				this.RouteValue = "";
			}
		}

		public WidgetControllerAttribute(string areaName) {
			this.RouteKey = RouteInfo.Keys.Area;
			if (!BaseWidgetController.WidgetStandaloneMode) {
				this.RouteValue = areaName;
			} else {
				this.RouteValue = "";
			}
		}

		public string RouteKey { get; } = RouteInfo.Keys.Area;

		public string RouteValue { get; } = string.Empty;
	}
}