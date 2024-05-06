using CarrotCake.CMS.Plugins.FAQ2.Data;
using CarrotCake.CMS.Plugins.FAQ2.Models;
using Carrotware.CMS.Interface;
using Carrotware.CMS.Interface.Controllers;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

	[WidgetController(typeof(AdminController))]
	public class AdminController : BaseAdminWidgetController {

		public IActionResult Index() {
			var model = new PagedData<CarrotFaqCategory>();
			model.InitOrderBy(x => x.FaqTitle, true);
			model.PageSize = 25;
			model.PageNumber = 1;

			return Index(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(PagedData<CarrotFaqCategory> model) {
			model.ToggleSort();
			var srt = model.ParseSort();

			using (var fh = new FaqHelper(this.SiteID)) {
				var query = fh.CategoryListGetBySiteID();
				query = query.SortByParm(srt.SortField, srt.SortDirection);

				model.TotalRecords = fh.CategoryListGetBySiteIDCount();

				model.DataSource = query.Skip(model.PageSize * model.PageNumberZeroIndex).Take(model.PageSize).ToList();
			}

			ModelState.Clear();

			return View(model);
		}

		public IActionResult CreateFaq() {
			var model = new CarrotFaqCategory();
			model.SiteId = this.SiteID;

			return View("EditFaq", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CreateFaq(CarrotFaqCategory model) {
			return EditFaq(model);
		}

		public IActionResult EditFaq(Guid id) {
			using (var fh = new FaqHelper(this.SiteID)) {
				return View("EditFaq", fh.CategoryGetByID(id));
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EditFaq(CarrotFaqCategory model) {
			if (ModelState.IsValid) {
				using (var fh = new FaqHelper(this.SiteID)) {
					var fc = fh.CategoryGetByID(model.FaqCategoryId);

					if (fc == null || model.FaqCategoryId == Guid.Empty) {
						model.FaqCategoryId = Guid.NewGuid();
						fc = new CarrotFaqCategory();
						fc.SiteId = this.SiteID;
						fc.FaqCategoryId = model.FaqCategoryId;
					}

					fc.FaqTitle = model.FaqTitle;

					fh.Save(fc);
				}

				return RedirectToAction("Index");
			} else {
				return View("EditFaq", model);
			}
		}

		public IActionResult ListFaqItems(Guid id) {
			var model = new FaqListing();
			model.Faq.FaqCategoryId = id;
			model.Faq.SiteId = this.SiteID;

			return ListFaqItems(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ListFaqItems(FaqListing model) {
			model.Items.ToggleSort();
			var srt = model.Items.ParseSort();

			using (var fh = new FaqHelper(this.SiteID)) {
				model.Faq = fh.CategoryGetByID(model.Faq.FaqCategoryId);

				var query = fh.FaqItemListGetByFaqCategoryID(model.Faq.FaqCategoryId);
				query = query.SortByParm<CarrotFaqItem>(srt.SortField, srt.SortDirection);

				model.Items.TotalRecords = fh.FaqItemListGetByFaqCategoryIDCount(model.Faq.FaqCategoryId);

				model.Items.DataSource = query.Skip(model.Items.PageSize * model.Items.PageNumberZeroIndex).Take(model.Items.PageSize).ToList();
			}

			ModelState.Clear();

			return View(model);
		}

		public IActionResult CreateFaqItem(Guid parent) {
			var model = new CarrotFaqItem();
			model.FaqCategoryId = parent;

			return View("EditFaqItem", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CreateFaqItem(CarrotFaqItem model) {
			return EditFaqItem(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteFaqItem(CarrotFaqItem model) {
			using (var fh = new FaqHelper(this.SiteID)) {
				fh.DeleteItem(model.FaqItemId);
			}

			return RedirectToAction("ListFaqItems", new { @id = model.FaqCategoryId });
		}

		public IActionResult EditFaqItem(Guid id) {
			using (var fh = new FaqHelper(this.SiteID)) {
				return View("EditFaqItem", fh.FaqItemGetByID(id));
			}
		}

		protected ModelStateDictionary ClearOptionalItemProperties(ModelStateDictionary modelState) {
			// these child objects are for display only, and their validation is not needed
			foreach (var ms in modelState.ToArray()) {
				if (ms.Key.ToLowerInvariant() == "faqcategory") {
					modelState.Remove(ms.Key);
				}
			}

			return modelState;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EditFaqItem(CarrotFaqItem model) {
			ClearOptionalItemProperties(ModelState);

			if (ModelState.IsValid) {
				using (var fh = new FaqHelper(this.SiteID)) {
					var fc = fh.FaqItemGetByID(model.FaqItemId);

					if (fc == null || model.FaqCategoryId == Guid.Empty) {
						model.FaqItemId = Guid.NewGuid();
						fc = new CarrotFaqItem();
						fc.FaqCategoryId = model.FaqCategoryId;
						fc.FaqItemId = model.FaqItemId;
					}

					fc.Caption = model.Caption;
					fc.Question = model.Question;
					fc.Answer = model.Answer;
					fc.ItemOrder = model.ItemOrder;
					fc.IsActive = model.IsActive;

					fh.Save(fc);
				}

				return RedirectToAction("ListFaqItems", new { @id = model.FaqCategoryId });
			} else {
				return View("EditFaqItem", model);
			}
		}
	}
}