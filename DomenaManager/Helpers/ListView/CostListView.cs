using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
    public class CostListView
    {
        public string CategoryName { get; set; }
        public double Cost { get; set; }
        public DateTime BegginingDate { get; set; }
        public CostDistributionCollectionItem CostUnit { get; set; }
    }
}
