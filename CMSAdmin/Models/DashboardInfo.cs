/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.CoreMVC.UI.Admin.Models {

	public class DashboardInfo {

		public DashboardInfo() { }

		public int Pages { get; set; }
		public int Posts { get; set; }
		public int Tags { get; set; }
		public int Categories { get; set; }

		public int Snippets { get; set; }
	}
}