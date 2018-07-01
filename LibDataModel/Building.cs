using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class Building
    {   
        public Building(Building b)
        {
            BuildingId = b.BuildingId;
            Name = b.Name;
            City = b.City;
            ZipCode = b.ZipCode;
            RoadName = b.RoadName;
            BuildingNumber = b.BuildingNumber;
            IsDeleted = b.IsDeleted;
            CostCollection = b.CostCollection;
            MeterCollection = b.MeterCollection;
        }

        public Building()
        {
            BuildingId = Guid.NewGuid();
            Name = null;
            City = null;
            ZipCode = null;
            RoadName = null;
            BuildingNumber = null;
            IsDeleted = false;
            CostCollection = new List<BuildingChargeBasis>();
            MeterCollection = new List<MeterType>();
        }

        public string GetAddress()
        {
            return "ul. " + RoadName + " " + BuildingNumber + Environment.NewLine + ZipCode + " " + City;
        }

        [Key]
        public Guid BuildingId {get; set;}
        public string Name { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string RoadName { get; set; }
        public string BuildingNumber { get; set; }
        public bool IsDeleted { get; set; }
        public List<BuildingChargeBasis> CostCollection { get; set; }
        public List<MeterType> MeterCollection { get; set; }
    }
}
