using System;
using System.Collections.Generic;

namespace Carrotware.CMS.Data.Models {
	public partial class AspNetRoleClaim {
		public int Id { get; set; }
		public string RoleId { get; set; } = null!;
		public string? ClaimType { get; set; }
		public string? ClaimValue { get; set; }

		public virtual AspNetRole Role { get; set; } = null!;
	}
}
