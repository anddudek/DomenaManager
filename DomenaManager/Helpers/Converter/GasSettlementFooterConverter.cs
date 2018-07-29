using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace DomenaManager.Helpers
{
    public class GasSettlementFooterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = value as IEnumerable<object>;
            if (items == null)
                return "-";

            double sum = 0;
            
            switch ((string)parameter)
            {    
                default:
                    break;
                case "Variable":
                    foreach (ApartamentMeterDataGrid u in items) { sum += u.VariableCost; }
                    break;
                case "Area":
                    foreach (ApartamentMeterDataGrid u in items) { sum = u.SettleArea; }
                    break;
                case "Constant":
                    foreach (ApartamentMeterDataGrid u in items) { sum += u.ConstantCost; }
                    break;
                case "Charge":
                    foreach (ApartamentMeterDataGrid u in items) { sum += u.CostSettled; }
                    break;
                case "Payments":
                    foreach (ApartamentMeterDataGrid u in items) { sum += u.Charge; }
                    break;
                case "Saldo":
                    foreach (ApartamentMeterDataGrid u in items) { sum += u.Saldo; }
                    break;
            }
            
            return sum.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
