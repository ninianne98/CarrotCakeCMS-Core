using Microsoft.AspNetCore.Html;

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

		public virtual IHtmlContent RenderBody() {
			return new HtmlString(GetBody());
		}

		public virtual IHtmlContent RenderHead() {
			return new HtmlString(GetHead());
		}

		public override string GetHtml() {
			return GetHead() + Environment.NewLine + GetBody();
		}
	}
}