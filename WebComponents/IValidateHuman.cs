/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.Web.UI.Components {

	public interface IValidateHuman {

		bool ValidateValue(string testValue);

		string AltValidationFailText { get; set; }

		string Instructions { get; set; }
	}
}