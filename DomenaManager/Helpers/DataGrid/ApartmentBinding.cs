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

        private Guid _bindingId;
        public Guid BindingId
        {
            get { return _bindingId; }
            set
            {
                if (value != _bindingId)
                {
                    _bindingId = value;
                    OnPropertyChanged("BindingId");
                }
            }
        }

        private bool _isDeleted;
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                if (value != _isDeleted)
                {
                    _isDeleted = value;
                    OnPropertyChanged("IsDeleted");
                }
            }
        }

        private ObservableCollection<BindingDataGrid> _boundApartments;
        public ObservableCollection<BindingDataGrid> BoundApartments
        {
            get { return _boundApartments; }
            set
            {
                if (value != _boundApartments)
                {
                    _boundApartments = value;
                    OnPropertyChanged("BoundApartments");
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
