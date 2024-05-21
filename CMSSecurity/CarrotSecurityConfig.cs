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

			var addSettings = new AdditionalSettings(config);
			security.AdditionalSettings = addSettings;

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
	public class AdditionalSettings : AdditionalSettingsJson {

		public AdditionalSettings() {
			if (CarrotHttpHelper.Configuration != null) {
				var configuration = CarrotHttpHelper.Configuration;
				Settings(configuration);
			}
		}

		public AdditionalSettings(IConfigurationRoot configuration) {
			Settings(configuration);
		}

		private static AdditionalSettingsJson GetSettings(IConfigurationRoot config) {
			var section = config.GetSection("CarrotSecurity:AdditionalSettings");
			return section.Get<AdditionalSettingsJson>();
		}

		private void Settings(IConfigurationRoot config) {
			var settingsJson = GetSettings(config);

			var adminFolder = "/c3-admin/";

			if (config != null) {
				var settings = config.GetSection("CarrotCakeCMS:Config");
				var folder = settings.GetSection("AdminFolderPath").Get<string>();

				if (!string.IsNullOrWhiteSpace(folder) && folder.Length > 2) {
					adminFolder = "/" + folder + "/";
					adminFolder = adminFolder.Replace("\\", "/");
					adminFolder = adminFolder.Replace("//", "/").Replace("//", "/").Replace("//", "/");
				}
			}

			this.MaxFailedAccessAttempts = settingsJson.MaxFailedAccessAttempts;
			this.DefaultLockoutTimeSpan = settingsJson.DefaultLockoutTimeSpan;
			this.TokenLifespan = settingsJson.TokenLifespan;
			this.UserLockoutEnabledByDefault = settingsJson.UserLockoutEnabledByDefault;
			this.ExpireTimeSpan = settingsJson.ExpireTimeSpan;
			this.SetCookieExpireTimeSpan = settingsJson.SetCookieExpireTimeSpan;
			this.ValidateInterval = settingsJson.ValidateInterval;

			this.LoginPath = adminFolder + "Login";
			this.Unauthorized = adminFolder + "NotAuthorized";
			this.ResetPath = adminFolder + "ResetPassword";
		}

		public string DataProtectionProviderAppName { get; internal set; } = "CarrotCake CMS";
		public string LoginPath { get; internal set; } = "/Login";
		public string Unauthorized { get; internal set; } = "/NotAuthorized";
		public string ResetPath { get; internal set; } = "/ResetPassword";
	}

	//==============================
	public class AdditionalSettingsJson {

		public AdditionalSettingsJson() {
		}

		public int MaxFailedAccessAttempts { get; set; } = 5;
		public int DefaultLockoutTimeSpan { get; set; } = 15;
		public int TokenLifespan { get; set; } = 2;
		public bool UserLockoutEnabledByDefault { get; set; } = true;
		public int ExpireTimeSpan { get; set; } = 360;
		public bool SetCookieExpireTimeSpan { get; set; } = true;
		public int ValidateInterval { get; set; } = 30;
	}
}