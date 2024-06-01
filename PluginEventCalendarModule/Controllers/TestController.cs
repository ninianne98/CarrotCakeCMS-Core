using CarrotCake.CMS.Plugins.EventCalendarModule.Models;
using Carrotware.CMS.Core;
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

namespace CarrotCake.CMS.Plugins.EventCalendarModule.Controllers {

	public class TestController : BaseController {
		private Guid _siteId = Guid.Empty;

		protected readonly IWebHostEnvironment _webenv;
		protected readonly ICarrotSite _site;

		public TestController(IWebHostEnvironment environment, ICarrotSite site) {
			_site = site;
			_webenv = environment;
		}

		public override void OnActionExecuting(ActionExecutingContext context) {
			base.OnActionExecuting(context);

			RouteValueDictionary vals = context.RouteData.Values;

			// use the test id to build a fake payload so the widget can be loaded for dev
			string action = vals["action"].ToString().ToLowerInvariant();
			string controller = vals["controller"].ToString().ToLowerInvariant();

			// since there are different models, set them up as needed to match the test

			if (this.TestSiteID != Guid.Empty.ToString()) {
				_siteId = new Guid(this.TestSiteID);
			}
			if (_site != null) {
				_siteId = _site.SiteID;
			}
			if (_siteId == Guid.Empty && SiteData.CurrentSiteExists) {
				_siteId = SiteData.CurrentSiteID;
			}

			if (action.ToLowerInvariant() == "testview1") {
				var settings = new CalendarUpcomingSettings();
				settings.SiteID = _siteId;
				settings.DaysInPast = -7;
				settings.DaysInFuture = 14;
				settings.TakeTop = 15;

				this.WidgetPayload = settings;
			}

			if (action.ToLowerInvariant() == "testview2") {
				var settings = new CalendarDisplaySettings();
				settings.SiteID = _siteId;
				settings.GenerateCss = true;

				this.WidgetPayload = settings;
			}

			if (action.ToLowerInvariant() == "testview3") {
				var settings = new CalendarSimpleSettings();
				settings.SiteID = _siteId;

				this.WidgetPayload = settings;
			}
		}

		public IActionResult Index() {
			return View();
		}

		public IActionResult TestView1() {
			var model = new TestModel();

			var ctrl = this.CreateController(typeof(HomeController), "CalendarUpcoming", this.AreaName, this.WidgetPayload);
			var result = RenderWidgetHelper.ExecuteAction(ctrl);
			model.RenderedContent = new HtmlString(result.ResultToString(ctrl));

			ViewBag.WidgetTitle = "Test Widget Display 1";

			return View("TestView", model);
		}

		public IActionResult TestView2() {
			var model = new TestModel();

			var ctrl = this.CreateController(typeof(HomeController), "EventCalendarDisplay", this.AreaName, this.WidgetPayload);
			var result = RenderWidgetHelper.ExecuteAction(ctrl);
			model.RenderedContent = new HtmlString(result.ResultToString(ctrl));

			ViewBag.WidgetTitle = "Test Widget Display 2";

			return View("TestView", model);
		}

		public IActionResult TestView3() {
			var model = new TestModel();

			var ctrl = this.CreateController(typeof(HomeController), "EventCalendarDisplay2", this.AreaName, this.WidgetPayload);
			var result = RenderWidgetHelper.ExecuteAction(ctrl);
			model.RenderedContent = new HtmlString(result.ResultToString(ctrl));

			ViewBag.WidgetTitle = "Test Widget Display 3";

			return View("TestView", model);
		}
	}
}