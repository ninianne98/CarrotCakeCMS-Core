using System.ComponentModel.DataAnnotations;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace CarrotCake.CMS.Plugins.EventCalendarModule.Code {

	public interface ICalendarEventProfile {

		[Required]
		Guid CalendarEventCategoryId { get; set; }

		[Required]
		Guid CalendarEventProfileId { get; set; }

		Guid CalendarFrequencyId { get; set; }

		[Required]
		DateTime EventEndDate { get; set; }

		TimeSpan? EventEndTime { get; set; }
		int? EventRepeatPattern { get; set; }

		[Required]
		DateTime EventStartDate { get; set; }

		TimeSpan? EventStartTime { get; set; }

		[Required]
		string EventTitle { get; set; }

		string EventDetail { get; set; }
		bool IsAllDayEvent { get; set; }
		bool IsAnnualHoliday { get; set; }
		bool IsCancelled { get; set; }
		bool IsCancelledPublic { get; set; }
		bool IsHoliday { get; set; }
		bool IsPublic { get; set; }
		int RecursEvery { get; set; }

		[Required]
		Guid SiteID { get; set; }
	}
}