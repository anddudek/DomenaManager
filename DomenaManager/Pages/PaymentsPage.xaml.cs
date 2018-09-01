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
using LibDataModel;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Collections;

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for OwnersPage.xaml
    /// </summary>
    public partial class PaymentsPage : UserControl, INotifyPropertyChanged
    {
        private IList _selectedPayments;
        public IList SelectedPayments
        {
            get { return _selectedPayments; }
            set
            {
                if (value != _selectedPayments)
                {
                    _selectedPayments = value;
                    OnPropertyChanged("SelectedPayments");
                }
            }
        }
       
        private ObservableCollection<PaymentDataGrid> _payments;
        public ObservableCollection<PaymentDataGrid> Payments
        {
            get { return _payments; }
            set
            {
                _payments = value;
                OnPropertyChanged("Payments");
            }
        }

        private ICollectionView _paymentsCV;
        public ICollectionView PaymentsCV
        {
            get
            {
                return _paymentsCV;
            }
            set
            {
                if (value != _paymentsCV)
                {
                    _paymentsCV = value;
                    OnPropertyChanged("PaymentsCV");
                }
            }
        }

        private PaymentDataGrid _selectedPayment;
        public PaymentDataGrid SelectedPayment
        {
            get { return _selectedPayment; }
            set
            {
                _selectedPayment = value;
                OnPropertyChanged("SelectedPayment");                
            }
        }

        private bool _groupByBuilding;
        public bool GroupByBuilding
        {
            get { return _groupByBuilding; }
            set
            { 
                if (value != _groupByBuilding)
                {
                    ICollectionView cvCharges = (CollectionView)CollectionViewSource.GetDefaultView(Payments);
                    if (value)
                    {
                        cvCharges.GroupDescriptions.Add(new PropertyGroupDescription("Building.Name")); //nameof(Building.BUildingName)
                    }
                    else
                    {
                        cvCharges.GroupDescriptions.Remove(cvCharges.GroupDescriptions.Cast<PropertyGroupDescription>().Where(x => x.PropertyName == "Building.Name").FirstOrDefault());
                    }
                    _groupByBuilding = value;
                    OnPropertyChanged("GroupByBuilding");
                }
            }
        }

        private bool _groupByApartment;
        public bool GroupByApartment
        {
            get { return _groupByApartment; }
            set
            {
                if (value != _groupByApartment)
                {
                    ICollectionView cvCharges = (CollectionView)CollectionViewSource.GetDefaultView(Payments);
                    if (value)
                    {
                        cvCharges.GroupDescriptions.Add(new PropertyGroupDescription("Apartment.ApartmentNumber"));
                    }
                    else
                    {
                        cvCharges.GroupDescriptions.Remove(cvCharges.GroupDescriptions.Cast<PropertyGroupDescription>().Where(x => x.PropertyName == "Apartment.ApartmentNumber").FirstOrDefault());
                    }
                    _groupByApartment = value;
                    OnPropertyChanged("GroupByApartment");
                }
            }
        }

        private ObservableCollection<Owner> _ownersNames;
        public ObservableCollection<Owner> OwnersNames
        {
            get { return _ownersNames; }
            set
            {
                _ownersNames = value;
                OnPropertyChanged("OwnersNames");
            }
        }

        private Owner _selectedOwnerName;
        public Owner SelectedOwnerName
        {
            get { return _selectedOwnerName; }
            set
            {
                _selectedOwnerName = value;
                OnPropertyChanged("SelectedOwnerName");
                PaymentsCV.Refresh();
            }
        }

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
                InitializeApartmentsNumbers();
                OnPropertyChanged("ApartmentsNumbers");
                OnPropertyChanged("SelectedBuildingName");
                PaymentsCV.Refresh();
            }
        }

        private ObservableCollection<int> _apartmentsNumbers;
        public ObservableCollection<int> ApartmentsNumbers
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

        private int? _selectedApartmentNumber;
        public int? SelectedApartmentNumber
        {
            get { return _selectedApartmentNumber; }
            set
            {
                if (value != _selectedApartmentNumber)
                {
                    _selectedApartmentNumber = value;
                    OnPropertyChanged("SelectedApartmentNumber");
                    PaymentsCV.Refresh();
                }
            }
        }
        
        public ICommand ClearFilterCommand
        {
            get { return new Helpers.RelayCommand(ClearFilter, CanClearFilter); }
        }

        public ICommand DeletePaymentCommand
        {
            get { return new Helpers.RelayCommand(DeletePayment, CanDeletePayment); }
        }

        public ICommand EditPaymentCommand
        {
            get { return new Helpers.RelayCommand(Edit, CanEdit); }
        }

        public ICommand AddPaymentCommand
        {
            get { return new Helpers.RelayCommand(Add, CanAdd); }
        }

        public ICommand ShowPaymentDetails
        {
            get { return new Helpers.RelayCommand(ShowDetails, CanShowDetails); }
        }

        public PaymentsPage()
        {
            DataContext = this;
            InitializeCollection();
            InitializeLists();
            InitializeApartmentsNumbers();
            SelectedPayments = new List<PaymentDataGrid>();
            InitializeComponent();
            GroupByBuilding = true;
        }

        public PaymentsPage(Apartment apartment)
        {
            DataContext = this;
            InitializeCollection();
            InitializeLists();
            InitializeApartmentsNumbers();
            SelectedPayments = new List<PaymentDataGrid>();
            InitializeComponent();
            GroupByBuilding = true;

            using (var db = new DB.DomenaDBContext())
            {
                var apar = db.Apartments.FirstOrDefault(x => x.ApartmentId.Equals(apartment.ApartmentId));
                SelectedBuildingName = BuildingsNames.FirstOrDefault(x => x.BuildingId.Equals(apar.BuildingId));
                SelectedApartmentNumber = apar.ApartmentNumber;
                SelectedOwnerName = OwnersNames.FirstOrDefault(x => x.OwnerId.Equals(apar.OwnerId));
            }
        }

        private void InitializeCollection()
        {
            Payments = new ObservableCollection<PaymentDataGrid>();
            using (var db = new DB.DomenaDBContext())
            {
                foreach (var p in db.Payments.Where(x => !x.IsDeleted))
                {
                    var pdg = new PaymentDataGrid(p);
                    Payments.Add(pdg); 
                }
            }

            PaymentsCV = (CollectionView)CollectionViewSource.GetDefaultView(Payments);
            PaymentsCV.SortDescriptions.Add(new SortDescription("PaymentRegistrationDate", ListSortDirection.Ascending));
            PaymentsCV.Filter = FilterCollection;
            if (GroupByBuilding)
            {
                ICollectionView cvCharges = (CollectionView)CollectionViewSource.GetDefaultView(Payments);
                cvCharges.GroupDescriptions.Add(new PropertyGroupDescription("Building.Name")); //nameof(Building.BUildingName)
            }
        }

        private void InitializeLists()
        {
            using (var db = new DB.DomenaDBContext())
            {
                _buildingsNames = new ObservableCollection<Building>(db.Buildings.ToList());
                _ownersNames = new ObservableCollection<Owner>(db.Owners.ToList());
            }
        }

        private void InitializeApartmentsNumbers()
        {            
            if (Payments != null && SelectedBuildingName != null)
            {
                var a = SelectedBuildingName.BuildingId;
                var b = Payments.Where(x => x.Building.BuildingId.Equals(SelectedBuildingName.BuildingId)).ToList();
                var c = b.Select(x => x.Apartment.ApartmentNumber).ToList();
                var d = c.Distinct().ToList();
                ApartmentsNumbers = new ObservableCollection<int>(Payments.Where(x => x.Building.BuildingId.Equals(SelectedBuildingName.BuildingId)).Select(x => x.Apartment.ApartmentNumber).Distinct().OrderBy(x => x).ToList());
            }
        }

        private bool CanEdit()
        {
            return true;
        }

        private void Edit(object param)
        {
            Wizards.EditPaymentWizard eow = new Wizards.EditPaymentWizard(SelectedPayment);

            SwitchPage.SwitchMainPage(eow, this);
        }

        private bool CanAdd()
        {
            return true;
        }

        private void Add(object param)
        {
            Wizards.EditPaymentWizard eow = new Wizards.EditPaymentWizard();

            SwitchPage.SwitchMainPage(eow, this);
        }

        private bool CanShowDetails()
        {
            return true;
        }

        private void ShowDetails(object param)
        {
            Wizards.EditPaymentWizard eow = new Wizards.EditPaymentWizard(SelectedPayment);

            SwitchPage.SwitchMainPage(eow, this);
        }

        private void ClearFilter(object param)
        {
            SelectedOwnerName = null;
            SelectedBuildingName = null;
            SelectedApartmentNumber = null;
            PaymentsCV.Refresh();
        }

        private bool CanClearFilter()
        {
            return true;
        }

        private bool FilterCollection(object item)
        {
            var pdg = item as PaymentDataGrid;
            if (SelectedBuildingName != null && !pdg.Building.BuildingId.Equals(SelectedBuildingName.BuildingId))
            {
                return false;
            }
            if (SelectedApartmentNumber != null && !pdg.Apartment.ApartmentNumber.Equals(SelectedApartmentNumber))
            {
                return false;
            }
            if (SelectedOwnerName != null && !pdg.Owner.OwnerId.Equals(SelectedOwnerName.OwnerId))
            {
                return false;
            }
            
            return true;
        }

        private async void DeletePayment(object param)
        {
            bool ynResult = await Helpers.YNMsg.Show("Czy chcesz usunąć zaznaczoną wpłatę?");
            if (ynResult)
            {
                using (var db = new DB.DomenaDBContext())
                {
                    db.Payments.Where(x => x.PaymentId.Equals(SelectedPayment.PaymentId)).FirstOrDefault().IsDeleted = true;
                    db.SaveChanges();
                }
                InitializeCollection();
            }
        }

        private bool CanDeletePayment()
        {
            return SelectedPayment != null;
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
