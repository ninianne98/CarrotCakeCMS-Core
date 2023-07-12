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

	public class CustomErrorConfig {

		public static CustomErrorConfig GetConfig(IConfigurationRoot config) {
			var settings = config.GetSection("CustomErrors").Get<CustomErrorConfig>();

			return settings ?? new CustomErrorConfig();
		}

		public static CustomErrorConfig GetConfig() {
			return GetConfig(CarrotWebHelper.Configuration);
		}

		public bool Developer { get; set; } = false;
		public string DefaultRedirect { get; set; } = string.Empty;

		[ConfigurationKeyName("ErrorCodes")]
		public List<ErrorCode> ErrorCodes { get; set; } = new List<ErrorCode>();
	}

	//==============================
	public class ErrorCode {
		public int StatusCode { get; set; } = 200;
		public string Uri { get; set; } = "/";
	}
}