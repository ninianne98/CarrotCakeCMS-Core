using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

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
			var areaName = type.Assembly.GetAssemblyName();

			this.RouteKey = "area";
			this.RouteValue = areaName;
		}

		public WidgetControllerAttribute(string areaName) {
			this.RouteKey = "area";
			this.RouteValue = areaName;
		}

		public string RouteKey { get; }

		public string RouteValue { get; }
	}
}
