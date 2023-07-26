using CarrotCake.CMS.Plugins.EventCalendarModule.Data;

namespace CarrotCake.CMS.Plugins.EventCalendarModule.Models {
	public class EventSingleModel {
		public EventSingleModel() {
			this.SiteID = Guid.Empty;
			this.ItemID = Guid.Empty;

			this.ItemData = new CalendarSingleEvent();
			this.ItemProfile = new CalendarEventProfile();
		}

		public EventSingleModel(Guid siteId, Guid itemId) : this() {
			this.SiteID = siteId;
			this.ItemID = itemId;

			this.ItemData = CalendarHelper.GetEvent(this.ItemID);

			if (this.ItemData != null) {
				if (this.ItemData.EventStartTime.HasValue) {
					this.EventStartTime = CalendarHelper.GetFullDateTime(this.ItemData.EventStartTime);
				}
				if (this.ItemData.EventEndTime.HasValue) {
					this.EventEndTime = CalendarHelper.GetFullDateTime(this.ItemData.EventEndTime);
				}
			}

			Load();
		}

		public void Load() {
			if (this.ItemData != null) {
				this.ItemProfile = CalendarHelper.GetProfile(this.ItemData.CalendarEventProfileId);
				this.ProfileEventStartTime = CalendarHelper.GetFullDateTime(this.ItemProfile.EventStartTime).ToString(WebHelper.ShortTimePattern);
			}
		}

		public Guid SiteID { get; set; }
		public Guid ItemID { get; set; }

		public DateTime? EventStartTime { get; set; }
		public DateTime? EventEndTime { get; set; }
		public string ProfileEventStartTime { get; set; }

		public CalendarSingleEvent? ItemData { get; set; }
		public CalendarEventProfile? ItemProfile { get; set; }
	}
}