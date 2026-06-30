using Carrotware.CMS.Interface;
using Carrotware.CMS.UI.Components.Controllers;
using Carrotware.Web.UI.Components;
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

namespace Carrotware.CMS.UI.Components {

	public class CarrotCakeAreaReg : BaseWidgetLoader {

		public CarrotCakeAreaReg() : base() { }

		public override void LoadWidgets(IServiceCollection services) {
			base.LoadWidgets(services);

			Assembly thisAssembly = this.GetType().Assembly;

			services.AddTransient(typeof(HomeController));
			services.AddTransient(typeof(FormsController));

			//services.AddControllersWithViews()
			//		.AddApplicationPart(thisAssembly)
			//		.AddControllersAsServices()
			//		.AddRazorRuntimeCompilation();

			var provider = services.BuildServiceProvider();

			services.AddControllersWithViews()
					.AddApplicationPart(thisAssembly)
					.AddControllersAsServices()
					.AddRazorRuntimeCompilation(options => {
						options.FileProviders.Add(new VirtualViewFileProvider(provider));
					});
		}

		public override void RegisterWidgets(WebApplication app) {
			base.RegisterWidgets(app);

			string home = typeof(HomeController).GetControllerName();

			app.MapControllerRoute(
					name: this.AreaName + "_GetNavigationCss",
					pattern: TwoLevelNavigation.NavigationStylePath + "/{id?}",
					defaults: new { controller = home, action = nameof(HomeController.GetNavigationCss) });

			app.MapControllerRoute(
					name: this.AreaName + "_GetAminScriptValues",
					pattern: CarrotCakeCmsHelper.AdminScriptValues + "/{id?}",
					defaults: new { controller = home, action = nameof(HomeController.GetAdminScriptValues) });
		}
	}
}