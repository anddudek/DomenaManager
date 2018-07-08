using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibDataModel
{
    public class MeterType : INotifyPropertyChanged
    {
        [Key]
        public Guid MeterId { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }                
        public bool IsDeleted { get; set; }

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
