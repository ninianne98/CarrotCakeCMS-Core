using Carrotware.CMS.Core;
using System.Collections.Generic;

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

	public class WidgetProperties {

		public WidgetProperties() {
			this.Properties = new List<ObjectProperty>();
		}

		public WidgetProperties(Widget widget, List<ObjectProperty> properties) {
			this.Widget = widget;
			this.Properties = properties;
		}

		public Widget Widget { get; set; }

		public List<ObjectProperty> Properties { get; set; }
	}
}