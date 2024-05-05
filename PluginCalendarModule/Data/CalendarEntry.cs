using CarrotCake.CMS.Plugins.CalendarModule.Code;
using System.ComponentModel.DataAnnotations;

namespace CarrotCake.CMS.Plugins.CalendarModule.Data {

	[MetadataType(typeof(ICalendar))]
	public partial class CalendarEntry : ICalendar {
		public Guid CalendarID { get; set; }
		public DateTime? EventDate { get; set; }
		public string? EventTitle { get; set; }
		public string? EventDetail { get; set; }
		public bool? IsActive { get; set; }
		public Guid? SiteID { get; set; }
	}
}