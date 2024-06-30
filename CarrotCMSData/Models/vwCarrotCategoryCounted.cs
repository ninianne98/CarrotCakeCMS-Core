using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {
	public partial class vwCarrotCategoryCounted {
		[Key]
		public Guid ContentCategoryId { get; set; }
		public Guid SiteId { get; set; }
		public string CategoryText { get; set; } = null!;
		public string CategorySlug { get; set; } = null!;
		public bool IsPublic { get; set; }
		public int UseCount { get; set; }
	}
}
