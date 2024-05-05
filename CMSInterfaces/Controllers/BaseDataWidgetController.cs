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

	public class BaseDataWidgetController : BaseWidgetController, IWidgetDataObject {
		public virtual object WidgetPayload { get; set; } = new object();
	}
}