using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDataModel;

namespace DomenaManager.Helpers
{
    public class CostDistributionCollectionItem
    {
        public string Name { get; set; }
        public int EnumValue { get; set; }

        public CostDistributionCollectionItem(CostDistribution value)
        {
            this.Name = EnumCostDistribution.CostDistributionToString(value);
            this.EnumValue = (int)value;
        }
    }
}
