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
        public String Owner { get; set; }
        public int ApartmentNumber { get; set; }
        public double ApartmentArea { get; set; }
        public double AdditionalArea { get; set; }
        public DateTime CreatedDate { get; set; }
        public string MailAddress { get; set; }
        public bool HasWaterMeter { get; set; }
        public bool IsDeleted { get; set; }
    }
}
