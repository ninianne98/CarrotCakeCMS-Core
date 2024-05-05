using CarrotCake.CMS.Plugins.FAQ2.Data;
using CarrotCake.CMS.Plugins.FAQ2.Models;
using Carrotware.CMS.Interface;
using Microsoft.AspNetCore.Mvc;

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

	[WidgetController(typeof(HomeController))]
	public class HomeController : BaseController {

		public ActionResult Index() {
			return View();
		}

		[WidgetActionSettingModel(typeof(FaqPublic))]
		public ActionResult ShowFaqList() {
			FaqItems model = new FaqItems();
			FaqPublic payload = new FaqPublic();

			if (this.WidgetPayload is FaqPublic) {
				payload = (FaqPublic)this.WidgetPayload;
				payload.LoadData();
			}

			ModelState.Clear();

			if (payload.FaqCategoryID != Guid.Empty) {
				model.Faq = payload.GetFaq();
				model.Items = payload.GetList();
			}

			if (String.IsNullOrEmpty(payload.AlternateViewFile)) {
				return PartialView("FaqList", model);
			} else {
				return PartialView(payload.AlternateViewFile, model);
			}
		}

		[WidgetActionSettingModel(typeof(FaqPublic))]
		public ActionResult ShowRandomFaq() {
			CarrotFaqItem model = new CarrotFaqItem();
			FaqPublic payload = new FaqPublic();

			if (this.WidgetPayload is FaqPublic) {
				payload = (FaqPublic)this.WidgetPayload;
				payload.LoadData();
			}

			ModelState.Clear();

			if (payload.FaqCategoryID != Guid.Empty) {
				model = payload.GetRandomItem();
			}

			if (String.IsNullOrEmpty(payload.AlternateViewFile)) {
				return PartialView("FaqItem", model);
			} else {
				return PartialView(payload.AlternateViewFile, model);
			}
		}

		[WidgetActionSettingModel(typeof(FaqPublicTop))]
		public ActionResult ShowFaqTopList() {
			FaqItems model = new FaqItems();
			FaqPublicTop payload = new FaqPublicTop();

			if (this.WidgetPayload is FaqPublicTop) {
				payload = (FaqPublicTop)this.WidgetPayload;
				payload.LoadData();
			}

			ModelState.Clear();

			if (payload.FaqCategoryID != Guid.Empty) {
				model.Faq = payload.GetFaq();
				model.Items = payload.GetListTop(payload.TakeTop);
			}

			if (String.IsNullOrEmpty(payload.AlternateViewFile)) {
				return PartialView("FaqList", model);
			} else {
				return PartialView(payload.AlternateViewFile, model);
			}
		}
	}
}