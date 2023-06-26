using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Html;
using System.Text;

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

	public abstract class BaseTwoPartCmsComponent : BaseCmsComponent, ICmsChildrenComponent, ICmsMainComponent, ITwoPartWebComponent {

		public virtual string GetBody() {
			TweakData();

			var output = new StringBuilder();
			output = WriteTopLevel(output);

			return ControlUtilities.HtmlFormat(output);
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