using Carrotware.CMS.Interface.Controllers;
using Northwind.Data;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Models {

	public class TestModel {

		public TestModel() {
			if (string.IsNullOrEmpty(this.SelectedView)) {
				this.SelectedView = this.Views.FirstOrDefault();
			}
			this.ProductSearch = new ProductSearch();
		}

		[Display(Name = "Selected View")]
		public string SelectedView { get; set; }

		private List<string> _views = new List<string>();

		public List<string> Views {
			get {
				if (_views == null || !_views.Any()) {
					if (BaseWidgetController.WidgetStandaloneMode) {
						_views.Add("~/Views/Home/ProductSearchMulti.cshtml");
						_views.Add("~/Views/Home/ProductSearchAltMulti.cshtml");
						_views.Add("~/Views/Home/ProductSearchAlt2Multi.cshtml");
					} else {
						_views.Add("~/Views/Northwind/Home/ProductSearchMulti.cshtml");
						_views.Add("~/Views/Northwind/Home/ProductSearchAltMulti.cshtml");
						_views.Add("~/Views/Northwind/Home/ProductSearchAlt2Multi.cshtml");
					}
				}

				return _views;
			}
		}

		private List<Category> _cats = new List<Category>();

		public List<Category> Categories {
			get {
				if (_cats == null || !_cats.Any()) {
					using (var db = new NorthwindContext()) {
						_cats = db.Categories.ToList();
					}
				}

				return _cats;
			}
		}

		[Display(Name = "Selected Categories")]
		public List<string> SelectedCategories { get; set; }

		public ProductSearch ProductSearch { get; set; }
	}
}