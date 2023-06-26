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

	public class LiteralMessage : IHtmlContent {

		public LiteralMessage() { }

		public LiteralMessage(Exception ex, string key, string path) {
			string msg = String.Empty;

			if (ex != null) {
				msg = String.Format("<pre>{0}</pre>", ex);

				if (ex.InnerException != null) {
					msg = String.Format("<pre>\r\n{0}\r\n{1}\r\n</pre>", ex, ex.InnerException);
				}
			} else {
				msg = "<p>There was an error loading the widget.</p>";
			}

			this.Message = String.Format("<div>\r\n<p><b>{0}:</b>  {1}</p>\r\n{2}</div>", key, path, msg);
		}

		public LiteralMessage(string message, string key, string path) {
			string msg = "<p>There was an error loading the widget.</p>";

			if (message != null) {
				msg = message;
			}

			this.Message = String.Format("<div>\r\n<p><b>{0}:</b>  {1}</p>\r\n{2}</div>", key, path, msg);
		}

		public string Message { get; set; }

		public HtmlString RenderHtml() {
			return new HtmlString(ToHtmlString());
		}

		public string ToHtmlString() {
			return this.Message;
		}

		public void WriteTo(TextWriter writer, HtmlEncoder encoder) {
			writer.Write(ToHtmlString());
		}
	}
}