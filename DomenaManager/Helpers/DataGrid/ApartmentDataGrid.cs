using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using LiveCharts;

namespace DomenaManager.Helpers
{
    public class ApartmentDataGrid
    {
        public Guid ApartmentId { get; set; }
        public Guid ApartmentBinding { get; set; }
        public string BuildingName { get; set; }
        public string BulidingAddress { get; set; }
        public int ApartmentNumber { get; set; }
        public string ApartmentOwner { get; set; }
        public string ApartmentOwnerAddress { get; set; }
        public double ApartmentArea { get; set; }
        public double ApartmentAdditionalArea { get; set; }
        public double ApartmentTotalArea { get; set; }
        public string ApartmentPercentageDistribution { get; set; }
        public double Balance { get; set; }
        public string HasWaterMeter { get; set; }
        public DateTime BoughtDate { get; set; }
        public DateTime WaterMeterExp { get; set; }
        public ObservableCollection<string> CostHistory { get; set; }
        public ObservableCollection<string> PaymentHistory { get; set; }
        public SeriesCollection ApartmentAreaSeries { get; set; }
        public SeriesCollection BuildingAreaSeries { get; set; }
        public int LocatorsAmount { get; set; }
    }
}
