using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Globalization;

namespace DomenaManager.Helpers
{    
    public class StringToDoubleInvoiceValidationRule : ValidationRule
    {
        public StringToDoubleInvoiceValidationRule()
        {
        }        

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {        

            try
            {
                if (String.IsNullOrWhiteSpace((string)value))
                {
                    return new ValidationResult(false, "Pole nie może być puste");
                }
                return new ValidationResult(true, null);
                
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Niedozwolone znaki: " + e.Message);
            }            
        }
    }
}
