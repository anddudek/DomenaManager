using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class Charge
    {
        [Key]
        public Guid ChargeId { get; set; }
        public Guid ApartmentId { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsClosed { get; set; }
        public List<ChargeComponent> Components { get; set; }
        public bool IsDeleted { get; set; }
    }
}
