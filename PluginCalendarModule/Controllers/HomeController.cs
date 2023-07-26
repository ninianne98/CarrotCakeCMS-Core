using CarrotCake.CMS.Plugins.CalendarModule.Data;
using CarrotCake.CMS.Plugins.CalendarModule.Models;
using Carrotware.CMS.Interface;
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

	[WidgetController(typeof(HomeController))]
	public class HomeController : BaseController {
		private CalendarContext db = CalendarContext.Create();

		protected readonly IWebHostEnvironment _webenv;
		protected readonly ICarrotSite _site;

		public HomeController(IWebHostEnvironment environment, ICarrotSite site) {
			_site = site;
			_webenv = environment;
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			if (db != null) {
				db.Dispose();
			}
		}

		public ActionResult Index() {
			return View();
		}

		[HttpGet]
		public ActionResult CalendarDisplay() {
			CalendarViewModel model = new CalendarViewModel();

			return CalendarDisplay(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[WidgetActionSettingModel(typeof(CalendarDisplaySettings))]
		public ActionResult CalendarDisplay(CalendarViewModel model) {
			var settings = model.GetSettings();

			if (string.IsNullOrEmpty(model.EncodedSettings)) {
				var payload = new CalendarDisplaySettings();

				if (this.WidgetPayload is CalendarDisplaySettings) {
					payload = (CalendarDisplaySettings)this.WidgetPayload;
					payload.LoadData();
				}

				settings = model.ConvertSettings(payload);
				model.AssignSettings(settings);

				model.SetSettings(payload);
			}

			ModelState.Clear();

			model.LoadData(settings.SiteID, true);

			if (string.IsNullOrEmpty(settings.AlternateViewFile)) {
				return PartialView(model);
			} else {
				return PartialView(settings.AlternateViewFile, model);
			}
		}

		[HttpGet]
		public ActionResult CalendarDisplaySimple() {
			CalendarViewModel model = new CalendarViewModel();

			return CalendarDisplaySimple(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[WidgetActionSettingModel(typeof(CalendarSimpleSettings))]
		public ActionResult CalendarDisplaySimple(CalendarViewModel model) {
			var settings = model.GetSettings();

			if (string.IsNullOrEmpty(model.EncodedSettings)) {
				var payload = new CalendarSimpleSettings();

				if (this.WidgetPayload is CalendarSimpleSettings) {
					payload = (CalendarSimpleSettings)this.WidgetPayload;
					payload.LoadData();
				}

				settings = model.ConvertSettings(payload);
				model.AssignSettings(settings);

				model.SetSettings(payload);
			}

			ModelState.Clear();

			model.LoadData(settings.SiteID, true);

			if (string.IsNullOrEmpty(settings.AlternateViewFile)) {
				return PartialView(model);
			} else {
				return PartialView(settings.AlternateViewFile, model);
			}
		}

		[WidgetActionSettingModel(typeof(CalendarUpcomingSettings))]
		public ActionResult CalendarUpcoming() {
			var payload = new CalendarUpcomingSettings();

			if (this.WidgetPayload is CalendarUpcomingSettings) {
				payload = (CalendarUpcomingSettings)this.WidgetPayload;
				payload.LoadData();
			}

			ViewBag.CalendarPageUri = payload.CalendarPageUri;

			DateTime dtStart = DateTime.Now.Date.AddDays(payload.DaysInPast);
			DateTime dtEnd = DateTime.Now.Date.AddDays(payload.DaysInFuture);

			List<CalendarEntry> model = (from c in db.CalendarDates
										 where c.EventDate >= dtStart
										  && c.EventDate < dtEnd
										  && c.IsActive == true
										  && c.SiteID == payload.SiteID
										 orderby c.EventDate ascending
										 select c).ToList();

			if (string.IsNullOrEmpty(payload.AlternateViewFile)) {
				return PartialView(model);
			} else {
				return PartialView(payload.AlternateViewFile, model);
			}
		}

		[WidgetActionSettingModel(typeof(CalendarSimpleSettings))]
		public ActionResult CalendarDateInfo(DateTime? calendardate) {
			DateTime theEventDate = calendardate ?? DateTime.Now.Date;

			var payload = new CalendarSimpleSettings();

			if (this.WidgetPayload is CalendarSimpleSettings) {
				payload = (CalendarSimpleSettings)this.WidgetPayload;
				payload.LoadData();
			}

			DateModel model = new DateModel(theEventDate, payload.SiteID);

			if (string.IsNullOrEmpty(payload.AlternateViewFile)) {
				return PartialView(model);
			} else {
				return PartialView(payload.AlternateViewFile, model);
			}
		}
	}
}