using CarrotCake.CMS.Plugins.EventCalendarModule.Code;
using System.ComponentModel.DataAnnotations;

namespace CarrotCake.CMS.Plugins.EventCalendarModule.Data {

	[MetadataType(typeof(ICalendarEventCategory))]
	public partial class CalendarEventCategory : ICalendarEventCategory {

		public CalendarEventCategory() {
			this.CalendarEventProfiles = new HashSet<CalendarEventProfile>();
		}

		public Guid CalendarEventCategoryId { get; set; }
		public string CategoryFGColor { get; set; } = null!;
		public string CategoryBGColor { get; set; } = null!;
		public string CategoryName { get; set; } = null!;
		public Guid SiteID { get; set; }

		public virtual ICollection<CalendarEventProfile> CalendarEventProfiles { get; set; }
	}
}