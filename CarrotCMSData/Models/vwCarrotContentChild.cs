using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {
	public partial class vwCarrotContentChild {
		public Guid SiteId { get; set; }
		[Key]
		public Guid RootContentId { get; set; }
		public string FileName { get; set; } = null!;
		public DateTime RetireDate { get; set; }
		public DateTime GoLiveDate { get; set; }
		public bool? IsRetired { get; set; }
		public bool? IsUnReleased { get; set; }
		public Guid ParentContentId { get; set; }
		public string ParentFileName { get; set; } = null!;
		public DateTime ParentRetireDate { get; set; }
		public DateTime ParentGoLiveDate { get; set; }
		public bool? IsParentRetired { get; set; }
		public bool? IsParentUnReleased { get; set; }
	}
}
