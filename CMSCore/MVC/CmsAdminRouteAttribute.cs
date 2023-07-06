using Carrotware.CMS.Interface;
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


namespace Carrotware.CMS.Core.MVC {
	public enum AdminArea {
		AdminGui,
		AdminApi,
	}

	//===============
	public class CmsAdminRouteAttribute : Attribute, IRouteTemplateProvider {
		public CmsAdminRouteAttribute(AdminArea type) {

			var cccConfig = CarrotCakeConfig.GetConfig();
			var adminFolder = cccConfig.MainConfig.AdminFolderPath.TrimPathSlashes();

			switch (type) {
				case AdminArea.AdminGui:
					this.Template = adminFolder;
					this.RouteKey = "controller";
					this.RouteValue = CmsRouteConstants.CmsController.Admin;
					break;
				case AdminArea.AdminApi:
					this.Template = "api/" + adminFolder;
					this.RouteKey = "controller";
					this.RouteValue = CmsRouteConstants.CmsController.AdminApi;
					break;
				default:
					break;
			}
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