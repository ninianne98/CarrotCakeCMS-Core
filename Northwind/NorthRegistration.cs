using Carrotware.CMS.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind {

	public class NorthRegistration : BaseWidgetLoader {
		public override void LoadWidgets(IServiceCollection services) {
			base.LoadWidgets(services);

			//services.AddScoped(typeof(Controllers.HomeController));
			//services.AddScoped(typeof(Controllers.AdminController));

			services.AddTransient(typeof(Controllers.HomeController));
			services.AddTransient(typeof(Controllers.AdminController));
		}

		public override void RegisterWidgets(WebApplication app) {
			base.RegisterWidgets(app);
		}
	}
}