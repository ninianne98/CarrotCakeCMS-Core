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
		private GalleryHelper _helper;

		public AdminController(IWebHostEnvironment environment, ICarrotSite site) {
			_site = site;
			_webenv = environment;

			_helper = new GalleryHelper(_site.SiteID);
		}

		public ActionResult Index() {
			var model = new PagedData<GalleryGroup>();
			model.InitOrderBy(x => x.GalleryTitle, true);
			model.PageSize = 25;
			model.PageNumber = 1;

			return Index(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(PagedData<GalleryGroup> model) {
			model.ToggleSort();
			var srt = model.ParseSort();

			model.TotalRecords = _helper.GalleryGroupListGetBySiteIDCount();

			var query = _helper.GalleryGroupListGetBySiteID();
			query = query.SortByParm(srt.SortField, srt.SortDirection);

			model.DataSource = query.Skip(model.PageSize * model.PageNumberZeroIndex)
						.Take(model.PageSize).Select(x => new GalleryGroup(x)).ToList();

			ModelState.Clear();

			return View(model);
		}

		public ActionResult EditGallery(Guid id) {
			return View("EditGallery", _helper.GalleryGroupGetByID(id));
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
				GalleryGroup m = _helper.GalleryGroupGetByID(model.GalleryId);
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
					lst.Add(ex.ToString());
					throw;
				}
			}

			return View(lst);
		}

		[HttpGet]
		public ActionResult EditImageMetaData(string path) {
			string imageFile = string.Empty;

			if (!string.IsNullOrEmpty(path)) {
				imageFile = path.DecodeBase64();
			}

			ValidateGalleryImage(imageFile);

			GalleryMetaData model = _helper.GalleryMetaDataGetByFilename(imageFile);
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
			GalleryMetaData meta = _helper.GalleryMetaDataGetByFilename(model.GalleryImageName);

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