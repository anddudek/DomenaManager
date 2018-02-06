using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Globalization;

namespace DomenaManager.Helpers
{    
    public class StringToDoubleValidationRule : ValidationRule
    {
        public StringToDoubleValidationRule()
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
                if (double.Parse((string)value, new CultureInfo("pl-PL")) > 0)
                {
                    return new ValidationResult(true, null);
                }
                else
                {
                    return new ValidationResult(false, "Wartość musi być większa od 0");
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Niedozwolone znaki: " + e.Message);
            }            
        }
    }
}
