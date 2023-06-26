using Carrotware.Web.UI.Components;

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

	public static class CoreHelper {

		internal static string ReadEmbededScript(string sResouceName) {
			return CarrotWebHelper.GetManifestResourceText(typeof(CoreHelper), sResouceName);
		}

		internal static byte[] ReadEmbededBinary(string sResouceName) {
			return CarrotWebHelper.GetManifestResourceBytes(typeof(CoreHelper), sResouceName);
		}

		internal static string GetWebResourceUrl(string resource) {
			string sPath = string.Empty;

			try {
				sPath = CarrotWebHelper.GetWebResourceUrl(typeof(CoreHelper), resource);
			} catch { }

			return sPath;
		}
	}
}