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

	public class DatabaseSetupModel {

		public DatabaseSetupModel() {
			this.CreateUser = true;
			this.Messages = new List<string>();
		}

		public bool CreateUser { get; set; }

		public List<string> Messages { get; set; }
	}
}