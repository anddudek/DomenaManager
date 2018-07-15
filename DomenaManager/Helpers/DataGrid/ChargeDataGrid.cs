using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDataModel;

namespace DomenaManager.Helpers
{
    public class ChargeDataGrid : Charge
    {
        public Building Building { get; set;}
        public Apartment Apartment { get; set; }
        public Owner Owner { get; set; }
        public double Sum { get; set; }

        public ChargeDataGrid(Charge _charge)
        {
            this.ApartmentId = _charge.ApartmentId;
            this.ChargeId = _charge.ChargeId;
            this.Components = _charge.Components;
            this.ChargeDate = _charge.ChargeDate;
            this.CreatedDate = _charge.CreatedDate;
            this.IsClosed = _charge.IsClosed;
            this.SettlementId = _charge.SettlementId;

            using (var db = new DB.DomenaDBContext())
            {
                if (_charge.Components != null)
                {
                    this.Sum = _charge.Components.Select(x => x.Sum).Sum();
                }
                this.Apartment = db.Apartments.Where(x => x.ApartmentId.Equals(this.ApartmentId)).FirstOrDefault();
                this.Building = db.Buildings.Where(x => x.BuildingId.Equals(this.Apartment.BuildingId)).FirstOrDefault();
                this.Owner = db.Owners.Where(x => x.OwnerId.Equals(this.Apartment.OwnerId)).FirstOrDefault();                
            }
        }
    }
}
