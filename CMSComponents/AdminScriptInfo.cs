using Carrotware.CMS.Core;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

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

	public class AdminScriptInfo : IHtmlContent {

		public AdminScriptInfo() { }

		public HtmlString RenderHtml() {
			return new HtmlString(ToHtmlString());
		}

		public string ToHtmlString() {
			var tag = new HtmlTag(HtmlTag.EasyTag.JavaScript);
			var key = SecurityData.IsAuthenticated ? DateTime.UtcNow.Ticks.ToString().Substring(0, 8) : CarrotWebHelper.DateKey();
			tag.Uri = CarrotCakeCmsHelper.AdminScriptValues + "?ts=" + key;

			return tag.ToString();
		}

		public void WriteTo(TextWriter writer, HtmlEncoder encoder) {
			writer.Write(ToHtmlString());
		}
	}
}