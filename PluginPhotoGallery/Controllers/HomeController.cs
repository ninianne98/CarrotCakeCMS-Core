using Carrotware.CMS.Interface;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace CarrotCake.CMS.Plugins.PhotoGallery.Controllers {

	[WidgetController(typeof(BasePublicController))]
	public class HomeController : BasePublicController {
		protected readonly IWebHostEnvironment _webHostEnvironment;
		protected readonly ICarrotSite _site;

		public HomeController(IWebHostEnvironment environment, ICarrotSite site) {
			_site = site;
			_webHostEnvironment = environment;
		}

		public override void OnActionExecuting(ActionExecutingContext context) {
			base.OnActionExecuting(context);

			var routeInfo = context.RouteData.GetRouteInfo();
			// use the test id to build a fake payload so the widget can be loaded for dev
			string action = routeInfo.Action;
			string controller = routeInfo.Controller;
		}

		public ActionResult Index() {
			return View(nameof(this.Index));
		}

		public ActionResult Index2() {
			return Index();
		}

		[WidgetActionSettingModel("CarrotCake.CMS.Plugins.PhotoGallery.GallerySettings, CarrotCake.CMS.Plugins.PhotoGallery")]
		public PartialViewResult ShowPrettyPhotoGallery() {
			var settings = new GallerySettings();

			if (this.WidgetPayload is GallerySettings) {
				settings = (GallerySettings)this.WidgetPayload;
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