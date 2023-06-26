using Microsoft.AspNetCore.Html;
using System.Web;

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

	public interface IMetaDataLinks {
		IHtmlContent Text { get; }
		string Uri { get; }
		int Count { get; }
	}
}