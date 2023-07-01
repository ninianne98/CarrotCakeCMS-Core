using Carrotware.CMS.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CarrotCake.CMS.Plugins.PhotoGallery.Controllers {

	public class HomeController : BasePublicController {

		protected readonly IWebHostEnvironment _webHostEnvironment;
		protected readonly ICarrotSite _site;

		public HomeController(IWebHostEnvironment environment, ICarrotSite site) {

			_site = site;
			_webHostEnvironment = environment;
		}

		public override void OnActionExecuting(ActionExecutingContext context) {
			base.OnActionExecuting(context);

			RouteValueDictionary vals = context.RouteData.Values;

			// use the test id to build a fake payload so the widget can be loaded for dev
			string action = vals["action"].ToString().ToLowerInvariant();
			string controller = vals["controller"].ToString().ToLowerInvariant();
		}

		public ActionResult Index() {
			return View("Index");
		}

		public ActionResult Index2() {
			return Index();
		}

		[WidgetActionSettingModel("CarrotCake.CMS.Plugins.PhotoGallery.GallerySettings, CarrotCake.CMS.Plugins.PhotoGallery")]
		public PartialViewResult ShowPrettyPhotoGallery() {
			GallerySettings settings = new GallerySettings();

			if (WidgetPayload is GallerySettings) {
				settings = (GallerySettings)WidgetPayload;
				settings.LoadData();
			}

			var model = BuildModel(settings);

			if (string.IsNullOrEmpty(settings.AlternateViewFile)) {
				return PartialView(model);
			} else {
				return PartialView(settings.AlternateViewFile, model);
			}
		}
	}
}