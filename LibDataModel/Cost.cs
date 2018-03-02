using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class Cost
    {
        public Cost(Cost c)
        {
            CostId = c.CostId;
            CostCategoryId = c.CostCategoryId;
            CostDistribution = c.CostDistribution;
            CostPerUnit = c.CostPerUnit;
            BegginingDate = c.BegginingDate;
            EndingDate = c.EndingDate;
        }

        public Cost()
        {
            CostId = Guid.NewGuid();
            CostCategoryId = Guid.Empty;
            CostDistribution = -1;
            CostPerUnit = -1;
            BegginingDate = new DateTime(1900, 01, 01);
            EndingDate = new DateTime(1900, 01, 01);
        }

        [Key]
        public Guid CostId { get; set; }
        public Guid CostCategoryId { get; set; }
        public int CostDistribution { get; set; }
        public double CostPerUnit { get; set; }
        public DateTime BegginingDate { get; set; }
        public DateTime EndingDate { get; set; }
    }
}
