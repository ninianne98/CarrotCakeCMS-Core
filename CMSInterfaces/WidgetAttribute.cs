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

	public class WidgetAttribute : Attribute {
		private string _field;
		private FieldMode _mode = FieldMode.Unknown;

		public enum FieldMode {
			Unknown,
			DictionaryList,
			DropDownList,
			CheckBoxList,
			TextBox,
			ColorBox,
			MultiLineTextBox,
			RichHTMLTextBox,
			CheckBox
		}

		public WidgetAttribute() {
			_mode = FieldMode.Unknown;
		}

		public WidgetAttribute(FieldMode mode) {
			_mode = mode;
		}

		public WidgetAttribute(FieldMode mode, string field) {
			_mode = mode;
			_field = field;
		}

		public FieldMode Mode {
			get {
				return _mode;
			}
		}

		public string SelectFieldSource {
			get {
				return _field;
			}
		}
	}
}