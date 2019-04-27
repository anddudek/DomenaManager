using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }
        public Guid ApartmentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentRegistrationDate { get; set; }
        public DateTime PaymentAddDate { get; set; }
        public bool IsDeleted { get; set; }
        public BuildingChargeGroupName ChargeGroup { get; set; }
    }
}
