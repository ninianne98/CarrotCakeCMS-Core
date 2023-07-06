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

	//===============
	public class CmsTestHomeAttribute : Attribute, IRouteTemplateProvider {
		public static string DefaultPage { get { return "Default.WidgetLanding.TestHomePage"; } }

		public CmsTestHomeAttribute() {
			this.Template = string.Format("/{0}", DefaultPage);
		}

		public string Template { get; }

		private int? _order;
		public string RouteKey { get; }

		public string RouteValue { get; }

		public int Order {
			get { return _order ?? 0; }
			set { _order = value; }
		}

		int? IRouteTemplateProvider.Order => _order;

		public string? Name { get; set; }
	}
}