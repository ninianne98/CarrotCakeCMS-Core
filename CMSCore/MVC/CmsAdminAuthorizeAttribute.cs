using Carrotware.CMS.Interface;
using Carrotware.CMS.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
			RouteValueDictionary vals = context.RouteData.Values;
			string action = vals["action"].ToString().ToLowerInvariant();
			string controller = vals["controller"].ToString().ToLowerInvariant();

			if (!SecurityData.GetIsAdminFromCache()) {
				var _config = CarrotSecurityConfig.GetConfig(CarrotHttpHelper.Configuration);
				context.Result = new UnauthorizedResult();
				context.HttpContext.Response.Redirect(_config.AdditionalSettings.LoginPath);
			}

			return;
		}
	}
}