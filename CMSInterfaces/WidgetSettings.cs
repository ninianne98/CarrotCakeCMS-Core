/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Interface {

	public class WidgetSettings : IWidgetSettings {

		public WidgetSettings() { }

		public Guid PageWidgetID { get; set; }

		public Guid RootContentID { get; set; }

		public Guid SiteID { get; set; }

		public virtual string WidgetClientID { get; set; }

		public bool IsBeingEdited { get; set; }

		public bool IsDynamicInserted { get; set; }

		public virtual string AlternateViewFile { get; set; }
	}
}