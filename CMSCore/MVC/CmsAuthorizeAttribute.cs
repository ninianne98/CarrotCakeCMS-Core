using Carrotware.CMS.Interface;
using Carrotware.CMS.Security;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

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

	public class CmsAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter {

		public CmsAuthorizeAttribute() : base() {
		}

		public void OnAuthorization(AuthorizationFilterContext context) {
			var routeInfo = context.RouteData.GetRouteInfo();
			string action = routeInfo.Action.ToLowerInvariant();
			string controller = routeInfo.Controller.ToLowerInvariant();

			List<string> anonMethods = (new string[] { "login", "logoff", "about", "forgotpassword", "notauthorized" }).ToList();

			var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
			if (descriptor != null) {
				// use reflection to see if the method/action has an anon permission and honor it
				var type = descriptor.ControllerTypeInfo;
				anonMethods = type.GetMethods()
							  .Where(m => m.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Length > 0)
							  .Select(x => x.Name.ToLowerInvariant())
							  .Where(x => x == action)
							  .Distinct().ToList();
			}

			if (anonMethods.Contains(action)) {
				return;
			}

			if (!(SecurityData.GetIsAdminFromCache() || SecurityData.GetIsSiteEditorFromCache())) {
				var config = CarrotSecurityConfig.GetConfig(CarrotHttpHelper.Configuration);
				//context.Result = new UnauthorizedResult();
				context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
				context.HttpContext.Response.Redirect(config.AdditionalSettings.LoginPath);
			}

			return;
		}
	}
}