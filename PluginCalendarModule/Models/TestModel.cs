using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarrotCake.CMS.Plugins.CalendarModule.Models {

	public class TestModel {

		public TestModel() { }

		public CalendarDisplaySettings Settings { get; set; }

		public IHtmlContent RenderedContent { get; set; }

		public PartialViewResult PartialResult { get; set; }

		public CalendarViewModel ViewModel { get; set; }
	}
}