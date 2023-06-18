using System;
using System.Collections.Generic;

namespace Carrotware.CMS.Data.Models {
	public partial class CarrotContentComment {
		public Guid ContentCommentId { get; set; }
		public Guid RootContentId { get; set; }
		public DateTime CreateDate { get; set; }
		public string CommenterIp { get; set; } = null!;
		public string CommenterName { get; set; } = null!;
		public string CommenterEmail { get; set; } = null!;
		public string CommenterUrl { get; set; } = null!;
		public string? PostComment { get; set; }
		public bool IsApproved { get; set; }
		public bool IsSpam { get; set; }

		public virtual CarrotRootContent RootContent { get; set; } = null!;
	}
}
