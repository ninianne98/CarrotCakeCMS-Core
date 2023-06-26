/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

using Microsoft.AspNetCore.Html;

namespace Carrotware.Web.UI.Components {

	public interface ITwoPartWebComponent {

		string GetBody();

		string GetHead();

		IHtmlContent RenderBody();

		IHtmlContent RenderHead();
	}
}