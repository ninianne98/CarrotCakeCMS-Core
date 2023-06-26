using Carrotware.CMS.Interface;
using Carrotware.CMS.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

// interrogates controller to see if this should be authenticated and/or inject some metadata
namespace Carrotware.CMS.Core {

	public class CmsActivator : IControllerActivator {
		private readonly ServiceBasedControllerActivator _act;

		public CmsActivator() {
			_act = new ServiceBasedControllerActivator();
		}

		public object Create(ControllerContext controllerContext) {
			var type = controllerContext.ActionDescriptor.ControllerTypeInfo.AsType();

			var isAdmin = type.GetInterfaces().Contains(typeof(IAdminModule));
			var isWidget = type.GetInterfaces().Where(x => x == typeof(IWidget) || x == typeof(IWidgetController) || x == typeof(IWidgetDataObject)).Any();

			if (isAdmin || isWidget) {
				var site = new SiteBasicInfo();

				var principal = controllerContext.HttpContext.User;

				if (isAdmin && !principal.Identity.IsAuthenticated) {
					var _config = CarrotSecurityConfig.GetConfig(CarrotHttpHelper.Configuration);

					// enforce logging in for admin modules
					// not auth, get us out of here
					controllerContext.HttpContext.Response.Redirect(_config.AdditionalSettings.LoginPath);
					return _act.Create(controllerContext);
				}

				if ((isAdmin && principal.Identity.IsAuthenticated) || isWidget) {
					var ctrl = (Controller)_act.Create(controllerContext);

					if (ctrl is IAdminModule) {
						IAdminModule m = (ctrl as IAdminModule);
						m.SiteID = site.SiteID;
						m.ModuleID = Guid.Empty;
					}

					if (ctrl is IWidgetDataObject) {
						var settings = new WidgetActionSettingModel();
						settings.SiteID = site.SiteID;

						IWidgetDataObject w = (ctrl as IWidgetDataObject);
						w.WidgetPayload = settings;
					}

					// we've injected the admin stuff, return it
					return ctrl;
				}
			}

			return _act.Create(controllerContext);
		}

		public void Release(ControllerContext context, object controller) {
			_act.Release(context, controller);
		}
	}
}