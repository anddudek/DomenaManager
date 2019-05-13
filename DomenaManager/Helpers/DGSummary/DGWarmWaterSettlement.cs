using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LibDataModel;

namespace DomenaManager.Helpers.DGSummary
{
    public class DGWarmWaterSettlement : INotifyPropertyChanged
    {
        private Apartment _apartment;
        public Apartment Apartment
        {
            get
            {
                return _apartment;
            }
            set
            {
                if (value != _apartment)
                {
                    _apartment = value;
                    OnPropertyChanged("Apartment");
                }
            }
        }

        private Owner _owner;
        public Owner Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                if (value != _owner)
                {
                    _owner = value;
                    OnPropertyChanged("Owner");
                }
            }
        }

        private double _warmWaterLastMeasure;
        public double WarmWaterLastMeasure
        {
            get
            {
                return _warmWaterLastMeasure;
            }
            set
            {
                if (value != _warmWaterLastMeasure)
                {
                    _warmWaterLastMeasure = value;
                    OnPropertyChanged("WarmWaterLastMeasure");
                    OnPropertyChanged("WarmWaterUsage");
                }
            }
        }

        private double _warmWaterCurrentMeasure;
        public double WarmWaterCurrentMeasure
        {
            get
            {
                return _warmWaterCurrentMeasure;
            }
            set
            {
                if (value != _warmWaterCurrentMeasure)
                {
                    _warmWaterCurrentMeasure = value;
                    OnPropertyChanged("WarmWaterCurrentMeasure");
                    OnPropertyChanged("WarmWaterUsage");
                }
            }
        }

        private double _warmWaterUsage;
        public double WarmWaterUsage
        {
            get
            {
                return _warmWaterUsage;
            }
            set
            {
                if (value != _warmWaterUsage)
                {
                    _warmWaterUsage = value;
                    OnPropertyChanged("WarmWaterUsage");
                    OnPropertyChanged("TotalUsage");
                }
            }
        }

        private double _coldWaterLastMeasure;
        public double ColdWaterLastMeasure
        {
            get
            {
                return _coldWaterLastMeasure;
            }
            set
            {
                if (value != _coldWaterLastMeasure)
                {
                    _coldWaterLastMeasure = value;
                    OnPropertyChanged("ColdWaterLastMeasure");
                    OnPropertyChanged("ColdWaterUsage");
                }
            }
        }

        private double _coldWaterCurrentMeasure;
        public double ColdWaterCurrentMeasure
        {
            get
            {
                return _coldWaterCurrentMeasure;
            }
            set
            {
                if (value != _coldWaterCurrentMeasure)
                {
                    _coldWaterCurrentMeasure = value;
                    OnPropertyChanged("ColdWaterCurrentMeasure");
                    OnPropertyChanged("ColdWaterUsage");
                }
            }
        }

        private double _coldWaterUsage;
        public double ColdWaterUsage
        {
            get
            {
                return _coldWaterUsage;
            }
            set
            {
                if (value != _coldWaterUsage)
                {
                    _coldWaterUsage = value;
                    OnPropertyChanged("ColdWaterUsage");
                    OnPropertyChanged("TotalUsage");
                }
            }
        }
        
        public double TotalUsage
        {
            get
            {
                return WarmWaterUsage + ColdWaterUsage;
            }
        }

        private double _usageCost;
        public double UsageCost
        {
            get
            {
                return _usageCost;
            }
            set
            {
                if (value != _usageCost)
                {
                    _usageCost = value;
                    OnPropertyChanged("UsageCost");
                }
            }
        }

        private decimal _payments;
        public decimal Payments
        {
            get
            {
                return _payments;
            }
            set
            {
                if (value != _payments)
                {
                    _payments = value;
                    OnPropertyChanged("Payments");
                }
            }
        }

        private double _saldoCost;
        public double SaldoCost
        {
            get
            {
                return _saldoCost;
            }
            set
            {
                if (value != _saldoCost)
                {
                    _saldoCost = value;
                    OnPropertyChanged("SaldoCost");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
