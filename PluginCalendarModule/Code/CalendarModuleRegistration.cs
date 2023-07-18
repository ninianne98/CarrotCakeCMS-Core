using CarrotCake.CMS.Plugins.CalendarModule.Data;
using Carrotware.CMS.Interface;
using Microsoft.EntityFrameworkCore;

namespace CarrotCake.CMS.Plugins.CalendarModule {

	public class CalendarModuleRegistration : BaseWidgetLoader {
		public override void LoadWidgets(IServiceCollection services) {
			base.LoadWidgets(services);

			services.AddTransient(typeof(Controllers.HomeController));
			services.AddTransient(typeof(Controllers.AdminController));

			var config = services.BuildServiceProvider().GetRequiredService<IConfigurationRoot>();
			services.AddDbContext<CalendarContext>(opt => opt.UseSqlServer(config.GetConnectionString("CarrotwareCMS")));
		}

		public override void RegisterWidgets(WebApplication app) {
			base.RegisterWidgets(app);

			app.MigrateDatabase();
		}
	}
}