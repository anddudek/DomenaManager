using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDataModel
{
    public class EnumCostDistribution
    {
        public enum CostDistribution
        {
            PerApartment = 0,
            PerApartmentArea = 1,
            PerLocators = 2,
            PerApartmentTotalArea = 3,
            PerAdditionalArea = 4,
        }

        public static string CostDistributionToString(CostDistribution costDist)
        {
            switch (costDist)
            {
                default:
                    return "";
                case CostDistribution.PerApartment:
                    return "Od lokalu";
                case CostDistribution.PerApartmentTotalArea:
                    return "Od powierzchni całkowitej (m2)";
                case CostDistribution.PerLocators:
                    return "Ilość mieszkanów";
                case CostDistribution.PerAdditionalArea:
                    return "Od powierzchni przynależnej (m2)";
                case CostDistribution.PerApartmentArea:
                    return "Od powierzchni mieszkania (m2)";
            }
        }
    }
}
