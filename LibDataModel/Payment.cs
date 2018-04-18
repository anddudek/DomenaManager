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
        public double PaymentAmount { get; set; }
        public DateTime PaymentRegistrationDate { get; set; }
        public DateTime PaymentAddDate { get; set; }
    }
}
