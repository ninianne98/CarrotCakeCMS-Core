using Microsoft.AspNetCore.Mvc;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Core {

	public interface IContentController {
		string TemplateFile { get; set; }

		PartialViewResult ViewOnly();

		ActionResult Default();

		ActionResult Default(FormCollection model);

		ActionResult RSSFeed(string type);

		ActionResult SiteMap();

		int WidgetCount { get; set; }
	}
}