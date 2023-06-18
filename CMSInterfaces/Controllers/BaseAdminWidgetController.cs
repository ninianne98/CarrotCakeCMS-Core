using System;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Interface.Controllers {

	public class BaseAdminWidgetController : BaseWidgetController, IAdminModule {
		protected Guid requestKey = Guid.NewGuid();

		public BaseAdminWidgetController()
			: base() {
			//CarrotViewEngine.EngineLoad(this, requestKey);
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			//CarrotViewEngine.EngineDispose(this, requestKey);
		}

		public Guid SiteID { get; set; }

		public Guid ModuleID { get; set; }

		public string ModuleName { get; set; }
	}
}