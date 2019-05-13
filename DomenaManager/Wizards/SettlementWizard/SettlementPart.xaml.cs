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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data.Entity;
using LibDataModel;
using DomenaManager.Helpers;
using MaterialDesignThemes.Wpf;
using DomenaManager.Helpers.DGSummary;

namespace DomenaManager.Wizards.SettlementWizard
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class SettlementPart : UserControl, INotifyPropertyChanged
    {
        public SettlementData SettlementData { get; set; }

        #region UnitSettlement

        public string SelectedSettlementType
        {
            get
            {
                if (SettlementData == null || SettlementData.InvoiceData == null || SettlementData.InvoiceData.MasterData == null)
                    return "Nie zaznaczono";
                return SettlementData.InvoiceData.MasterData.SettlementType.Description();
            }
        }

        public Visibility IsUnitSettlement
        {
            get
            {
                if (SettlementData == null || SettlementData.InvoiceData == null || SettlementData.InvoiceData.MasterData == null)
                    return Visibility.Hidden;
                return SettlementData.InvoiceData.MasterData.SettlementType == SettlementTypeEnum.UNITS ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public decimal VarSum
        {
            get
            {
                if (SettlementData == null || SettlementData.InvoiceData == null || SettlementData.InvoiceData.Invoices == null)
                    return 0;
                return SettlementData.InvoiceData.Invoices.Where(x => x.IsChecked).Select(x => x.CostAmountVariableGross).DefaultIfEmpty(0).Sum();
            }
        }

        public decimal TotalSum
        {
            get
            {
                if (SettlementData == null || SettlementData.InvoiceData == null || SettlementData.InvoiceData.Invoices == null)
                    return 0;
                return SettlementData.InvoiceData.Invoices.Where(x => x.IsChecked).Select(x => x.CostAmountGross).DefaultIfEmpty(0).Sum();
            }
        }

        public decimal ConstSum
        {
            get
            {
                if (SettlementData == null || SettlementData.InvoiceData == null || SettlementData.InvoiceData.Invoices == null)
                    return 0;
                return SettlementData.InvoiceData.Invoices.Where(x => x.IsChecked).Select(x => x.CostAmountConstGross).DefaultIfEmpty(0).Sum();
            }
        }

        private bool _isConstVarMutual;
        public bool IsConstVarMutual
        {
            get { return _isConstVarMutual; }
            set
            {
                if (value != _isConstVarMutual)
                {
                    _isConstVarMutual = value;
                    OnPropertyChanged("IsConstVarMutual");
                    OnPropertyChanged("IsMutualVisible");
                    OnPropertyChanged("IsMutualNotVisible");
                }
            }
        }

        private SettlementUnitType _mutualSummaryType;
        public SettlementUnitType MutualSummaryType
        {
            get { return _mutualSummaryType; }
            set
            {
                if (value != _mutualSummaryType)
                {
                    _mutualSummaryType = value;
                    OnPropertyChanged("MutualSummaryType");
                    OnPropertyChanged("MutualUnitsCount");
                    OnPropertyChanged("MutualUnitCost");
                }
            }
        }

        private SettlementUnitType _constSummaryType;
        public SettlementUnitType ConstSummaryType
        {
            get { return _constSummaryType; }
            set
            {
                if (value != _constSummaryType)
                {
                    _constSummaryType = value;
                    OnPropertyChanged("ConstSummaryType");
                    OnPropertyChanged("ConstUnitsCount");
                    OnPropertyChanged("ConstUnitCost");
                }
            }
        }

        private SettlementUnitType _varSummaryType;
        public SettlementUnitType VarSummaryType
        {
            get { return _varSummaryType; }
            set
            {
                if (value != _varSummaryType)
                {
                    _varSummaryType = value;
                    OnPropertyChanged("VarSummaryType");
                    OnPropertyChanged("VarUnitsCount");
                    OnPropertyChanged("VarUnitCost");
                }
            }
        }

        public string MutualUnitsCount
        {
            get
            {
                return CalculateUnitCount(MutualSummaryType).ToString() + CalculateUnitSuffix(MutualSummaryType);
            }
        }

        public decimal MutualUnitCost
        {
            get
            {
                if (CalculateUnitCount(MutualSummaryType) == 0)
                    return 0;
                //return (decimal.Ceiling(100 * TotalSum / Convert.ToDecimal(CalculateUnitCount(MutualSummaryType)))) / 100;
                return TotalSum / Convert.ToDecimal(CalculateUnitCount(MutualSummaryType));
            }
        }

        public string ConstUnitsCount
        {
            get
            {
                return CalculateUnitCount(ConstSummaryType).ToString() + CalculateUnitSuffix(ConstSummaryType);
            }
        }

        public decimal ConstUnitCost
        {
            get
            {
                if (CalculateUnitCount(ConstSummaryType) == 0)
                    return 0;
                //return (decimal.Ceiling(100 * ConstSum / Convert.ToDecimal(CalculateUnitCount(ConstSummaryType)))) / 100;
                return ConstSum / Convert.ToDecimal(CalculateUnitCount(ConstSummaryType));
            }
        }

        public string VarUnitsCount
        {
            get 
            {
                return CalculateUnitCount(VarSummaryType).ToString() + CalculateUnitSuffix(ConstSummaryType);
            }
        }

        public decimal VarUnitCost
        {
            get
            {
                if (CalculateUnitCount(VarSummaryType) == 0)
                    return 0;
                return VarSum / Convert.ToDecimal(CalculateUnitCount(VarSummaryType));
                //return (decimal.Ceiling(100 * VarSum / Convert.ToDecimal(CalculateUnitCount(VarSummaryType)))) / 100;
            }
        }

        public Visibility IsMutualVisible
        {
            get
            {
                return IsConstVarMutual ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility IsMutualNotVisible
        {
            get
            {
                return IsConstVarMutual ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public ICommand AddMeter
        {
            get
            {
                //return new Helpers.RelayCommand(AddNewMeter, CanAddNewMeter);
                return null;
            }
        }

        #endregion

        #region WaterSettlement

        public Visibility IsWaterSettlement
        {
            get
            {
                if (SettlementData == null || SettlementData.InvoiceData == null || SettlementData.InvoiceData.MasterData == null)
                    return Visibility.Hidden;
                return SettlementData.InvoiceData.MasterData.SettlementType == SettlementTypeEnum.WATER ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private decimal _waterUnitCost;
        public decimal WaterUnitCost
        {
            get
            {
                return _waterUnitCost;
            }
            set
            {
                if (value != _waterUnitCost)
                {
                    _waterUnitCost = value;
                    OnPropertyChanged("WaterUnitCost");
                }
            }
        }

        private decimal _sewageUnitCost;
        public decimal SewageUnitCost
        {
            get
            {
                return _sewageUnitCost;
            }
            set
            {
                if (value != _sewageUnitCost)
                {
                    _sewageUnitCost = value;
                    OnPropertyChanged("SewageUnitCost");
                }
            }
        }

        private double _totalCounter;
        public double TotalCounter
        {
            get
            {
                return _totalCounter;
            }
            set
            {
                if (value != _totalCounter)
                {
                    _totalCounter = value;
                    OnPropertyChanged("TotalCounter");
                    OnPropertyChanged("ProposedUnitCost");
                }
            }
        }
        
        public decimal ProposedUnitCost
        {
            get
            {
                if (SettlementData?.InvoiceData?.Invoices != null && TotalCounter != 0)
                    return decimal.Round(SettlementData.InvoiceData.Invoices.Select(x => x.CostAmount).DefaultIfEmpty(0).Sum() / Convert.ToDecimal(TotalCounter), 2);
                return 0;
            }
        }

        private double _totalUsage;
        public double TotalUsage
        {
            get
            {
                return _totalUsage;
            }
            set
            {
                if (value != _totalUsage)
                {
                    _totalUsage = value;
                    OnPropertyChanged("TotalUsage");
                }
            }
        }

        private double _shortage;
        public double Shortage
        {
            get
            {
                return _shortage;
            }
            set
            {
                if (value != _shortage)
                {
                    _shortage = value;
                    OnPropertyChanged("Shortage");
                }
            }
        }

        private ObservableCollection<MeterType> _counters;
        public ObservableCollection<MeterType> Counters
        {
            get
            {
                return _counters;
            }
            set
            {
                if (value != _counters)
                {
                    _counters = value;
                    OnPropertyChanged("Counters");
                }
            }
        }

        private MeterType _hotWaterCounter;
        public MeterType HotWaterCounter
        {
            get
            {
                return _hotWaterCounter;
            }
            set
            {
                if (value != _hotWaterCounter)
                {
                    _hotWaterCounter = value;
                    OnPropertyChanged("HotWaterCounter");
                }
            }
        }

        private MeterType _coldWaterCounter;
        public MeterType ColdWaterCounter
        {
            get
            {
                return _coldWaterCounter;
            }
            set
            {
                if (value != _coldWaterCounter)
                {
                    _coldWaterCounter = value;
                    OnPropertyChanged("ColdWaterCounter");
                }
            }
        }

        private ObservableCollection<BuildingChargeBasisCategory> _paymentsCategories;
        public ObservableCollection<BuildingChargeBasisCategory> PaymentsCategories
        {
            get
            {
                return _paymentsCategories;
            }
            set
            {
                if (value != _paymentsCategories)
                {
                    _paymentsCategories = value;
                    OnPropertyChanged("PaymentsCategories");
                }
            }
        }

        private BuildingChargeBasisCategory _paymentCategory;
        public BuildingChargeBasisCategory PaymentCategory
        {
            get
            {
                return _paymentCategory;
            }
            set
            {
                if (value != _paymentCategory)
                {
                    _paymentCategory = value;
                    OnPropertyChanged("PaymentCategory");
                }
            }
        }

        private ObservableCollection<DGWarmWaterSettlement> _waterSettlementCounters;
        public ObservableCollection<DGWarmWaterSettlement> WaterSettlementCounters
        {
            get
            {
                return _waterSettlementCounters;
            }
            set
            {
                if (value != _waterSettlementCounters)
                {
                    _waterSettlementCounters = value;
                    OnPropertyChanged("WaterSettlementCounters");
                }
            }
        }

        #endregion  

        private List<Apartment> _apartmentsList;

        public SettlementPart()
        {
            InitializeComponent();
            DataContext = this;
            InitializeView();
        }

        public void InitializeView()
        {
            using (var db = new DB.DomenaDBContext())
            {
                _apartmentsList = db.Apartments.Include(x => x.MeterCollection).ToList();
                if (SettlementData?.InvoiceData?.MasterData.Building != null)
                {
                    var building = SettlementData?.InvoiceData?.MasterData.Building;
                    _counters = new ObservableCollection<MeterType>(db.Buildings.Include(x => x.MeterCollection).Where(x => x.BuildingId == building.BuildingId).FirstOrDefault().MeterCollection.Where(x => !x.IsDeleted && x.IsApartment).ToList());
                    PaymentsCategories = new ObservableCollection<BuildingChargeBasisCategory>(db.CostCategories.Where(x => !x.IsDeleted).ToList());
                }

                if (WaterSettlementCounters == null || WaterSettlementCounters.Count == 0)
                {
                    var buildingId = SettlementData?.InvoiceData?.MasterData?.Building?.BuildingId;
                    var startingDate = SettlementData?.InvoiceData?.MasterData?.StartingDate;
                    var endingDate = SettlementData?.InvoiceData?.MasterData?.EndingDate;
                    var paymentCategory = PaymentCategory?.BuildingChargeBasisCategoryId;
                    WaterSettlementCounters = new ObservableCollection<DGWarmWaterSettlement>();
                    var currentBuildingApartments = _apartmentsList.Where(x => x.BuildingId == buildingId && !x.IsDeleted);
                    var components = db.Charges.Include(x => x.Components).Where(x => x.ChargeDate >= startingDate && x.ChargeDate <= endingDate && x.Components.Any(c => c.CostCategoryId == paymentCategory)).Select(x => x.Components).SelectMany(x => x).Where(x => x.CostCategoryId == paymentCategory);
                    foreach (var ap in currentBuildingApartments)
                    {
                        var dgwws = new DGWarmWaterSettlement()
                        {
                            Apartment = ap,
                            Owner = db.Owners.FirstOrDefault(x => x.OwnerId == ap.OwnerId),
                            Payments = components.Select(x => x.Sum).DefaultIfEmpty(0).Sum(),
                            
                        };
                        WaterSettlementCounters.Add(dgwws);
                    }
                    WaterSettlementCounters.OrderBy(x => x.Apartment.ApartmentNumber);
                }
            }
            OnPropertyChanged("");
        }

        public void PackViewResult()
        {
            SettlementData.IsMutualSettlement = IsConstVarMutual;
            SettlementData.MutualUnitCost = MutualUnitCost;
            SettlementData.VarUnitCost = VarUnitCost;
            SettlementData.ConstUnitCost = ConstUnitCost;
            SettlementData.MutualSummaryType = MutualSummaryType;
            SettlementData.VarSummaryType = VarSummaryType;
            SettlementData.ConstSummaryType = ConstSummaryType;
        }

        #region UnitSettlement

        private double CalculateUnitCount(SettlementUnitType type)
        {
            if (SettlementData == null || SettlementData.InvoiceData == null || SettlementData.InvoiceData.MasterData == null || SettlementData.InvoiceData.MasterData.Building == null || _apartmentsList == null)
                return 0;
            
            switch (type)
            {
                default:
                    return 0;
                case SettlementUnitType.ADDITIONAL_AREA:
                    return _apartmentsList
                        .Where(x => !x.IsDeleted && x.BuildingId == SettlementData.InvoiceData.MasterData.Building.BuildingId)
                        .Select(x => x.AdditionalArea)
                        .DefaultIfEmpty(0)
                        .Sum();
                case SettlementUnitType.APARTMENT_AREA:
                    return _apartmentsList
                        .Where(x => !x.IsDeleted && x.BuildingId == SettlementData.InvoiceData.MasterData.Building.BuildingId)
                        .Select(x => x.ApartmentArea)
                        .DefaultIfEmpty(0)
                        .Sum();
                case SettlementUnitType.TOTAL_AREA:
                    return (_apartmentsList
                        .Where(x => !x.IsDeleted && x.BuildingId == SettlementData.InvoiceData.MasterData.Building.BuildingId)
                        .Select(x => x.ApartmentArea)
                        .DefaultIfEmpty(0)
                        .Sum() +
                        _apartmentsList
                        .Where(x => !x.IsDeleted && x.BuildingId == SettlementData.InvoiceData.MasterData.Building.BuildingId)
                        .Select(x => x.AdditionalArea)
                        .DefaultIfEmpty(0)
                        .Sum());
                case SettlementUnitType.PER_APARTMENT:
                    return _apartmentsList.Where(x => !x.IsDeleted && x.BuildingId == SettlementData.InvoiceData.MasterData.Building.BuildingId).Count();
                case SettlementUnitType.LOCATORS:
                    return _apartmentsList
                        .Where(x => !x.IsDeleted && x.BuildingId == SettlementData.InvoiceData.MasterData.Building.BuildingId)
                        .Select(x => x.Locators)
                        .DefaultIfEmpty(0)
                        .Sum();
            }
        }

        private string CalculateUnitSuffix(SettlementUnitType type)
        {
            if (SettlementData == null || SettlementData.InvoiceData == null || SettlementData.InvoiceData.MasterData == null || SettlementData.InvoiceData.MasterData.Building == null || _apartmentsList == null)
                return "";

            switch (type)
            {
                default:
                    return "";
                case SettlementUnitType.ADDITIONAL_AREA:
                case SettlementUnitType.APARTMENT_AREA:
                case SettlementUnitType.TOTAL_AREA:
                    return " m2";
                case SettlementUnitType.PER_APARTMENT:
                    return " mieszkań";
                case SettlementUnitType.LOCATORS:
                    return " lokatorów";
            }
        }

        #endregion

        #region WaterSettlement



        #endregion

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

    public class SettlementData
    {
        public InvoiceData InvoiceData { get; set; }
        public bool IsMutualSettlement { get; set; }
        public decimal MutualUnitCost { get; set; }
        public decimal VarUnitCost { get; set; }
        public decimal ConstUnitCost { get; set; }
        public SettlementUnitType MutualSummaryType { get; set; }
        public SettlementUnitType VarSummaryType { get; set; }
        public SettlementUnitType ConstSummaryType { get; set; }
    }
}
