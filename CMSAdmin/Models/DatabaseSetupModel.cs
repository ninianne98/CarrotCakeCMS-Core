using Carrotware.CMS.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

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
			this.CreateUser = false;
			this.Messages = new List<SelectListItem>();

			try {
				this.CreateUser = !SecurityData.CheckUserExistance();
				this.Messages = SecurityData.CheckMigrationHistory();
			} catch { }
		}

		public bool CreateUser { get; set; }

		public List<SelectListItem> Messages { get; set; }
	}
}