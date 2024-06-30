namespace Carrotware.CMS.Data.Models {
	public partial class CarrotWidgetData {
		public Guid WidgetDataId { get; set; }
		public Guid RootWidgetId { get; set; }
		public bool IsLatestVersion { get; set; }
		public DateTime EditDate { get; set; }
		public string? ControlProperties { get; set; }

		public virtual CarrotWidget RootWidget { get; set; } = null!;
	}
}
