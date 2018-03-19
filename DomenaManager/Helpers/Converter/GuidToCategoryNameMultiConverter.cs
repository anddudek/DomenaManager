using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DomenaManager.Helpers
{
    public class GuidToCategoryNameMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return ((IEnumerable<LibDataModel.CostCategory>)values[1]).Where(x => x.CostCategoryId.Equals((Guid)values[0])).FirstOrDefault().CategoryName;                
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
