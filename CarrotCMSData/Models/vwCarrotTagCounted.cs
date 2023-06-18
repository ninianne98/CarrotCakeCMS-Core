using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {
	public partial class vwCarrotTagCounted {
		[Key]
		public Guid ContentTagId { get; set; }
		public Guid SiteId { get; set; }
		public string TagText { get; set; } = null!;
		public string TagSlug { get; set; } = null!;
		public bool IsPublic { get; set; }
		public int UseCount { get; set; }
	}
}
