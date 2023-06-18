using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {
	public partial class vwCarrotUserData {
		[Key]
		public string Id { get; set; } = null!;
		public string? Email { get; set; }
		public bool EmailConfirmed { get; set; }
		public string? PasswordHash { get; set; }
		public string? SecurityStamp { get; set; }
		public string? PhoneNumber { get; set; }
		public bool PhoneNumberConfirmed { get; set; }
		public bool TwoFactorEnabled { get; set; }
		public DateTime? LockoutEndDateUtc { get; set; }
		public bool LockoutEnabled { get; set; }
		public int AccessFailedCount { get; set; }
		public string? UserName { get; set; }
		public Guid? UserId { get; set; }
		public string? UserKey { get; set; }
		public string? UserNickName { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? UserBio { get; set; }
	}
}
