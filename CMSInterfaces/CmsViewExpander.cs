using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Interface {

	public class CmsViewExpander : IViewLocationExpander {

		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
					IEnumerable<string> viewLocations) {

			var workingFolder = new List<string>();

			if (context.ActionContext is ControllerContext) {
				// get the folder of the view, specifically because of partial postbacks
				var controller = context.ActionContext as ControllerContext;
				var filename = controller.RestoreViewPath();

				if (!string.IsNullOrWhiteSpace(filename) && filename.Length > 4) {
					var folder = Path.GetDirectoryName(filename).FixPathSlashes();

					workingFolder.Add(folder + "/{0}.cshtml");
					workingFolder.Add(folder + "/{0}.vbhtml");
				}
			}

			if (context.ActionContext is ViewContext) {
				// get the folder of the view + 1 level deeper - specifically because of templates
				var view = context.ActionContext as ViewContext;
				var root = CarrotHttpHelper.MapPath("/");

				var filename = view.ExecutingFilePath;
				var folder = Path.GetDirectoryName(filename).FixPathSlashes();

				workingFolder.Add(folder + "/{0}.cshtml");
				workingFolder.Add(folder + "/{0}.vbhtml");

				try {
					var folders = Directory.GetDirectories(Path.Join(root, folder));

					foreach (var f in folders) {
						var fldr = f.Replace(root, string.Empty).FixPathSlashes();
						workingFolder.Add((fldr + "/{0}.cshtml").FixPathSlashes());
						workingFolder.Add((fldr + "/{0}.vbhtml").FixPathSlashes());
					}
				} catch { }
			}

			var views = new[] {
					"/Views/{2}/{0}.cshtml",
					"/Views/{2}/{0}.vbhtml",
					"/Views/{2}/{1}/{0}.cshtml",
					"/Views/{2}/{1}/{0}.vbhtml",
					"/Views/{2}/Shared/{0}.cshtml",
					"/Views/{2}/Shared/{0}.vbhtml",

					"/Areas/{2}/Views/{0}.cshtml",
					"/Areas/{2}/Views/{0}.vbhtml",
					"/Areas/{2}/Views/{1}/{0}.cshtml",
					"/Areas/{2}/Views/{1}/{0}.vbhtml",
					"/Areas/{2}/Views/Shared/{0}.cshtml",
					"/Areas/{2}/Views/Shared/{0}.vbhtml"}.ToList();

			return viewLocations.Union(workingFolder).Union(views);
		}

		public void PopulateValues(ViewLocationExpanderContext context) {

		}
	}
}