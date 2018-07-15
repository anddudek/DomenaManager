using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LibDataModel;

namespace DomenaManager.Helpers
{
    public class ApartamentMeterDataGrid : INotifyPropertyChanged
    {
        private Apartment _apartmentO;
        public Apartment ApartmentO
        {
            get { return _apartmentO; }
            set
            {
                if (value != _apartmentO)
                {
                    _apartmentO = value;
                    OnPropertyChanged("ApartmentO");
                }
            }
        }

        private Owner _ownerO;
        public Owner OwnerO
        {
            get { return _ownerO; }
            set
            {
                if (value != _ownerO)
                {
                    _ownerO = value;
                    OnPropertyChanged("OwnerO");
                }
            }
        }

        private MeterType _meter;
        public MeterType Meter
        {
            get { return _meter; }
            set
            {
                if (value != _meter)
                {
                    _meter = value;
                    OnPropertyChanged("Meter");
                }
            }
        }

        private double _settleArea;
        public double SettleArea
        {
            get { return _settleArea; }
            set
            {
                if (value != _settleArea)
                {
                    _settleArea = value;
                    OnPropertyChanged("SettleArea");
                }
            }
        }

        private bool _isMeterLegalized;
        public bool IsMeterLegalized
        {
            get { return _isMeterLegalized; }
            set
            {
                if (value != _isMeterLegalized)
                {
                    _isMeterLegalized = value;
                    OnPropertyChanged("IsMeterLegalized");
                }
            }
        }

        private double _lastMeasure;
        public double LastMeasure
        {
            get { return _lastMeasure; }
            set
            {
                if (value != _lastMeasure)
                {
                    _lastMeasure = value;
                    OnPropertyChanged("LastMeasure");
                    OnPropertyChanged("MeterDifference");
                }
            }
        }

        private double _currentMeasure;
        public double CurrentMeasure
        {
            get { return _currentMeasure; }
            set
            {
                if (value != _currentMeasure)
                {
                    _currentMeasure = value;
                    OnPropertyChanged("CurrentMeasure");
                    OnPropertyChanged("MeterDifference");
                }
            }
        }

        private double _meterDifference;
        public double MeterDifference
        {
            get 
            {
                _meterDifference = CurrentMeasure - LastMeasure;
                return _meterDifference; 
            }
            set
            {
                if (value != _meterDifference)
                {
                    _meterDifference = value;
                    OnPropertyChanged("MeterDifference");
                }
            }
        }

        private double _variableCost;
        public double VariableCost
        {
            get { return _variableCost; }
            set
            {
                if (value != _variableCost)
                {
                    _variableCost = value;
                    OnPropertyChanged("VariableCost");
                }
            }
        }

        private double _constantCost;
        public double ConstantCost
        {
            get { return _constantCost; }
            set
            {
                if (value != _constantCost)
                {
                    _constantCost = value;
                    OnPropertyChanged("ConstantCost");
                }
            }
        }

        private double _costSettled;
        public double CostSettled
        {
            get
            {
                return _costSettled;
            }
            set
            {
                if (value != _costSettled)
                {
                    _costSettled = value;
                    OnPropertyChanged("CostSettled");
                }
            }
        }

        private double _charge;
        public double Charge
        {
            get
            {
                return _charge;
            }
            set
            {
                if (value != _charge)
                {
                    _charge = value;
                    OnPropertyChanged("Charge");
                }
            }
        }

        private double _saldo;
        public double Saldo
        {
            get
            {
                return _saldo;
            }
            set
            {
                if (value != _saldo)
                {
                    _saldo = value;
                    OnPropertyChanged("Saldo");
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
