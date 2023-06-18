using System;
using System.Collections.Generic;

namespace Carrotware.CMS.Data.Models {
	public partial class CarrotUserSiteMapping {
		public Guid UserSiteMappingId { get; set; }
		public Guid UserId { get; set; }
		public Guid SiteId { get; set; }

		public virtual CarrotSite Site { get; set; } = null!;
		public virtual CarrotUserData User { get; set; } = null!;
	}
}
