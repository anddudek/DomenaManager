using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
    public class BuildingDataGrid
    {
        public Guid BuildingId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int ApartmentsCount { get; set; }
        public string Description { get; set; }
        public List<BuildingDescriptionListView> CostsList { get; set; }
    }
}
