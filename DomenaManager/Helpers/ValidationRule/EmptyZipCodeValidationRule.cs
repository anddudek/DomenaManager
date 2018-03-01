using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Globalization;

namespace DomenaManager.Helpers
{    
    public class EmptyZipCodeValidationRule : ValidationRule
    {
        public EmptyZipCodeValidationRule()
        {
        }        

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {        

            try
            {
                if (((string)value).Length != 6)
                {
                    return new ValidationResult(false, "Wartość musi być w formacie XX-XXX");
                }
                else if (((string)value)[2] != '-')
                {
                    return new ValidationResult(false, "Wartość musi być w formacie XX-XXX");
                }
                for (int i=0; i<6; i++)
                {
                    if (i == 2)
                        continue;

                    if (!Char.IsNumber(((string)value)[i]))
                        return new ValidationResult(false, "Wartość musi być w formacie XX-XXX");
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
