using System;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.CoreMVC.UI.Admin.Models {

	public class SplitDateTime {

		public SplitDateTime() {
			SetTimeStrings();
		}

		protected void SetTimeStrings() {
			DateTime dtModel = DateTime.MinValue;

			if (_combinedDateTime.HasValue) {
				dtModel = _combinedDateTime.Value;

				this.ValueDateString = dtModel.Date.ToString(Helper.ShortDatePattern);
				this.ValueTimeString = dtModel.ToString(Helper.ShortTimePattern);
				this.ValueDateAllString = string.Format("{0} {1}", this.ValueDateString, this.ValueTimeString);
			} else {
				this.ValueDateString = string.Empty;
				this.ValueTimeString = string.Empty;
				this.ValueDateAllString = string.Empty;
			}
		}

		public string FieldName { get; set; }
		public string TimeID { get { return string.Format("{0}_Time", this.FieldName); } }
		public string DateID { get { return string.Format("{0}_Date", this.FieldName); } }
		public string FieldID { get { return this.FieldName.Replace(".", "_").Replace("[", "_").Replace("]", "_"); } }

		private DateTime? _combinedDateTime = null;

		public DateTime? CombinedDateTime {
			get {
				SetTimeStrings();

				return _combinedDateTime;
			}

			set {
				_combinedDateTime = value;

				SetTimeStrings();
			}
		}

		public string ValueDateAllString { get; set; }
		public string ValueTimeString { get; set; }
		public string ValueDateString { get; set; }
	}
}