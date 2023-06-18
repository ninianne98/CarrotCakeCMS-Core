using System;
using System.Collections.Generic;

namespace Northwind.Data
{
    public partial class EmployeeTerritory
    {
        public int EmployeeId { get; set; }
        public string TerritoryId { get; set; } = null!;
        public bool Selected { get; set; }

        public virtual Employee Employee { get; set; } = null!;
        public virtual Territory Territory { get; set; } = null!;
    }
}
