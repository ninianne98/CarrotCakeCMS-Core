using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
using Carrotware.CMS.UI.Components;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, 2024 Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023, April 2024
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
				var config = CarrotCakeConfig.GetConfig();
				var adminFolder = config.MainConfig.AdminFolderPath.TrimPathSlashes();

				return "/api/" + adminFolder;
			}
		}

		private static string _siteSkin = "cmsSiteSkin";
		public static CmsSkin.SkinOption _theme = CmsSkin.SkinOption.None;

		public static CmsSkin.SkinOption SiteSkin {
			get {
				var actualSkin = CmsSkin.SkinOption.Classic;
				try {
					var cacheSkin = CarrotHttpHelper.CacheGet(_siteSkin);
					if (cacheSkin != null) {
						actualSkin = (CmsSkin.SkinOption)cacheSkin;
					} else {
						actualSkin = CmsSkin.SkinOption.Classic;
						_theme = CmsSkin.SkinOption.None;
					}
				} catch {
					actualSkin = CmsSkin.SkinOption.Classic;
					_theme = CmsSkin.SkinOption.None;
				}

				if (_theme == CmsSkin.SkinOption.None) {
					var config = CarrotCakeConfig.GetConfig();
					string skin = config.MainConfig.SiteSkin;

					try { actualSkin = (CmsSkin.SkinOption)Enum.Parse(typeof(CmsSkin.SkinOption), skin, true); } catch { }

					_theme = actualSkin;
					CarrotHttpHelper.CacheInsert(_siteSkin, _theme, 2);
				}

				return _theme;
			}
		}

		public static string MainColorCode {
			get {
				return CmsSkin.GetPrimaryColorCode(SiteSkin);
			}
		}

		private static string _useBootstrap = "cmsUseBootstrap";
		public static bool? _bootstrap = null;

		public static bool UseBootstrap {
			get {
				bool? bootstrap = null;
				try {
					var ret = CarrotHttpHelper.CacheGet(_useBootstrap);
					if (ret != null) {
						bootstrap = Convert.ToBoolean(ret);
					}
				} catch {
					bootstrap = null;
				}

				if (bootstrap.HasValue == false) {
					var config = CarrotCakeConfig.GetConfig();
					_bootstrap = config.MainConfig.UseBootstrap;
				} else {
					_bootstrap = bootstrap;
				}

				if (_bootstrap.HasValue && !bootstrap.HasValue) {
					CarrotHttpHelper.CacheInsert(_useBootstrap, _bootstrap.Value.ToString(), 2);
				}

				return _bootstrap.Value;
			}
		}

		public static string InsertSpecialView(ViewLocation CtrlKey) {
			string viewPath = string.Empty;
			var config = CarrotCakeConfig.GetConfig();

			switch (CtrlKey) {
				case ViewLocation.AdminPublicFooter:
					viewPath = config.AdminFooterControls.ViewPathPublic;
					break;

				case ViewLocation.AdminPopupFooter:
					viewPath = config.AdminFooterControls.ViewPathPopup;
					break;

				case ViewLocation.AdminMainFooter:
					viewPath = config.AdminFooterControls.ViewPathMain;
					break;

				case ViewLocation.PublicMainHeader:
					viewPath = config.PublicSiteControls.ViewPathHeader;
					break;

				case ViewLocation.PublicMainFooter:
					viewPath = config.PublicSiteControls.ViewPathFooter;
					break;
			}

			return viewPath;
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

		public static void AddErrors(this ModelStateDictionary stateDictionary, IdentityResult result) {
			foreach (var error in result.Errors) {
				stateDictionary.AddModelError(string.Empty, $"{error.Code}: {error.Description}");
			}
		}

		public static void HandleErrorDict(this ModelStateDictionary stateDictionary, Dictionary<string, string> validationsDictionary) {
			if (validationsDictionary.Any()) {
				stateDictionary.AddModelError(string.Empty, "Please review and correct the noted errors.");
			}

			foreach (KeyValuePair<string, string> valuePair in validationsDictionary) {
				stateDictionary.AddModelError(valuePair.Key, valuePair.Value);
			}
		}

		public static void HandleErrorDict(this ModelStateDictionary stateDictionary) {
			List<string> keys = stateDictionary.Keys.Where(x => !string.IsNullOrEmpty(x)).ToList();

			foreach (string d in keys) {
				foreach (var err in stateDictionary[d].Errors) {
					stateDictionary.AddModelError(string.Empty, string.Format("{0}: {1}", d, err.ErrorMessage));
				}
			}
		}

		public static void ForceValidation(this ModelStateDictionary stateDictionary, object m) {
			IValidatableObject? model = null;

			if (m is IValidatableObject) {
				model = m as IValidatableObject;

				IEnumerable<ValidationResult> errors = model != null ? model.Validate(new ValidationContext(model, null, null)) : new List<ValidationResult>();

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
							stateDictionary.AddModelError(memberName, error.ErrorMessage ?? string.Empty);
						}
					}
				}
			}
		}
	}
}