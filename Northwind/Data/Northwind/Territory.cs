using System;
using System.Collections.Generic;

namespace Northwind.Data
{
    public partial class Territory
    {
        public Territory()
        {
            EmployeeTerritories = new HashSet<EmployeeTerritory>();
        }

        public string TerritoryId { get; set; } = null!;
        public string TerritoryDescription { get; set; } = null!;
        public int RegionId { get; set; }

        public virtual Region Region { get; set; } = null!;
        public virtual ICollection<EmployeeTerritory> EmployeeTerritories { get; set; }
    }
}
