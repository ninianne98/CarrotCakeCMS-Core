using System;
using System.Collections.Generic;

namespace Carrotware.CMS.Data.Models {
	public partial class CarrotContentSnippet {
		public Guid ContentSnippetId { get; set; }
		public Guid RootContentSnippetId { get; set; }
		public bool IsLatestVersion { get; set; }
		public Guid EditUserId { get; set; }
		public DateTime EditDate { get; set; }
		public string? ContentBody { get; set; }

		public virtual CarrotRootContentSnippet RootContentSnippet { get; set; } = null!;
	}
}
