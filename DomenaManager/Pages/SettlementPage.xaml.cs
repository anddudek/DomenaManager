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
using LibDataModel;
using System.Collections.Specialized;
using DomenaManager.Helpers;

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for BuildingsPage.xaml
    /// </summary>
    public partial class SettlementPage : UserControl, INotifyPropertyChanged
    {
        #region Bindings

        private ObservableCollection<Building> _buildingNames;
        public ObservableCollection<Building> BuildingNames
        {
            get { return _buildingNames; }
            set
            {
                if (value != _buildingNames)
                {
                    _buildingNames = value;
                    OnPropertyChanged("BuildingNames");
                }
            }
        }

        private Building _selectedBuildingName;
        public Building SelectedBuildingName
        {
            get { return _selectedBuildingName; }
            set
            {
                if (value != _selectedBuildingName)
                {
                    _selectedBuildingName = value;
                    OnPropertyChanged("SelectedBuildingName");
                    OnPropertyChanged("SelectedBuildingAddress");
                    if (value != null)
                    {
                        InitializeSettlementInvoices();
                        InitializeChargeCategories();
                        PopulateChargeCategories();
                        InitializeMetersCollection();
                    }
                }
            }
        }

        public string SelectedBuildingValue { get; set; }

        public string SelectedBuildingAddress
        {
            get
            {
                return SelectedBuildingName == null ? string.Empty : SelectedBuildingName.GetAddress();
            }
            set { return; }           
        }

        private DateTime _settlementFrom;
        public DateTime SettlementFrom
        {
            get { return _settlementFrom; }
            set
            {
                _settlementFrom = value;
                OnPropertyChanged("SettlementFrom");
            }
        }

        private DateTime _settlementTo;
        public DateTime SettlementTo
        {
            get { return _settlementTo; }
            set
            {
                _settlementTo = value;
                OnPropertyChanged("SettlementTo");
            }
        }

        private ObservableCollection<InvoiceCategory> _invoiceCategories;
        public ObservableCollection<InvoiceCategory> InvoiceCategories
        {
            get { return _invoiceCategories; }
            set
            {
                if (value != _invoiceCategories)
                {
                    _invoiceCategories = value;
                    OnPropertyChanged("InvoiceCategories");
                }
            }
        }

        private InvoiceCategory _selectedInvoiceCategoryName;
        public InvoiceCategory SelectedInvoiceCategoryName
        {
            get { return _selectedInvoiceCategoryName; }
            set
            {
                if (value != _selectedInvoiceCategoryName)
                {
                    _selectedInvoiceCategoryName = value;
                    OnPropertyChanged("SelectedInvoiceCategoryName");
                    if (value != null)
                    {
                        InitializeSettlementInvoices();
                    }
                }
            }
        }

        public string SelectedInvoiceCategoryValue { get; set; }

        private ObservableCollection<Invoice> InvoicesList;

        private ObservableCollection<Invoice> _settledInvoices;
        public ObservableCollection<Invoice> SettledInvoices
        {
            get { return _settledInvoices; }
            set
            {
                if (value != _settledInvoices)
                {
                    _settledInvoices = value;
                    OnPropertyChanged("SettledInvoices");
                }
            }
        }

        private ObservableCollection<Invoice> _availableInvoices;
        public ObservableCollection<Invoice> AvailableInvoices
        {
            get { return _availableInvoices; }
            set
            {
                if (value != _availableInvoices)
                {
                    _availableInvoices = value;
                    OnPropertyChanged("AvailableInvoices");
                }
            }
        }

        public ObservableCollection<BuildingChargeBasisCategory> AllCategories;

        private ObservableCollection<BuildingChargeBasisCategory> _chargeCategories;
        public ObservableCollection<BuildingChargeBasisCategory> ChargeCategories
        {
            get { return _chargeCategories; }
            set
            {
                if (value != _chargeCategories)
                {
                    _chargeCategories = value;
                    OnPropertyChanged("ChargeCategories");
                }
            }
        }

        private BuildingChargeBasisCategory _selectedChargeCategoryName;
        public BuildingChargeBasisCategory SelectedChargeCategoryName
        {
            get { return _selectedChargeCategoryName; }
            set
            {
                if (value != null)
                {
                    _selectedChargeCategoryName = value;
                    OnPropertyChanged("SelectedChargeCategoryName");
                    PopulateChargeCategories();
                }
            }
        }

        private ObservableCollection<BuildingChargeBasisCategory> _settleChargeCategories;
        public ObservableCollection<BuildingChargeBasisCategory> SettleChargeCategories
        {
            get { return _settleChargeCategories; }
            set
            {
                if (value != _settleChargeCategories)
                {
                    _settleChargeCategories = value;
                    OnPropertyChanged("SettleChargeCategories");
                }
            }
        }
        
        private ObservableCollection<BuildingChargeBasisCategory> _availableChargeCategories;
        public ObservableCollection<BuildingChargeBasisCategory> AvailableChargeCategories
        {
            get { return _availableChargeCategories; }
            set
            {
                if (value != _availableChargeCategories)
                {
                    _availableChargeCategories = value;
                    OnPropertyChanged("AvailableChargeCategories");
                }
            }
        }

        public string SelectedChargeCategoryValue { get; set; }
        
        private ObservableCollection<BuildingChargeBasisCategory> _settlementCategories;
        public ObservableCollection<BuildingChargeBasisCategory> SettlementCategories
        {
            get { return _settlementCategories; }
            set
            {
                if (value != _settlementCategories)
                {
                    _settlementCategories = value;
                    OnPropertyChanged("SettlementCategories");
                }
            }
        }

        private BuildingChargeBasisCategory _settlementCategoryName;
        public BuildingChargeBasisCategory SettlementCategoryName
        {
            get { return _settlementCategoryName; }
            set
            {
                if (value != _settlementCategoryName)
                {
                    _settlementCategoryName = value;
                    OnPropertyChanged("SettlementCategoryName");
                }
            }
        }

        public string SettlementCategoryValue { get; set; }

        private SettlementMethodsEnum _settlementMethod;
        public SettlementMethodsEnum SettlementMethod
        {
            get { return _settlementMethod; }
            set
            {
                if (value != _settlementMethod)
                {
                    _settlementMethod = value;
                    OnPropertyChanged("SettlementMethod");
                    OnPropertyChanged("IsPerMetersSettlement");
                }
            }
        }

        public Visibility IsPerMetersSettlement
        {
            get { return SettlementMethod == SettlementMethodsEnum.PER_METERS ? Visibility.Visible : System.Windows.Visibility.Collapsed; }
            private set
            {
                return;
            }
        }

        private ObservableCollection<MeterType> _metersCollection;
        public ObservableCollection<MeterType> MetersCollection
        {
            get { return _metersCollection; }
            set
            {
                if (value != _metersCollection)
                {
                    _metersCollection = value;
                    OnPropertyChanged("MetersCollection");
                }
            }
        }

        private MeterType _selectedMeterName;
        public MeterType SelectedMeterName
        {
            get { return _selectedMeterName; }
            set
            {
                if (value != _selectedMeterName)
                {
                    _selectedMeterName = value;
                    OnPropertyChanged("SelectedMeterName");
                    InitializeApartmentMetersCollection();
                    InitializeMainMeter();
                }
            }
        }

        public string SelectedMeterValue { get; set; }

        private ObservableCollection<ApartamentMeterDataGrid> _apartmentMetersCollection;
        public ObservableCollection<ApartamentMeterDataGrid> ApartmentMetersCollection
        {
            get { return _apartmentMetersCollection; }
            set
            {
                if (value != _apartmentMetersCollection)
                {
                    _apartmentMetersCollection = value;
                    OnPropertyChanged("ApartmentMetersCollection");
                }
            }
        }

        public ObservableCollection<Apartment> ApartmentCollection;

        public ObservableCollection<Owner> OwnerCollection;

        private double _metersDiffSum;
        public double MetersDiffSum
        {
            get { return _metersDiffSum; }
            set
            {
                if (value != _metersDiffSum)
                {
                    _metersDiffSum = value;
                    OnPropertyChanged("MetersDiffSum");
                }
            }
        }

        private double _constantPeriod = 0;
        public double ConstantPeriod
        {
            get { return _constantPeriod; }
            set
            {
                if (value != _constantPeriod)
                {
                    _constantPeriod = value;
                    OnPropertyChanged("ConstantPeriod");
                    OnPropertyChanged("VariablePeriod");
                }
            }
        }
        
        public double VariablePeriod
        {
            get { return 100 - ConstantPeriod; }
            set
            {
                return;
            }
        }

        private double _meterLastMeasure;
        public double MeterLastMeasure
        {
            get { return _meterLastMeasure; }
            set
            {
                _meterLastMeasure = value;
                OnPropertyChanged("MeterLastMeasure");
            }
        }

        private double _meterCurrentMeasure;
        public double MeterCurrentMeasure
        {
            get { return _meterCurrentMeasure; }
            set
            {
                _meterCurrentMeasure = value;
                OnPropertyChanged("MeterCurrentMeasure");
            }
        }

        public ICommand RefreshMeterDiffSum
        {
            get { return new RelayCommand(RefreshMeters, CanRefreshMeters); }
        }

        public ICommand AddNewCategory
        {
            get { return new RelayCommand(AddCategory, CanAddCategory); }
        }

        public ICommand SettlementSummary
        {
            get { return new RelayCommand(Summary, CanSummary); }
        }

        #endregion

        public SettlementPage()
        {
            DataContext = this;
            InitializeComponent();
            InitializeCollections();
            SettlementFrom = new DateTime(DateTime.Now.Year, 1, 1);
            SettlementTo = new DateTime(DateTime.Now.Year, 12, 31);
        }

        private void InitializeCollections()
        {
            using (var db = new DB.DomenaDBContext())
            {
                BuildingNames = new ObservableCollection<Building>(db.Buildings.Include(x => x.CostCollection).Include(x => x.MeterCollection).Where(x => !x.IsDeleted).ToList());
                InvoiceCategories = new ObservableCollection<InvoiceCategory>(db.InvoiceCategories.Where(x => !x.IsDeleted).ToList());
                InvoicesList = new ObservableCollection<Invoice>(db.Invoices.Where(x => !x.IsDeleted));
                AllCategories = new ObservableCollection<BuildingChargeBasisCategory>(db.CostCategories.Where(x => !x.IsDeleted).ToList());
                SettlementCategories = new ObservableCollection<BuildingChargeBasisCategory>(db.CostCategories.Where(x => !x.IsDeleted).ToList());
                ApartmentCollection = new ObservableCollection<Apartment>(db.Apartments.Include(x => x.MeterCollection).Where(x => !x.IsDeleted).ToList());
                OwnerCollection = new ObservableCollection<Owner>(db.Owners.Where(x => !x.IsDeleted).ToList());
                InitializeSettlementInvoices();
                InitializeChargeCategories();
                InitializeMetersCollection();                
                InitializeApartmentMetersCollection();
            }
        }

        private void InitializeSettlementInvoices()
        {
            if (SelectedBuildingName != null)
            {
                if (SelectedInvoiceCategoryName != null)
                {
                    SettledInvoices = new ObservableCollection<Invoice>(InvoicesList.Where(x => x.InvoiceCategoryId.Equals(SelectedInvoiceCategoryName.CategoryId) && x.BuildingId.Equals(SelectedBuildingName.BuildingId) && x.InvoiceDate >= SettlementFrom && x.InvoiceDate <= SettlementTo).ToList());
                    AvailableInvoices = new ObservableCollection<Invoice>(InvoicesList.Where(x => !x.InvoiceCategoryId.Equals(SelectedInvoiceCategoryName.CategoryId) && x.BuildingId.Equals(SelectedBuildingName.BuildingId) && x.InvoiceDate >= SettlementFrom && x.InvoiceDate <= SettlementTo).ToList());
                }
                else
                {
                    SettledInvoices = new ObservableCollection<Invoice>();
                    AvailableInvoices = new ObservableCollection<Invoice>(InvoicesList.Where(x => x.BuildingId.Equals(SelectedBuildingName.BuildingId) && x.InvoiceDate >= SettlementFrom && x.InvoiceDate <= SettlementTo).ToList());
                }
            }
        }

        private void InitializeChargeCategories()
        {
            if (SelectedBuildingName != null)
            {
                ChargeCategories = new ObservableCollection<BuildingChargeBasisCategory>();
                var c = SelectedBuildingName.CostCollection.Select(x => x.BuildingChargeBasisCategoryId);
                foreach (var cc in c)
                {
                    ChargeCategories.Add(AllCategories.FirstOrDefault(x => x.BuildingChargeBasisCategoryId.Equals(cc)));
                }
            }
        }

        private void PopulateChargeCategories()
        {
            if (SelectedBuildingName != null)
            {
                if (SelectedChargeCategoryName == null)
                {
                    SettleChargeCategories = new ObservableCollection<BuildingChargeBasisCategory>();
                    AvailableChargeCategories = new ObservableCollection<BuildingChargeBasisCategory>(ChargeCategories);
                }
                else
                {
                    SettleChargeCategories = new ObservableCollection<BuildingChargeBasisCategory>(ChargeCategories.Where(x => !x.BuildingChargeBasisCategoryId.Equals(SelectedChargeCategoryName.BuildingChargeBasisCategoryId)));
                    AvailableChargeCategories = new ObservableCollection<BuildingChargeBasisCategory>(ChargeCategories.Where(x => x.BuildingChargeBasisCategoryId.Equals(SelectedChargeCategoryName.BuildingChargeBasisCategoryId)));
                }
            }
        }

        private void InitializeMetersCollection()
        {
            if (SelectedBuildingName != null)
            {
                MetersCollection = new ObservableCollection<MeterType>(SelectedBuildingName.MeterCollection.Where(x => !x.IsDeleted).ToList());
            }
        }

        private void InitializeApartmentMetersCollection()
        {
            if (ApartmentMetersCollection == null)
            {
                ApartmentMetersCollection = new ObservableCollection<ApartamentMeterDataGrid>();
            }
            else
            {
                ApartmentMetersCollection.Clear();
            }
            if (SelectedMeterName != null && SelectedBuildingName != null)
            {
                foreach (var ap in ApartmentCollection.Where(x => x.BuildingId.Equals(SelectedBuildingName.BuildingId)))
                {
                    var meter = ap.MeterCollection.FirstOrDefault(x => x.MeterTypeParent.MeterId.Equals(SelectedMeterName.MeterId));
                    if (meter != null)
                    {
                        ApartmentMetersCollection.Add(new ApartamentMeterDataGrid()
                        {
                            ApartmentO = ap,
                            CurrentMeasure = meter.LastMeasure,
                            LastMeasure = meter.LastMeasure,
                            Meter = SelectedMeterName,
                            IsMeterLegalized = meter.LegalizationDate > DateTime.Today ? true : false,
                            OwnerO = OwnerCollection.FirstOrDefault(x => x.OwnerId.Equals(ap.OwnerId))
                        });
                    }
                }
            }

            ICollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(ApartmentMetersCollection);
            cv.GroupDescriptions.Clear();
            cv.GroupDescriptions.Add(new PropertyGroupDescription(""));
            
        }

        private void RefreshMeters(object param)
        {
            MetersDiffSum = 0;
            if (ApartmentMetersCollection != null)
            {
                foreach (var a in ApartmentMetersCollection)
                {
                    MetersDiffSum += (a.CurrentMeasure - a.LastMeasure);
                }

            }
        }

        private void InitializeMainMeter()
        {
            if (SelectedBuildingName != null && SelectedMeterName != null)
            {
                MeterLastMeasure = SelectedMeterName.LastMeasure;
                MeterCurrentMeasure = SelectedMeterName.LastMeasure;
            }
        }
            

        private bool CanRefreshMeters()
        {
            return true;
        }

        private async void AddCategory(object param)
        {

        }

        private bool CanAddCategory()
        {
            return true;
        }

        private void Summary(object param)
        {

        }

        private bool CanSummary()
        {
            return true;
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
