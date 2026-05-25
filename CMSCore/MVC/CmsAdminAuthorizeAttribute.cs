using Carrotware.CMS.Interface;
using Carrotware.CMS.Security;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Authorization;
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

	public class CmsAdminAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter {

		public CmsAdminAuthorizeAttribute() : base() {
		}

		public void OnAuthorization(AuthorizationFilterContext context) {
			var routeInfo = context.RouteData.GetRouteInfo();
			string action = routeInfo.Action.ToLowerInvariant();
			string controller = routeInfo.Controller.ToLowerInvariant();

			if (!SecurityData.GetIsAdminFromCache()) {
				var config = CarrotSecurityConfig.GetConfig(CarrotHttpHelper.Configuration);
				//context.Result = new UnauthorizedResult();
				context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
				context.HttpContext.Response.Redirect(config.AdditionalSettings.LoginPath);
			}

			return;
		}
	}
}