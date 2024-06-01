using CarrotCake.CMS.Plugins.PhotoGallery.Data;
using CarrotCake.CMS.Plugins.PhotoGallery.Models;
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
	public class TestController : BasePublicController {
		protected readonly IWebHostEnvironment _webHostEnvironment;
		protected readonly ICarrotSite _site;

		private Guid _siteid = Guid.Empty;
		private GalleryHelper _helper;

		public TestController(IWebHostEnvironment environment, ICarrotSite site) {
			_site = site;
			_webHostEnvironment = environment;
			_siteid = _site != null ? _site.SiteID : new Guid(this.TestSiteID);

			_helper = new GalleryHelper(_siteid);
		}

		public override void OnActionExecuting(ActionExecutingContext context) {
			base.OnActionExecuting(context);

			RouteValueDictionary vals = context.RouteData.Values;

			// use the test id to build a fake payload so the widget can be loaded for dev
			string action = vals["action"].ToString().ToLowerInvariant();
			string controller = vals["controller"].ToString().ToLowerInvariant();

			var settings = new GallerySettings();
			settings.SiteID = _siteid;
			settings.ShowHeading = true;

			if (vals.ContainsKey("id")) {
				string id = vals["id"].ToString().ToLowerInvariant();
				settings.GalleryId = new Guid(id);
				settings.WidgetClientID = "Widget_" + id.Substring(0, 5);
			}

			settings.PublicParmValues.Add("SiteID", _siteid.ToString());
			settings.PublicParmValues.Add("WidgetClientID", settings.WidgetClientID);
			settings.PublicParmValues.Add("GalleryId", settings.GalleryId.ToString().ToLowerInvariant());
			settings.PublicParmValues.Add("ShowHeading", settings.ShowHeading.ToString());

			WidgetPayload = settings;
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			if (_helper != null) {
				_helper.Dispose();
			}
		}

		[HttpGet]
		public IActionResult Index() {
			var model = new PagedData<GalleryImage>();
			model.InitOrderBy(x => x.ImageOrder, true);

			using (var db = new GalleryContext()) {
				model.DataSource = (from c in db.GalleryImages
									join g in db.Galleries on c.GalleryId equals g.GalleryId
									where g.SiteId == _siteid
									orderby c.ImageOrder ascending
									select c).Skip(model.PageSize * (model.PageNumber - 1)).Take(model.PageSize).ToList();

				model.TotalRecords = (from c in db.GalleryImages select c).Count();
			}

			return View(model);
		}

		[HttpPost]
		public IActionResult Index(PagedData<GalleryImage> model) {
			model.ToggleSort();
			var srt = model.ParseSort();

			using (var db = new GalleryContext()) {
				var query = (from c in db.GalleryImages
							 join g in db.Galleries on c.GalleryId equals g.GalleryId
							 where g.SiteId == _siteid
							 orderby c.ImageOrder ascending
							 select c);

				query = query.SortByParm(srt.SortField, srt.SortDirection);

				model.DataSource = query.Skip(model.PageSize * (model.PageNumber - 1))
										.Take(model.PageSize).ToList();

				model.TotalRecords = (from c in db.GalleryImages
									  join g in db.Galleries on c.GalleryId equals g.GalleryId
									  where g.SiteId == _siteid
									  orderby c.ImageOrder ascending
									  select c).Count();
			}

			ModelState.Clear();

			return View(model);
		}

		public IActionResult Edit(Guid id) {
			return View(id);
		}

		public IActionResult View(Guid id) {
			var model = new GalleryImage();

			using (var db = new GalleryContext()) {
				model = (from c in db.GalleryImages
						 where c.GalleryImageId == id
						 select c).FirstOrDefault();
			}

			return View(model);
		}

		[HttpGet]
		public IActionResult GalleryList() {
			var model = new PagedData<Gallery>();

			model.InitOrderBy(x => x.GalleryTitle, true);

			using (var db = new GalleryContext()) {
				model.DataSource = (from c in db.Galleries
									where c.SiteId == _siteid
									orderby c.GalleryTitle ascending
									select c).Skip(model.PageSize * (model.PageNumber - 1)).Take(model.PageSize).ToList();

				model.TotalRecords = (from c in db.Galleries
									  where c.SiteId == _siteid
									  orderby c.GalleryTitle ascending
									  select c).Count();
			}

			return View(model);
		}

		[HttpPost]
		public IActionResult GalleryList(PagedData<Gallery> model) {
			model.ToggleSort();
			var srt = model.ParseSort();

			using (var db = new GalleryContext()) {
				IQueryable<Gallery> query = (from c in db.Galleries
											 where c.SiteId == _siteid
											 orderby c.GalleryTitle ascending
											 select c);

				query = query.SortByParm(srt.SortField, srt.SortDirection);

				model.DataSource = query.Skip(model.PageSize * (model.PageNumber - 1))
										.Take(model.PageSize).ToList();

				model.TotalRecords = (from c in db.Galleries
									  where c.SiteId == _siteid
									  orderby c.GalleryTitle ascending
									  select c).Count();
			}

			ModelState.Clear();

			return View(model);
		}

		public IActionResult GalleryView2(Guid id) {
			var model = new GalleryTest();
			model.Settings = (GallerySettings)this.WidgetPayload;

			var data = this.CreateController(typeof(HomeController), "ShowPrettyPhotoGallery", this.AreaName, this.WidgetPayload);
			Controller ctrl = data.Controller;

			var result = ((HomeController)ctrl).ShowPrettyPhotoGallery();
			model.PartialResult = result;

			string viewName = model.Settings.AlternateViewFile ?? result.ViewName;

			if (string.IsNullOrWhiteSpace(viewName)) {
				viewName = "ShowPrettyPhotoGallery";
			}

			model.RenderedContent = result.ResultToString(data, viewName);

			ViewBag.WidgetTitle = "Test Widget Display 2";

			return View(model);
		}

		public ViewResult GalleryView4(Guid id) {
			var model = new GalleryTest();
			model.Settings = (GallerySettings)this.WidgetPayload;

			var ctrl = this.CreateController(typeof(HomeController), "ShowPrettyPhotoGallery", this.AreaName, this.WidgetPayload);
			model.PartialResult = RenderWidgetHelper.ExecuteAction(ctrl);
			model.RenderedContent = model.PartialResult.ResultToString(ctrl);

			ViewBag.WidgetTitle = "Test Widget Display 4";

			return View("GalleryView2", model);
		}

		public IActionResult GalleryView(Guid id) {
			var settings = new GallerySettings();

			if (WidgetPayload is GallerySettings) {
				settings = (GallerySettings)WidgetPayload;
				settings.LoadData();
			}

			var model = BuildModel(settings);

			return View(model);
		}

		public IActionResult Index2() {
			var lst = new List<GalleryImage>();

			using (var db = new GalleryContext()) {
				lst = (from c in db.GalleryImages
					   select c).ToList();
			}

			return View(lst);
		}
	}
}