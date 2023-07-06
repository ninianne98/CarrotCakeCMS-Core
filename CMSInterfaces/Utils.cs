using System.Reflection;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Interface {

	public static class Utils {

		public static string GetAssemblyName(this Assembly assembly) {
			var assemblyName = assembly.ManifestModule.Name;
			return Path.GetFileNameWithoutExtension(assemblyName);
		}
	}
}