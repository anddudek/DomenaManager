using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Globalization;

namespace DomenaManager.Helpers
{    
    public class EmptyStringValidationRule : ValidationRule
    {
        public EmptyStringValidationRule()
        {
        }        

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {        

            try
            {
                if (((string)value).Length > 0)
                {
                    return new ValidationResult(true, null);
                }
                else
                {
                    return new ValidationResult(false, "Pole nie może być puste");
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Niedozwolone znaki: " + e.Message);
            }            
        }
    }
}
