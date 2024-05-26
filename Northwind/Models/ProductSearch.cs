﻿using Northwind.Data;

namespace Northwind.Models {

	public class ProductSearch {

		public ProductSearch() {
			this.AltViewName = string.Empty;

			this.Options = new List<Category>();
			this.Results = new List<Product>();
		}

		public List<Category> Options { get; set; }

		public int? SelectedCat { get; set; }

		public List<Product> Results { get; set; }

		public string AltViewName { get; set; }
	}
}