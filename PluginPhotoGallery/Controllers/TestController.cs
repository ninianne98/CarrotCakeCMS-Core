using CarrotCake.CMS.Plugins.PhotoGallery.Data;
using CarrotCake.CMS.Plugins.PhotoGallery.Models;
using Carrotware.CMS.Interface;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarrotCake.CMS.Plugins.PhotoGallery.Controllers {

	[WidgetController(typeof(BasePublicController))]
	public class TestController : BasePublicController {
		private GalleryHelper helper = new GalleryHelper();

		protected readonly IWebHostEnvironment _webHostEnvironment;
		protected readonly ICarrotSite _site;

		public TestController(IWebHostEnvironment environment, ICarrotSite site) {
			_site = site;
			_webHostEnvironment = environment;
		}

		public override void OnActionExecuting(ActionExecutingContext context) {
			base.OnActionExecuting(context);

			RouteValueDictionary vals = context.RouteData.Values;

			// use the test id to build a fake payload so the widget can be loaded for dev
			string action = vals["action"].ToString().ToLowerInvariant();
			string controller = vals["controller"].ToString().ToLowerInvariant();

			var settings = new GallerySettings();
			settings.SiteID = new Guid(this.TestSiteID);
			settings.ShowHeading = true;

			if (vals.ContainsKey("id")) {
				string id = vals["id"].ToString().ToLowerInvariant();
				settings.GalleryId = new Guid(id);
				settings.WidgetClientID = "Widget_" + id.Substring(0, 5);
			}

			settings.PublicParmValues.Add("SiteID", this.TestSiteID);
			settings.PublicParmValues.Add("WidgetClientID", settings.WidgetClientID);
			settings.PublicParmValues.Add("GalleryId", settings.GalleryId.ToString().ToLowerInvariant());
			settings.PublicParmValues.Add("ShowHeading", settings.ShowHeading.ToString());

			WidgetPayload = settings;
		}

		[HttpGet]
		public IActionResult Index() {
			var model = new PagedData<GalleryImage>();
			model.InitOrderBy(x => x.ImageOrder, true);

			using (GalleryContext db = new GalleryContext()) {
				model.DataSource = (from c in db.GalleryImages
									join g in db.Galleries on c.GalleryId equals g.GalleryId
									where g.SiteId == _site.SiteID
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

			using (GalleryContext db = new GalleryContext()) {
				IQueryable<GalleryImage> query = (from c in db.GalleryImages
												  join g in db.Galleries on c.GalleryId equals g.GalleryId
												  where g.SiteId == _site.SiteID
												  orderby c.ImageOrder ascending
												  select c);

				query = query.SortByParm(srt.SortField, srt.SortDirection);

				model.DataSource = query.Skip(model.PageSize * (model.PageNumber - 1)).Take(model.PageSize).ToList();

				model.TotalRecords = (from c in db.GalleryImages
									  join g in db.Galleries on c.GalleryId equals g.GalleryId
									  where g.SiteId == _site.SiteID
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
			GalleryImage model = new GalleryImage();

			using (GalleryContext db = new GalleryContext()) {
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

			using (GalleryContext db = new GalleryContext()) {
				model.DataSource = (from c in db.Galleries
									where c.SiteId == _site.SiteID
									orderby c.GalleryTitle ascending
									select c).Skip(model.PageSize * (model.PageNumber - 1)).Take(model.PageSize).ToList();

				model.TotalRecords = (from c in db.Galleries
									  where c.SiteId == _site.SiteID
									  orderby c.GalleryTitle ascending
									  select c).Count();
			}

			return View(model);
		}

		[HttpPost]
		public IActionResult GalleryList(PagedData<Gallery> model) {
			model.ToggleSort();
			var srt = model.ParseSort();

			using (GalleryContext db = new GalleryContext()) {
				IQueryable<Gallery> query = (from c in db.Galleries
											 where c.SiteId == _site.SiteID
											 orderby c.GalleryTitle ascending
											 select c);

				query = query.SortByParm(srt.SortField, srt.SortDirection);

				model.DataSource = query.Skip(model.PageSize * (model.PageNumber - 1)).Take(model.PageSize).ToList();

				model.TotalRecords = (from c in db.Galleries
									  where c.SiteId == _site.SiteID
									  orderby c.GalleryTitle ascending
									  select c).Count();
			}

			ModelState.Clear();

			return View(model);
		}

		public IActionResult GalleryView2(Guid id) {
			var model = new GalleryTest();
			model.Settings = (GallerySettings)this.WidgetPayload;

			var data = RenderWidgetHelper.CreateController(typeof(HomeController), this, "ShowPrettyPhotoGallery", this.AssemblyName, this.WidgetPayload);
			Controller ctrl = data.Controller;

			var result = ((HomeController)ctrl).ShowPrettyPhotoGallery();
			model.PartialResult = result;

			string viewName = model.Settings.AlternateViewFile ?? result.ViewName;

			if (string.IsNullOrEmpty(viewName)) {
				viewName = "ShowPrettyPhotoGallery";
			}

			model.RenderedContent = RenderWidgetHelper.ResultToString(data, result, viewName);

			ViewBag.WidgetTitle = "Test Widget Display 2";

			return View(model);
		}

		public ViewResult GalleryView4(Guid id) {
			var model = new GalleryTest();
			model.Settings = (GallerySettings)this.WidgetPayload;

			var ctrl = RenderWidgetHelper.CreateController(typeof(HomeController), this, "ShowPrettyPhotoGallery", this.AssemblyName, this.WidgetPayload);
			model.PartialResult = RenderWidgetHelper.ExecuteAction(ctrl);
			model.RenderedContent = RenderWidgetHelper.ResultToString(ctrl, model.PartialResult);

			ViewBag.WidgetTitle = "Test Widget Display 4";

			return View("GalleryView2", model);
		}

		public IActionResult GalleryView(Guid id) {
			GallerySettings settings = new GallerySettings();

			if (WidgetPayload is GallerySettings) {
				settings = (GallerySettings)WidgetPayload;
				settings.LoadData();
			}

			var model = BuildModel(settings);

			return View(model);
		}

		public IActionResult Index2() {
			List<GalleryImage> lst = new List<GalleryImage>();

			using (GalleryContext db = new GalleryContext()) {
				lst = (from c in db.GalleryImages
					   select c).ToList();
			}

			return View(lst);
		}
	}
}