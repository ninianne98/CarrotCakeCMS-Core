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

	public abstract class TextBodyUpdate : ITextBodyUpdate {

		public virtual string UpdateContent(string TextContent) {
			return TextContent;
		}

		public virtual string UpdateContentPlainText(string TextContent) {
			return TextContent;
		}

		public virtual string UpdateContentRichText(string TextContent) {
			return TextContent;
		}

		public virtual string UpdateContentComment(string TextContent) {
			return TextContent;
		}

		public virtual string UpdateContentSnippet(string TextContent) {
			return TextContent;
		}
	}
}