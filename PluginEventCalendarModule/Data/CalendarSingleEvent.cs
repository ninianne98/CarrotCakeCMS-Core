namespace CarrotCake.CMS.Plugins.EventCalendarModule.Data {

	public partial class CalendarSingleEvent {
		public Guid CalendarEventId { get; set; }
		public Guid CalendarEventProfileId { get; set; }
		public DateTime EventDate { get; set; }
		public string? EventDetail { get; set; }
		public bool IsCancelled { get; set; }
		public TimeSpan? EventStartTime { get; set; }
		public TimeSpan? EventEndTime { get; set; }

		public virtual CalendarEventProfile CalendarEventProfile { get; set; } = null!;
	}
}