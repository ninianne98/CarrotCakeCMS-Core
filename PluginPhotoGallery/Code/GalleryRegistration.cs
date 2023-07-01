using CarrotCake.CMS.Plugins.PhotoGallery.Data;
using Carrotware.CMS.Interface;

namespace CarrotCake.CMS.Plugins.PhotoGallery.Code {

	public class GalleryRegistration : BaseWidgetLoader {
		public override void LoadWidgets(IServiceCollection services) {
			base.LoadWidgets(services);

			services.AddTransient(typeof(Controllers.HomeController));
			services.AddTransient(typeof(Controllers.AdminController));
		}

		public override void RegisterWidgets(WebApplication app) {
			base.RegisterWidgets(app);

			app.MigrateDatabase();
		}
	}
}