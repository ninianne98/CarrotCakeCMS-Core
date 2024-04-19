using CarrotCake.CMS.Plugins.PhotoGallery.Data;
using CarrotCake.CMS.Plugins.PhotoGallery.Models;
using Carrotware.CMS.Interface;
using Carrotware.CMS.Interface.Controllers;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

	[WidgetController(typeof(AdminController))]
	public class AdminController : BaseAdminWidgetController {
		protected readonly IWebHostEnvironment _webenv;
		protected readonly ICarrotSite _site;

		public AdminController(IWebHostEnvironment environment, ICarrotSite site) {
			_site = site;
			_webenv = environment;
		}

		public ActionResult Index() {
			PagedData<GalleryGroup> model = new PagedData<GalleryGroup>();
			model.InitOrderBy(x => x.GalleryTitle, true);
			model.PageSize = 25;
			model.PageNumber = 1;

			return Index(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(PagedData<GalleryGroup> model) {
			GalleryHelper gh = new GalleryHelper(_site.SiteID);

			model.ToggleSort();
			var srt = model.ParseSort();

			List<GalleryGroup> lst = gh.GalleryGroupListGetBySiteID();

			IQueryable<GalleryGroup> query = lst.AsQueryable();
			query = query.SortByParm(srt.SortField, srt.SortDirection);

			model.DataSource = query.Skip(model.PageSize * model.PageNumberZeroIndex).Take(model.PageSize).ToList();

			model.TotalRecords = lst.Count();

			ModelState.Clear();

			return View(model);
		}

		public ActionResult EditGallery(Guid id) {
			GalleryHelper gh = new GalleryHelper(_site.SiteID);

			return View("EditGallery", gh.GalleryGroupGetByID(id));
		}

		public ModelStateDictionary ClearChildGalleryValid(ModelStateDictionary modelState) {
			// these child objects are for display only, and their validation is not needed
			foreach (var ms in modelState.ToArray()) {
				if (ms.Key.ToLowerInvariant().Contains("galleryimages")) {
					modelState.Remove(ms.Key);
				}
			}

			return modelState;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditGallery(GalleryGroup model) {
			ClearChildGalleryValid(ModelState);

			if (ModelState.IsValid) {
				GalleryHelper gh = new GalleryHelper(_site.SiteID);
				GalleryGroup m = gh.GalleryGroupGetByID(model.GalleryId);
				if (m == null) {
					m = new GalleryGroup();
					m.SiteID = _site.SiteID;
				}

				m.GalleryTitle = model.GalleryTitle;
				m.Save();

				return RedirectToAction("Index");
			} else {
				return View("EditGallery", model);
			}
		}

		public ActionResult CreateGallery() {
			return View("EditGallery", new GalleryGroup());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateGallery(GalleryGroup model) {
			return EditGallery(model);
		}

		public ActionResult EditGalleryPhotos(Guid id) {
			EditPhotoGalleryModel model = new EditPhotoGalleryModel(_site.SiteID, id);

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditGalleryPhotos(EditPhotoGalleryModel model) {
			model.SetSrcFiles();

			if (!model.SaveGallery) {
				model.LoadGallery();
				return View(model);
			} else {
				model.Save();
				model.LoadGallery();
				return RedirectToAction("EditGalleryPhotos", new { @id = model.GalleryId });
			}
		}

		public ActionResult GalleryDatabase() {
			List<string> lst = new List<string>();

			using (var ctx = new GalleryContext()) {
				try {
					ctx.Database.Initialize();
				} catch (Exception ex) {
					throw;
				}
			}

			return View(lst);
		}

		[HttpGet]
		public ActionResult EditImageMetaData(string path) {
			GalleryHelper gh = new GalleryHelper(_site.SiteID);
			string imageFile = string.Empty;

			if (!string.IsNullOrEmpty(path)) {
				imageFile = path.DecodeBase64();
			}

			ValidateGalleryImage(imageFile);

			GalleryMetaData model = gh.GalleryMetaDataGetByFilename(imageFile);
			if (model == null) {
				model = new GalleryMetaData();
				model.SiteID = _site.SiteID;
				model.GalleryImageMetaID = Guid.Empty;
				model.GalleryImageName = imageFile;
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditImageMetaData(GalleryMetaData model) {
			GalleryHelper gh = new GalleryHelper(_site.SiteID);

			GalleryMetaData meta = gh.GalleryMetaDataGetByFilename(model.GalleryImageName);

			if (meta == null) {
				meta = new GalleryMetaData();
				meta.GalleryImageMetaID = Guid.Empty;
				meta.SiteID = _site.SiteID;
				meta.GalleryImageName = model.GalleryImageName.ToLower();
			}

			meta.ValidateGalleryImage();

			meta.ImageMetaData = model.ImageMetaData;
			meta.ImageTitle = model.ImageTitle;
			meta.Save();

			return RedirectToAction("EditImageMetaData", new { @path = meta.GalleryImageName.EncodeBase64() });
		}

		protected void ValidateGalleryImage(string imageFile) {
			if (imageFile.Contains("../") || imageFile.Contains(@"..\")) {
				throw new Exception("Cannot use relative paths.");
			}
			if (imageFile.Contains(":")) {
				throw new Exception("Cannot specify drive letters.");
			}
			if (imageFile.Contains("//") || imageFile.Contains(@"\\")) {
				throw new Exception("Cannot use UNC paths.");
			}
			if (imageFile.Contains("<") || imageFile.Contains(">")) {
				throw new Exception("Cannot include html tags.");
			}
		}
	}
}