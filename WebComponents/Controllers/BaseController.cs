using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.Web.UI.Components.Controllers {

	public class BaseController : Microsoft.AspNetCore.Mvc.Controller {

		protected IWebHostEnvironment _environment;

		public BaseController(IWebHostEnvironment environment) {
			_environment = environment;
		}

		protected void DoCacheMagic(int interval) {
			var context = CarrotWebHelper.Current;

			DateTime dtModified = GetFauxModDate(10);
			DateTime? dtM = GetModDate(context);

			string strModifed = dtModified.ToUniversalTime().ToString("r");
			context.Response.Headers.Append("Last-Modified", strModifed);
			context.Response.Headers.Append("Date", strModifed);
			context.Response.GetTypedHeaders().CacheControl =
									new Microsoft.Net.Http.Headers.CacheControlHeaderValue() {
										Public = true,
										MaxAge = TimeSpan.FromMinutes(interval)
									};

			if (dtM == null || dtM.Value != dtModified) {
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
			} else {
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.NotModified;
			}
		}

		protected DateTime GetFauxModDate(int interval) {
			DateTime now = DateTime.Now;

			DateTime dtMod = now.AddMinutes(-90);
			TimeSpan ts = TimeSpan.FromMinutes(interval);
			DateTime dtModified = new DateTime(((dtMod.Ticks + ts.Ticks - 1) / ts.Ticks) * ts.Ticks);

			return dtModified;
		}

		protected DateTime? GetModDate(HttpContext context) {
			DateTime? dtModSince = null;

			string modSince = context.Request.Headers["If-Modified-Since"];

			if (!string.IsNullOrEmpty(modSince)) {
				dtModSince = DateTime.Parse(modSince);
				dtModSince = dtModSince.Value.ToUniversalTime();
			}

			return dtModSince;
		}
	}
}