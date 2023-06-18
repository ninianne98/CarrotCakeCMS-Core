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

	public interface ITextBodyUpdate {

		string UpdateContent(string textContent);

		string UpdateContentPlainText(string textContent);

		string UpdateContentRichText(string textContent);

		string UpdateContentComment(string textContent);

		string UpdateContentSnippet(string textContent);
	}
}