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
        public static void GenerateCharges(object param)
        {
            using (var db = new DB.DomenaDBContext())
            {
                var q = db.Buildings.Include(b => b.CostCollection);
                foreach (var a in db.Apartments.Where(x => q.Where(y => y.BuildingId.Equals(x.BuildingId)).FirstOrDefault().CostCollection.Count > 0))
                {
                    var c = new Charge() { ApartmentId = a.ApartmentId, ChargeId = Guid.NewGuid(), IsClosed = false, CreatedTime = DateTime.Today };
                    c.Components = new List<ChargeComponent>();
                    foreach (var costCollection in db.Buildings.Include(b => b.CostCollection).Where(x => x.BuildingId.Equals(a.BuildingId)).FirstOrDefault().CostCollection)
                    {
                        if (costCollection.BegginingDate < DateTime.Today || costCollection.EndingDate > DateTime.Today)
                        {
                            continue;
                        }
                        var cc = new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = costCollection.BuildingChargeBasisCategoryId, CostDistribution = costCollection.BuildingChargeBasisDistribution, CostPerUnit = costCollection.CostPerUnit };
                        double units;
                        switch ((EnumCostDistribution.CostDistribution)cc.CostDistribution)
                        {
                            case EnumCostDistribution.CostDistribution.PerApartment:
                                units = 1;
                                break;
                            case EnumCostDistribution.CostDistribution.PerMeasurement:
                                units = a.AdditionalArea + a.ApartmentArea;
                                break;
                            default:
                                units = 0;
                                break;
                        }
                        cc.Sum = units * cc.CostPerUnit;
                        c.Components.Add(cc);
                        db.Charges.Add(c);
                    }
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
                
                foreach (var costCollection in b.CostCollection)
                {
                    if (costCollection.BegginingDate < DateTime.Today || costCollection.EndingDate > DateTime.Today)
                    {
                        continue;
                    }
                    var cc = new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = costCollection.BuildingChargeBasisCategoryId, CostDistribution = costCollection.BuildingChargeBasisDistribution, CostPerUnit = costCollection.CostPerUnit };
                    double units;
                    switch ((EnumCostDistribution.CostDistribution)cc.CostDistribution)
                    {
                        case EnumCostDistribution.CostDistribution.PerApartment:
                            units = 1;
                            break;
                        case EnumCostDistribution.CostDistribution.PerMeasurement:
                            units = a.AdditionalArea + a.ApartmentArea;
                            break;
                        default:
                            units = 0;
                            break;
                    }
                    cc.Sum = units * cc.CostPerUnit;
                    charge.Components.Add(cc);
                }
                db.SaveChanges();
            }
        }
    }
}
