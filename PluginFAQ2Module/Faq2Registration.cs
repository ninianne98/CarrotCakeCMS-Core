using CarrotCake.CMS.Plugins.FAQ2.Data;
using Carrotware.CMS.Interface;
using Microsoft.EntityFrameworkCore;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2024, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: April 2024
*/

namespace CarrotCake.CMS.Plugins.FAQ2 {

	public class Faq2Registration : BaseWidgetLoader {

		public override void LoadWidgets(IServiceCollection services) {
			base.LoadWidgets(services);

			services.AddTransient(typeof(Controllers.HomeController));
			services.AddTransient(typeof(Controllers.AdminController));

			var config = services.BuildServiceProvider().GetRequiredService<IConfigurationRoot>();
			services.AddDbContext<FaqContext>(opt => opt.UseSqlServer(config.GetConnectionString("CarrotwareCMS")));
		}

		public override void RegisterWidgets(WebApplication app) {
			base.RegisterWidgets(app);
		}
	}
}
