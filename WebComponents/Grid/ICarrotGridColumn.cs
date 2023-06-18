using Microsoft.AspNetCore.Html;
using System;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.Web.UI.Components {

	public interface ICarrotGridColumn {
		string HeaderText { get; set; }
		object HeadAttributes { get; set; }
		object BodyAttributes { get; set; }
		bool HasHeadingText { get; set; }
		int Order { get; set; }
		CarrotGridColumnType Mode { get; set; }
	}

	public interface ICarrotGridColumnTemplate<T> where T : class {
		Func<T, IHtmlContent> FormatTemplate { get; set; }
	}

	public interface ICarrotGridColumnExt {
		string ColumnName { get; set; }
		bool Sortable { get; set; }
		object HeadLinkAttributes { get; set; }
		string CellFormatString { get; set; }
	}
}