using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class MetersHistory
    {
        [Key]
        public Guid MeterHistoryId { get; set; }
        public MeterType MeterType { get; set; }
        public ApartmentMeter ApartmentMeter { get; set; }
        public Apartment Apartment { get; set; }
        public Building Building { get; set; }
        public DateTime ModifiedDate { get; set; }
        public double OldValue { get; set; }
        public double NewValue { get; set; }
    }
}
