using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibDataModel
{
    public class ChargeComponent : INotifyPropertyChanged
    {
        private Guid _chargeComponentId;
        [Key]
        public Guid ChargeComponentId
        {
            get
            {
                return _chargeComponentId;
            }
            set
            {
                if (value != _chargeComponentId)
                {
                    _chargeComponentId = value;
                    OnPropertyChanged("ChargeComponentId");
                }

            }
        }

        private double _sum;
        public double Sum
        {
            get
            {
                return _sum;
            }
            set
            {
                if (value != _sum)
                {
                    _sum = value;
                    OnPropertyChanged("Sum");
                }

            }
        }

        private double _costPerUnit;
        public double CostPerUnit
        {
            get
            {
                return _costPerUnit;
            }
            set
            {
                if (value != _costPerUnit)
                {
                    _costPerUnit = value;
                    OnPropertyChanged("CostPerUnit");
                }

            }
        }

        private int _costDistribution;
        public int CostDistribution
        {
            get
            {
                return _costDistribution;
            }
            set
            {
                if (value != _costDistribution)
                {
                    _costDistribution = value;
                    OnPropertyChanged("CostDistribution");
                }

            }
        }

        private Guid _costCategoryId;
        public Guid CostCategoryId
        {
            get
            {
                return _costCategoryId;
            }
            set
            {
                if (value != _costCategoryId)
                {
                    _costCategoryId = value;
                    OnPropertyChanged("CostCategoryId");
                }

            }
        }

        private BuildingChargeGroupName _groupName;
        public BuildingChargeGroupName GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (value != _groupName)
                {
                    _groupName = value;
                    OnPropertyChanged("GroupName");
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
