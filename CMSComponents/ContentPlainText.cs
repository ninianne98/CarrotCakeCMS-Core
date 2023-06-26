﻿using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
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

namespace Carrotware.CMS.UI.Components {

	public class ContentPlainText : BaseToolboxComponent, IWidgetRawData {
		public string RawWidgetData { get; set; }

		public override string ToHtmlString() {
			return SiteData.CurrentSite.UpdateContentPlainText(this.RawWidgetData);
		}

		public override bool EnableEdit {
			get { return true; }
		}

		public override Dictionary<string, string> JSEditFunctions {
			get {
				Dictionary<string, string> lst = new Dictionary<string, string>();
				lst.Add("Edit Text", "cmsShowEditWidgetForm('" + this.PageWidgetID + "', 'plain');");
				return lst;
			}
		}
	}
}