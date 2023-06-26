using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
using Carrotware.CMS.UI.Components;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using static Carrotware.CMS.UI.Components.CmsSkin;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.CoreMVC.UI.Admin {

	public static class Helper {

		public enum ViewLocation {
			AdminPublicFooter,
			AdminPopupFooter,
			AdminMainFooter,
			PublicMainFooter,
			PublicMainHeader,
		}

		public static string WebServiceAddress {
			get {
				// TODO: read config entry
				return "/api/c3-admin";
			}
		}

		public static SkinOption _theme = SkinOption.None;

		public static SkinOption SiteSkin {
			get {
				var actualSkin = SkinOption.Classic;
				try {
					actualSkin = (SkinOption)CarrotHttpHelper.CacheGet("SiteSkin");
				} catch {
					actualSkin = SkinOption.Classic;
					_theme = SkinOption.None;
				}

				if (_theme == SkinOption.None) {
					CarrotCakeConfig config = CarrotCakeConfig.GetConfig();
					string skin = config.MainConfig.SiteSkin;

					try { actualSkin = (SkinOption)Enum.Parse(typeof(SkinOption), skin, true); } catch { }

					_theme = actualSkin;
					CarrotHttpHelper.CacheInsert("SiteSkin", _theme, 2);
				}

				return _theme;
			}
		}

		public static string MainColorCode {

			get {
				return CmsSkin.GetPrimaryColorCode(SiteSkin);
			}
		}


		public static string InsertSpecialView(ViewLocation CtrlKey) {
			string sViewPath = string.Empty;
			CarrotCakeConfig config = CarrotCakeConfig.GetConfig();

			switch (CtrlKey) {
				case ViewLocation.AdminPublicFooter:
					sViewPath = config.AdminFooterControls.ViewPathPublic;
					break;

				case ViewLocation.AdminPopupFooter:
					sViewPath = config.AdminFooterControls.ViewPathPopup;
					break;

				case ViewLocation.AdminMainFooter:
					sViewPath = config.AdminFooterControls.ViewPathMain;
					break;

				case ViewLocation.PublicMainHeader:
					sViewPath = config.PublicSiteControls.ViewPathHeader;
					break;

				case ViewLocation.PublicMainFooter:
					sViewPath = config.PublicSiteControls.ViewPathFooter;
					break;
			}

			return sViewPath;
		}

		public static Dictionary<bool, string> CreateBoolFilter() {
			var option = new Dictionary<bool, string>();
			option.Add(true, "Yes");
			option.Add(false, "No");

			return option;
		}

		public static string ShortDateFormatPattern {
			get {
				return CarrotWebHelper.ShortDateFormatPattern;
			}
		}

		public static string ShortDateTimeFormatPattern {
			get {
				return CarrotWebHelper.ShortDateTimeFormatPattern;
			}
		}

		public static string ShortDatePattern {
			get {
				return CarrotWebHelper.ShortDatePattern;
			}
		}

		public static string ShortTimePattern {
			get {
				return CarrotWebHelper.ShortTimePattern;
			}
		}

		public static string ReadEmbededScript(string sResouceName) {
			return CarrotWebHelper.GetManifestResourceText(typeof(Controllers.CmsContentController), sResouceName);
		}

		public static byte[] ReadEmbededBinary(string sResouceName) {
			return CarrotWebHelper.GetManifestResourceBytes(typeof(Controllers.CmsContentController), sResouceName);
		}

		public static string GetWebResourceUrl(string sResouceName) {
			return CarrotWebHelper.GetWebResourceUrl(typeof(Controllers.CmsContentController), sResouceName);
		}

		public static void AddErrors(ModelStateDictionary stateDictionary, IdentityResult result) {
			foreach (var error in result.Errors) {
				stateDictionary.AddModelError(string.Empty, $"{error.Code}: {error.Description}");
			}
		}

		public static void HandleErrorDict(ModelStateDictionary stateDictionary, Dictionary<string, string> validationsDictionary) {
			if (validationsDictionary.Any()) {
				stateDictionary.AddModelError(string.Empty, "Please review and correct the noted errors.");
			}

			foreach (KeyValuePair<string, string> valuePair in validationsDictionary) {
				stateDictionary.AddModelError(valuePair.Key, valuePair.Value);
			}
		}

		public static void HandleErrorDict(ModelStateDictionary stateDictionary) {
			List<string> keys = stateDictionary.Keys.Where(x => !string.IsNullOrEmpty(x)).ToList();

			foreach (string d in keys) {
				foreach (var err in stateDictionary[d].Errors) {
					stateDictionary.AddModelError(string.Empty, string.Format("{0}: {1}", d, err.ErrorMessage));
				}
			}
		}

		public static void ForceValidation(ModelStateDictionary stateDictionary, Object m) {
			IValidatableObject model = null;

			if (m is IValidatableObject) {
				model = m as IValidatableObject;

				IEnumerable<ValidationResult> errors = model.Validate(new ValidationContext(model, null, null));

				List<string> modelStateKeys = stateDictionary.Keys.ToList();
				List<ModelStateEntry> modelStateValues = stateDictionary.Values.ToList();

				foreach (ValidationResult error in errors) {
					List<string> errorMemberNames = error.MemberNames.ToList();
					if (errorMemberNames.Count == 0) {
						errorMemberNames.Add(string.Empty);
					}

					foreach (string memberName in errorMemberNames) {
						int index = modelStateKeys.IndexOf(memberName);
						if (index < 0 || !modelStateValues[index].Errors.Any(i => i.ErrorMessage == error.ErrorMessage)) {
							stateDictionary.AddModelError(memberName, error.ErrorMessage);
						}
					}
				}
			}
		}

	}
}