using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DomenaManager.Helpers
{
    public enum SettlementTypeEnum
    {
        //[Description("Ciepła woda")]
        //HOT_WATER,
        [Description("Ciepła woda i ogrzewanie")]
        HOT_WATER_AND_HEATING,
        [Description("Bieżąca woda")]
        WATER,
        [Description("Rozliczenie jednostkowe")]
        UNITS,
    }
}
