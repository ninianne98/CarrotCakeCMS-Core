using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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

	public class BaseController : Controller {
		protected IWebHostEnvironment _environment;

		public BaseController(IWebHostEnvironment environment) {
			_environment = environment;
		}

		protected void DoCacheMagic(double minutes) {
			if (minutes < 0) { minutes = 0.25; }
			if (minutes > 30) { minutes = 30; }

			var context = this.HttpContext;

			DateTime dtModified = GetFauxModDate(10);
			DateTime? dtM = GetModDate(context);
			string strModifed = dtModified.ToUniversalTime().ToString("r");

			context.Response.GetTypedHeaders().CacheControl =
									new CacheControlHeaderValue() {
										Public = true,
										MaxAge = TimeSpan.FromMinutes(minutes)
									};

			context.Response.Headers.LastModified = strModifed;
			context.Response.Headers.Date = strModifed;
			context.Response.Headers.Expires = dtModified.AddMinutes(minutes).AddSeconds(-5).ToString("r");

			if (dtM == null || dtM.Value != dtModified) {
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
			} else {
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.NotModified;
			}
		}

		protected DateTime GetFauxModDate(double minutes) {
			DateTime now = DateTime.Now;

			DateTime dtMod = now.AddMinutes(-90);
			TimeSpan ts = TimeSpan.FromMinutes(minutes);
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