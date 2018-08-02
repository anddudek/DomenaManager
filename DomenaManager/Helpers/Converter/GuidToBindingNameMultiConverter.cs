using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DomenaManager.Helpers
{
    public class GuidToBindingNameMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return values[0];
                var collection = (IEnumerable<LibDataModel.BindingParent>)values[1];
                if (collection == null)
                {
                    return values[0];
                }
                var b = ((IEnumerable<LibDataModel.BindingParent>)values[1]).Where(x => x.BindingId.Equals((Guid)values[0])).FirstOrDefault();   
                if (b == null)
                {
                    return "Brak powiązania";
                }
                return b.Name;
            }
            catch
            {
                return null;
            }           
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
