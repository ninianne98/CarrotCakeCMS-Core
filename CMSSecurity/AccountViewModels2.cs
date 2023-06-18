using System.ComponentModel.DataAnnotations;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Security.Models {

	public class ResetPassword {

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string Email { get; set; }
		public string Token { get; set; }
	}

	//===============================

	public class PasswordReset {

		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}

	//===============================

	public class UserLogin {

		[Required(ErrorMessage = "Username is required")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Remember me")]
		public bool RememberMe { get; set; }
	}

	//===============================

	public class UserRegister {

		[Required(ErrorMessage = "Name is required")]
		[StringLength(100)]
		public string Username { get; set; }

		[EmailAddress(ErrorMessage = "Invalid email address")]
		[Required(ErrorMessage = "Email is required")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirm password is required")]
		[Compare("Password", ErrorMessage = "The Password and Confirm Password do not match.")]
		public string ConfirmPassword { get; set; }

		public string? PhoneNumber { get; set; }
	}
}