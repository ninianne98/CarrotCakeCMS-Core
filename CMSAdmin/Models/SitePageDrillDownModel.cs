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

	public class SitePageDrillDownModel {

		public SitePageDrillDownModel() {
			this.SelectedPageID = Guid.Empty;
		}

		public Guid? SelectedPageID { get; set; }

		public Guid CurrentPageID { get; set; }

		public string FieldName { get; set; }

		public string FieldID { get { return this.FieldName.Replace(".", "_").Replace("[", "_").Replace("]", "_"); } }
	}
}