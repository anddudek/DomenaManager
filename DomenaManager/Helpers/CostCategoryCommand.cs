using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
    public class CostCategoryCommand
    {
        public CostCategoryEnum.CostCategoryCommandEnum category { get; set; }
        public LibDataModel.BuildingChargeBasisCategory costItem { get; set; }
    }
}
