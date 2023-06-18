using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {
	public partial class vwCarrotContentSnippet {
		public Guid RootContentSnippetId { get; set; }
		public Guid SiteId { get; set; }
		public string ContentSnippetName { get; set; } = null!;
		public string ContentSnippetSlug { get; set; } = null!;
		public Guid CreateUserId { get; set; }
		public DateTime CreateDate { get; set; }
		public bool ContentSnippetActive { get; set; }
		[Key]
		public Guid ContentSnippetId { get; set; }
		public bool IsLatestVersion { get; set; }
		public Guid EditUserId { get; set; }
		public DateTime EditDate { get; set; }
		public string? ContentBody { get; set; }
		public Guid? HeartbeatUserId { get; set; }
		public DateTime? EditHeartbeat { get; set; }
		public DateTime GoLiveDate { get; set; }
		public DateTime RetireDate { get; set; }
		public bool? IsRetired { get; set; }
		public bool? IsUnReleased { get; set; }
		public int? VersionCount { get; set; }
	}
}
