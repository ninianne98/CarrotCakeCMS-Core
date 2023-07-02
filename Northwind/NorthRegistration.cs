using Carrotware.CMS.Interface;
using Microsoft.EntityFrameworkCore;
using Northwind.Data;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Northwind {

	public class NorthRegistration : BaseWidgetLoader {
		public override void LoadWidgets(IServiceCollection services) {
			base.LoadWidgets(services);

			services.AddTransient(typeof(Controllers.HomeController));
			services.AddTransient(typeof(Controllers.AdminController));

			var config = services.BuildServiceProvider().GetRequiredService<IConfigurationRoot>();
			services.AddDbContext<NorthwindContext>(opt => opt.UseSqlServer(config.GetConnectionString("NorthwindConnection")));
		}

		public override void RegisterWidgets(WebApplication app) {
			base.RegisterWidgets(app);
		}
	}
}