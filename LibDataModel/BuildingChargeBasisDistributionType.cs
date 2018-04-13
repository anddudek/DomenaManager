using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class BuildingChargeBasisDistributionType
    {
        [Key]
        public Guid BuildingChargeBasisDistributionId { get; set; }
        public string BuildingChargeBasisDistributionName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
