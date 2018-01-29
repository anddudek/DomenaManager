using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using LibDataModel;
using MaterialDesignThemes.Wpf;
using DomenaManager.Helpers;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditOwnerWizard.xaml
    /// </summary>
    public partial class EditOwnerWizard : UserControl, INotifyPropertyChanged
    {
        private string _ownerName;
        public string OwnerName
        {
            get { return _ownerName; }
            set 
            { 
                _ownerName = value;
                OnPropertyChanged("OwnerName");
            }
        }

        private string _mailAddress;
        public string MailAddress
        {
            get { return _mailAddress; }
            set
            {
                _mailAddress = value;
                OnPropertyChanged("MailAddress");
            }
        }

        private ObservableCollection<ApartmentListView> _apartmentsOwned;
        public ObservableCollection<ApartmentListView> ApartmentsOwned
        {
            get { return _apartmentsOwned; }
            set
            {
                _apartmentsOwned = value;
                OnPropertyChanged("ApartmentsOwned");
            }
        }

        private Owner _ownerLocalCopy;

        public EditOwnerWizard(Owner SelectedOwner = null)
        {
            DataContext = this;
            InitializeComponent();
            if (SelectedOwner != null)
            {
                _ownerLocalCopy = new Owner(SelectedOwner);

                OwnerName = SelectedOwner.OwnerName;
                MailAddress = SelectedOwner.MailAddress;
                using (var db = new DB.DomenaDBContext())
                {
                    var q = db.Apartments.Where(x => x.OwnerId == SelectedOwner.OwnerId);
                    foreach (var a in q)
                    {
                        var apartment = new ApartmentListView();
                        apartment.BuildingName = db.Buildings.Where(x => x.BuildingId == a.BuildingId).Select(x => x.Name).FirstOrDefault();
                        apartment.ApartmentNumber = a.ApartmentNumber;
                        ApartmentsOwned.Add(apartment);
                    }
                    if (ApartmentsOwned != null && ApartmentsOwned.Count == 0)
                    {
                        ApartmentsOwned.Add(new ApartmentListView { BuildingName = "Brak mieszkań" });
                    }
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
