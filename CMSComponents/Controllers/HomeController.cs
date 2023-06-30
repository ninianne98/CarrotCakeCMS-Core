using Carrotware.CMS.Core;
using Carrotware.Web.UI.Components;
using Carrotware.Web.UI.Components.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.UI.Components.Controllers {

	public class HomeController : BaseController {
		protected MemoryStream _stream = new MemoryStream();

		public HomeController(IWebHostEnvironment environment) : base(environment) {
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			_stream.Dispose();
		}

		public IActionResult GetAdminScriptValues(string ts) {
			double time = 3;
			if (this.User.Identity.IsAuthenticated) {
				time = 1;
			}
			this.VaryCacheByQuery(new string[] { "ts" }, time);
			DoCacheMagic(time);

			var adminFolder = SiteData.AdminFolderPath;

			var sb = new StringBuilder();
			sb.Append(CarrotWebHelper.GetManifestResourceText(this.GetType(), "Carrotware.CMS.UI.Components.adminHelp.js"));

			sb.Replace("[[TIMESTAMP]]", DateTime.UtcNow.ToString("u"));

			if (SecurityData.UserPrincipal.Identity.IsAuthenticated) {
				if (SecurityData.IsAdmin || SecurityData.IsEditor) {
					sb.Replace("[[ADMIN_PATH]]", adminFolder.FixPathSlashes());
					sb.Replace("[[API_PATH]]", ("/api/" + adminFolder).FixPathSlashes());
					sb.Replace("[[TEMPLATE_PATH]]", SiteData.PreviewTemplateFilePage);
					sb.Replace("[[TEMPLATE_QS]]", SiteData.TemplatePreviewParameter);
				}
			}

			string sBody = sb.ToString();
			var byteArray = Encoding.UTF8.GetBytes(sBody);
			_stream = new MemoryStream(byteArray);

			return File(_stream, "text/javascript");
		}

		public IActionResult GetNavigationCss(string el, string sel, string tbg, string f, string bg, string ubg,
				string fc, string bc, string hbc, string hfc, string uf,
				string sbg, string sfg, string bc2, string fc2) {
			this.VaryCacheByQuery(new string[] { "el", "ts", "f", "bg", "fc", "bc" }, 5);

			DoCacheMagic(7);

			var nav = new TwoLevelNavigation(f, bg, ubg, fc, bc,
									hbc, hfc, uf, sbg, sfg, bc2, fc2);

			nav.ElementId = Utils.DecodeBase64(el).Replace("{", "").Replace(">", "").Replace("<", "").Replace(">", "")
									.Replace("'", "").Replace("\\", "").Replace("//", "").Replace(":", "");
			nav.CssSelected = Utils.DecodeBase64(sel).Replace("{", "").Replace(">", "").Replace("<", "").Replace(">", "")
									.Replace("'", "").Replace("\\", "").Replace("//", "").Replace(":", "");
			nav.CssTopBackground = Utils.DecodeBase64(tbg).Replace("{", "").Replace(">", "").Replace("<", "").Replace(">", "")
									.Replace("'", "").Replace("\\", "").Replace("//", "").Replace(":", "");

			var txt = nav.GenerateCSS();
			var byteArray = Encoding.UTF8.GetBytes(txt);

			_stream = new MemoryStream(byteArray);

			return File(_stream, "text/css");
		}
	}
}