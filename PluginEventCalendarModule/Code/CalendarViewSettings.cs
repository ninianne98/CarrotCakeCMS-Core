using Carrotware.CMS.Interface;

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

	public class CalendarViewSettings : WidgetSettings {

		public CalendarViewSettings() {
			this.GenerateCss = true;
			this.SpecifiedCssFile = string.Empty;
		}

		public override void SettingsFromWidget(object widgetObject) {
			base.SettingsFromWidget(widgetObject);

			if (widgetObject != null) {
				if (widgetObject is CalendarDisplaySettings) {
					var widget = (CalendarDisplaySettings)widgetObject;

					this.GenerateCss = widget.GenerateCss;
					this.SpecifiedCssFile = widget.SpecifiedCssFile;
				}
			}
		}

		public bool GenerateCss { get; set; }
		public string SpecifiedCssFile { get; set; }
	}
}