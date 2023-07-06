using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

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

	public static class Utils {

		internal static string GetAssemblyName(this Assembly assembly) {
			var assemblyName = assembly.ManifestModule.Name;
			return Path.GetFileNameWithoutExtension(assemblyName);
		}

		public static string ScrubQueryElement(this string text) {
			return text.Replace("{", "").Replace(">", "").Replace("<", "").Replace(">", "")
										.Replace("'", "").Replace("\\", "").Replace("//", "").Replace(":", "");
		}

		public static string SafeQueryString(this HttpContext context, string key) {
			return SafeQueryString(context, key, string.Empty);
		}

		public static string SafeQueryString(this HttpContext context, string key, string defaultVal) {
			var query = context.Request.QueryString;

			if (query.HasValue) {
				var dict = QueryHelpers.ParseQuery(query.Value);

				if (dict != null && dict.ContainsKey(key)) {
					return dict[key].ToString();
				}
			}

			return defaultVal;
		}

		public static string DecodeBase64(this string text) {
			string val = string.Empty;
			if (!string.IsNullOrEmpty(text)) {
				Encoding enc = Encoding.GetEncoding("ISO-8859-1"); //Western European (ISO)
				val = enc.GetString(Convert.FromBase64String(text));
			}
			return val;
		}

		public static string EncodeBase64(this string text) {
			string val = string.Empty;
			if (!string.IsNullOrEmpty(text)) {
				Encoding enc = Encoding.GetEncoding("ISO-8859-1"); //Western European (ISO)
				byte[] toEncodeAsBytes = enc.GetBytes(text);
				val = Convert.ToBase64String(toEncodeAsBytes);
			}
			return val;
		}
	}
}