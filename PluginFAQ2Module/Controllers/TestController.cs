using CarrotCake.CMS.Plugins.FAQ2.Data;
using CarrotCake.CMS.Plugins.FAQ2.Models;
using Carrotware.CMS.Interface;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2024, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: April 2024
*/

namespace CarrotCake.CMS.Plugins.FAQ2.Controllers {

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

			if (this.TestSiteID != Guid.Empty.ToString()) {
				_siteId = new Guid(this.TestSiteID);
			}

			// since there are different models, set them up as needed to match the test
			if (action.ToLowerInvariant() == "testview1" || action.ToLowerInvariant() == "testview2") {
				var settings = new FaqPublic();
				settings.SiteID = _siteId;

				if (vals.ContainsKey("id")) {
					string id = vals["id"].ToString().ToLowerInvariant();
					settings.FaqCategoryID = new Guid(id);
					settings.WidgetClientID = "Widget_" + settings.FaqCategoryID.ToString().ToLowerInvariant().Substring(0, 5);
				}

				this.WidgetPayload = settings;
			}

			if (action.ToLowerInvariant() == "testview3") {
				var settings = new FaqPublicTop();
				settings.SiteID = _siteId;

				if (vals.ContainsKey("id")) {
					string id = vals["id"].ToString().ToLowerInvariant();
					settings.FaqCategoryID = new Guid(id);
					settings.WidgetClientID = "Widget_" + settings.FaqCategoryID.ToString().ToLowerInvariant().Substring(0, 5);
				}

				int top = 3;

				if (CarrotHttpHelper.QueryString("top") != null) {
					top = int.Parse(CarrotHttpHelper.QueryString("top"));
				} else {
					if (vals.ContainsKey("top")) {
						top = int.Parse(vals["top"].ToString());
					}
				}

				settings.TakeTop = top;

				this.WidgetPayload = settings;
			}
		}

		// ViewBag.WidgetTitle

		public ActionResult Index() {
			List<CarrotFaqCategory> lst = null;

			using (var fh = new FaqHelper(_siteId)) {
				lst = fh.CategoryListGetBySiteID().OrderBy(x => x.FaqTitle).ToList();
			}

			return View(lst);
		}

		public ActionResult TestView1(Guid id) {
			var model = new TestModel();

			var ctrl = this.CreateController(typeof(HomeController), "ShowFaqList", this.AreaName, this.WidgetPayload);
			var result = RenderWidgetHelper.ExecuteAction(ctrl);
			model.RenderedContent = new HtmlString(RenderWidgetHelper.ResultToString(ctrl, result));

			ViewBag.WidgetTitle = "Test Widget Display 1";

			return View("TestView", model);
		}

		public ActionResult TestView2(Guid id) {
			var model = new TestModel();

			var ctrl = this.CreateController(typeof(HomeController), "ShowRandomFaq", this.AreaName, this.WidgetPayload);
			var result = RenderWidgetHelper.ExecuteAction(ctrl);
			model.RenderedContent = new HtmlString(RenderWidgetHelper.ResultToString(ctrl, result));

			ViewBag.WidgetTitle = "Test Widget Display 2";

			return View("TestView", model);
		}

		public ActionResult TestView3(Guid id, int top) {
			var model = new TestModel();

			var ctrl = this.CreateController(typeof(HomeController), "ShowFaqTopList", this.AreaName, this.WidgetPayload);
			var result = RenderWidgetHelper.ExecuteAction(ctrl);
			model.RenderedContent = new HtmlString(RenderWidgetHelper.ResultToString(ctrl, result));

			ViewBag.WidgetTitle = "Test Widget Display 3";

			return View("TestView", model);
		}
	}
}