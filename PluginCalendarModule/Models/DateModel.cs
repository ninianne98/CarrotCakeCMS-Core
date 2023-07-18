using CarrotCake.CMS.Plugins.CalendarModule.Data;

namespace CarrotCake.CMS.Plugins.CalendarModule.Models {

	public class DateModel {

		public DateModel() {
			this.SelectedDate = DateTime.Now.Date;
			this.Dates = new List<CalendarEntry>();
		}

		public DateModel(DateTime date, Guid siteId)
			: this() {
			this.SelectedDate = date;

			using (CalendarContext db = CalendarContext.Create()) {
				this.Dates = (from c in db.CalendarDates
							  where c.EventDate == this.SelectedDate
						  && c.IsActive == true
						  && c.SiteID == siteId
							  orderby c.EventDate
							  select c).ToList();
			}
		}

		public DateTime SelectedDate { get; set; }
		public List<CalendarEntry> Dates { get; set; }
	}
}