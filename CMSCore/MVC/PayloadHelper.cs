using Carrotware.CMS.Interface;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Core {

	public static class PayloadHelper {

		public static PagePayload GetSamplerPayload() {
			var page = new PagePayload();
			page.ThePage = ContentPageHelper.GetSamplerView();
			page.ThePage.TemplateFile = SiteData.PreviewTemplateFile;

			CarrotHttpHelper.HttpContext.Items[PagePayload.ItemKey] = page;

			return page;
		}

		public static PagePayload SetSamplerPayload(ContentPage cp) {
			var page = new PagePayload();
			page.ThePage = cp;
			page.ThePage.TemplateFile = SiteData.PreviewTemplateFile;

			CarrotHttpHelper.HttpContext.Items[PagePayload.ItemKey] = page;

			return page;
		}

		public static PagePayload GetContentFromContext() {
			var page = new PagePayload();

			if (CarrotHttpHelper.HttpContext.Items[PagePayload.ItemKey] != null) {
				page = CarrotHttpHelper.HttpContext.Items[PagePayload.ItemKey] as PagePayload;
			} else {
				if (SiteData.CurrentRoutePageID != null) {
					page = GetContent();
				} else {
					page = GetSamplerPayload();
				}

				CarrotHttpHelper.HttpContext.Items[PagePayload.ItemKey] = page;
			}

			return page;
		}

		public static PagePayload GetCurrentContent() {
			PagePayload page = new PagePayload();
			page.ThePage = SiteData.GetCurrentPage();

			page.Load();
			return page;
		}

		public static PagePayload GetContent(Guid id) {
			PagePayload page = new PagePayload();
			page.ThePage = SiteData.GetPage(id);

			page.Load();
			return page;
		}

		public static PagePayload GetContent(SiteNav nav) {
			PagePayload page = new PagePayload();
			page.ThePage = nav.GetContentPage();

			page.Load();
			return page;
		}

		public static PagePayload GetContent(ContentPage cp) {
			PagePayload page = new PagePayload();
			page.ThePage = cp;

			page.Load();
			return page;
		}

		public static PagePayload GetContent() {
			return GetContent(CarrotHttpHelper.Request.Path);
		}

		public static PagePayload GetContent(string uri) {
			var page = new PagePayload();

			if (SecurityData.AdvancedEditMode) {
				using (CMSConfigHelper cmsHelper = new CMSConfigHelper()) {
					cmsHelper.OverrideKey(uri);

					if (cmsHelper.cmsAdminContent == null) {
						page.ThePage = SiteData.GetPage(uri);

						if (!page.ThePage.IsPageLocked) {
							if (page.ThePage.ContentType == ContentPageType.PageType.BlogEntry) {
								var c = page.ThePage.ContentCategories;
								var t = page.ThePage.ContentTags;
							}

							cmsHelper.cmsAdminContent = page.ThePage;
						} else {
							cmsHelper.cmsAdminContent = null;
						}
					} else {
						page.ThePage = cmsHelper.cmsAdminContent;
						if (page.IsPageLocked) {
							cmsHelper.cmsAdminContent = null;
							page.ThePage = SiteData.GetPage(uri);
						}
					}
				}
			} else {
				page.ThePage = SiteData.GetPage(uri);
			}

			page.Load();

			CarrotHttpHelper.HttpContext.Items[PagePayload.ItemKey] = page;

			return page;
		}
	}
}