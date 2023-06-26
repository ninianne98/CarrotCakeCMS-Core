using System;
using System.Collections.Generic;

namespace Carrotware.CMS.Data.Models {
	public partial class CarrotTextWidget {
		public Guid TextWidgetId { get; set; }
		public Guid SiteId { get; set; }
		public string TextWidgetAssembly { get; set; } = null!;
		public bool ProcessBody { get; set; }
		public bool ProcessPlainText { get; set; }
		public bool ProcessHtmlText { get; set; }
		public bool ProcessComment { get; set; }
		public bool ProcessSnippet { get; set; }

		public virtual CarrotSite Site { get; set; } = null!;
	}
}
