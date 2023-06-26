using Carrotware.CMS.Interface;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Security {

	public class CarrotSecurityConfig {

		public static CarrotSecurityConfig GetConfig(IConfigurationRoot config) {
			var security = config.GetSection("CarrotSecurity").Get<CarrotSecurityConfig>();
			return security ?? new CarrotSecurityConfig();
		}

		public static CarrotSecurityConfig GetConfig() {
			return GetConfig(CarrotHttpHelper.Configuration);
		}

		public UserValidator UserValidator { get; set; } = new UserValidator();
		public PasswordValidator PasswordValidator { get; set; } = new PasswordValidator();
		public AdditionalSettings AdditionalSettings { get; set; } = new AdditionalSettings();
	}

	//==============================
	public class UserValidator {
		public string AllowedUserNameCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.";
		public bool RequireUniqueEmail { get; set; } = true;
	}

	//==============================
	public class PasswordValidator {
		public int RequiredLength { get; set; } = 8;
		public bool RequireNonAlphanumeric { get; set; } = true;
		public bool RequireDigit { get; set; } = true;
		public bool RequireLowercase { get; set; } = true;
		public bool RequireUppercase { get; set; } = true;
	}

	//==============================
	public class AdditionalSettings {
		public int MaxFailedAccessAttempts { get; set; } = 5;
		public int DefaultLockoutTimeSpan { get; set; } = 15;
		public int TokenLifespan { get; set; } = 2;
		public bool UserLockoutEnabledByDefault { get; set; } = true;
		public string DataProtectionProviderAppName { get; set; } = "CarrotCake CMS";
		public string LoginPath { get; set; } = "/c3-admin/Login";
		public string Unauthorized { get; set; } = "/c3-admin/NotAuthorized";
		public string ResetPath { get; set; } = "/c3-admin/ResetPassword";
		public int ExpireTimeSpan { get; set; } = 360;
		public bool SetCookieExpireTimeSpan { get; set; } = true;
		public int ValidateInterval { get; set; } = 30;
	}
}