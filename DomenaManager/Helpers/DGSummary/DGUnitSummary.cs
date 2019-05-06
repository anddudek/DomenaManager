using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDataModel;

namespace DomenaManager.Helpers.DGSummary
{
    public class DGUnitSummary
    {
        public Apartment Apartment { get; set; }
        public Owner Owner { get; set; }
        public double MutualUnits { get; set; }
        public decimal MutualUnitCost { get; set; }
        public decimal MutualCost { get; set; }
        public double ConstUnits { get; set; }
        public decimal ConstUnitCost { get; set; }
        public decimal ConstCost { get; set; }
        public double VarUnits { get; set; }
        public decimal VarUnitCost { get; set; }
        public decimal VarCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}
