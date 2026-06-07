using Carrotware.Web.UI.Components;
using System.Xml;

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

		internal static string ReadEmbededScript(string resource) {
			return CarrotWebHelper.GetManifestResourceText(typeof(CoreHelper), resource);
		}

		internal static byte[] ReadEmbededBinary(string resource) {
			return CarrotWebHelper.GetManifestResourceBytes(typeof(CoreHelper), resource);
		}

		internal static string GetWebResourceUrl(string resource) {
			string path = string.Empty;

			try {
				path = CarrotWebHelper.GetWebResourceUrl(typeof(CoreHelper), resource);
			} catch { }

			return path;
		}

		public static XmlReaderSettings GetXmlReaderSettings() {
			var settings = new XmlReaderSettings {
				ConformanceLevel = ConformanceLevel.Fragment
			};

			return settings;
		}

		public static XmlWriterSettings GetXmlWriterSettings() {
			var settings = new XmlWriterSettings {
				OmitXmlDeclaration = true,
				Indent = true
			};

			return settings;
		}

		public static RouteValueDictionary MarkSpecial(this RouteValueDictionary routeData, string pageId) {
			routeData.Add(CmsRouting.Keys.PageId, pageId);
			routeData[CmsRouting.Keys.Special] = true;
			return routeData;
		}
	}
}