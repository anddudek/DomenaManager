using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
    public class GroupNameCommand
    {
        public CostCategoryEnum.CostCategoryCommandEnum category { get; set; }
        public LibDataModel.BuildingChargeBasisGroup Item { get; set; }        
    }
}
