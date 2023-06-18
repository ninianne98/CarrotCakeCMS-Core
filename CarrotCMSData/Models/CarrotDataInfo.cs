using System;
using System.Collections.Generic;

namespace Carrotware.CMS.Data.Models {
	public partial class CarrotDataInfo {
		public Guid DataInfoId { get; set; }
		public string DataKey { get; set; } = null!;
		public string DataValue { get; set; } = null!;
	}
}
