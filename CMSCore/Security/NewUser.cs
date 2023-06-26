using Microsoft.AspNetCore.Identity;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Core.Security {

	public class NewUser {

		public NewUser() {
			this.ExtendedUserData = null;

			var err = new List<IdentityError>();
			err.Add(new IdentityError() { Code = "init", Description = "init" });

			this.IdentityResult = IdentityResult.Failed(err.ToArray());
		}

		public NewUser(ExtendedUserData user, IdentityResult result) {
			this.ExtendedUserData = user;
			this.IdentityResult = result;
		}

		public ExtendedUserData ExtendedUserData { get; set; }
		public IdentityResult IdentityResult { get; set; }
	}
}