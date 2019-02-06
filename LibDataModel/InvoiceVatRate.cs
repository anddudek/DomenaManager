using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibDataModel
{
    public class InvoiceVatRate : INotifyPropertyChanged
    {
        [Key]
        public Guid InvoiceVatRateId { get; set; }
        private double _rate;
        public double Rate
        {
            get
            {
                return _rate;
            }
            set
            {
                if (value != _rate)
                {
                    _rate = value;
                    OnPropertyChanged("Rate");
                }
            }
        }
        public bool IsDeleted { get; set; }

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
