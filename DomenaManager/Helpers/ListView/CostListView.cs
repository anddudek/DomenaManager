using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DomenaManager.Helpers
{
    public class CostListView : INotifyPropertyChanged
    {
        private string _categoryName;
        public string CategoryName
        {
            get { return _categoryName; }
            set
            {
                if (value != _categoryName)
                {
                    _categoryName = value;
                    OnPropertyChanged("CategoryName");
                }
            }
        }

        private double _cost;
        public double Cost
        {
            get { return _cost; }
            set
            {
                if (value != _cost)
                {
                    _cost = value;
                    OnPropertyChanged("Cost");
                }
            }
        }

        private DateTime _begginingDate;
        public DateTime BegginingDate
        {
            get { return _begginingDate; }
            set
            {
                if (value != _begginingDate)
                {
                    _begginingDate = value;
                    OnPropertyChanged("BegginingDate");
                }
            }
        }

        private DateTime _endingDate;
        public DateTime EndingDate
        {
            get { return _endingDate; }
            set
            {
                if (value != _endingDate)
                {
                    _endingDate = value;
                    OnPropertyChanged("EndingDate");
                }
            }
        }

        private CostDistributionCollectionItem _costUnit;
        public CostDistributionCollectionItem CostUnit
        {
            get { return _costUnit; }
            set
            {
                if (value != _costUnit)
                {
                    _costUnit = value;
                    OnPropertyChanged("CostUnit");
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
