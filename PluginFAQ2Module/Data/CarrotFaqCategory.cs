using System.ComponentModel.DataAnnotations;

namespace CarrotCake.CMS.Plugins.FAQ2.Data;

public partial class CarrotFaqCategory {
	public Guid FaqCategoryId { get; set; }

	[Required]
	[Display(Name = "Title")]
	public string? FaqTitle { get; set; } = string.Empty;

	public Guid? SiteId { get; set; }

	public virtual ICollection<CarrotFaqItem> CarrotFaqItems { get; set; } = new List<CarrotFaqItem>();
}