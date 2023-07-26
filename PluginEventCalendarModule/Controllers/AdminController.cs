using CarrotCake.CMS.Plugins.EventCalendarModule.Data;
using CarrotCake.CMS.Plugins.EventCalendarModule.Models;
using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
using Carrotware.CMS.Interface.Controllers;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Policy;

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

	[WidgetController(typeof(AdminController))]
	public class AdminController : BaseAdminWidgetController {
		private CalendarContext db = CalendarContext.GetDataContext();

		protected readonly IWebHostEnvironment _webenv;
		protected readonly ICarrotSite _site;

		public AdminController(IWebHostEnvironment environment, ICarrotSite site) {
			_site = site;
			_webenv = environment;
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			if (db != null) {
				db.Dispose();
			}
		}

		public override void OnActionExecuting(ActionExecutingContext context) {
			base.OnActionExecuting(context);

			if (this.TestSiteID != Guid.Empty.ToString()) {
				this.SiteID = new Guid(this.TestSiteID);
			}
			if (_site != null) {
				this.SiteID = _site.SiteID;
			}
			if (this.SiteID == Guid.Empty && SiteData.CurrentSiteExists) {
				this.SiteID = SiteData.CurrentSiteID;
			}
		}

		public IActionResult Index() {
			return View();
		}

		public IActionResult Database() {
			List<string> lst = new List<string>();

			CalendarHelper.SeedCalendarCategories(this.SiteID);

			return View(lst);
		}

		public IActionResult CategoryList() {
			CalendarHelper.SeedCalendarCategories(this.SiteID);

			var model = new PagedData<CalendarEventCategory>();
			model.InitOrderBy(x => x.CategoryName);

			return CategoryList(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CategoryList(PagedData<CalendarEventCategory> model) {
			model.ToggleSort();
			var srt = model.ParseSort();

			var query = CalendarHelper.GetCalendarCategories(this.SiteID).SortByParm(srt.SortField, srt.SortDirection);
			model.DataSource = query.ToList();
			model.TotalRecords = query.Count();

			ModelState.Clear();
			return View(model);
		}

		public IActionResult CategoryDetail(Guid? id) {
			var itemId = id.HasValue ? id.Value : Guid.Empty;
			var model = new CalendarEventCategory();

			if (itemId == Guid.Empty) {
				model.CalendarEventCategoryId = itemId;
				model.CategoryBGColor = CalendarHelper.HEX_White;
				model.CategoryFGColor = CalendarHelper.HEX_Black;
			} else {
				model = CalendarHelper.GetCalendarCategory(itemId);
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CategoryDetail(CalendarEventCategory model) {
			if (ModelState.IsValid) {
				bool bAdd = false;

				using (CalendarContext db = CalendarContext.GetDataContext()) {
					var itm = (from c in db.CalendarEventCategories
							   where c.CalendarEventCategoryId == model.CalendarEventCategoryId
							   select c).FirstOrDefault();

					if (itm == null) {
						bAdd = true;
						itm = new CalendarEventCategory();
						itm.CalendarEventCategoryId = Guid.NewGuid();
						itm.SiteID = this.SiteID;

						model.CalendarEventCategoryId = itm.CalendarEventCategoryId;
					}

					itm.CategoryName = model.CategoryName;
					itm.CategoryFGColor = model.CategoryFGColor;
					itm.CategoryBGColor = model.CategoryBGColor;

					if (bAdd) {
						db.CalendarEventCategories.Add(itm);
					}

					db.SaveChanges();
				}
				return RedirectToAction(this.GetActionName(x => x.CategoryDetail(model.CalendarEventCategoryId)), new { @id = model.CalendarEventCategoryId });
			}

			return View(model);
		}

		public IActionResult ProfileList() {
			CalendarHelper.SeedCalendarCategories(this.SiteID);

			var model = new ProfileDisplayModel(this.SiteID);
			model.Load();

			return View(model);
		}

		[HttpPost]
		public IActionResult ProfileList(ProfileDisplayModel model) {
			var selected = model.SelectedValue;
			model.SiteID = this.SiteID;
			model.Load();

			ModelState.Clear();
			return View(model);
		}

		public IActionResult EventDetail(Guid? id) {
			CalendarHelper.SeedCalendarCategories(this.SiteID);

			var itemId = id.HasValue ? id.Value : Guid.Empty;
			var model = new EventDetailModel(this.SiteID, itemId);

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EventDetail(EventDetailModel model) {
			model.ClearOptionalItems(ModelState);
			model.SiteID = this.SiteID;

			if (model.Operation.ToLowerInvariant() == "delete") {
				CalendarHelper.RemoveEvent(model.ItemID);
				return RedirectToAction(this.GetActionName(x => x.ProfileList()));
			}

			if (ModelState.IsValid) {
				bool bAdd = false;
				if (model.Operation.ToLowerInvariant() == "copy") {
					var p = CalendarHelper.CopyEvent(model.ItemID);
					model.ItemID = p.CalendarEventProfileId;
					model.ItemData.CalendarEventProfileId = p.CalendarEventProfileId;
				}

				var currItem = (from c in db.CalendarEventProfiles
								where c.CalendarEventProfileId == model.ItemID
								select c).FirstOrDefault();

				var origItem = new CalendarEvent(currItem);

				if (currItem == null) {
					bAdd = true;
					model.ItemID = Guid.NewGuid();
					currItem = new CalendarEventProfile();
					currItem.CalendarEventProfileId = model.ItemID;
					currItem.SiteID = this.SiteID;
					currItem.IsHoliday = false;
					currItem.IsAnnualHoliday = false;
					currItem.RecursEvery = 1;
				}

				currItem.CalendarFrequencyId = model.ItemData.CalendarFrequencyId;
				currItem.CalendarEventCategoryId = model.ItemData.CalendarEventCategoryId;

				currItem.EventRepeatPattern = null;

				List<int> days = model.DaysOfTheWeek.Where(x => x.Selected).Select(x => int.Parse(x.Value)).ToList();

				if (CalendarFrequencyHelper.GetFrequencyTypeByID(currItem.CalendarFrequencyId) == CalendarFrequencyHelper.FrequencyType.Weekly
							&& days.Count > 0) {
					int dayMask = (from d in days select d).Sum();

					if (dayMask > 0) {
						currItem.EventRepeatPattern = dayMask;
					}
				}

				currItem.EventTitle = model.ItemData.EventTitle;
				currItem.EventDetail = model.ItemData.EventDetail;
				currItem.RecursEvery = model.ItemData.RecursEvery;

				currItem.IsPublic = model.ItemData.IsPublic;
				currItem.IsAllDayEvent = model.ItemData.IsAllDayEvent;
				currItem.IsCancelled = model.ItemData.IsCancelled;
				currItem.IsCancelledPublic = model.ItemData.IsCancelledPublic;

				currItem.EventStartDate = model.ItemData.EventStartDate.Date;
				currItem.EventStartTime = currItem.IsAllDayEvent ? null : CalendarHelper.GetTimeSpan(model.EventStartTime);

				currItem.EventEndDate = model.ItemData.EventEndDate.Date;
				currItem.EventEndTime = currItem.IsAllDayEvent ? null : CalendarHelper.GetTimeSpan(model.EventEndTime);

				if ((currItem.EventEndDate + currItem.EventEndTime) < (currItem.EventStartDate + currItem.EventStartTime)) {
					currItem.EventEndDate = currItem.EventStartDate;
					currItem.EventEndTime = currItem.EventStartTime;
				}

				if (bAdd) {
					db.CalendarEventProfiles.Add(currItem);
				}

				CalendarFrequencyHelper.SaveFrequencies(db, new CalendarEvent(currItem), origItem);

				db.SaveChanges();

				return RedirectToAction(this.GetActionName(x => x.EventDetail(model.ItemID)), new { @id = model.ItemID });
			}

			model.Load();

			return View(model);
		}

		public IActionResult EventList() {
			CalendarHelper.SeedCalendarCategories(this.SiteID);

			CalendarViewModel model = new CalendarViewModel();

			return EventList(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EventList(CalendarViewModel model) {
			if (ModelState.IsValid) {
				ModelState.Clear();

				model.LoadData(this.SiteID, false);
			}

			return View(model);
		}

		public IActionResult EventDetailSingle(Guid? id) {
			var itemId = id.HasValue ? id.Value : Guid.Empty;
			var model = new EventSingleModel(this.SiteID, itemId);

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EventDetailSingle(EventSingleModel model) {
			model.SiteID = this.SiteID;

			if (ModelState.IsValid) {
				bool bAdd = false;

				using (CalendarContext db = CalendarContext.GetDataContext()) {
					var currItem = (from c in db.CalendarEvents
									where c.CalendarEventId == model.ItemID
									select c).FirstOrDefault();

					if (currItem == null) {
						bAdd = true;
						model.ItemID = Guid.NewGuid();
						currItem = new CalendarSingleEvent();
						currItem.CalendarEventId = model.ItemID;
					}

					currItem.EventDetail = model.ItemData.EventDetail;
					currItem.IsCancelled = model.ItemData.IsCancelled;

					currItem.EventStartTime = CalendarHelper.GetTimeSpan(model.EventStartTime);
					currItem.EventEndTime = CalendarHelper.GetTimeSpan(model.EventEndTime);

					if (bAdd) {
						db.CalendarEvents.Add(currItem);
					}

					db.SaveChanges();
				}

				return RedirectToAction(this.GetActionName(x => x.EventDetailSingle(model.ItemID)), new { @id = model.ItemID });
			}

			model.Load();

			return View(model);
		}
	}
}