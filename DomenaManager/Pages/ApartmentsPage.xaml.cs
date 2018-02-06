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
using System.Data.Entity;
using MaterialDesignThemes.Wpf;
using DomenaManager.Helpers;
using System.Windows.Threading;

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for OwnersPage.xaml
    /// </summary>
    public partial class ApartmentsPage : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<ApartmentDataGrid> _apartments;
        public ObservableCollection<ApartmentDataGrid> Apartments
        {
            get { return _apartments; }
            set
            {
                _apartments = value;
                OnPropertyChanged("Apartments");
            }
        }

        private ApartmentDataGrid _selectedApartment;
        public ApartmentDataGrid SelectedApartment
        {
            get { return _selectedApartment; }
            set
            {
                _selectedApartment = value;
                OnPropertyChanged("SelectedApartment");
                OpenDrawer();   
            }
        }

        async Task PutTaskDelay()
        {
            await Task.Delay(300);
        }

        private async void OpenDrawer()
        {
            // Wait till all events in bg that can takes focus away from drawer (thus cancel it) finishes
            await PutTaskDelay();
            DrawerHost.OpenDrawerCommand.Execute(Dock.Bottom, this.DH);
        }
        
        public ICommand AddApartmentCommand
        {
            get
            {
                return new Helpers.RelayCommand(AddApartment, CanAddApartment);
            }
        }

        public ICommand EditApartmentCommand
        {
            get
            {
                return new Helpers.RelayCommand(EditApartment, CanEditApartment);
            }
        }

        public ICommand DeleteApartmentCommand
        {
            get
            {
                return new Helpers.RelayCommand(DeleteApartment, CanDeleteApartment);
            }
        }

        public ApartmentsPage()
        {
            DataContext = this;
            InitializeComponent();
            InitializeCollection();
        }

        public void InitializeCollection()
        {
            Apartments = new ObservableCollection<ApartmentDataGrid>();
            using (var db = new DB.DomenaDBContext())
            {
                var q = db.Apartments.Where(x => x.IsDeleted == false);
                foreach (var apar in q)
                {
                    var a = new ApartmentDataGrid
                    {
                        BuildingName = db.Buildings.Where(x => x.BuildingId == apar.BuildingId).FirstOrDefault().Name,
                        ApartmentNumber = apar.ApartmentNumber,
                        ApartmentArea = apar.ApartmentArea,
                        ApartmentAdditionalArea = apar.AdditionalArea,
                        ApartmentTotalArea = apar.ApartmentArea + apar.AdditionalArea,
                        ApartmentOwner = db.Owners.Where(x => x.OwnerId == apar.OwnerId).FirstOrDefault().OwnerName,
                        HasWaterMeter = apar.HasWaterMeter
                    };                   
                    Apartments.Add(a);
                }

                foreach (var apartment in Apartments)
                {
                    apartment.Balance = 0;
                    //TODO
                }
            }
        }

        private void AddApartment(object param)
        {

        }

        private bool CanAddApartment()
        {
            return true;
        }

        private void EditApartment(object param)
        {

        }

        private bool CanEditApartment()
        {
            return SelectedApartment != null;
        }

        public void DeleteApartment(object param)
        {

        }

        private bool CanDeleteApartment()
        {
            return SelectedApartment != null;
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
