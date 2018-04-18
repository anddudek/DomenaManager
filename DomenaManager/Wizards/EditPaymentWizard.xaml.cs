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
    /// Interaction logic for EditPaymentWizard.xaml
    /// </summary>
    public partial class EditPaymentWizard : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<Building> _buildingsNames;
        public ObservableCollection<Building> BuildingsNames
        {
            get { return _buildingsNames; }
            set
            {
                _buildingsNames = value;
                OnPropertyChanged("BuildingsNames");
            }
        }

        private Building _selectedBuildingName;
        public Building SelectedBuildingName
        {
            get { return _selectedBuildingName; }
            set
            {
                _selectedBuildingName = value;
                OnPropertyChanged("SelectedBuildingName");
                OnPropertyChanged("SelectedBuildingAddress");
                InitializeApartmentsNumbers();
            }
        }

        private ObservableCollection<Apartment> _apartmentsOC { get; set; }

        private ObservableCollection<Owner> _ownersOC { get; set; }

        private ObservableCollection<Apartment> _apartmentsNumbers;
        public ObservableCollection<Apartment> ApartmentsNumbers
        {
            get { return _apartmentsNumbers; }
            set
            {
                if (value != _apartmentsNumbers)
                {
                    _apartmentsNumbers = value;
                    OnPropertyChanged("ApartmentsNumbers");
                }
            }
        }

        private Apartment _selectedApartmentNumber;
        public Apartment SelectedApartmentNumber
        {
            get { return _selectedApartmentNumber; }
            set
            {
                if (value != _selectedApartmentNumber)
                {
                    _selectedApartmentNumber = value;
                    OnPropertyChanged("SelectedApartmentNumber");

                    if (SelectedApartmentNumber != null)
                    {
                        OwnerMailAddress = _ownersOC.Where(x => x.OwnerId.Equals(SelectedApartmentNumber.OwnerId)).FirstOrDefault().OwnerName + Environment.NewLine;
                        OwnerMailAddress += SelectedApartmentNumber.CorrespondenceAddress == null ? _ownersOC.Where(x => x.OwnerId.Equals(SelectedApartmentNumber.OwnerId)).FirstOrDefault().MailAddress : SelectedApartmentNumber.CorrespondenceAddress;
                    }
                }
            }
        }

        private string _ownerMailAddress;

        public string OwnerMailAddress
        {
            get { return _ownerMailAddress; }
            set
            {
                if (value != _ownerMailAddress)
                {
                    _ownerMailAddress = value;
                    OnPropertyChanged("OwnerMailAddress");
                }
            }
        }

        public string SelectedApartmentNumberValue { get; set; }

        public string SelectedBuildingValue { get; set; }

        private string _paymentAmount;
        
        public string PaymentAmount
        {
            get { return _paymentAmount; }
            set
            {
                if (value != _paymentAmount)
                {
                    _paymentAmount = value;
                    OnPropertyChanged("PaymentAmount");
                }
            }
        }

        private DateTime _paymentRegistrationDate;

        public DateTime PaymentRegistrationDate
        {
            get { return _paymentRegistrationDate; }
            set
            {
                if (value != _paymentRegistrationDate)
                {
                    _paymentRegistrationDate = value;
                    OnPropertyChanged("PaymentRegistrationDate");
                }
            }
        }

        public Payment _lpc;

        public EditPaymentWizard(Payment _payment = null)
        {
            DataContext = this;
            InitializeComponent();
            InitializeBuildingList();
            using (var db = new DB.DomenaDBContext())
            {
                _apartmentsOC = new ObservableCollection<Apartment>(db.Apartments.ToList());
                _ownersOC = new ObservableCollection<Owner>(db.Owners.ToList());
            }
            InitializeApartmentsNumbers();
            PaymentRegistrationDate = DateTime.Today;
        }

        private void InitializeApartmentsNumbers()
        {
            using (var db = new DB.DomenaDBContext())
            {
                if (SelectedBuildingName == null)
                {
                    ApartmentsNumbers = new ObservableCollection<Apartment>();
                }
                else
                {
                    ApartmentsNumbers = new ObservableCollection<Apartment>(_apartmentsOC.Where(x => x.BuildingId.Equals(SelectedBuildingName.BuildingId)).ToList());
                }
            }
            OnPropertyChanged("ApartmentsNumbers");
        }

        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private async void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            /*if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditOwnerWizard);
                //Accept
                if (dc._ownerLocalCopy == null)
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.OwnerName) || string.IsNullOrEmpty(dc.MailAddress)))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Add new owner
                    using (var db = new DB.DomenaDBContext())
                    {
                        var newOwner = new LibDataModel.Owner { OwnerId = Guid.NewGuid(), MailAddress = dc.MailAddress, OwnerName = dc.OwnerName, IsDeleted = false };
                        db.Owners.Add(newOwner);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.OwnerName) || string.IsNullOrEmpty(dc.MailAddress)))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Edit Owner
                    using (var db = new DB.DomenaDBContext())
                    {
                        var q = db.Owners.Where(x => x.OwnerId.Equals(dc._ownerLocalCopy.OwnerId)).FirstOrDefault();
                        q.OwnerName = dc.OwnerName;
                        q.MailAddress = dc.MailAddress;
                        db.SaveChanges();
                    }
                }
            }
            else if (!(bool)eventArgs.Parameter)
            {

                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    //eventArgs.Cancel();
                    var dc = (eventArgs.Session.Content as Wizards.EditOwnerWizard);
                    var result = await DialogHost.Show(dc, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
                }
            }
            InitializeCollection();*/
        }

        private void InitializeBuildingList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                BuildingsNames = new ObservableCollection<Building>(db.Buildings.Where(x => x.IsDeleted == false).ToList());
            }
        }

        private bool IsValid(DependencyObject obj)
        {
            // The dependency object is valid if it has no errors and all
            // of its children (that are dependency objects) are error-free.
            return !Validation.GetHasError(obj) &&
            LogicalTreeHelper.GetChildren(obj)
            .OfType<DependencyObject>()
            .All(IsValid);
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
