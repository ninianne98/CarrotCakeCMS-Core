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

namespace Carrotware.Web.UI.Components {

	public abstract class BaseTwoPartWebComponent : BaseWebComponent, ITwoPartWebComponent {

		public virtual string GetBody() {
			return string.Empty;
		}

		public virtual string GetHead() {
			return string.Empty;
		}

		public virtual Microsoft.AspNetCore.Html.HtmlString RenderBody() {
			return new Microsoft.AspNetCore.Html.HtmlString(GetBody());
		}

		public virtual Microsoft.AspNetCore.Html.HtmlString RenderHead() {
			return new Microsoft.AspNetCore.Html.HtmlString(GetHead());
		}

		public override string GetHtml() {
			return GetHead() + Environment.NewLine + GetBody();
		}
	}
}