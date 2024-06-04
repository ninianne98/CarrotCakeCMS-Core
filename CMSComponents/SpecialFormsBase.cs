using Carrotware.CMS.Interface;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Xml.Serialization;

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

	public abstract class FormSettingRootBase : IFormSettingRootBase {

		public FormSettingRootBase() {
			this.Uri = CarrotHttpHelper.HttpContext.Request.Path;
		}

		public string PostPartialName { get; set; } = string.Empty;
		public string Uri { get; set; } = string.Empty;
	}

	//==============================

	public interface IFormSettingRootBase {
		string PostPartialName { get; set; }
		string Uri { get; set; }
	}

	//==============================

	public abstract class FormSettingBase : IFormSettingBase {

		public FormSettingBase() {
			this.ValidationFailText = "Failed to validate as a human.";
			this.Uri = CarrotHttpHelper.HttpContext.Request.Path;
		}

		public string PostPartialName { get; set; } = string.Empty;
		public string Uri { get; set; } = string.Empty;

		public bool UseValidateHuman { get; set; }
		public string ValidateHumanClass { get; set; } = string.Empty;
		public string ValidationFailText { get; set; } = string.Empty;

		public void GetSettingFromConfig(FormConfigBase config) {
			if (config != null && config.ValidateHuman != null) {
				this.UseValidateHuman = true;
				this.ValidateHumanClass = config.ValidateHuman.GetType().AssemblyQualifiedName;
				if (!string.IsNullOrEmpty(config.ValidateHuman.AltValidationFailText)) {
					this.ValidationFailText = config.ValidateHuman.AltValidationFailText;
				}
			} else {
				this.UseValidateHuman = false;
				this.ValidateHumanClass = string.Empty;
				this.ValidationFailText = string.Empty;
			}
		}

		public void SetHuman(IValidateHuman validateHuman) {
			if (validateHuman != null) {
				this.UseValidateHuman = true;
				this.ValidateHumanClass = validateHuman.GetType().AssemblyQualifiedName;
				if (!string.IsNullOrEmpty(validateHuman.AltValidationFailText)) {
					this.ValidationFailText = validateHuman.AltValidationFailText;
				}
			} else {
				this.UseValidateHuman = false;
				this.ValidateHumanClass = string.Empty;
				this.ValidationFailText = string.Empty;
			}
		}
	}

	//==============================

	public interface IFormSettingBase : IFormSettingRootBase {
		bool UseValidateHuman { get; set; }
		string ValidateHumanClass { get; set; }
		string ValidationFailText { get; set; }
	}

	//==============================

	public abstract class FormConfigRootBase : IFormConfigRootBase {

		public FormConfigRootBase() {
			this.PostPartialName = string.Empty;
		}

		public FormConfigRootBase(string partialName)
			: this() {
			this.PostPartialName = partialName;
		}

		public string PostPartialName { get; set; } = string.Empty;
	}

	//==============================

	public interface IFormConfigRootBase {
		string PostPartialName { get; set; }
	}

	//==============================

	public abstract class FormConfigBase : IFormConfigBase {

		public FormConfigBase() {
			this.ValidateHuman = null;
			this.PostPartialName = string.Empty;
		}

		public FormConfigBase(string partialName)
			: this() {
			this.PostPartialName = partialName;
		}

		public FormConfigBase(string partialName, IValidateHuman validateHuman)
			: this(partialName) {
			this.ValidateHuman = validateHuman;
		}

		public string PostPartialName { get; set; } = string.Empty;

		public IValidateHuman? ValidateHuman { get; set; }
	}

	//==============================

	public interface IFormConfigBase : IFormConfigRootBase {
		IValidateHuman? ValidateHuman { get; set; }
	}

	//==============================

	public interface IFormModelBase<S> {
		string EncodedSettings { get; set; }
		IValidateHuman ValidateHuman { get; set; }
		object ValidateSettings { get; set; }
		string ValidationValue { get; set; }

		S Settings { get; set; }

		void GetSettings();

		string SerializeSettings();
	}

	//==============================

	public abstract class FormModelBase<P> : IFormModelBase<P> {

		public FormModelBase() { }

		public string? EncodedSettings { get; set; }
		public IValidateHuman? ValidateHuman { get; set; }
		public object? ValidateSettings { get; set; }
		public string? ValidationValue { get; set; }

		public virtual P Settings { get; set; } = default;

		public void GetSettings() {
			this.ValidateSettings = null;
			Type type = typeof(P);  //  this.Settings.GetType();

			if (!string.IsNullOrEmpty(this.EncodedSettings)) {
				string xml = this.EncodedSettings.DecodeBase64();
				var xmlSerializer = new XmlSerializer(type);
				using (var sr = new StringReader(xml)) {
					this.ValidateSettings = xmlSerializer.Deserialize(sr);
				}

				if (this.ValidateSettings != null && this.ValidateSettings is IFormSettingBase) {
					IFormSettingBase settings = this.ValidateSettings as IFormSettingBase;

					if (!string.IsNullOrEmpty(settings.ValidateHumanClass)) {
						Type objType = ReflectionUtilities.GetTypeFromString(settings.ValidateHumanClass);
						object obj = Activator.CreateInstance(objType);

						this.ValidateHuman = (IValidateHuman)obj;
						this.ValidateHuman.AltValidationFailText = settings.ValidationFailText;
					}
				}
			}
		}

		public virtual string SerializeSettings() {
			Type type = typeof(P);  //  this.Settings.GetType();
			string xml = string.Empty;

			var xmlSerializer = new XmlSerializer(type);
			using (var sw = new StringWriter()) {
				xmlSerializer.Serialize(sw, this.Settings);
				xml = sw.ToString();
			}

			xml = xml.EncodeBase64();
			this.EncodedSettings = xml;

			return xml;
		}

		public virtual void WriteCache<T>(IHtmlHelper helper, IHtmlHelper<T> specialForm) where T : IFormModelBase<P> {
			string frmTag = Environment.NewLine
							+ specialForm.AntiForgeryToken().RenderToString()
							+ Environment.NewLine
							+ specialForm.HiddenFor(x => x.EncodedSettings).RenderToString()
							+ Environment.NewLine;

			helper.ViewContext.Writer.Write(frmTag);
		}

		public virtual ModelStateDictionary ClearOptionalItems(ModelStateDictionary modelState) {
			// these child objects are for display only, and their validation is not needed
			foreach (var ms in modelState.ToArray()) {
				if (ms.Key.ToLowerInvariant().Contains("settings")) {
					modelState.Remove(ms.Key);
				}
			}

			return modelState;
		}
	}
}