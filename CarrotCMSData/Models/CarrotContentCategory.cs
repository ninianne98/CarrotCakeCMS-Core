using System;
using System.Collections.Generic;

namespace Carrotware.CMS.Data.Models {
	public partial class CarrotContentCategory {
		public CarrotContentCategory() {
			CarrotCategoryContentMappings = new HashSet<CarrotCategoryContentMapping>();
		}

		public Guid ContentCategoryId { get; set; }
		public Guid SiteId { get; set; }
		public string CategoryText { get; set; } = null!;
		public string CategorySlug { get; set; } = null!;
		public bool IsPublic { get; set; }

		public virtual CarrotSite Site { get; set; } = null!;
		public virtual ICollection<CarrotCategoryContentMapping> CarrotCategoryContentMappings { get; set; }
	}
}
