using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class YearSummary
    {
        [Key]
        public Guid YearSummaryId { get; set; }
        public Building Building { get; set; }
        public decimal BuildingOutcome { get; set; }
        public decimal BuildingIncome { get; set; }
        public decimal BuildingSaldo { get; set; }
        public bool IsClosed { get; set; }
    }
}
