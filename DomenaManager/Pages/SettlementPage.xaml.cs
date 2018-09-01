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

        private ObservableCollection<BuildingChargeBasisCategory> _allChargeCategories;
        public ObservableCollection<BuildingChargeBasisCategory> AllChargeCategories
        {
            get { return _allChargeCategories; }
            set
            {
                if (value != _allChargeCategories)
                {
                    _allChargeCategories = value;
                    OnPropertyChanged("AllChargeCategories");
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

        private BuildingChargeBasisCategory _warmWaterChargeCategoryName;
        public BuildingChargeBasisCategory WarmWaterChargeCategoryName
        {
            get { return _warmWaterChargeCategoryName; }
            set
            {
                if (value != _warmWaterChargeCategoryName)
                {
                    _warmWaterChargeCategoryName = value;
                    OnPropertyChanged("WarmWaterChargeCategoryName");
                }
            }
        }

        public string WarmWaterChargeCategoryValue { get; set; }

        private BuildingChargeBasisCategory _heatChargeCategoryName;
        public BuildingChargeBasisCategory HeatChargeCategoryName
        {
            get { return _heatChargeCategoryName; }
            set
            {
                if (value != _heatChargeCategoryName)
                {
                    _heatChargeCategoryName = value;
                    OnPropertyChanged("HeatChargeCategoryName");
                }
            }
        }

        public string HeatChargeCategoryValue { get; set; }

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
                    OnPropertyChanged("IsPerGasSettlement");
                }
            }
        }

        private SettlementMethodsEnum _constantSettlementMethod;
        public SettlementMethodsEnum ConstantSettlementMethod
        {
            get { return _constantSettlementMethod; }
            set
            {
                if (value != _constantSettlementMethod)
                {
                    _constantSettlementMethod = value;
                    OnPropertyChanged("ConstantSettlementMethod");
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

        public Visibility IsPerGasSettlement
        {
            get { return SettlementMethod == SettlementMethodsEnum.GAS ? Visibility.Visible : System.Windows.Visibility.Collapsed; }
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

        private MeterType _gasMeterName;
        public MeterType GasMeterName
        {
            get { return _gasMeterName; }
            set
            {
                if (value != _gasMeterName)
                {
                    _gasMeterName = value;
                    OnPropertyChanged("GasMeterName");
                    InitializeGasApartmentMetersCollection();
                    InitializeGasMeter();
                }
            }
        }

        public string GasMeterValue { get; set; }

        private MeterType _warmWaterMeterName;
        public MeterType WarmWaterMeterName
        {
            get { return _warmWaterMeterName; }
            set
            {
                if (value != _warmWaterMeterName)
                {
                    _warmWaterMeterName = value;
                    OnPropertyChanged("WarmWaterMeterName");
                    InitializeGasApartmentMetersCollection();
                    InitializeWarmWaterMeter();
                }
            }
        }

        public string WarmWaterMeterValue { get; set; }

        private MeterType _heatMeterName;
        public MeterType HeatMeterName
        {
            get { return _heatMeterName; }
            set
            {
                if (value != _heatMeterName)
                {
                    _heatMeterName = value;
                    OnPropertyChanged("HeatMeterName");
                    InitializeGasApartmentMetersCollection();
                    InitializeHeatMeter();
                }
            }
        }

        public string HeatMeterValue { get; set; }

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

        private ObservableCollection<ApartamentMeterDataGrid> _apartmentGasMetersCollection;
        public ObservableCollection<ApartamentMeterDataGrid> ApartmentGasMetersCollection
        {
            get { return _apartmentGasMetersCollection; }
            set
            {
                if (value != _apartmentGasMetersCollection)
                {
                    _apartmentGasMetersCollection = value;
                    OnPropertyChanged("ApartmentGasMetersCollection");
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

        private double _gasMetersDiffSum;
        public double WarmWaterDiffSum
        {
            get { return _gasMetersDiffSum; }
            set
            {
                if (value != _gasMetersDiffSum)
                {
                    _gasMetersDiffSum = value;
                    OnPropertyChanged("WarmWaterDiffSum");
                }
            }
        }

        private double _heatMetersDiffSum;
        public double HeatMetersDiffSum
        {
            get { return _heatMetersDiffSum; }
            set
            {
                if (value != _heatMetersDiffSum)
                {
                    _heatMetersDiffSum = value;
                    OnPropertyChanged("HeatMetersDiffSum");
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
                OnPropertyChanged("MainMeterDiff");
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
                OnPropertyChanged("MainMeterDiff");
            }
        }
        
        private double _noMeterConstantCharge;
        public double NoMeterConstantCharge
        {
            get { return _noMeterConstantCharge; }
            set
            {
                _noMeterConstantCharge = value;
                OnPropertyChanged("NoMeterConstantCharge");
            }
        }

        private double _noMeterConstantAdjustment;
        public double NoMeterConstantAdjustment
        {
            get { return _noMeterConstantAdjustment; }
            set
            {
                _noMeterConstantAdjustment = value;
                OnPropertyChanged("NoMeterConstantAdjustment");
            }
        }

        private bool _chargeDeficit;
        public bool ChargeDeficit
        {
            get { return _chargeDeficit; }
            set
            {
                _chargeDeficit = value;
                OnPropertyChanged("ChargeDeficit");
            }
        }

        public double MainMeterDiff
        {
            get
            {                
                return  MeterCurrentMeasure - MeterLastMeasure;
            }
            set { return; }
        }

        private double _gasMeterLastMeasure;
        public double GasMeterLastMeasure
        {
            get { return _gasMeterLastMeasure; }
            set
            {
                _gasMeterLastMeasure = value;
                OnPropertyChanged("GasMeterLastMeasure");
                OnPropertyChanged("GasMeterDiff");
            }
        }

        private double _gasMeterCurrentMeasure;
        public double GasMeterCurrentMeasure
        {
            get { return _gasMeterCurrentMeasure; }
            set
            {
                _gasMeterCurrentMeasure = value;
                OnPropertyChanged("GasMeterCurrentMeasure");
                OnPropertyChanged("GasMeterDiff");
            }
        }

        private double _noGasMeterConstantCharge;
        public double NoGasMeterConstantCharge
        {
            get { return _noGasMeterConstantCharge; }
            set
            {
                _noGasMeterConstantCharge = value;
                OnPropertyChanged("NoGasMeterConstantCharge");
            }
        }

        private double _noGasMeterConstantAdjustment;
        public double NoGasMeterConstantAdjustment
        {
            get { return _noGasMeterConstantAdjustment; }
            set
            {
                _noGasMeterConstantAdjustment = value;
                OnPropertyChanged("NoGasMeterConstantAdjustment");
            }
        }

        public double GasMeterDiff
        {
            get { return GasMeterCurrentMeasure - GasMeterLastMeasure; }
            set
            {
                return;
            }
        }

        private double _heatWaterMeterLastMeasure;
        public double HeatWaterMeterLastMeasure
        {
            get { return _heatWaterMeterLastMeasure; }
            set
            {
                _heatWaterMeterLastMeasure = value;
                OnPropertyChanged("HeatWaterMeterLastMeasure");
                OnPropertyChanged("HeatWaterMeterDiff");
            }
        }

        private double _heatWaterMeterCurrentMeasure;
        public double HeatWaterMeterCurrentMeasure
        {
            get { return _heatWaterMeterCurrentMeasure; }
            set
            {
                _heatWaterMeterCurrentMeasure = value;
                OnPropertyChanged("HeatWaterMeterCurrentMeasure");
                OnPropertyChanged("HeatWaterMeterDiff");
            }
        }
                
        public double HeatWaterMeterDiff
        {
            get { return HeatWaterMeterCurrentMeasure - HeatWaterMeterLastMeasure; }
            set
            {
                return;
            }
        }

        private double _heatWaterConstantCharge;
        public double HeatWaterConstantCharge
        {
            get { return _heatWaterConstantCharge; }
            set
            {
                _heatWaterConstantCharge = value;
                OnPropertyChanged("HeatWaterConstantCharge");
            }
        }

        private double _heatWaterConstantAdjustment;
        public double HeatWaterConstantAdjustment
        {
            get { return _heatWaterConstantAdjustment; }
            set
            {
                _heatWaterConstantAdjustment = value;
                OnPropertyChanged("HeatWaterConstantAdjustment");
            }
        }

        private double _heatMeterLastMeasure;
        public double HeatMeterLastMeasure
        {
            get { return _heatMeterLastMeasure; }
            set
            {
                _heatMeterLastMeasure = value;
                OnPropertyChanged("HeatMeterLastMeasure");
                OnPropertyChanged("HeatMeterDiff");
            }
        }

        private double _heatMeterCurrentMeasure;
        public double HeatMeterCurrentMeasure
        {
            get { return _heatMeterCurrentMeasure; }
            set
            {
                _heatMeterCurrentMeasure = value;
                OnPropertyChanged("HeatMeterCurrentMeasure");
                OnPropertyChanged("HeatMeterDiff");
            }
        }

        public double HeatMeterDiff
        {
            get { return HeatMeterCurrentMeasure - HeatMeterLastMeasure; }
            set
            {
                return;
            }
        }

        private double _noHeatMeterConstantCharge;
        public double NoHeatMeterConstantCharge
        {
            get { return _noHeatMeterConstantCharge; }
            set
            {
                _noHeatMeterConstantCharge = value;
                OnPropertyChanged("NoHeatMeterConstantCharge");
            }
        }

        private double _noHeatMeterConstantAdjustment;
        public double NoHeatMeterConstantAdjustment
        {
            get { return _noHeatMeterConstantAdjustment; }
            set
            {
                _noHeatMeterConstantAdjustment = value;
                OnPropertyChanged("NoHeatMeterConstantAdjustment");
            }
        }

        private bool _chargeGasDeficit;
        public bool ChargeGasDeficit
        {
            get { return _chargeGasDeficit; }
            set
            {
                if (value != _chargeGasDeficit)
                {
                    _chargeGasDeficit = value;
                    OnPropertyChanged("ChargeGasDeficit");
                }
            }
        }

        private bool _chargeHeatMeterDeficit;
        public bool ChargeHeatMeterDeficit
        {
            get { return _chargeHeatMeterDeficit; }
            set
            {
                if (value != _chargeHeatMeterDeficit)
                {
                    _chargeHeatMeterDeficit = value;
                    OnPropertyChanged("ChargeHeatMeterDeficit");
                }
            }
        }

        private bool _chargeHeatDeficit;
        public bool ChargeHeatDeficit
        {
            get { return _chargeHeatDeficit; }
            set
            {
                if (value != _chargeHeatDeficit)
                {
                    _chargeHeatDeficit = value;
                    OnPropertyChanged("ChargeHeatDeficit");
                }
            }
        }

        private bool _gasUnitCostAuto;
        public bool GasUnitCostAuto
        {
            get { return _gasUnitCostAuto; }
            set
            {
                if (value != _gasUnitCostAuto)
                {
                    _gasUnitCostAuto = value;
                    OnPropertyChanged("GasUnitCostAuto");
                }
            }
        }

        private string _gasUnitCost;
        public string GasUnitCost
        {
            get { return _gasUnitCost; }
            set
            {
                _gasUnitCost = value;
                OnPropertyChanged("GasUnitCost");
            }
        }

        private double _gasNeededToHeatWater = 10;
        public double GasNeededToHeatWater
        {
            get { return _gasNeededToHeatWater; }
            set
            {
                _gasNeededToHeatWater = value;
                OnPropertyChanged("GasNeededToHeatWater");
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
                ApartmentCollection = new ObservableCollection<Apartment>(db.Apartments.Include(x => x.MeterCollection.Select(y => y.MeterTypeParent)).Where(x => !x.IsDeleted).ToList());
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

        private void InitializeCategoriesList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                SettlementCategories = new ObservableCollection<BuildingChargeBasisCategory>(db.CostCategories.Where(x => !x.IsDeleted).ToList());
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

                AllChargeCategories = new ObservableCollection<BuildingChargeBasisCategory>(ChargeCategories);
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
                    SettleChargeCategories = new ObservableCollection<BuildingChargeBasisCategory>(ChargeCategories.Where(x => x.BuildingChargeBasisCategoryId.Equals(SelectedChargeCategoryName.BuildingChargeBasisCategoryId)));
                    AvailableChargeCategories = new ObservableCollection<BuildingChargeBasisCategory>(ChargeCategories.Where(x => !x.BuildingChargeBasisCategoryId.Equals(SelectedChargeCategoryName.BuildingChargeBasisCategoryId)));
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

        private void InitializeGasApartmentMetersCollection()
        {
            if (ApartmentGasMetersCollection == null)
            {
                ApartmentGasMetersCollection = new ObservableCollection<ApartamentMeterDataGrid>();
            }
            else
            {
                ApartmentGasMetersCollection.Clear();
            }
            if (WarmWaterMeterName != null && HeatMeterName != null && SelectedBuildingName != null)
            {
                foreach (var ap in ApartmentCollection.Where(x => x.BuildingId.Equals(SelectedBuildingName.BuildingId)))
                {
                    var meter = ap.MeterCollection.FirstOrDefault(x => x.MeterTypeParent.MeterId.Equals(WarmWaterMeterName.MeterId));
                    if (meter != null)
                    {
                        ApartmentGasMetersCollection.Add(new ApartamentMeterDataGrid()
                        {
                            ApartmentO = ap,
                            CurrentMeasure = meter.LastMeasure,
                            LastMeasure = meter.LastMeasure,
                            Meter = WarmWaterMeterName,
                            IsMeterLegalized = meter.LegalizationDate > DateTime.Today ? true : false,
                            OwnerO = OwnerCollection.FirstOrDefault(x => x.OwnerId.Equals(ap.OwnerId))
                        });
                    }

                    meter = ap.MeterCollection.FirstOrDefault(x => x.MeterTypeParent.MeterId.Equals(HeatMeterName.MeterId));
                    if (meter != null)
                    {
                        ApartmentGasMetersCollection.Add(new ApartamentMeterDataGrid()
                        {
                            ApartmentO = ap,
                            CurrentMeasure = meter.LastMeasure,
                            LastMeasure = meter.LastMeasure,
                            Meter = HeatMeterName,
                            IsMeterLegalized = meter.LegalizationDate > DateTime.Today ? true : false,
                            OwnerO = OwnerCollection.FirstOrDefault(x => x.OwnerId.Equals(ap.OwnerId))
                        });
                    }

                    /*meter = ap.MeterCollection.FirstOrDefault(x => x.MeterTypeParent.MeterId.Equals(GasMeterName.MeterId));
                    if (meter != null)
                    {
                        ApartmentGasMetersCollection.Add(new ApartamentMeterDataGrid()
                        {
                            ApartmentO = ap,
                            CurrentMeasure = meter.LastMeasure,
                            LastMeasure = meter.LastMeasure,
                            Meter = GasMeterName,
                            IsMeterLegalized = meter.LegalizationDate > DateTime.Today ? true : false,
                            OwnerO = OwnerCollection.FirstOrDefault(x => x.OwnerId.Equals(ap.OwnerId))
                        });
                    }*/
                }
            }

            ICollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(ApartmentGasMetersCollection);
            cv.GroupDescriptions.Clear();
            cv.GroupDescriptions.Add(new PropertyGroupDescription("ApartmentO.ApartmentId"));

        }

        private void RefreshMeters(object param)
        {
            if (SettlementMethod == SettlementMethodsEnum.PER_METERS)
            {
                MetersDiffSum = 0;
                if (ApartmentMetersCollection != null)
                {
                    foreach (var a in ApartmentMetersCollection)
                    {
                        if (a.IsMeterLegalized)
                            MetersDiffSum += (a.CurrentMeasure - a.LastMeasure);
                    }

                }
            }
            else if (SettlementMethod == SettlementMethodsEnum.GAS)
            {
                WarmWaterDiffSum = 0;
                HeatMetersDiffSum = 0;
                if (ApartmentGasMetersCollection != null && GasMeterName != null && HeatMeterName != null)
                {
                    foreach (var a in ApartmentGasMetersCollection)
                    {
                        if (a.IsMeterLegalized)
                        {
                            if (a.Meter.MeterId.Equals(WarmWaterMeterName.MeterId))
                            {
                                WarmWaterDiffSum += (a.CurrentMeasure - a.LastMeasure);
                            }
                            else if (a.Meter.MeterId.Equals(HeatMeterName.MeterId))
                            {
                                HeatMetersDiffSum += (a.CurrentMeasure - a.LastMeasure);
                            }
                        }
                    }
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

        private void InitializeGasMeter()
        {
            if (SelectedBuildingName != null && GasMeterName != null)
            {
                GasMeterLastMeasure = GasMeterName.LastMeasure;
                GasMeterCurrentMeasure = GasMeterName.LastMeasure;
            }
        }

        private void InitializeWarmWaterMeter()
        {
            if (SelectedBuildingName != null && WarmWaterMeterName != null)
            {
                HeatWaterMeterLastMeasure = WarmWaterMeterName.LastMeasure;
                HeatWaterMeterCurrentMeasure = WarmWaterMeterName.LastMeasure;
            }
        }

        private void InitializeHeatMeter()
        {
            if (SelectedBuildingName != null && HeatMeterName != null)
            {
                HeatMeterLastMeasure = HeatMeterName.LastMeasure;
                HeatMeterCurrentMeasure = HeatMeterName.LastMeasure;
            }
        }


        private bool CanRefreshMeters()
        {
            return true;
        }

        private async void AddCategory(object param)
        {
            var ecc = new Wizards.EditCostCategories();
            var result = await DialogHost.Show(ecc, "HelperDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
        }

        private bool CanAddCategory()
        {
            return true;
        }

        private void Summary(object param)
        {
            if (IsValid(ucSettlement as DependencyObject))
            {
                //var mw = (((((this.Parent as MahApps.Metro.Controls.TransitioningContentControl).Parent as Grid).Parent as DialogHost).Parent as DialogHost).Parent as DialogHost).Parent as Windows.MainWindow;

                var CurrentPage = new SettlementSummaryPage(this);
                SwitchPage.SwitchMainPage(CurrentPage, this);
            }

        }

        private bool CanSummary()
        {
            return true;
        }

        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private async void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditCostCategories);
                //Accept
                using (var db = new DB.DomenaDBContext())
                {
                    foreach (var cmd in dc.commandBuffer)
                    {
                        switch (cmd.category)
                        {
                            default:
                                break;
                            case Helpers.CostCategoryEnum.CostCategoryCommandEnum.Add:
                                db.CostCategories.Add(cmd.costItem);
                                db.SaveChanges();
                                break;
                            case Helpers.CostCategoryEnum.CostCategoryCommandEnum.Remove:
                                db.CostCategories.Where(x => x.BuildingChargeBasisCategoryId.Equals(cmd.costItem.BuildingChargeBasisCategoryId)).FirstOrDefault().IsDeleted = true;
                                db.SaveChanges();
                                break;
                            case Helpers.CostCategoryEnum.CostCategoryCommandEnum.Update:
                                db.CostCategories.Where(x => x.BuildingChargeBasisCategoryId.Equals(cmd.costItem.BuildingChargeBasisCategoryId)).FirstOrDefault().CategoryName = cmd.costItem.CategoryName;
                                db.SaveChanges();
                                break;
                        }
                    }
                }
                InitializeCategoriesList();
            }
            else if (!(bool)eventArgs.Parameter)
            {

                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    //eventArgs.Cancel();
                    var dc = (eventArgs.Session.Content as Wizards.EditCostCategories);
                    var result = await DialogHost.Show(dc, "HelperDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
                }
            }
            InitializeCategoriesList();
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
