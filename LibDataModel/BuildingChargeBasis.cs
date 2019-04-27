using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    /// <summary>
    /// Each building has a list of costs. At the begging of the month, based on the costs of the buidling, the charges are calculated.
    /// </summary>
    public class BuildingChargeBasis
    {
        public BuildingChargeBasis(BuildingChargeBasis c)
        {
            BuildingChargeBasisId = c.BuildingChargeBasisId;
            BuildingChargeBasisCategoryId = c.BuildingChargeBasisCategoryId;
            BuildingChargeBasisDistribution = c.BuildingChargeBasisDistribution;
            BuildingChargeGroupNameId = c.BuildingChargeGroupNameId;
            CostPerUnit = c.CostPerUnit;
            BegginingDate = c.BegginingDate;
            EndingDate = c.EndingDate;
        }

        public BuildingChargeBasis()
        {
            BuildingChargeBasisId = Guid.NewGuid();
            BuildingChargeBasisCategoryId = Guid.Empty;
            BuildingChargeBasisDistribution = -1;
            CostPerUnit = -1;
            BegginingDate = new DateTime(1900, 01, 01);
            EndingDate = new DateTime(1900, 01, 01);
        }

        [Key]
        public Guid BuildingChargeBasisId { get; set; }
        public Guid BuildingChargeBasisCategoryId { get; set; }
        public int BuildingChargeBasisDistribution { get; set; }
        public decimal CostPerUnit { get; set; }
        public DateTime BegginingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public Guid BuildingChargeGroupNameId { get; set; }
    }
}
