using Carrotware.CMS.Core;
using System;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.CoreMVC.UI.Admin.Models {

	public class ContentSingleModel {

		public ContentSingleModel() {
			this.Mode = SiteData.RawMode;
		}

		public Guid PageId { get; set; }

		public Guid? WidgetId { get; set; }

		public string Mode { get; set; } = SiteData.RawMode;

		public string? Field { get; set; }

		public string? PageText { get; set; }
	}
}