using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDataModel;

namespace DomenaManager.Helpers
{
    public class PaymentDataGrid : Payment
    {
        public Building Building { get; set;}
        public Apartment Apartment { get; set; }
        public Owner Owner { get; set; }

        public PaymentDataGrid(Payment _payment)
        {
            this.ApartmentId = _payment.ApartmentId;
            this.PaymentAddDate = _payment.PaymentAddDate;
            this.PaymentAmount = _payment.PaymentAmount;
            this.PaymentId = _payment.PaymentId;
            this.PaymentRegistrationDate = _payment.PaymentRegistrationDate;
            this.IsDeleted = _payment.IsDeleted;
            this.ChargeGroup = _payment.ChargeGroup;

            using (var db = new DB.DomenaDBContext())
            {
                this.Apartment = db.Apartments.Where(x => x.ApartmentId.Equals(this.ApartmentId)).FirstOrDefault();
                this.Building = db.Buildings.Where(x => x.BuildingId.Equals(this.Apartment.BuildingId)).FirstOrDefault();
                this.Owner = db.Owners.Where(x => x.OwnerId.Equals(this.Apartment.OwnerId)).FirstOrDefault();                
            }
        }
    }
}
