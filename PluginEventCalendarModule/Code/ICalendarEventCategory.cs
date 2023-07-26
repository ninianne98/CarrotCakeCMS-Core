using System;
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

	public interface ICalendarEventCategory {

		[Required]
		string CategoryName { get; set; }

		Guid CalendarEventCategoryId { get; set; }

		[Required]
		string CategoryBGColor { get; set; }

		[Required]
		string CategoryFGColor { get; set; }

		Guid SiteID { get; set; }
	}
}