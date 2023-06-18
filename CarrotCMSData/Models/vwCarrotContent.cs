using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {
	public partial class vwCarrotContent {
		public Guid RootContentId { get; set; }
		public Guid SiteId { get; set; }
		public Guid? HeartbeatUserId { get; set; }
		public DateTime? EditHeartbeat { get; set; }
		public string FileName { get; set; } = null!;
		public bool PageActive { get; set; }
		public bool ShowInSiteNav { get; set; }
		public bool ShowInSiteMap { get; set; }
		public bool BlockIndex { get; set; }
		public Guid CreateUserId { get; set; }
		public DateTime CreateDate { get; set; }
		[Key]
		public Guid ContentId { get; set; }
		public Guid? ParentContentId { get; set; }
		public bool IsLatestVersion { get; set; }
		public string? TitleBar { get; set; }
		public string? NavMenuText { get; set; }
		public string? PageHead { get; set; }
		public string? PageText { get; set; }
		public string? LeftPageText { get; set; }
		public string? RightPageText { get; set; }
		public int NavOrder { get; set; }
		public Guid? EditUserId { get; set; }
		public Guid? CreditUserId { get; set; }
		public DateTime EditDate { get; set; }
		public string? TemplateFile { get; set; }
		public string? MetaKeyword { get; set; }
		public string? MetaDescription { get; set; }
		public int? VersionCount { get; set; }
		public Guid ContentTypeId { get; set; }
		public string ContentTypeValue { get; set; } = null!;
		public string? PageSlug { get; set; }
		public string? PageThumbnail { get; set; }
		public string? TimeZone { get; set; }
		public DateTime RetireDate { get; set; }
		public DateTime GoLiveDate { get; set; }
		public DateTime GoLiveDateLocal { get; set; }
		public bool? IsRetired { get; set; }
		public bool? IsUnReleased { get; set; }
	}
}
