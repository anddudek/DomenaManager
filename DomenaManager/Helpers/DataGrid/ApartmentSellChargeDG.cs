using LibDataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
   public class ApartmentSellChargeDG : INotifyPropertyChanged
   {
      private Charge _charge;
      public Charge Charge
      {
         get
         {
            return _charge;
         }
         set
         {
            if (value != _charge)
            {
               _charge = value;
               OnPropertyChanged("Change");
            }
         }
      }

      private ApartmentSellChargeSettlementType _settlementType;
      public ApartmentSellChargeSettlementType SettlementType
      {
         get
         {
            return _settlementType;
         }
         set
         {
            if (value != _settlementType)
            {
               _settlementType = value;
               OnPropertyChanged("SettlementType");
            }
         }
      }

      private decimal _totalCost;
      public decimal TotalCost
      {
         get
         {
            return _totalCost;
         }
         set
         {
            if (value != _totalCost)
            {
               _totalCost = value;
               OnPropertyChanged("TotalCost");
            }
         }
      }

      private decimal _oldOwnerCost;
      public decimal OldOwnerCost
      {
         get
         {
            return _oldOwnerCost;
         }
         set
         {
            if (value != _oldOwnerCost)
            {
               _oldOwnerCost = value;
               OnPropertyChanged("OldOwnerCost");
            }
         }
      }

      private decimal _newOwnerCost;
      public decimal NewOwnerCost
      {
         get
         {
            return _newOwnerCost;
         }
         set
         {
            if (value != _newOwnerCost)
            {
               _newOwnerCost = value;
               OnPropertyChanged("NewOwnerCost");
            }
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;
      private void OnPropertyChanged(string propertyName)
      {
         PropertyChangedEventHandler handler = PropertyChanged;
         if (handler != null)
         {
            handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }
   }
}
