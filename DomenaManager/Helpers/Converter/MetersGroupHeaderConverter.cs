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
    public class MetersGroupHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = value as IEnumerable<object>;
            if (items == null)
                return "-";
            
            
            switch ((string)parameter)
            {    
                default:
                    break;
                case "Owner":
                    var a = ((System.Collections.ObjectModel.ReadOnlyObservableCollection<Object>)items).FirstOrDefault();
                    return ((ApartamentMeterDataGrid)a).OwnerO.OwnerName();                    
                case "ApartmentNumber":
                    var b = ((System.Collections.ObjectModel.ReadOnlyObservableCollection<Object>)items).FirstOrDefault();
                    return ((ApartamentMeterDataGrid)b).ApartmentO.ApartmentNumber;
            }

            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
