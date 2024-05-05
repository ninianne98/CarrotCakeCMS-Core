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

	public class WidgetActionSettingModelAttribute : Attribute {
		private string _field;

		public WidgetActionSettingModelAttribute(string className) {
			_field = className;
		}

		public WidgetActionSettingModelAttribute(Type dataType) {
			_field = dataType.AssemblyQualifiedName;
		}

		public string ClassName {
			get {
				return _field;
			}
		}

		public object SettingsModel {
			get {
				Type typ = null;
				if (!string.IsNullOrWhiteSpace(this.ClassName)) {
					typ = Type.GetType(this.ClassName);

					//if (typ == null && this.ClassName.IndexOf(",") < 1) {
					//	typ = BuildManager.GetType(this.ClassName, true);
					//}
				}

				return Activator.CreateInstance(typ);
			}
		}
	}
}