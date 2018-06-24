using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace DomenaManager.Helpers
{
    public class ApartmentBinding : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public Guid BindingId { get; set; }
        public ObservableCollection<BindingDataGrid> BoundApartments { get; set; }

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
