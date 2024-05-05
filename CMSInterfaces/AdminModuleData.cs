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

	public class AdminModuleData : IAdminModule {
		public Guid SiteID { get; set; }

		public Guid ModuleID { get; set; }

		public string ModuleName { get; set; } = string.Empty;

		public virtual void LoadData(IAdminModule data) { }
	}
}