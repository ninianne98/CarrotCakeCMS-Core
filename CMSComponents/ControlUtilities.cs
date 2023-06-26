using Carrotware.CMS.Core;
using Carrotware.Web.UI.Components;
using System.Text;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.UI.Components {

	public static class ControlUtilities {

		public static string ReadEmbededScript(string sResouceName) {
			return CarrotWebHelper.GetManifestResourceText(typeof(ControlUtilities), sResouceName);
		}

		public static byte[] ReadEmbededBinary(string sResouceName) {
			return CarrotWebHelper.GetManifestResourceBytes(typeof(ControlUtilities), sResouceName);
		}

		public static string GetWebResourceUrl(string resource) {
			string sPath = string.Empty;

			try {
				sPath = CarrotWebHelper.GetWebResourceUrl(typeof(ControlUtilities), resource);
			} catch { }

			return sPath;
		}

		public static List<SiteNav> TweakData(List<SiteNav> navs) {
			return CMSConfigHelper.TweakData(navs);
		}

		public static SiteNav FixNavLinkText(SiteNav nav) {
			return CMSConfigHelper.FixNavLinkText(nav);
		}

		public static List<SiteNav> GetPageNavTree() {
			using (ISiteNavHelper navHelper = SiteNavFactory.GetSiteNavHelper()) {
				return navHelper.GetPageCrumbNavigation(SiteData.CurrentSiteID, SiteData.AlternateCurrentScriptName, !SecurityData.IsAuthEditor);
			}
		}

		public static SiteNav GetParentPage() {
			using (ISiteNavHelper navHelper = SiteNavFactory.GetSiteNavHelper()) {
				SiteNav pageNav = navHelper.GetParentPageNavigation(SiteData.CurrentSiteID, SiteData.AlternateCurrentScriptName);

				//assign bogus page name for comp purposes
				if (pageNav == null || pageNav.SiteID == Guid.Empty) {
					pageNav = new SiteNav();
					pageNav.Root_ContentID = Guid.Empty;
					pageNav.FileName = "/#";
					pageNav.TemplateFile = "/##/##/";
				}

				return pageNav;
			}
		}

		public static SiteNav GetCurrentPage() {
			using (ISiteNavHelper navHelper = SiteNavFactory.GetSiteNavHelper()) {
				SiteNav pageNav = navHelper.FindByFilename(SiteData.CurrentSiteID, SiteData.AlternateCurrentScriptName);
				//assign bogus page name for comp purposes
				if (pageNav == null) {
					pageNav = new SiteNav();
					pageNav.Root_ContentID = Guid.Empty;
					pageNav.FileName = "/#";
					pageNav.TemplateFile = "/##/##/";
				}

				pageNav.SiteID = SiteData.CurrentSiteID;

				return pageNav;
			}
		}

		public static string GetParentPageName() {
			SiteNav nav = ControlUtilities.GetParentPage();

			return nav.FileName.ToLowerInvariant();
		}

		public static bool AreFilenamesSame(string sParm1, string sParm2) {
			if (sParm1 == null || sParm2 == null) {
				return false;
			}

			return (sParm1.ToLowerInvariant() == sParm2.ToLowerInvariant()) ? true : false;
		}

		public static string HtmlFormat(StringBuilder input) {
			return CarrotWebHelper.HtmlFormat(input);
		}

		public static string HtmlFormat(string input) {
			return CarrotWebHelper.HtmlFormat(input);
		}
	}

}