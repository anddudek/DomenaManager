using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class CostDistributionTyp2e
    {
        [Key]
        public Guid CostDistributionId { get; set; }
        public string CostDistributionName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
