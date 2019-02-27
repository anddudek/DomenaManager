using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDataModel;
using System.Data.Entity;

namespace DomenaManager.Helpers
{
    public static class RepairFundOperations
    {
        public static double CalculateAcumulateFund(Guid ApartmentId, int Month, int Year)
        {
            using (var db = new DB.DomenaDBContext())
            {
                var apartment = db.Apartments.FirstOrDefault(x => x.ApartmentId == ApartmentId);

                var paymentsdb = db.Payments.Include(x => x.ChargeGroup).Where(x => !x.IsDeleted &&
                x.ApartmentId == ApartmentId &&
                x.ChargeGroup.BuildingChargeGroupNameId == BuildingChargeGroupName.RepairFundGroupId).ToList();
                var payments = paymentsdb.Where(x => x.PaymentRegistrationDate <= new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month))).ToList();

                var paidAmount = payments.Select(x => x.PaymentAmount).DefaultIfEmpty(0).Sum();
                var invoiceSettlement = db.BuildingInvoceBindings.Include(x => x.InvoiceCategory).FirstOrDefault(x => !x.IsDeleted && x.InvoiceCategory.CategoryId == InvoiceCategory.RepairFundInvoiceCategoryId);
                if (invoiceSettlement == null)
                {
                    return paidAmount;
                }

                var invoicesdb = db.Invoices.Where(x => !x.IsDeleted &&
                x.BuildingId == apartment.BuildingId &&
                x.InvoiceCategoryId == InvoiceCategory.RepairFundInvoiceCategoryId &&
                x.IsSettled).ToList();
                var invoices = invoicesdb.Where(x => x.InvoiceDate <= new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month))).ToList();

                var totalCost = invoices.Select(x => x.CostAmountGross).DefaultIfEmpty(0).Sum();

                var apArea = db.Apartments.Where(x => x.BuildingId == apartment.BuildingId && !x.IsDeleted).Select(x => x.ApartmentArea).DefaultIfEmpty(0).Sum();
                var addArea = db.Apartments.Where(x => x.BuildingId == apartment.BuildingId && !x.IsDeleted).Select(x => x.AdditionalArea).DefaultIfEmpty(0).Sum();
                var buildingTotalArea = apArea + addArea;
                var apartmentsCount = db.Apartments.Where(x => !x.IsDeleted && x.BuildingId == apartment.BuildingId).Count();
                var totalLocators = db.Apartments.Where(x => !x.IsDeleted && x.BuildingId == apartment.BuildingId).Select(x => x.Locators).DefaultIfEmpty(0).Sum();
                double scale;
                double apartmentCost = 0;

                switch (invoiceSettlement.Distribution)
                {
                    default:
                        break;
                    case CostDistribution.PerAdditionalArea:
                        scale = Math.Floor(10000 * (apartment.AdditionalArea / addArea)) / 10000;
                        apartmentCost = Math.Floor(100 * (totalCost * scale)) / 100;
                        break;
                    case CostDistribution.PerApartment:
                        scale = Math.Floor(10000 * (1 / (double)apartmentsCount)) / 10000;
                        apartmentCost = Math.Floor(100 * (totalCost * scale)) / 100;
                        break;
                    case CostDistribution.PerApartmentArea:
                        scale = Math.Floor(10000 * (apartment.ApartmentArea / apArea)) / 10000;
                        apartmentCost = Math.Floor(100 * (totalCost * scale)) / 100;
                        break;
                    case CostDistribution.PerApartmentTotalArea:
                        scale = Math.Floor(10000 * ((apartment.ApartmentArea + apartment.AdditionalArea) / buildingTotalArea)) / 10000;
                        apartmentCost = Math.Floor(100 * (totalCost * scale)) / 100;
                        break;
                    case CostDistribution.PerLocators:
                        scale = Math.Floor(10000 * ((double)apartment.Locators / (double)totalLocators)) / 10000;
                        apartmentCost = Math.Floor(100 * (totalCost * scale)) / 100;
                        break;
                }

                return paidAmount - apartmentCost;
            }            
        }
    }
}
