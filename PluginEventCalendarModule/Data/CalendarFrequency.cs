namespace CarrotCake.CMS.Plugins.EventCalendarModule.Data {

	public partial class CalendarFrequency {

		public CalendarFrequency() {
			this.CalendarEventProfiles = new HashSet<CalendarEventProfile>();
		}

		public Guid CalendarFrequencyId { get; set; }
		public int FrequencySortOrder { get; set; }
		public string FrequencyValue { get; set; } = null!;
		public string FrequencyName { get; set; } = null!;

		public virtual ICollection<CalendarEventProfile> CalendarEventProfiles { get; set; }
	}
}