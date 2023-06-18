using System;
using System.Collections.Generic;

namespace Carrotware.CMS.Data.Models {
	public partial class CarrotCategoryContentMapping {
		public Guid CategoryContentMappingId { get; set; }
		public Guid ContentCategoryId { get; set; }
		public Guid RootContentId { get; set; }

		public virtual CarrotContentCategory ContentCategory { get; set; } = null!;
		public virtual CarrotRootContent RootContent { get; set; } = null!;
	}
}
