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
        public DateTime? SoldDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? OnSellCreatedApartmentGuid { get; set; }
        public string CorrespondenceAddress { get; set; }
        public Guid BindingParent { get; set; }
        public int Locators { get; set; }
        public List<ApartmentMeter> MeterCollection { get; set; }

        public Apartment()
        {
            this.ApartmentId = Guid.NewGuid();
            this.CreatedDate = DateTime.Now;
            this.IsDeleted = false;
            this.BindingParent = Guid.Empty;
            this.MeterCollection = new List<ApartmentMeter>();
            this.SoldDate = null;
            this.OnSellCreatedApartmentGuid = null;
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
            this.IsDeleted = CopySource.IsDeleted;
            this.BoughtDate = CopySource.BoughtDate;
            this.CorrespondenceAddress = CopySource.CorrespondenceAddress;
            this.BindingParent = CopySource.BindingParent;
            this.MeterCollection = CopySource.MeterCollection;
            this.Locators = CopySource.Locators;
            this.SoldDate = CopySource.SoldDate;
            this.OnSellCreatedApartmentGuid = CopySource.OnSellCreatedApartmentGuid;
        }
    }
}
