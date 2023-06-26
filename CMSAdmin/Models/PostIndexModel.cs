using Carrotware.CMS.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Linq;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.CoreMVC.UI.Admin.Models {

	public class PostIndexModel : PageIndexModel {

		public PostIndexModel()
			: base() {
			this.SearchDate = SiteData.CurrentSite.Now.Date;
			this.SelectedRange = 30;

			this.DateRanges = new Dictionary<int, string>();
			this.DateRanges.Add(1, "1 Days +/-");
			this.DateRanges.Add(7, "7 Days +/-");
			this.DateRanges.Add(30, "30 Days +/-");
			this.DateRanges.Add(60, "60 Days +/-");
			this.DateRanges.Add(90, "90 Days +/-");
			this.DateRanges.Add(120, "120 Days +/-");
		}

		[Required]
		public DateTime SearchDate { get; set; }

		public int SelectedRange { get; set; }

		public Dictionary<int, string> DateRanges { get; set; }
	}
}