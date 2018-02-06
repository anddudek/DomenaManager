﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
    public class ApartmentDataGrid
    {
        public string BuildingName { get; set; }
        public int ApartmentNumber { get; set; }
        public string ApartmentOwner { get; set; }
        public double ApartmentArea { get; set; }
        public double ApartmentAdditionalArea { get; set; }
        public double ApartmentTotalArea { get; set; }
        public double Balance { get; set; }
        public bool HasWaterMeter { get; set; }
    }
}
