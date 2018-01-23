using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
    public class PageTitleToFAIconConverter
    {
        public static string Get(string pageTitle)
        {
            switch (pageTitle)
            {
                default:
                    return "";
                case "Budynki":
                    return "\uf015";                    
            }
        }
    }
}
