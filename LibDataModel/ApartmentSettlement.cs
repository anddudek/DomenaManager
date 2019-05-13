using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class ApartmentSettlement
    {
        [Key]
        public Guid ApartmentSettlementId { get; set; }
        public Guid BuildingSettlementId { get; set; }
        public Guid ApartmentId { get; set; }
        public string SettlementData { get; set; }
        public DateTime SettlementDate { get; set; }
        public decimal Payments { get; set; }
        public decimal UsageCost { get; set; }
        public decimal Saldo { get; set; }
    }
}
