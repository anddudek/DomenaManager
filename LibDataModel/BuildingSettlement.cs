using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class BuildingSettlement
    {
        [Key]
        public Guid BuildingSettlementId { get; set; }
        public int SettlementType { get; set; }
        public Guid BuildingId { get; set; }
        public string SettlementData { get; set; }
        public DateTime SettlementDate { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }
    }
    
}
