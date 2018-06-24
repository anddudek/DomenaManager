using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using LibDataModel;

namespace DomenaManager.Helpers
{
    public class BindingDataGrid : INotifyPropertyChanged
    {
        public Apartment apartment { get; set; }
        public Building building { get; set; }
        public Owner owner { get; set; }

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
