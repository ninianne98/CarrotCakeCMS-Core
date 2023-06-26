using Carrotware.CMS.Interface;
using System.ComponentModel;
using System.Text.Json.Serialization;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Core {

	public class CarrotCakeConfig {

		public static CarrotCakeConfig GetConfig(IConfigurationRoot config) {
			var security = config.GetSection("CarrotCakeCMS").Get<CarrotCakeConfig>();

			return security ?? new CarrotCakeConfig();
		}

		public static CarrotCakeConfig GetConfig() {
			return GetConfig(CarrotHttpHelper.Configuration);
		}

		[ConfigurationKeyName("Config")]
		public MainConfig MainConfig { get; set; } = new MainConfig();

		[ConfigurationKeyName("SiteMapping")]
		public List<DynamicSite> SiteMapping { get; set; } = new List<DynamicSite>();

		[ConfigurationKeyName("FileManager")]
		public FileBrowser FileManagerConfig { get; set; } = new FileBrowser();

		[ConfigurationKeyName("Options")]
		public Options ExtraOptions { get; set; } = new Options();

		[ConfigurationKeyName("AdminFooter")]
		public AdminFooter AdminFooterControls { get; set; } = new AdminFooter();

		[ConfigurationKeyName("PublicSite")]
		public PublicSite PublicSiteControls { get; set; } = new PublicSite();

		[ConfigurationKeyName("ConfigFile")]
		public ConfigFile ConfigFileLocation { get; set; } = new ConfigFile();

	}

	//==============================
	public class MainConfig {

		[Description("Site identity")]
		public Guid? SiteID { get; set; } = null;

		[Description("Override parameter for admin folder")]
		public string AdminFolderPath { get; set; } = "/c3-admin/";

		[Description("Override parameter for site skin")]
		public string SiteSkin { get; set; } = "Classic";
	}

	//==============================
	public class FileBrowser {

		[Description("File extensions to block from the CMS file browser")]
		public string BlockedExtensions { get; set; } = null;
	}

	//==============================
	public class Options {

		[Description("Indicates if error log should be written to")]
		public bool WriteErrorLog { get; set; } = false;
	}

	//==============================
	public class ConfigFile {

		public string TextContentProcessors { get; set; } = "TextContentProcessors.config";

		public string TemplatePath { get; set; } = "/Views/Templates/";

		public string PluginPath { get; set; } = "/Views/";
	}

	//==============================
	public class AdminFooter {

		public string ViewPathMain { get; set; } = null;

		public string ViewPathPopup { get; set; } = null;

		public string ViewPathPublic { get; set; } = null;
	}

	//==============================
	public class PublicSite {
		public string ViewPathHeader { get; set; } = null;

		public string ViewPathFooter { get; set; } = null;
	}
}