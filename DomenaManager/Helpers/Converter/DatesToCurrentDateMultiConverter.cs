using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DomenaManager.Helpers
{
    public class DatesToCurrentDateMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((((DateTime)values[0]).Date <= DateTime.Today && ((DateTime)values[1]).Date >= DateTime.Today) || (((DateTime)values[0]).Date <= DateTime.Today && Helpers.DateTimeOperations.IsDateNull((DateTime)values[1])))
            {
                return true;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
