using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {
	public partial class vwCarrotComment {
		[Key]
		public Guid ContentCommentId { get; set; }
		public DateTime CreateDate { get; set; }
		public string CommenterIp { get; set; } = null!;
		public string CommenterName { get; set; } = null!;
		public string CommenterEmail { get; set; } = null!;
		public string CommenterUrl { get; set; } = null!;
		public string? PostComment { get; set; }
		public bool IsApproved { get; set; }
		public bool IsSpam { get; set; }
		public Guid RootContentId { get; set; }
		public Guid SiteId { get; set; }
		public string FileName { get; set; } = null!;
		public string? PageHead { get; set; }
		public string? TitleBar { get; set; }
		public string? NavMenuText { get; set; }
		public bool? IsRetired { get; set; }
		public bool? IsUnReleased { get; set; }
		public DateTime RetireDate { get; set; }
		public DateTime GoLiveDate { get; set; }
		public bool PageActive { get; set; }
		public bool ShowInSiteNav { get; set; }
		public bool ShowInSiteMap { get; set; }
		public bool BlockIndex { get; set; }
		public Guid ContentTypeId { get; set; }
		public string ContentTypeValue { get; set; } = null!;
	}
}
