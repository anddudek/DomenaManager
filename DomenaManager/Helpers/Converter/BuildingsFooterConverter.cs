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
    public class BuildingsFooterConverter : IValueConverter
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
                case "AppNumber":
                    sum = items.Count();
                    break;
                case "AppArea":
                    foreach (ApartmentDataGrid u in items) { sum += u.ApartmentArea; }
                    break;
                case "AddArea":
                    foreach (ApartmentDataGrid u in items) { sum += u.ApartmentAdditionalArea; }
                    break;
                case "TotalArea":
                    foreach (ApartmentDataGrid u in items) { sum += u.ApartmentTotalArea; }
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
