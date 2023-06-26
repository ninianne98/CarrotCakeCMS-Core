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

// used for test purposes so auth isn't required and a test site id can be injected vs having to pull in the full cms libraries
namespace Carrotware.CMS.Interface {

	public class CmsTestActivator : IControllerActivator {
		private readonly ServiceBasedControllerActivator _act;

		public CmsTestActivator() {
			_act = new ServiceBasedControllerActivator();
		}

		public object Create(ControllerContext controllerContext) {
			var type = controllerContext.ActionDescriptor.ControllerTypeInfo.AsType();

			var isAdmin = type.GetInterfaces().Contains(typeof(IAdminModule));
			var isWidget = type.GetInterfaces().Where(x => x == typeof(IWidget) || x == typeof(IWidgetController) || x == typeof(IWidgetDataObject)).Any();

			if (isAdmin || isWidget) {
				var site = new SiteTestInfo();

				var ctrl = (Controller)_act.Create(controllerContext);

				if (ctrl is IAdminModule) {
					IAdminModule m = (ctrl as IAdminModule);
					m.SiteID = site.SiteID;
					m.ModuleID = Guid.Empty;
				}

				if (ctrl is IWidget) {
					IWidget w = (ctrl as IWidget);
					w.SiteID = site.SiteID;
				}

				if (ctrl is IWidgetDataObject) {
					var settings = new WidgetActionSettingModel();
					settings.SiteID = site.SiteID;

					IWidgetDataObject w = (ctrl as IWidgetDataObject);
					w.WidgetPayload = settings;
				}

				// we've injected the widget stuff, return it
				return ctrl;
			}

			return _act.Create(controllerContext);
		}

		public void Release(ControllerContext context, object controller) {
			_act.Release(context, controller);
		}
	}
}