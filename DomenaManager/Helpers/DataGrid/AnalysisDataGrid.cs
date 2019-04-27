using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
    public class AnalysisDataGrid
    {
        public string Id { get; set; }
        public string Group { get; set; }
        public decimal TotalCost { get; set; }
        public decimal ApartmentCost { get; set; }
        public bool IsRepairFund { get; set; }
    }
}
