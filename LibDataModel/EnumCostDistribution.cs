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
            PerMeasurement = 1,
        }

        public static string CostDistributionToString(CostDistribution costDist)
        {
            switch (costDist)
            {
                default:
                    return "";
                case CostDistribution.PerApartment:
                    return "Od lokalu";
                case CostDistribution.PerMeasurement:
                    return "Od powierzchni";
            }
        }
    }
}
