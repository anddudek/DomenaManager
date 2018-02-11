using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDataModel
{
    public class EnumCostCategory
    {
        public enum Category
        {
            Maintenance = 0,
            Service = 1,
            Water = 2,
            Electricity = 3,
            Outdoors = 4,
            Renovation = 5,
            Insurance = 6, 
            Other = 7,
            LandlordSalary = 8,
            Janitors = 9,
        }        

        public string CategoryToString(Category catEnum)
        {
            switch (catEnum)
            {
                default:
                    return "";
                case Category.Electricity:
                    return "Prąd";
                case Category.Insurance:
                    return "Ubezpieczenie";
                case Category.Janitors:
                    return "Utrzymanie czystości";
                case Category.LandlordSalary:
                    return "Wynagrodzenie zarządcy";
                case Category.Maintenance:
                    return "Przeglądy";
                case Category.Other:
                    return "Inne";
                case Category.Outdoors:
                    return "Teren zewnętrzny";
                case Category.Renovation:
                    return "Remonty";
                case Category.Service:
                    return "Naprawy i konserwacje";
                case Category.Water:
                    return "Woda i ścieki";
            }
        }
    }
}
