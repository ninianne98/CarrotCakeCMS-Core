using CarrotCake.CMS.Plugins.CalendarModule.Data;
using CarrotCake.CMS.Plugins.CalendarModule.Models;
using Carrotware.CMS.Interface;
using Carrotware.CMS.Interface.Controllers;
using Microsoft.AspNetCore.Mvc;

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

	[WidgetController(typeof(AdminController))]
	public class AdminController : BaseAdminWidgetController {
		private CalendarContext db = CalendarContext.Create();

		protected readonly IWebHostEnvironment _webenv;
		protected readonly ICarrotSite _site;

		public AdminController(IWebHostEnvironment environment, ICarrotSite site) {
			_site = site;
			_webenv = environment;

			if (_site != null) {
				this.SiteID = _site.SiteID;
			}
		}

		[HttpGet]
		public ActionResult Index() {
			CalendarViewModel model = new CalendarViewModel();

			return Index(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(CalendarViewModel model) {
			if (ModelState.IsValid) {
				ModelState.Clear();

				model.LoadData(this.SiteID, false);
			}

			return View(model);
		}

		[HttpGet]
		public ActionResult CalendarDatabase() {
			using (CalendarContext db = CalendarContext.Create()) {
				db.Database.Initialize();
			}
			return View();
		}

		[HttpGet]
		public ActionResult CalendarAdminAdd(Guid? id) {
			return CalendarAdminAddEdit(Guid.Empty);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CalendarAdminAdd(CalendarDetail model) {
			return CalendarAdminAddEdit(model);
		}

		[HttpGet]
		public ActionResult CalendarAdminAddEdit(Guid? id) {
			Guid ItemGuid = id ?? Guid.Empty;

			var model = (from c in db.CalendarDates
						 where c.CalendarID == ItemGuid
							  && SiteID == this.SiteID
						 select new CalendarDetail(c)).FirstOrDefault();

			if (model == null) {
				model = new CalendarDetail {
					SiteID = this.SiteID,
					CalendarID = Guid.Empty,
					IsActive = true,
					EventDate = DateTime.Now.Date
				};
			}

			return View("CalendarAdminAddEdit", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CalendarAdminAddEdit(CalendarDetail model) {
			if (ModelState.IsValid) {
				ModelState.Clear();

				var itm = (from c in db.CalendarDates
						   where c.CalendarID == model.CalendarID
								&& SiteID == this.SiteID
						   select c).FirstOrDefault();

				if (itm == null || model.CalendarID == Guid.Empty) {
					itm = new CalendarEntry {
						SiteID = this.SiteID,
						CalendarID = Guid.NewGuid(),
						EventDate = DateTime.Now.Date
					};

					db.CalendarDates.Add(itm);
				}

				itm.EventDate = model.EventDate;
				itm.EventTitle = model.EventTitle;
				itm.EventDetail = model.EventDetail;
				itm.IsActive = model.IsActive;

				db.SaveChanges();

				return RedirectToAction("CalendarAdminAddEdit", new { @id = itm.CalendarID });
			}

			return View("CalendarAdminAddEdit", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CalendarAdminDelete(CalendarDetail model) {
			var itm = (from c in db.CalendarDates
					   where c.CalendarID == model.CalendarID
							&& SiteID == this.SiteID
					   select c).FirstOrDefault();

			db.CalendarDates.Remove(itm);
			db.SaveChanges();

			return RedirectToAction("Index");
		}

		public ActionResult CalendarAdminCat() {
			return View();
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			if (db != null) {
				db.Dispose();
			}
		}
	}
}