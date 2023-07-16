using Carrotware.CMS.Interface;
using Carrotware.CMS.Security;
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
			RouteValueDictionary vals = context.RouteData.Values;
			string action = vals["action"].ToString().ToLowerInvariant();
			string controller = vals["controller"].ToString().ToLowerInvariant();

			List<string> anonMethods = (new string[] { "login", "logoff", "about" }).ToList();

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
				var _config = CarrotSecurityConfig.GetConfig(CarrotHttpHelper.Configuration);
				context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
				context.HttpContext.Response.Redirect(_config.AdditionalSettings.LoginPath);
			}

			return;
		}
	}
}