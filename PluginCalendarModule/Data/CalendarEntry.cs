using CarrotCake.CMS.Plugins.CalendarModule.Code;

namespace CarrotCake.CMS.Plugins.CalendarModule.Data {
	public partial class CalendarEntry : ICalendar {
		public Guid CalendarID { get; set; }
		public DateTime? EventDate { get; set; }
		public string? EventTitle { get; set; }
		public string? EventDetail { get; set; }
		public bool? IsActive { get; set; }
		public Guid? SiteID { get; set; }
	}
}
