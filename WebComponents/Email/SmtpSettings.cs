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

	public class SmtpSettings {
		public string FromEmail { get; set; }
		public string DisplayName { get; set; }
		public string SmtpUsername { get; internal set; }
		public string SmtpPassword { get; set; }
		public string Host { get; set; } = "localhost";
		public int Port { get; set; } = 587;
		public bool UseTls { get; set; } = false;
		public bool UseSpecifiedPickupDirectory { get; set; } = true;
		public string PickupDirectoryLocation { get; set; } = Path.GetTempPath();

		public static SmtpSettings GetEMailSettings() {
			var config = CarrotWebHelper.ServiceProvider.GetRequiredService<IConfigurationRoot>();

			if (config.GetSection(nameof(SmtpSettings)).Exists()) {
				return config.GetSection(nameof(SmtpSettings)).Get<SmtpSettings>();
			}

			return new SmtpSettings();
		}
	}
}