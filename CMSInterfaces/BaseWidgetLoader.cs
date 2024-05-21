using Carrotware.CMS.Interface.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;
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

namespace Carrotware.CMS.Interface {

	public abstract class BaseWidgetLoader : IWidgetLoader {
		protected string _areaName = string.Empty;

		public string AreaName { get { return _areaName; } }

		protected void LoadArea() {
			if (!BaseWidgetController.WidgetStandaloneMode) {
				Assembly assembly = this.GetType().Assembly;

				_areaName = assembly.GetAssemblyName();
			} else {
				_areaName = string.Empty;
			}
		}

		public virtual void LoadWidgets(IServiceCollection services) {
			LoadArea();

			string nsp = typeof(BaseWidgetLoader).Namespace;

			if (_areaName.ToLowerInvariant() != nsp.ToLowerInvariant()) {
				Assembly assembly = this.GetType().Assembly;
				var part = new AssemblyPart(assembly);

				services.AddControllersWithViews()
						.AddApplicationPart(assembly)
						.AddControllersAsServices()
						.AddRazorRuntimeCompilation();

				services.Configure<MvcRazorRuntimeCompilationOptions>(options => {
					options.FileProviders.Add(new EmbeddedFileProvider(assembly));
				});
			}
		}

		public virtual void RegisterWidgets(WebApplication app) {
			LoadArea();

			string nsp = typeof(BaseWidgetLoader).Namespace;

			if (!string.IsNullOrEmpty(this.AreaName) && this.AreaName.ToLowerInvariant() != nsp.ToLowerInvariant()) {
				try {
					app.MapControllerRoute(
								name: string.Format("{0}_Area", this.AreaName),
								pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
				} catch (Exception ex) { }
			}
		}
	}
}