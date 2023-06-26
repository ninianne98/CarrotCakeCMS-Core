using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;

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

	public class WidgetBase : IWidget {

		#region IWidget Attributes

		public Guid PageWidgetID { get; set; }

		public Guid RootContentID { get; set; }

		public Guid SiteID { get; set; }

		public virtual bool EnableEdit {
			get { return false; }
		}

		public bool IsBeingEdited { get; set; }

		public bool IsDynamicInserted { get; set; }

		public string WidgetClientID { get; set; }

		public virtual Dictionary<string, string> PublicParmValues { get; set; }

		public virtual Dictionary<string, string> JSEditFunctions {
			get { return new Dictionary<string, string>(); }
		}

		public virtual void LoadData() {
			if (this.PublicParmValues == null) {
				this.PublicParmValues = new Dictionary<string, string>();
			}
		}

		#endregion IWidget Attributes

		#region Common Parser Routines

		public string GetParmValue(string sKey) {
			return ParmParser.GetParmValue(this.PublicParmValues, sKey);
		}

		public string GetParmValue(string sKey, string sDefault) {
			return ParmParser.GetParmValue(this.PublicParmValues, sKey, sDefault);
		}

		public string GetParmValueDefaultEmpty(string sKey, string sDefault) {
			return ParmParser.GetParmValueDefaultEmpty(this.PublicParmValues, sKey, sDefault);
		}

		public List<string> GetParmValueList(string sKey) {
			return ParmParser.GetParmValueList(this.PublicParmValues, sKey);
		}

		#endregion Common Parser Routines
	}
}