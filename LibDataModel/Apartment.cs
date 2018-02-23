using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class Apartment
    {
        [Key]
        public Guid ApartmentId { get; set; }
        public Guid BuildingId { get; set; }
        public Guid OwnerId { get; set; }
        public int ApartmentNumber { get; set; }
        public double ApartmentArea { get; set; }
        public double AdditionalArea { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime BoughtDate { get; set; }
        public DateTime WaterMeterExp { get; set; }
        public bool HasWaterMeter { get; set; }
        public bool IsDeleted { get; set; }
        public string CorrespondenceAddress { get; set; }

        public Apartment()
        {
            this.ApartmentId = Guid.NewGuid();
            this.CreatedDate = DateTime.Now;
            this.IsDeleted = false;
        }

        public Apartment(Apartment CopySource)
        {
            this.ApartmentId = CopySource.ApartmentId;
            this.BuildingId = CopySource.BuildingId;
            this.OwnerId = CopySource.OwnerId;
            this.ApartmentNumber = CopySource.ApartmentNumber;
            this.ApartmentArea = CopySource.ApartmentArea;
            this.AdditionalArea = CopySource.AdditionalArea;
            this.CreatedDate = CopySource.CreatedDate;
            this.HasWaterMeter = CopySource.HasWaterMeter;
            this.IsDeleted = CopySource.IsDeleted;
            this.BoughtDate = CopySource.BoughtDate;
            this.WaterMeterExp = CopySource.WaterMeterExp;
            this.CorrespondenceAddress = CopySource.CorrespondenceAddress;
        }
    }
}
