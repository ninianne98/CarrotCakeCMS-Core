using CarrotCake.CMS.Plugins.EventCalendarModule.Code;
using System.ComponentModel.DataAnnotations;

namespace CarrotCake.CMS.Plugins.EventCalendarModule.Data {

	[MetadataType(typeof(ICalendarEventProfile))]
	public partial class CalendarEventProfile : ICalendarEventProfile {

		public CalendarEventProfile() {
			this.CalendarEvents = new HashSet<CalendarSingleEvent>();
		}

		public Guid CalendarEventProfileId { get; set; }
		public Guid CalendarFrequencyId { get; set; }
		public Guid CalendarEventCategoryId { get; set; }
		public DateTime EventStartDate { get; set; }
		public TimeSpan? EventStartTime { get; set; }
		public DateTime EventEndDate { get; set; }
		public TimeSpan? EventEndTime { get; set; }
		public string? EventTitle { get; set; }
		public string? EventDetail { get; set; }
		public int? EventRepeatPattern { get; set; }
		public bool IsAllDayEvent { get; set; }
		public bool IsPublic { get; set; }
		public bool IsCancelled { get; set; }
		public bool IsCancelledPublic { get; set; }
		public Guid SiteID { get; set; }
		public bool IsHoliday { get; set; }
		public bool IsAnnualHoliday { get; set; }
		public int RecursEvery { get; set; }

		public virtual CalendarEventCategory CalendarEventCategory { get; set; } = null!;
		public virtual CalendarFrequency CalendarFrequency { get; set; } = null!;
		public virtual ICollection<CalendarSingleEvent> CalendarEvents { get; set; }
	}
}