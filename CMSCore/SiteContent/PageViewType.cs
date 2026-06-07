/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Core {

	public class PageViewType {

		public PageViewType() { }

		public PageViewType(ViewType type) {
			this.CurrentViewType = type;
			this.ExtraTitle = string.Empty;
			this.RawValue = null;
		}

		public PageViewType(ViewType type, string extraTitle) {
			this.CurrentViewType = type;
			this.ExtraTitle = extraTitle;
			this.RawValue = null;
		}

		public PageViewType(ViewType type, string extraTitle, object value) {
			this.CurrentViewType = type;
			this.ExtraTitle = extraTitle;
			this.RawValue = value;
		}

		public enum ViewType {
			SinglePage,
			SearchResults,
			AuthorIndex,
			DateIndex,
			DateMonthIndex,
			DateDayIndex,
			DateYearIndex,
			TagIndex,
			CategoryIndex,
		}

		public ViewType CurrentViewType { get; set; } = PageViewType.ViewType.SinglePage;

		public string ExtraTitle { get; set; } = string.Empty;

		public object? RawValue { get; set; } = null;
	}

	//=======================

	public class TypeHeadingOption {

		public TypeHeadingOption()
			: this(PageViewType.ViewType.SinglePage, string.Empty, string.Empty) {
		}

		public TypeHeadingOption(PageViewType.ViewType key, string labelText) {
			this.KeyValue = key;
			this.LabelText = labelText;
			this.FormatText = string.Empty;
		}

		public TypeHeadingOption(PageViewType.ViewType key, string labelText, string formatText) {
			this.KeyValue = key;
			this.LabelText = labelText;
			this.FormatText = formatText;
		}

		public PageViewType.ViewType KeyValue { get; set; } = PageViewType.ViewType.SinglePage;
		public string LabelText { get; set; } = string.Empty;
		public string FormatText { get; set; } = string.Empty;
	}
}