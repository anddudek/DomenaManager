using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDataModel;

namespace DomenaManager.Helpers
{
    public class MultiPaymentDataGrid
    {
        public Building Building { get; set; }
        public Apartment Apartment { get; set; }
        public Owner Owner { get; set; }
        public DateTime PaymentAddDate { get; set; }
        public double PaymentAmount { get; set; }
        public Guid? PaymentId { get; set; }
        public DateTime PaymentRegistrationDate { get; set; }
        public BuildingChargeGroupName ChargeGroup { get; set; }
    }
}
