using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DomenaManager.Helpers
{
    public enum SettlementUnitType
    {
        [Description("Powierzchnia całkowita")]
        TOTAL_AREA,
        [Description("Powierzchnia dodatkowa")]
        ADDITIONAL_AREA,
        [Description("Powierzchnia grzewcza")]
        APARTMENT_AREA,
        [Description("Ilość lokatorów")]
        LOCATORS,
        [Description("Lokal")]
        PER_APARTMENT,
    }
}
