using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using System.IO;
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

namespace Carrotware.Web.UI.Components {

	public abstract class BaseWebComponent : IHtmlContent, IWebComponent {

		protected string CurrentScriptName {
			get { return CarrotWebHelper.Request.PathBase; }
		}

		public bool IsPostBack {
			get {
				string requestMethod = HttpMethods.Post.ToUpperInvariant();
				try { requestMethod = CarrotWebHelper.Request.Method.ToUpperInvariant(); } catch { }
				return requestMethod == HttpMethods.Post.ToUpperInvariant() ? true : false;
			}
		}

		public virtual string GetHtml() {
			return string.Empty;
		}

		public virtual HtmlString RenderHtml() {
			return new HtmlString(GetHtml());
		}

		public virtual string ToHtmlString() {
			return GetHtml();
		}

		public void WriteTo(TextWriter writer, HtmlEncoder encoder) {
			writer.Write(ToHtmlString());
		}
	}
}