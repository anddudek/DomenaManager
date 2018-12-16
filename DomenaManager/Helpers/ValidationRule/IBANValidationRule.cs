using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Globalization;

namespace DomenaManager.Helpers
{    
    public class IBANValidationRule : ValidationRule
    {
        public IBANValidationRule()
        {
        }        

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {        
            try
            {
                string bankAccount = (string)value;
                if (bankAccount == null)
                {
                    return new ValidationResult(true, null);
                }
                string account = bankAccount.Replace(" ", "");
                if (account.Length != 26)
                    return new ValidationResult(false, "Błędna ilość znaków");
                if (account.Any(x => !char.IsDigit(x)))
                    return new ValidationResult(false, "Numer konta nie może zawierać znaków alfabetycznych");
                account = account.Substring(2, account.Length - 2) + "2521" + account.Substring(0, 2);
                int checksum = int.Parse(account.Substring(0, 1));
                for (int i = 1; i < account.Length; i++)
                {
                    int v = int.Parse(account.Substring(i, 1));
                    checksum *= 10;
                    checksum += v;
                    checksum %= 97;
                }
                return checksum == 1 ? new ValidationResult(true, null) : new ValidationResult(false, "Błędny numer konta");
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Niedozwolone znaki: " + e.Message);
            }            
        }
    }
}
