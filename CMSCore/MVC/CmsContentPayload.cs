using Carrotware.CMS.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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

	public class CmsContentPayload {
		private readonly IConfiguration _configuration;
		private readonly ViewContext _viewContext;

		public CmsContentPayload(IConfiguration configuration, ViewContext viewContext) {
			_configuration = configuration;
			_viewContext = viewContext;

			this.Page = new vwCarrotContent();
			this.Widgets = new List<vwCarrotWidget>();
			this.Categories = new List<vwCarrotCategoryUrl>();

			var key = _viewContext.HttpContext?.GetRouteValue(CmsRouting.PageIdKey);

			if (key != null) {
				Guid id = new Guid(key.ToString() ?? "");

				using (var db = CarrotCakeContext.Create()) {
					var content = db.vwCarrotContents
						.Where(x => x.IsLatestVersion && x.RootContentId == id);

					var widgets = db.vwCarrotWidgets
						.OrderBy(x => x.PlaceholderName)
						.OrderBy(x => x.WidgetOrder)
						.Where(x => x.IsLatestVersion && x.RootContentId == id);

					var cats = db.vwCarrotCategoryUrls
						.Where(c => db.CarrotCategoryContentMappings.Where(x => x.RootContentId == id)
								.Select(x => x.ContentCategoryId).Contains(c.ContentCategoryId))
						.OrderBy(x => x.CategoryText)
						.Where(x => x.IsPublic);

					this.Page = content.FirstOrDefault();
					this.Widgets = widgets.ToList();
					this.Categories = cats.ToList();
				}
			}
		}

		public vwCarrotContent Page { get; set; }

		public List<vwCarrotWidget> Widgets { get; set; }

		public List<vwCarrotCategoryUrl> Categories { get; set; }
	}
}