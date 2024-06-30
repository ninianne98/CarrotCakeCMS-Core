namespace Carrotware.CMS.Data.Models {
	public partial class CarrotWidget {
		public CarrotWidget() {
			CarrotWidgetData = new HashSet<CarrotWidgetData>();
		}

		public Guid RootWidgetId { get; set; }
		public Guid RootContentId { get; set; }
		public int WidgetOrder { get; set; }
		public string PlaceholderName { get; set; } = null!;
		public string ControlPath { get; set; } = null!;
		public bool WidgetActive { get; set; }
		public DateTime GoLiveDate { get; set; }
		public DateTime RetireDate { get; set; }

		public virtual CarrotRootContent RootContent { get; set; } = null!;
		public virtual ICollection<CarrotWidgetData> CarrotWidgetData { get; set; }
	}
}
