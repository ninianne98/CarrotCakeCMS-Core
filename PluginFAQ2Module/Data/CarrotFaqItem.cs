using System.ComponentModel.DataAnnotations;

namespace CarrotCake.CMS.Plugins.FAQ2.Data;

public partial class CarrotFaqItem {
	public Guid FaqItemId { get; set; } = Guid.Empty;

	public Guid FaqCategoryId { get; set; } = Guid.Empty;

	[Required]
	[Display(Name = "Caption")]
	public string? Caption { get; set; } = string.Empty;

	[Required]
	[Display(Name = "A")]
	public string? Answer { get; set; } = string.Empty;

	[Required]
	[Display(Name = "Q")]
	public string? Question { get; set; } = string.Empty;

	[Required]
	[Display(Name = "Is Active")]
	public bool IsActive { get; set; }

	[Required]
	[Display(Name = "Item Order")]
	public int ItemOrder { get; set; } = 0;

	public virtual CarrotFaqCategory FaqCategory { get; set; } = null!;
}