using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Interface.Controllers {

	public class BaseWidgetController : Controller, IWidgetController {
		public static bool WidgetStandaloneMode { get; set; } = true;

		public string AssemblyName { get; set; } = string.Empty;

		public string AreaName { get; set; } = string.Empty;

		public bool UseStdWidgetLayout { get; set; } = true;
		public bool UseArea { get; set; } = !WidgetStandaloneMode;

		public string TestSiteID {
			get {
				return CarrotHttpHelper.Configuration.GetValue<string>("TestSiteID") != null
					? CarrotHttpHelper.Configuration.GetValue<string>("TestSiteID").ToString()
					: Guid.Empty.ToString();
			}
		}

		public virtual void LoadAreaInfo() {
			Assembly asmbly = this.GetType().Assembly;
			string assemblyName = asmbly.GetAssemblyName();

			this.AssemblyName = assemblyName;
			ViewData["WidgetAssemblyName"] = assemblyName;
			ViewBag.WidgetAssemblyName = assemblyName;

			if (this.UseArea) {
				if (string.IsNullOrEmpty(this.AreaName)
						|| ViewData["WidgetAreaName"] == null) {
					this.AreaName = assemblyName;
					ViewData["WidgetAreaName"] = assemblyName;
					ViewBag.WidgetAreaName = assemblyName;
				}
			}

			ViewData[CarrotLayout.LayoutKey] = this.UseStdWidgetLayout;
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}

		public override void OnActionExecuting(ActionExecutingContext context) {
			base.OnActionExecuting(context);

			LoadAreaInfo();
			SetArea(context);
		}

		public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
			LoadAreaInfo();
			SetArea(context);

			return base.OnActionExecutionAsync(context, next);
		}

		public virtual void SetArea(ActionExecutingContext context) {
			if (this.UseArea) {
				RouteData routeData = context.RouteData;

				if (routeData != null) {
					RouteValueDictionary vals = routeData.Values;

					if (vals["area"] == null && !vals.ContainsKey("area")) {
						vals.Add("area", this.AssemblyName);
					}
				}
			}
		}
	}
}