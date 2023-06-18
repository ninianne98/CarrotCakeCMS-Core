using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {
	public partial class vwCarrotCategoryUrl {
		public Guid SiteId { get; set; }
		[Key]
		public Guid ContentCategoryId { get; set; }
		public string CategoryText { get; set; } = null!;
		public bool IsPublic { get; set; }
		public DateTime? EditDate { get; set; }
		public int UseCount { get; set; }
		public int PublicUseCount { get; set; }
		public string? CategoryUrl { get; set; }
	}
}
