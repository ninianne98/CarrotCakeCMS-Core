using Carrotware.CMS.Interface;
using Carrotware.CMS.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static Org.BouncyCastle.Math.EC.ECCurve;

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
			List<string> lstInitSiteActions = (new string[] { "login", "logoff", "about", "forgotpassword", "createfirstadmin", "databasesetup", "notauthorized" }).ToList();
			RouteValueDictionary vals = context.RouteData.Values;
			string action = vals["action"].ToString().ToLowerInvariant();
			string controller = vals["controller"].ToString().ToLowerInvariant();

			if (CmsRouteConstants.CmsController.Admin.ToLowerInvariant() == controller
					&& lstInitSiteActions.Contains(action)) {
				return;
			}

			if (!(SecurityData.GetIsAdminFromCache() || SecurityData.GetIsSiteEditorFromCache())) {
				var _config = CarrotSecurityConfig.GetConfig(CarrotHttpHelper.Configuration);
				//context.Result = new UnauthorizedObjectResult(string.Empty);
				context.HttpContext.Response.Redirect(_config.AdditionalSettings.LoginPath);
			}

			return;
		}
	}
}