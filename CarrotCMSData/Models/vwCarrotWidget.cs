using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {
	public partial class vwCarrotWidget {
		[Key]
		public Guid RootWidgetId { get; set; }
		public Guid RootContentId { get; set; }
		public int WidgetOrder { get; set; }
		public string PlaceholderName { get; set; } = null!;
		public string ControlPath { get; set; } = null!;
		public DateTime GoLiveDate { get; set; }
		public DateTime RetireDate { get; set; }
		public bool? IsRetired { get; set; }
		public bool? IsUnReleased { get; set; }
		public bool WidgetActive { get; set; }
		public Guid WidgetDataId { get; set; }
		public bool IsLatestVersion { get; set; }
		public DateTime EditDate { get; set; }
		public string? ControlProperties { get; set; }
		public Guid SiteId { get; set; }
	}
}
