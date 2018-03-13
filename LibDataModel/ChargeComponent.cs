using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class ChargeComponent
    {
        [Key]
        public Guid ChargeComponentId { get; set; }
        public double Sum { get; set; }
        public double CostPerUnit { get; set; }
        public int CostDistribution { get; set; }
        public Guid CostCategoryId { get; set; }
    }
}
