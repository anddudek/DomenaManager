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
    public class GuidToBindingNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var bindingsList = (IEnumerable<LibDataModel.BindingParent>)(((System.Windows.Data.CollectionViewSource)parameter).Source);
                var g = (Guid)value;
                if (g == Guid.Empty)
                {
                    return "Brak powiązania";
                }
                return bindingsList.FirstOrDefault(x => x.BindingId == g).Name;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
