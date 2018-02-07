using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
    public class ApartmentDataGrid
    {
        public Guid ApartmentId { get; set; }
        public string BuildingName { get; set; }
        public string BulidingAddress { get; set; }
        public int ApartmentNumber { get; set; }
        public string ApartmentOwner { get; set; }
        public string ApartmentOwnerAddress { get; set; }
        public double ApartmentArea { get; set; }
        public double ApartmentAdditionalArea { get; set; }
        public double ApartmentTotalArea { get; set; }
        public double Balance { get; set; }
        public bool HasWaterMeter { get; set; }
        public DateTime BoughtDate { get; set; }
    }
}
