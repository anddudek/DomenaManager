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
        public static decimal CalculateSaldo(int year, Apartment apartment)
        {
            using (var db = new DB.DomenaDBContext())
            {
                decimal paym = db.Payments.Where(x => x.ApartmentId.Equals(apartment.ApartmentId) && x.PaymentRegistrationDate.Year <= year && !x.IsDeleted).Select(x => x.PaymentAmount).DefaultIfEmpty(0).Sum();
                var charg = db.Charges.Include(x => x.Components).Where(x => x.ApartmentId.Equals(apartment.ApartmentId) && x.ChargeDate.Year <= year && !x.IsDeleted);
                
                foreach (var ch in charg)
                {
                    paym -= ch.Components.Select(x => x.Sum).DefaultIfEmpty(0).Sum();
                }

                return paym;
            }
        }

        public static decimal CalculateBuildingSaldo(int year, Building building)
        {
            using (var db = new DB.DomenaDBContext())
            {
                var apartments = db.Apartments.Where(x => x.BuildingId == building.BuildingId && !x.IsDeleted).Select(x => x.ApartmentId).ToList();
                decimal paym = db.Payments.Where(x => apartments.Contains(x.ApartmentId) && x.PaymentRegistrationDate.Year <= year && !x.IsDeleted).Select(x => x.PaymentAmount).DefaultIfEmpty(0).Sum();
                var invoices = db.Invoices.Where(x => x.BuildingId == building.BuildingId && x.InvoiceDate.Year <= year && !x.IsDeleted);

                foreach (var inv in invoices)
                {
                    paym -= inv.CostAmountGross;
                }

                return paym;
            }
        }
    }
}
