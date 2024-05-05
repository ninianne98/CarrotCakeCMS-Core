using Carrotware.CMS.Interface;
using System.ComponentModel;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace CarrotCake.CMS.Plugins.EventCalendarModule {

	public class CalendarUpcomingSettings : WidgetActionSettingModel {

		public CalendarUpcomingSettings() {
			this.DaysInPast = -3;
			this.DaysInFuture = 30;
			this.TakeTop = 20;
			this.CalendarPageUri = string.Empty;
		}

		public override bool EnableEdit {
			get {
				return true;
			}
		}

		[Widget(WidgetAttribute.FieldMode.TextBox)]
		[Description("Days in past")]
		public int DaysInPast { get; set; }

		[Widget(WidgetAttribute.FieldMode.TextBox)]
		[Description("Days in future")]
		public int DaysInFuture { get; set; }

		[Widget(WidgetAttribute.FieldMode.TextBox)]
		[Description("Limit total events")]
		public int TakeTop { get; set; }

		[Widget(WidgetAttribute.FieldMode.TextBox)]
		[Description("Calendar page uri")]
		public string CalendarPageUri { get; set; }

		public override void LoadData() {
			base.LoadData();

			this.DaysInPast = Convert.ToInt32(this.GetParmValue("DaysInPast", "-3"));
			this.DaysInFuture = Convert.ToInt32(this.GetParmValue("DaysInFuture", "30"));
			this.TakeTop = Convert.ToInt32(this.GetParmValue("TakeTop", "20"));
			this.CalendarPageUri = this.GetParmValue("CalendarPageUri", string.Empty);
		}
	}
}