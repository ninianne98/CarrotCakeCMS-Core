using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

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
			string[] views = new[] {
					"/Views/{2}/{0}.cshtml",
					"/Views/{2}/{0}.vbhtml",
					"/Views/{2}/{1}/{0}.cshtml",
					"/Views/{2}/{1}/{0}.vbhtml",
					"/Views/{2}/Shared/{0}.cshtml",
					"/Views/{2}/Shared/{0}.vbhtml",

					//"/Areas/{2}/{0}.cshtml",
					//"/Areas/{2}/{0}.vbhtml",
					//"/Areas/{2}/{1}/{0}.cshtml",
					//"/Areas/{2}/{1}/{0}.vbhtml",
					//"/Areas/{2}/Shared/{0}.cshtml",
					//"/Areas/{2}/Shared/{0}.vbhtml",

					"/Areas/{2}/Views/{0}.cshtml",
					"/Areas/{2}/Views/{0}.vbhtml",
					"/Areas/{2}/Views/{1}/{0}.cshtml",
					"/Areas/{2}/Views/{1}/{0}.vbhtml",
					"/Areas/{2}/Views/Shared/{0}.cshtml",
					"/Areas/{2}/Views/Shared/{0}.vbhtml"};

			return viewLocations.Union(views);
		}

		public void PopulateValues(ViewLocationExpanderContext context) {
		}
	}
}