using System;
using System.Collections.Generic;

namespace Carrotware.CMS.Data.Models {
	public partial class CarrotContentTag {
		public CarrotContentTag() {
			CarrotTagContentMappings = new HashSet<CarrotTagContentMapping>();
		}

		public Guid ContentTagId { get; set; }
		public Guid SiteId { get; set; }
		public string TagText { get; set; } = null!;
		public string TagSlug { get; set; } = null!;
		public bool IsPublic { get; set; }

		public virtual CarrotSite Site { get; set; } = null!;
		public virtual ICollection<CarrotTagContentMapping> CarrotTagContentMappings { get; set; }
	}
}
