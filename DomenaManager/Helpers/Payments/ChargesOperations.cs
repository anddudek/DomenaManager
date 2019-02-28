using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using LibDataModel;

namespace DomenaManager.Helpers
{
    public static class ChargesOperations
    {
        public static void GenerateCharges(DateTime chargeDate, Guid autoChargeId)
        {
            using (var db = new DB.DomenaDBContext())
            {
                var q = db.Buildings.Include(b => b.CostCollection);
                foreach (var a in db.Apartments.Where(x => !x.IsDeleted && x.SoldDate == null && q.Where(y => y.BuildingId.Equals(x.BuildingId)).FirstOrDefault().CostCollection.Count > 0))
                {
                    var c = new Charge() { ApartmentId = a.ApartmentId, ChargeId = Guid.NewGuid(), IsClosed = false, ChargeDate = chargeDate, CreatedDate=DateTime.Today, SettlementId=Guid.Empty, AutoChargeId=autoChargeId, OwnerId = a.OwnerId };
                    c.Components = new List<ChargeComponent>();
                    foreach (var costCollection in db.Buildings.Include(b => b.CostCollection).Where(x => x.BuildingId.Equals(a.BuildingId)).FirstOrDefault().CostCollection)
                    {
                        if (costCollection.BegginingDate > chargeDate || (costCollection.EndingDate.Year > 1901 && costCollection.EndingDate < chargeDate))
                        {
                            continue;
                        }
                        var group = db.GroupName.FirstOrDefault(x => x.BuildingChargeGroupNameId == costCollection.BuildingChargeGroupNameId);
                        var cc = new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = costCollection.BuildingChargeBasisCategoryId, CostDistribution = costCollection.BuildingChargeBasisDistribution, CostPerUnit = costCollection.CostPerUnit, GroupName=group};
                        double units;
                        switch ((CostDistribution)cc.CostDistribution)
                        {
                            case CostDistribution.PerApartment:
                                units = 1;
                                break;
                            case CostDistribution.PerApartmentTotalArea:
                                units = a.AdditionalArea + a.ApartmentArea;
                                break;
                            case CostDistribution.PerApartmentArea:
                                units = a.ApartmentArea;
                                break;
                            case CostDistribution.PerAdditionalArea:
                                units = a.AdditionalArea;
                                break;
                            case CostDistribution.PerLocators:
                                units = a.Locators;
                                break;
                            default:
                                units = 0;
                                break;
                        }
                        cc.Sum = Math.Round((units * cc.CostPerUnit), 2);
                        c.Components.Add(cc);
                        db.Entry(cc.GroupName).State = EntityState.Unchanged; 
                    }
                    db.Charges.Add(c);
                }
                db.SaveChanges();
            }
        }

        public static void RecalculateCharge(Charge _charge)
        {
            using (var db = new DB.DomenaDBContext())
            {
                var charge = db.Charges.Include(x => x.Components).FirstOrDefault(y => y.ChargeId.Equals(_charge.ChargeId));
                var a = db.Apartments.FirstOrDefault(x => x.ApartmentId.Equals(charge.ApartmentId));
                var b = db.Buildings.Include(x => x.CostCollection).FirstOrDefault(y => y.BuildingId.Equals(db.Apartments.FirstOrDefault(z => z.ApartmentId.Equals(charge.ApartmentId)).BuildingId));
                charge.Components.RemoveAll(x => true);
                var nullDate = new DateTime(1900, 01, 01);

                foreach (var costCollection in b.CostCollection)
                {
                    if (costCollection.EndingDate != nullDate && (costCollection.EndingDate < charge.ChargeDate || costCollection.BegginingDate > charge.ChargeDate))
                    {
                        continue;
                    }
                    var group = db.GroupName.FirstOrDefault(x => x.BuildingChargeGroupNameId == costCollection.BuildingChargeGroupNameId);
                    var cc = new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = costCollection.BuildingChargeBasisCategoryId, CostDistribution = costCollection.BuildingChargeBasisDistribution, CostPerUnit = costCollection.CostPerUnit, GroupName = group };
                    double units;
                    switch ((CostDistribution)cc.CostDistribution)
                    {
                        case CostDistribution.PerApartment:
                            units = 1;
                            break;
                        case CostDistribution.PerApartmentTotalArea:
                            units = a.AdditionalArea + a.ApartmentArea;
                            break;
                        case CostDistribution.PerApartmentArea:
                            units = a.ApartmentArea;
                            break;
                        case CostDistribution.PerAdditionalArea:
                            units = a.AdditionalArea;
                            break;
                        case CostDistribution.PerLocators:
                            units = a.Locators;
                            break;
                        default:
                            units = 0;
                            break;
                    }
                    cc.Sum = Math.Round((units * cc.CostPerUnit), 2);
                    charge.Components.Add(cc);
                    db.Entry(cc.GroupName).State = EntityState.Unchanged;
                }
                db.SaveChanges();
            }
        }

        public static Charge CreateMonthlyCharge(Apartment apartment)
        {
            Charge c;
            using (var db = new DB.DomenaDBContext())
            {
                var building = db.Buildings.Include(b => b.CostCollection).Where(x => x.BuildingId == apartment.BuildingId).FirstOrDefault();
                
                c = new Charge() { ApartmentId = apartment.ApartmentId, ChargeId = Guid.NewGuid(), IsClosed = false, ChargeDate = DateTime.Today, CreatedDate = DateTime.Today, SettlementId = Guid.Empty, AutoChargeId = Guid.Empty, OwnerId = apartment.OwnerId };
                c.Components = new List<ChargeComponent>();
                foreach (var costCollection in building.CostCollection)
                {
                    if (costCollection.BegginingDate > DateTime.Today || (costCollection.EndingDate.Year > 1901 && costCollection.EndingDate < DateTime.Today))
                    {
                        continue;
                    }
                    var group = db.GroupName.FirstOrDefault(x => x.BuildingChargeGroupNameId == costCollection.BuildingChargeGroupNameId);
                    var cc = new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = costCollection.BuildingChargeBasisCategoryId, CostDistribution = costCollection.BuildingChargeBasisDistribution, CostPerUnit = costCollection.CostPerUnit, GroupName = group };
                    double units;
                    switch ((CostDistribution)cc.CostDistribution)
                    {
                        case CostDistribution.PerApartment:
                            units = 1;
                            break;
                        case CostDistribution.PerApartmentTotalArea:
                            units = apartment.AdditionalArea + apartment.ApartmentArea;
                            break;
                        case CostDistribution.PerApartmentArea:
                            units = apartment.ApartmentArea;
                            break;
                        case CostDistribution.PerAdditionalArea:
                            units = apartment.AdditionalArea;
                            break;
                        case CostDistribution.PerLocators:
                            units = apartment.Locators;
                            break;
                        default:
                            units = 0;
                            break;
                    }
                    cc.Sum = Math.Round((units * cc.CostPerUnit), 2);
                    c.Components.Add(cc);
                }
            }
            return c;
        }
    }
}
