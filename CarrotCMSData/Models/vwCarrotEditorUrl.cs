using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {
	public partial class vwCarrotEditorUrl {
		public Guid SiteId { get; set; }
		[Key]
		public Guid? UserId { get; set; }
		public string UserName { get; set; } = null!;
		public string? LoweredEmail { get; set; }
		public DateTime? EditDate { get; set; }
		public int UseCount { get; set; }
		public int PublicUseCount { get; set; }
		public string? UserUrl { get; set; }
	}
}
