using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrotware.CMS.Data.Models {

	// for sproc return
	public partial class CarrotContentTally {
		public int? ContentCount { get; set; }

		public Guid SiteID { get; set; }

		public DateTime? DateMonth { get; set; }

		[Key]
		public string DateSlug { get; set; }
	}
}