using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using LibDataModel;

namespace DomenaManager.Helpers
{
    public class IntToCostDistributionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return EnumCostDistribution.CostDistributionToString((EnumCostDistribution.CostDistribution)value);
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        } 
    }
}
