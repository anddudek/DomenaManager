using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
   public enum ApartmentSellChargeSettlementType
   {
      [Description("Kupujący")]
      ALL_ON_BUYER,
      [Description("Sprzedający")]
      ALL_ON_SELLER,
      [Description("Podziel na pół")]
      SPLIT_IN_HALF,
      [Description("Według daty sprzedaży")]
      SPLIT_BY_SELL_DATE,
   }
}
