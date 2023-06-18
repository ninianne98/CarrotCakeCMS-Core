using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {
	public partial class vwCarrotEditHistory {
		public Guid SiteId { get; set; }
		[Key]
		public Guid ContentId { get; set; }
		public Guid RootContentId { get; set; }
		public bool IsLatestVersion { get; set; }
		public string? TitleBar { get; set; }
		public string? NavMenuText { get; set; }
		public string? PageHead { get; set; }
		public Guid? CreditUserId { get; set; }
		public DateTime EditDate { get; set; }
		public DateTime CreateDate { get; set; }
		public string FileName { get; set; } = null!;
		public Guid ContentTypeId { get; set; }
		public string ContentTypeValue { get; set; } = null!;
		public bool PageActive { get; set; }
		public DateTime GoLiveDate { get; set; }
		public DateTime RetireDate { get; set; }
		public Guid? EditUserId { get; set; }
		public string? EditUserName { get; set; }
		public string? EditEmail { get; set; }
		public Guid CreateUserId { get; set; }
		public string? CreateUserName { get; set; }
		public string? CreateEmail { get; set; }
	}
}
