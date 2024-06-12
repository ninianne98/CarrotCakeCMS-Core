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

	public class WidgetSettings : IWidgetSettings {

		public WidgetSettings() { }

		public Guid PageWidgetID { get; set; }

		public Guid RootContentID { get; set; }

		public Guid SiteID { get; set; }

		public virtual string WidgetClientID { get; set; } = string.Empty;

		public bool IsBeingEdited { get; set; }

		public bool IsDynamicInserted { get; set; }

		public virtual string AlternateViewFile { get; set; } = string.Empty;

		public virtual void SettingsFromWidget(object widgetObject) {
			if (widgetObject != null) {
				if (widgetObject is IWidget) {
					var widget = (IWidget)widgetObject;

					this.SiteID = widget.SiteID;
					this.WidgetClientID = widget.WidgetClientID;
					this.RootContentID = widget.RootContentID;
					this.PageWidgetID = widget.PageWidgetID;
					this.IsBeingEdited = widget.IsBeingEdited;
					this.IsDynamicInserted = widget.IsDynamicInserted;
				}

				if (widgetObject is IWidgetView) {
					var widget = (IWidgetView)widgetObject;
					this.AlternateViewFile = widget.AlternateViewFile;
				}
			}
		}
	}
}