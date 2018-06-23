using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDataModel;
using System.Data.Entity;

namespace DomenaManager.Helpers
{
    public static class Payments
    {
        public static double CalculateSaldo(int year, Apartment apartment)
        {
            using (var db = new DB.DomenaDBContext())
            {
                double paym = db.Payments.Where(x => x.ApartmentId.Equals(apartment.ApartmentId) && x.PaymentRegistrationDate.Year <= year && !x.IsDeleted).Select(x => x.PaymentAmount).DefaultIfEmpty(0).Sum();
                var charg = db.Charges.Include(x => x.Components).Where(x => x.ApartmentId.Equals(apartment.ApartmentId) && x.CreatedTime.Year <= year && !x.IsDeleted);
                
                foreach (var ch in charg)
                {
                    paym -= ch.Components.Select(x => x.Sum).DefaultIfEmpty(0).Sum();
                }

                return paym;
            }
        }
    }
}
