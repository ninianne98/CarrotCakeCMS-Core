using Carrotware.CMS.Interface;
using Microsoft.AspNetCore.Html;
using System;
using System.Text.Encodings.Web;
using System.Web;

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

	public abstract class BaseToolboxComponent : WidgetBase, IHtmlContent {
		public virtual string ToHtmlString() {
			return string.Empty;
		}

		public override bool EnableEdit {
			get { return false; }
		}

		public void WriteTo(TextWriter writer, HtmlEncoder encoder) {
			writer.Write(ToHtmlString());
		}
	}
}