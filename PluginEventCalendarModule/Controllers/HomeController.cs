using CarrotCake.CMS.Plugins.EventCalendarModule.Data;
using CarrotCake.CMS.Plugins.EventCalendarModule.Models;
using Carrotware.CMS.Core;
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

namespace CarrotCake.CMS.Plugins.EventCalendarModule.Controllers {

	[WidgetController(typeof(HomeController))]
	public class HomeController : BaseController {
		private CalendarContext _db = CalendarContext.GetDataContext();

		protected readonly IWebHostEnvironment _webenv;
		protected readonly ICarrotSite _site;

		public HomeController(IWebHostEnvironment environment, ICarrotSite site) {
			_site = site;
			_webenv = environment;
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			if (_db != null) {
				_db.Dispose();
			}
		}

		[WidgetActionSettingModel(typeof(CalendarUpcomingSettings))]
		public IActionResult CalendarUpcoming() {
			SiteData site = SiteData.CurrentSite;

			var payload = new CalendarUpcomingSettings();

			if (this.WidgetPayload is CalendarUpcomingSettings) {
				payload = (CalendarUpcomingSettings)this.WidgetPayload;
				payload.LoadData();
			}

			ViewBag.CalendarPageUri = payload.CalendarPageUri;

			DateTime dtStart = site.Now.Date.AddDays(payload.DaysInPast);
			DateTime dtEnd = site.Now.Date.AddDays(payload.DaysInFuture);

			var model = CalendarHelper.GetDisplayEvents(payload.SiteID, dtStart, dtEnd, payload.TakeTop, true);

			model = CalendarHelper.MassageDateTime(model);

			if (string.IsNullOrEmpty(payload.AlternateViewFile)) {
				return PartialView(model);
			} else {
				return PartialView(payload.AlternateViewFile, model);
			}
		}

		[HttpGet]
		[WidgetActionSettingModel(typeof(CalendarDisplaySettings))]
		public IActionResult EventCalendarDisplay() {
			CalendarViewModel model = new CalendarViewModel();

			return EventCalendarDisplay(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[WidgetActionSettingModel(typeof(CalendarDisplaySettings))]
		public IActionResult EventCalendarDisplay(CalendarViewModel model) {
			var settings = new CalendarViewSettings();
			var payload = new CalendarDisplaySettings();

			if (string.IsNullOrWhiteSpace(model.EncodedSettings)) {
				if (this.WidgetPayload is CalendarDisplaySettings) {
					payload = (CalendarDisplaySettings)this.WidgetPayload;
					payload.LoadData();
				}
				settings = model.ConvertSettings(payload);
			} else {
				settings = model.GetSettings();
			}

			model.AssignSettings(settings);

			ModelState.Clear();

			model.LoadData(settings.SiteID, true);

			if (string.IsNullOrEmpty(settings.AlternateViewFile)) {
				return PartialView(model);
			} else {
				return PartialView(settings.AlternateViewFile, model);
			}
		}

		[HttpGet]
		[WidgetActionSettingModel(typeof(CalendarSimpleSettings))]
		public IActionResult EventCalendarDisplay2() {
			CalendarViewModel model = new CalendarViewModel();

			return EventCalendarDisplay2(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[WidgetActionSettingModel(typeof(CalendarSimpleSettings))]
		public IActionResult EventCalendarDisplay2(CalendarViewModel model) {
			var settings = new CalendarViewSettings();
			var payload = new CalendarSimpleSettings();

			if (string.IsNullOrWhiteSpace(model.EncodedSettings)) {
				if (this.WidgetPayload is CalendarSimpleSettings) {
					payload = (CalendarSimpleSettings)this.WidgetPayload;
					payload.LoadData();
				}
				settings = model.ConvertSettings(payload);
			} else {
				settings = model.GetSettings();
			}

			model.AssignSettings(settings);

			ModelState.Clear();

			model.LoadData(settings.SiteID, true);

			if (string.IsNullOrEmpty(settings.AlternateViewFile)) {
				return PartialView(model);
			} else {
				return PartialView(settings.AlternateViewFile, model);
			}
		}

		public IActionResult Index() {
			return View();
		}
	}
}