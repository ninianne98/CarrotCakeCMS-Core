using CarrotCake.CMS.Plugins.CalendarModule.Models;
using Carrotware.CMS.Interface;
using Microsoft.AspNetCore.Html;
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

namespace CarrotCake.CMS.Plugins.CalendarModule.Controllers {

	public class TestController : BaseController {
		protected readonly IWebHostEnvironment _webenv;
		protected readonly ICarrotSite _site;

		public TestController(IWebHostEnvironment environment, ICarrotSite site) {
			_site = site;
			_webenv = environment;
		}

		public override void OnActionExecuting(ActionExecutingContext context) {
			base.OnActionExecuting(context);

			RouteValueDictionary vals = context.RouteData.Values;
			var siteId = this.TestSiteID;

			if (siteId == Guid.Empty.ToString()) {
				siteId = _site.SiteID.ToString();
			}

			// use the test id to build a fake payload so the widget can be loaded for dev
			string action = vals["action"].ToString().ToLowerInvariant();
			string controller = vals["controller"].ToString().ToLowerInvariant();

			// since there are different models, set them up as needed to match the test

			if (action.ToLowerInvariant() == "testview1") {
				var settings = new CalendarUpcomingSettings();
				settings.SiteID = new Guid(siteId);
				settings.DaysInPast = -21;
				settings.DaysInFuture = 14;

				this.WidgetPayload = settings;
			}

			if (action.ToLowerInvariant() == "testview2") {
				var settings = new CalendarDisplaySettings();
				settings.SiteID = new Guid(siteId);

				this.WidgetPayload = settings;
			}

			if (action.ToLowerInvariant() == "testview3") {
				var settings = new CalendarSimpleSettings();
				settings.SiteID = new Guid(siteId);

				this.WidgetPayload = settings;
			}
		}

		public ActionResult Index() {
			return View();
		}

		public ActionResult TestView1() {
			var model = new TestModel();

			var ctrl = this.CreateController(typeof(HomeController), "CalendarUpcoming", this.AreaName, this.WidgetPayload);
			model.PartialResult = RenderWidgetHelper.ExecuteAction(ctrl);
			model.RenderedContent = new HtmlString(model.PartialResult.ResultToString(ctrl));

			ViewBag.WidgetTitle = "Test Widget Display 1";

			return View("TestView", model);
		}

		public ActionResult TestView2() {
			var model = new TestModel();

			var ctrl = this.CreateController(typeof(HomeController), "CalendarDisplay", this.AreaName, this.WidgetPayload);
			model.PartialResult = RenderWidgetHelper.ExecuteAction(ctrl);
			model.RenderedContent = new HtmlString(model.PartialResult.ResultToString(ctrl));

			ViewBag.WidgetTitle = "Test Widget Display 2";

			return View("TestView", model);
		}

		public ActionResult TestView3() {
			var model = new TestModel();

			var ctrl = this.CreateController(typeof(HomeController), "CalendarDisplaySimple", this.AreaName, this.WidgetPayload);
			model.PartialResult = RenderWidgetHelper.ExecuteAction(ctrl);
			model.RenderedContent = new HtmlString(model.PartialResult.ResultToString(ctrl));

			ViewBag.WidgetTitle = "Test Widget Display 3";

			return View("TestView", model);
		}
	}
}