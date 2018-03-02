using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
    public static class DateTimeOperations
    {
        public static bool IsDateNull(DateTime value)
        {
            return value.Date.CompareTo(new DateTime(1900, 01, 01)) == 0 ? true : false;
        }
    }
}
