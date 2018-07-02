using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class ApartmentMeter
    {
        [Key]
        public Guid MeterId { get; set; }
        public MeterType MeterTypeParent { get; set; }
        public double LastMeasure { get; set; }
        public DateTime LegalizationDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
