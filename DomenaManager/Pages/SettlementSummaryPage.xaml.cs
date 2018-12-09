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
using DomenaManager.Helpers;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Xml.Serialization;
using LibDataModel;

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for SettlementSummary.xaml
    /// </summary>
    public partial class SettlementSummaryPage : UserControl, INotifyPropertyChanged
    {
        public Visibility IsSettlementPerApartment { get; set; }

        private string _selectedBuilding;
        public string SelectedBuilding
        {
            get { return _selectedBuilding; }
            set
            {
                if (value != _selectedBuilding)
                {
                    _selectedBuilding = value;
                    OnPropertyChanged("SelectedBuilding");
                }
            }
        }

        private double _invoiceSum;
        public double InvoiceSum
        {
            get { return _invoiceSum; }
            set
            {
                if (value != _invoiceSum)
                {
                    _invoiceSum = value;
                    OnPropertyChanged("InvoiceSum");
                }
            }
        }

        private int _apartmentsAmount;
        public int ApartmentsAmount
        {
            get { return _apartmentsAmount; }
            set
            {
                if (value != _apartmentsAmount)
                {
                    _apartmentsAmount = value;
                    OnPropertyChanged("ApartmentsAmount");
                }
            }
        }

        private string _settlementCattegory;
        public string SettlementCattegory
        {
            get { return _settlementCattegory; }
            set
            {
                if (value != _settlementCattegory)
                {
                    _settlementCattegory = value;
                    OnPropertyChanged("SettlementCattegory");
                }
            }
        }

        private double _settlePerApartment;
        public double SettlePerApartment
        {
            get { return _settlePerApartment; }
            set
            {
                if (value != _settlePerApartment)
                {
                    _settlePerApartment = value;
                    OnPropertyChanged("SettlePerApartment");
                }
            }
        }

        private ObservableCollection<ApartamentMeterDataGrid> _perApartmentCollection;
        public ObservableCollection<ApartamentMeterDataGrid> PerApartmentCollection
        {
            get { return _perApartmentCollection; }
            set
            {
                if (value != _perApartmentCollection)
                {
                    _perApartmentCollection = value;
                    OnPropertyChanged("PerApartmentCollection");
                }
            }
        }

        private ObservableCollection<ApartamentMeterDataGrid> _perAreaCollection;
        public ObservableCollection<ApartamentMeterDataGrid> PerAreaCollection
        {
            get { return _perAreaCollection; }
            set
            {
                if (value != _perAreaCollection)
                {
                    _perAreaCollection = value;
                    OnPropertyChanged("PerAreaCollection");
                }
            }
        }

        private ObservableCollection<ApartamentMeterDataGrid> _perMetersCollection;
        public ObservableCollection<ApartamentMeterDataGrid> PerMetersCollection
        {
            get { return _perMetersCollection; }
            set
            {
                if (value != _perMetersCollection)
                {
                    _perMetersCollection = value;
                    OnPropertyChanged("PerMetersCollection");
                }
            }
        }

        private ObservableCollection<ApartamentMeterDataGrid> _perGasCollection;
        public ObservableCollection<ApartamentMeterDataGrid> PerGasCollection
        {
            get { return _perGasCollection; }
            set
            {
                if (value != _perGasCollection)
                {
                    _perGasCollection = value;
                    OnPropertyChanged("PerGasCollection");
                }
            }
        }

        private double _buildingAreaSum;
        public double BuildingAreaSum
        {
            get { return _buildingAreaSum; }
            set
            {
                if (value != _buildingAreaSum)
                {
                    _buildingAreaSum = value;
                    OnPropertyChanged("BuildingAreaSum");
                }
            }
        }        

        private double _settlePerSquareMeter;
        public double SettlePerSquareMeter
        {
            get { return _settlePerSquareMeter; }
            set
            {
                if (value != _settlePerSquareMeter)
                {
                    _settlePerSquareMeter = value;
                    OnPropertyChanged("SettlePerSquareMeter");
                }
            }
        }        

        private double _mainMeterDiff;
        public double MainMeterDiff
        {
            get { return _mainMeterDiff; }
            set
            {
                if (value != _mainMeterDiff)
                {
                    _mainMeterDiff = value;
                    OnPropertyChanged("MainMeterDiff");
                }
            }
        }

        private double _apartmentDiffSum;
        public double ApartmentDiffSum
        {
            get { return _apartmentDiffSum; }
            set
            {
                if (value != _apartmentDiffSum)
                {
                    _apartmentDiffSum = value;
                    OnPropertyChanged("ApartmentDiffSum");
                }
            }
        }

        private double _settlePerMeter;
        public double SettlePerMeter
        {
            get { return _settlePerMeter; }
            set
            {
                if (value != _settlePerMeter)
                {
                    _settlePerMeter = value;
                    OnPropertyChanged("SettlePerMeter");
                }
            }
        }

        private double _totalSum;
        public double TotalSum
        {
            get { return _totalSum; }
            set
            {
                if (value != _totalSum)
                {
                    _totalSum = value;
                    OnPropertyChanged("TotalSum");
                }
            }
        }

        private double _gasMeterDiff;
        public double GasMeterDiff
        {
            get { return _gasMeterDiff; }
            set
            {
                if (value != _gasMeterDiff)
                {
                    _gasMeterDiff = value;
                    OnPropertyChanged("GasMeterDiff");
                }
            }
        }

        private double _gasUnitCost;
        public double GasUnitCost
        {
            get { return _gasUnitCost; }
            set
            {
                if (value != _gasUnitCost)
                {
                    _gasUnitCost = value;
                    OnPropertyChanged("GasUnitCost");
                }
            }
        }

        private double _GJSettlePerSquareMeter;
        public double GJSettlePerSquareMeter
        {
            get { return _GJSettlePerSquareMeter; }
            set
            {
                if (value != _GJSettlePerSquareMeter)
                {
                    _GJSettlePerSquareMeter = value;
                    OnPropertyChanged("GJSettlePerSquareMeter");
                }
            }
        }

        private double _warmWaterSettlePerSquareMeter;
        public double WarmWaterSettlePerSquareMeter
        {
            get { return _warmWaterSettlePerSquareMeter; }
            set
            {
                if (value != _warmWaterSettlePerSquareMeter)
                {
                    _warmWaterSettlePerSquareMeter = value;
                    OnPropertyChanged("WarmWaterSettlePerSquareMeter");
                }
            }
        }

        private double _apartmentGJMeterDiffSum;
        public double ApartmentGJMeterDiffSum
        {
            get { return _apartmentGJMeterDiffSum; }
            set
            {
                if (value != _apartmentGJMeterDiffSum)
                {
                    _apartmentGJMeterDiffSum = value;
                    OnPropertyChanged("ApartmentGJMeterDiffSum");
                }
            }
        }

        private double _apartmentHeatWaterMeterDiffSum;
        public double ApartmentHeatWaterMeterDiffSum
        {
            get { return _apartmentHeatWaterMeterDiffSum; }
            set
            {
                if (value != _apartmentHeatWaterMeterDiffSum)
                {
                    _apartmentHeatWaterMeterDiffSum = value;
                    OnPropertyChanged("ApartmentHeatWaterMeterDiffSum");
                }
            }
        }

        private double _GJSettlePerMeter;
        public double GJSettlePerMeter
        {
            get { return _GJSettlePerMeter; }
            set
            {
                if (value != _GJSettlePerMeter)
                {
                    _GJSettlePerMeter = value;
                    OnPropertyChanged("GJSettlePerMeter");
                }
            }
        }

        private double _warmWaterSettlePerMeter;
        public double WarmWaterSettlePerMeter
        {
            get { return _warmWaterSettlePerMeter; }
            set
            {
                if (value != _warmWaterSettlePerMeter)
                {
                    _warmWaterSettlePerMeter = value;
                    OnPropertyChanged("WarmWaterSettlePerMeter");
                }
            }
        }

        private double _GJConstantCharge;
        public double GJConstantCharge
        {
            get { return _GJConstantCharge; }
            set
            {
                if (value != _GJConstantCharge)
                {
                    _GJConstantCharge = value;
                    OnPropertyChanged("GJConstantCharge");
                }
            }
        }

        private double _GJConstantAdjustment;
        public double GJConstantAdjustment
        {
            get { return _GJConstantAdjustment; }
            set
            {
                if (value != _GJConstantAdjustment)
                {
                    _GJConstantAdjustment = value;
                    OnPropertyChanged("GJConstantAdjustment");
                }
            }
        }

        private bool _isGJDeficitShared;
        public bool IsGJDeficitShared
        {
            get { return _isGJDeficitShared; }
            set
            {
                if (value != _isGJDeficitShared)
                {
                    _isGJDeficitShared = value;
                    OnPropertyChanged("IsGJDeficitShared");
                }
            }
        }

        private double _warmWaterConstantCharge;
        public double WarmWaterConstantCharge
        {
            get { return _warmWaterConstantCharge; }
            set
            {
                if (value != _warmWaterConstantCharge)
                {
                    _warmWaterConstantCharge = value;
                    OnPropertyChanged("WarmWaterConstantCharge");
                }
            }
        }

        private double _warmWaterConstantAdjustment;
        public double WarmWaterConstantAdjustment
        {
            get { return _warmWaterConstantAdjustment; }
            set
            {
                if (value != _warmWaterConstantAdjustment)
                {
                    _warmWaterConstantAdjustment = value;
                    OnPropertyChanged("WarmWaterConstantAdjustment");
                }
            }
        }

        private double _totalGJSum;
        public double TotalGJSum
        {
            get { return _totalGJSum; }
            set
            {
                if (value != _totalGJSum)
                {
                    _totalGJSum = value;
                    OnPropertyChanged("TotalGJSum");
                }
            }
        }

        private double _totalWarmWaterCubicMeterSum;
        public double TotalWarmWaterCubicMeterSum
        {
            get { return _totalWarmWaterCubicMeterSum; }
            set
            {
                if (value != _totalWarmWaterCubicMeterSum)
                {
                    _totalWarmWaterCubicMeterSum = value;
                    OnPropertyChanged("TotalWarmWaterCubicMeterSum");
                }
            }
        }

        private double _warmWaterCost;
        public double WarmWaterCost
        {
            get { return _warmWaterCost; }
            set
            {
                if (value != _warmWaterCost)
                {
                    _warmWaterCost = value;
                    OnPropertyChanged("WarmWaterCost");
                }
            }
        }

        private double _COCost;
        public double COCost
        {
            get { return _COCost; }
            set
            {
                if (value != _COCost)
                {
                    _COCost = value;
                    OnPropertyChanged("COCost");
                }
            }
        }

        public double ConstantCharge
        {
            get { return _settlementPage.NoMeterConstantCharge; }
            set
            {
                return;
            }
        }
        
        public double ConstantAdjustment
        {
            get { return _settlementPage.NoMeterConstantAdjustment; }
            set
            {
                return;
            }
        }

        public bool IsDeficitShared
        {
            get { return _settlementPage.ChargeDeficit; }
            set
            {
                return;
            }
        }

        public bool IsWarmWaterDeficitShared
        {
            get { return _settlementPage.ChargeHeatMeterDeficit; }
            set
            {
                return;
            }
        }

        private List<Apartment> Apartments;

        private List<Payment> Payments;

        private List<Charge> Charges;

        private List<Owner> Owners;

        public Visibility IsSettlementPerMeter { get; set; }

        public Visibility IsSettlementPerArea { get; set; }

        public Visibility IsSettlementPerGas { get; set; }

        public ICommand SettlementProceed
        {
            get { return new RelayCommand(PerformSettlement, CanPerformSettlement); }
        }

        public ICommand SettlementBack
        {
            get { return new RelayCommand(CancelSettlement, CanPerformSettlement); }
        }
        private SettlementPage _settlementPage;

        public SettlementSummaryPage(SettlementPage settlementPage)
        {
            _settlementPage = settlementPage;
            IsSettlementPerApartment = Visibility.Collapsed;
            IsSettlementPerMeter = Visibility.Collapsed;
            IsSettlementPerArea = Visibility.Collapsed;
            IsSettlementPerGas = Visibility.Collapsed;
            switch (settlementPage.SettlementMethod)
            {
                default:
                    break;
                case SettlementMethodsEnum.PER_APARTMENT:
                    IsSettlementPerApartment = System.Windows.Visibility.Visible;
                    break;
                case SettlementMethodsEnum.PER_AREA:
                    IsSettlementPerArea = System.Windows.Visibility.Visible;
                    break;
                case SettlementMethodsEnum.PER_METERS:
                    IsSettlementPerMeter = System.Windows.Visibility.Visible;
                    break;
                case SettlementMethodsEnum.GAS:
                    IsSettlementPerGas = System.Windows.Visibility.Visible;
                    break;
            }
            DataContext = this;
            PrepareData();
            PopulateData();
            InitializeComponent();
        }

        private void PrepareData()
        {
            using (var db = new DB.DomenaDBContext())
            {
                Apartments =(db.Apartments.Include(x => x.MeterCollection.Select(y => y.MeterTypeParent)).Where(x => !x.IsDeleted && x.SoldDate == null).ToList());
                Charges = (db.Charges.Include(x => x.Components).Where(x => !x.IsDeleted).ToList());
                Owners = (db.Owners.Where(x => !x.IsDeleted).ToList());
            }
        }

        private void PopulateData()
        {
            SelectedBuilding = _settlementPage.SelectedBuildingName.Name;
            InvoiceSum = Math.Ceiling(_settlementPage.SettledInvoices.Select(x => x.CostAmount).DefaultIfEmpty(0).Sum() * 100) / 100;
            ApartmentsAmount = _settlementPage.ApartmentCollection.Where(x => !x.IsDeleted && x.SoldDate == null && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)).Count();
            SettlementCattegory = _settlementPage.SettlementCategoryName.CategoryName;

            if (_settlementPage.SettlementMethod == SettlementMethodsEnum.PER_APARTMENT)
            {
                var notRounded = InvoiceSum / ApartmentsAmount;
                SettlePerApartment = (Math.Ceiling(notRounded * 100)) / 100;
                PerApartmentCollection = new ObservableCollection<ApartamentMeterDataGrid>();
                foreach (var a in Apartments.Where(x => !x.IsDeleted && x.SoldDate == null && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)))
                {
                    double chargeAmount = 0;
                    var c = Charges.Where(x => x.ApartmentId.Equals(a.ApartmentId) && x.ChargeDate >= _settlementPage.SettlementFrom && x.ChargeDate <= _settlementPage.SettlementTo);
                    foreach (var cc in c)
                    {
                        chargeAmount += cc.Components.Where(y => _settlementPage.SettleChargeCategories.Any(x => x.BuildingChargeBasisCategoryId.Equals(y.CostCategoryId))).Select(x => x.Sum).DefaultIfEmpty(0).Sum();
                    }

                    PerApartmentCollection.Add(new ApartamentMeterDataGrid()
                    {
                        ApartmentO = a,
                        Charge = chargeAmount,
                        OwnerO = Owners.FirstOrDefault(x => x.OwnerId.Equals(a.OwnerId)),
                        CostSettled = SettlePerApartment,
                        Saldo = chargeAmount - SettlePerApartment,
                    });
                }

                ICollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(PerApartmentCollection);
                cv.GroupDescriptions.Clear();
                cv.GroupDescriptions.Add(new PropertyGroupDescription(""));
            }

            if (_settlementPage.SettlementMethod == SettlementMethodsEnum.PER_AREA)
            {
                BuildingAreaSum = Math.Ceiling((Apartments.Where(x => x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)).Select(x => new { totalArea = x.AdditionalArea + x.ApartmentArea }).Sum(x => x.totalArea) * 100)) / 100;
                SettlePerSquareMeter = Math.Ceiling((InvoiceSum / BuildingAreaSum) * 100)/ 100;
                PerAreaCollection = new ObservableCollection<ApartamentMeterDataGrid>();

                foreach (var a in Apartments.Where(x => !x.IsDeleted && x.SoldDate == null && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)))
                {
                    double chargeAmount = 0;
                    var c = Charges.Where(x => x.ApartmentId.Equals(a.ApartmentId) && x.ChargeDate >= _settlementPage.SettlementFrom && x.ChargeDate <= _settlementPage.SettlementTo);
                    foreach (var cc in c)
                    {
                        chargeAmount += cc.Components.Where(y => _settlementPage.SettleChargeCategories.Any(x => x.BuildingChargeBasisCategoryId.Equals(y.CostCategoryId))).Select(x => x.Sum).DefaultIfEmpty(0).Sum();
                    }

                    PerAreaCollection.Add(new ApartamentMeterDataGrid()
                    {
                        SettleArea = (a.AdditionalArea + a.ApartmentArea),
                        ApartmentO = a,
                        Charge = chargeAmount,
                        OwnerO = Owners.FirstOrDefault(x => x.OwnerId.Equals(a.OwnerId)),
                        CostSettled = SettlePerSquareMeter * (a.AdditionalArea + a.ApartmentArea),
                        Saldo = chargeAmount - SettlePerSquareMeter * (a.AdditionalArea + a.ApartmentArea),
                    });
                }

                ICollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(PerAreaCollection);
                cv.GroupDescriptions.Clear();
                cv.GroupDescriptions.Add(new PropertyGroupDescription(""));
            }

            if (_settlementPage.SettlementMethod == SettlementMethodsEnum.PER_METERS)
            {
                var ap = Apartments.Where(x => !x.IsDeleted && x.SoldDate == null && x.SoldDate == null && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId));
                int notValidMetersCount = ap.Where(x => x.MeterCollection.FirstOrDefault(y => !y.IsDeleted && y.MeterTypeParent.MeterId.Equals(_settlementPage.SelectedMeterName.MeterId)).LegalizationDate <= DateTime.Now).Count();

                MainMeterDiff = _settlementPage.MainMeterDiff;
                ApartmentDiffSum = _settlementPage.ApartmentMetersCollection.Where(x => x.IsMeterLegalized).Select(x => x.MeterDifference).DefaultIfEmpty(0).Sum() + _settlementPage.NoMeterConstantAdjustment * notValidMetersCount;
                ApartmentsAmount = _settlementPage.ApartmentCollection.Where(x => !x.IsDeleted && x.SoldDate == null && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)).Count();
                BuildingAreaSum = Math.Ceiling((Apartments.Where(x => x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)).Select(x => new { totalArea = x.AdditionalArea + x.ApartmentArea }).Sum(x => x.totalArea) * 100)) / 100;
                
                
                var measureToDivide = MainMeterDiff;
                double deficitPerApartment = 0;
                if (!_settlementPage.ChargeDeficit)
                {
                    if (MainMeterDiff > 0)
                    {
                        measureToDivide = Math.Min(MainMeterDiff, ApartmentDiffSum);
                    }
                    else
                    {
                        measureToDivide = ApartmentDiffSum;
                    }
                }
                else
                {
                    if (MainMeterDiff > ApartmentDiffSum)
                    {
                        deficitPerApartment = (MainMeterDiff - ApartmentDiffSum) / notValidMetersCount;
                    }
                }

                var adjustedInvoiceSum = InvoiceSum - (notValidMetersCount * _settlementPage.NoMeterConstantCharge);

                if (_settlementPage.ConstantSettlementMethod == SettlementMethodsEnum.PER_AREA)
                {
                    SettlePerSquareMeter = Math.Ceiling((((_settlementPage.ConstantPeriod / 100) * adjustedInvoiceSum) / BuildingAreaSum) * 100) / 100;
                }
                else
                {
                    SettlePerSquareMeter = Math.Ceiling((((_settlementPage.ConstantPeriod / 100) * adjustedInvoiceSum) / ApartmentsAmount) * 100) / 100;
                }
                SettlePerMeter = Math.Ceiling((((_settlementPage.VariablePeriod / 100) * adjustedInvoiceSum) / measureToDivide) * 100) / 100;
                PerMetersCollection = new ObservableCollection<ApartamentMeterDataGrid>();

                foreach (var a in ap)
                {
                    var selectedAmdg = _settlementPage.ApartmentMetersCollection.FirstOrDefault(x => x.ApartmentO.ApartmentId.Equals(a.ApartmentId));
                    
                    double chargeAmount = 0;
                    var c = Charges.Where(x => x.ApartmentId.Equals(a.ApartmentId) && x.ChargeDate >= _settlementPage.SettlementFrom && x.ChargeDate <= _settlementPage.SettlementTo);
                    foreach (var cc in c)
                    {
                        chargeAmount += cc.Components.Where(y => _settlementPage.SettleChargeCategories.Any(x => x.BuildingChargeBasisCategoryId.Equals(y.CostCategoryId))).Select(x => x.Sum).DefaultIfEmpty(0).Sum();
                    }
                    //var meterDiff = selectedAmdg.MeterDifference;
                    //var costSett = Math.Ceiling((meterDiff * SettlePerMeter) * 100) / 100;

                    /*if (!selectedAmdg.IsMeterLegalized)
                    {
                        selectedAmdg.LastMeasure = 0;
                        selectedAmdg.CurrentMeasure = (_settlementPage.NoMeterConstantAdjustment + deficitPerApartment);
                        costSett += _settlementPage.NoMeterConstantCharge;
                    }*/

                    var amd = new ApartamentMeterDataGrid()
                    {
                        LastMeasure = selectedAmdg.LastMeasure,
                        CurrentMeasure = selectedAmdg.CurrentMeasure,
                        IsMeterLegalized = selectedAmdg.IsMeterLegalized,
                        ApartmentO = a,
                        Charge = chargeAmount,
                        OwnerO = Owners.FirstOrDefault(x => x.OwnerId.Equals(a.OwnerId)),                        
                        SettleArea = a.ApartmentArea + a.AdditionalArea,         
                    };

                    var costSett = Math.Ceiling((amd.MeterDifference * SettlePerMeter) * 100) / 100;
                    if (!amd.IsMeterLegalized)
                    {
                        amd.LastMeasure = 0;
                        amd.CurrentMeasure = (_settlementPage.NoMeterConstantAdjustment + deficitPerApartment);
                        costSett = Math.Ceiling((amd.MeterDifference * SettlePerMeter) * 100) / 100;
                        costSett += _settlementPage.NoMeterConstantCharge;                        
                    }
                    amd.VariableCost = costSett;
                    amd.Saldo = chargeAmount - costSett;

                    if (_settlementPage.ConstantSettlementMethod == SettlementMethodsEnum.PER_AREA)
                    {
                        amd.ConstantCost = SettlePerSquareMeter * (amd.ApartmentO.AdditionalArea + amd.ApartmentO.ApartmentArea);
                    }
                    else
                    {
                        amd.ConstantCost = SettlePerSquareMeter;
                    }
                    amd.CostSettled = amd.VariableCost + amd.ConstantCost;
                    PerMetersCollection.Add(amd);
                }
                TotalSum = PerMetersCollection.Select(x => x.CostSettled).DefaultIfEmpty(0).Sum();
                ICollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(PerMetersCollection);
                cv.GroupDescriptions.Clear();
                cv.GroupDescriptions.Add(new PropertyGroupDescription(""));
            }

            if (_settlementPage.SettlementMethod == SettlementMethodsEnum.GAS)
            {
                var ap = Apartments.Where(x => !x.IsDeleted && x.SoldDate == null && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId));
                ApartmentsAmount = _settlementPage.ApartmentCollection.Where(x => !x.IsDeleted && x.SoldDate == null && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)).Count();
                BuildingAreaSum = Math.Ceiling((Apartments.Where(x => x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)).Select(x => new { totalArea = x.AdditionalArea + x.ApartmentArea }).Sum(x => x.totalArea) * 100)) / 100;
                
                var GJsum = _settlementPage.ApartmentGasMetersCollection.Where(x => x.IsMeterLegalized && x.Meter.MeterId.Equals(_settlementPage.HeatMeterName.MeterId)).Select(x => x.MeterDifference).DefaultIfEmpty(0).Sum();
                var warmWaterCubicMeterSum = _settlementPage.ApartmentGasMetersCollection.Where(x => x.IsMeterLegalized && x.Meter.MeterId.Equals(_settlementPage.WarmWaterMeterName.MeterId)).Select(x => x.MeterDifference).DefaultIfEmpty(0).Sum();

                ApartmentGJMeterDiffSum = GJsum;
                ApartmentHeatWaterMeterDiffSum = warmWaterCubicMeterSum;

                var notValidHeatMetersCount = _settlementPage.ApartmentGasMetersCollection.Where(x => !x.IsMeterLegalized && x.Meter.MeterId.Equals(_settlementPage.HeatMeterName.MeterId)).Count();
                var notValidWarmWaterMetersCount = _settlementPage.ApartmentGasMetersCollection.Where(x => !x.IsMeterLegalized && x.Meter.MeterId.Equals(_settlementPage.WarmWaterMeterName.MeterId)).Count();

                GJConstantCharge = _settlementPage.NoHeatMeterConstantCharge;
                GJConstantAdjustment = _settlementPage.NoHeatMeterConstantAdjustment;
                IsGJDeficitShared = _settlementPage.ChargeHeatDeficit;

                WarmWaterConstantCharge = _settlementPage.HeatWaterConstantCharge;
                WarmWaterConstantAdjustment = _settlementPage.HeatWaterConstantAdjustment;
                IsWarmWaterDeficitShared = _settlementPage.ChargeHeatMeterDeficit;

                var adjustedInvoiceSum = InvoiceSum - (notValidHeatMetersCount * _settlementPage.NoHeatMeterConstantCharge + notValidWarmWaterMetersCount * _settlementPage.HeatWaterConstantCharge);
                var adjustedGJsum = GJsum + (notValidHeatMetersCount * _settlementPage.NoHeatMeterConstantAdjustment);
                var adjustedWarmMeterCubicMeterSum = warmWaterCubicMeterSum + (notValidWarmWaterMetersCount * _settlementPage.HeatWaterConstantAdjustment);
                if (warmWaterCubicMeterSum == 0)
                {
                    warmWaterCubicMeterSum = adjustedWarmMeterCubicMeterSum;
                }
                
                if (_settlementPage.GasUnitCostAuto && _settlementPage.GasMeterDiff != 0)
                {
                    GasUnitCost = adjustedInvoiceSum / _settlementPage.GasMeterDiff;
                }
                else
                {
                    double gasUnitC;
                    double.TryParse(_settlementPage.GasUnitCost, out gasUnitC);
                    GasUnitCost = GasUnitCost;
                }

                var valueToDivideGJ = _settlementPage.HeatMeterDiff == 0 ? adjustedGJsum : Math.Min(_settlementPage.HeatMeterDiff, adjustedGJsum);

                var waterHeatCost = GasUnitCost * _settlementPage.GasNeededToHeatWater;
                var waterHeatTotalCost = waterHeatCost * _settlementPage.HeatWaterMeterDiff;
                WarmWaterCost = waterHeatTotalCost;
                var heatTotalCost = InvoiceSum - waterHeatTotalCost;
                COCost = heatTotalCost;

                // Do adjusted dodać deficity jezeli rozlicane i min z głownego i adjusted)
                var valueToDivideWarmWater = _settlementPage.HeatWaterMeterDiff;
                if (!IsWarmWaterDeficitShared)
                {
                    valueToDivideWarmWater = Math.Min(_settlementPage.HeatWaterMeterDiff, adjustedWarmMeterCubicMeterSum);
                }

                var heatTotalCostAdjusted = heatTotalCost - (notValidHeatMetersCount * _settlementPage.NoHeatMeterConstantCharge);
                double GJunitCost = Math.Ceiling(100 * ((_settlementPage.VariablePeriod / 100) * heatTotalCostAdjusted) / valueToDivideGJ) / 100;
                var waterHeatTotalCostAdjusted = waterHeatTotalCost - (notValidWarmWaterMetersCount * _settlementPage.HeatWaterConstantCharge);
                double warmWaterCubicMeterUnitCost = Math.Ceiling(100 * ((_settlementPage.VariablePeriod / 100) * waterHeatTotalCostAdjusted) / valueToDivideWarmWater) / 100;

                GJSettlePerMeter = GJunitCost;
                WarmWaterSettlePerMeter = warmWaterCubicMeterUnitCost;

                double GJconstantCost;
                double warmWaterConstantCost;
                if (_settlementPage.ConstantSettlementMethod == SettlementMethodsEnum.PER_APARTMENT)
                {
                    GJconstantCost = Math.Ceiling(100 * ((_settlementPage.ConstantPeriod / 100) * heatTotalCost) / ApartmentsAmount) / 100;
                    warmWaterConstantCost = Math.Ceiling(100 * ((_settlementPage.ConstantPeriod / 100) * waterHeatTotalCost) / ApartmentsAmount) / 100;
                }
                else
                {
                    GJconstantCost = Math.Ceiling(100 * ((_settlementPage.ConstantPeriod / 100) * heatTotalCost) / BuildingAreaSum) / 100;
                    warmWaterConstantCost = Math.Ceiling(100 * ((_settlementPage.ConstantPeriod / 100) * waterHeatTotalCost) / BuildingAreaSum) / 100;
                }
                GJSettlePerSquareMeter = GJconstantCost;
                WarmWaterSettlePerSquareMeter = warmWaterConstantCost;
                                
                double warmWaterDeficitPerApartment = 0;
                if (_settlementPage.ChargeHeatMeterDeficit)
                {
                    if (_settlementPage.HeatWaterMeterDiff > ApartmentHeatWaterMeterDiffSum)
                    {
                        warmWaterDeficitPerApartment = (_settlementPage.HeatWaterMeterDiff - adjustedWarmMeterCubicMeterSum) / notValidWarmWaterMetersCount;
                    }
                }

                double GJDeficitPerApartment = 0;
                if (_settlementPage.ChargeHeatDeficit)
                {
                    if (_settlementPage.HeatMeterDiff > ApartmentGJMeterDiffSum)
                    {
                        GJDeficitPerApartment = (_settlementPage.HeatMeterDiff - adjustedGJsum) / notValidHeatMetersCount;
                    }
                }

                PerGasCollection = new ObservableCollection<ApartamentMeterDataGrid>();

                foreach (var a in ap)
                {
                    var selectedAmdgs = _settlementPage.ApartmentGasMetersCollection.Where(x => x.ApartmentO.ApartmentId.Equals(a.ApartmentId));

                    //Warm water

                    var amdg = selectedAmdgs.FirstOrDefault(x => x.Meter.MeterId.Equals(_settlementPage.WarmWaterMeterName.MeterId));
                    
                    double chargeAmount = 0;
                    var c = Charges.Where(x => x.ApartmentId.Equals(a.ApartmentId) && x.ChargeDate >= _settlementPage.SettlementFrom && x.ChargeDate <= _settlementPage.SettlementTo);
                    foreach (var cc in c)
                    {
                        chargeAmount += cc.Components.Where(y => _settlementPage.WarmWaterChargeCategoryName.Equals(y.CostCategoryId)).Select(x => x.Sum).DefaultIfEmpty(0).Sum();
                    }

                    var amd = new ApartamentMeterDataGrid()
                    {
                        Meter = amdg.Meter,
                        LastMeasure = amdg.LastMeasure,
                        CurrentMeasure = amdg.CurrentMeasure,
                        IsMeterLegalized = amdg.IsMeterLegalized,
                        ApartmentO = a,
                        Charge = chargeAmount,
                        OwnerO = Owners.FirstOrDefault(x => x.OwnerId.Equals(a.OwnerId)),
                        SettleArea = a.ApartmentArea + a.AdditionalArea,
                    };

                    var costSett = Math.Ceiling((amd.MeterDifference * WarmWaterSettlePerMeter) * 100) / 100;
                    if (!amd.IsMeterLegalized)
                    {
                        amd.LastMeasure = 0;
                        amd.CurrentMeasure = (WarmWaterConstantAdjustment + warmWaterDeficitPerApartment);
                        costSett = Math.Ceiling((amd.MeterDifference * WarmWaterSettlePerMeter) * 100) / 100;
                        costSett += WarmWaterConstantCharge;
                    }
                    amd.VariableCost = costSett;
                    amd.Saldo = chargeAmount - costSett;

                    if (_settlementPage.ConstantSettlementMethod == SettlementMethodsEnum.PER_AREA)
                    {
                        amd.ConstantCost = WarmWaterSettlePerSquareMeter * (amd.ApartmentO.AdditionalArea + amd.ApartmentO.ApartmentArea);
                    }
                    else
                    {
                        amd.ConstantCost = WarmWaterSettlePerSquareMeter;
                    }
                    amd.CostSettled = amd.VariableCost + amd.ConstantCost;
                    TotalWarmWaterCubicMeterSum += amd.CostSettled;
                    PerGasCollection.Add(amd);

                    // GJ
                    amdg = selectedAmdgs.FirstOrDefault(x => x.Meter.MeterId.Equals(_settlementPage.HeatMeterName.MeterId));

                    chargeAmount = 0;
                    c = Charges.Where(x => x.ApartmentId.Equals(a.ApartmentId) && x.ChargeDate >= _settlementPage.SettlementFrom && x.ChargeDate <= _settlementPage.SettlementTo);
                    foreach (var cc in c)
                    {
                        chargeAmount += cc.Components.Where(y => _settlementPage.HeatChargeCategoryName.Equals(y.CostCategoryId)).Select(x => x.Sum).DefaultIfEmpty(0).Sum();
                    }

                    amd = new ApartamentMeterDataGrid()
                    {
                        Meter = amdg.Meter,
                        LastMeasure = amdg.LastMeasure,
                        CurrentMeasure = amdg.CurrentMeasure,
                        IsMeterLegalized = amdg.IsMeterLegalized,
                        ApartmentO = a,
                        Charge = chargeAmount,
                        OwnerO = Owners.FirstOrDefault(x => x.OwnerId.Equals(a.OwnerId)),
                        SettleArea = a.ApartmentArea + a.AdditionalArea,
                    };

                    costSett = Math.Ceiling((amd.MeterDifference * GJSettlePerMeter) * 100) / 100;
                    if (!amd.IsMeterLegalized)
                    {
                        amd.LastMeasure = 0;
                        amd.CurrentMeasure = (GJConstantAdjustment + GJDeficitPerApartment);
                        costSett = Math.Ceiling((amd.MeterDifference * GJSettlePerMeter) * 100) / 100;
                        costSett += GJConstantCharge;
                    }
                    amd.VariableCost = costSett;
                    amd.Saldo = chargeAmount - costSett;

                    if (_settlementPage.ConstantSettlementMethod == SettlementMethodsEnum.PER_AREA)
                    {
                        amd.ConstantCost = GJSettlePerSquareMeter * (amd.ApartmentO.AdditionalArea + amd.ApartmentO.ApartmentArea);
                    }
                    else
                    {
                        amd.ConstantCost = GJSettlePerSquareMeter;
                    }
                    amd.CostSettled = amd.VariableCost + amd.ConstantCost;
                    TotalGJSum += amd.CostSettled;
                    PerGasCollection.Add(amd);
                    //  dodać footer z podsumowaniem  

                }
                TotalSum = TotalGJSum + TotalWarmWaterCubicMeterSum;

                ICollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(PerGasCollection);
                cv.GroupDescriptions.Clear();
                cv.GroupDescriptions.Add(new PropertyGroupDescription("ApartmentO.ApartmentId"));
            }
        }

        private void PerformSettlement(object param)
        {
            var s = new Settlement() { SettlementId = Guid.NewGuid(), CreatedDate = DateTime.Now, };

            using (var db = new DB.DomenaDBContext())
            {
                db.Settlements.Add(s);

                switch (_settlementPage.SettlementMethod)
                {
                    default:
                        break;

                    case SettlementMethodsEnum.PER_APARTMENT:
                        foreach (var a in PerApartmentCollection)
                        {
                            var c = new Charge()
                            {
                                ApartmentId = a.ApartmentO.ApartmentId,
                                ChargeDate = DateTime.Today,
                                ChargeId = Guid.NewGuid(),
                                Components = new List<ChargeComponent>() { new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = _settlementPage.SettlementCategoryName.BuildingChargeBasisCategoryId, CostDistribution = (int)EnumCostDistribution.CostDistribution.PerApartment, CostPerUnit = -a.Saldo, Sum = -a.Saldo } },
                                CreatedDate = DateTime.Today,
                                IsClosed = false,
                                IsDeleted = false,
                                SettlementId = s.SettlementId,
                                OwnerId = a.ApartmentO.OwnerId,
                            };
                            db.Charges.Add(c);
                        }
                        break;

                    case SettlementMethodsEnum.PER_AREA:
                        foreach (var a in PerAreaCollection)
                        {
                            var c = new Charge()
                            {
                                ApartmentId = a.ApartmentO.ApartmentId,
                                ChargeDate = DateTime.Today,
                                ChargeId = Guid.NewGuid(),
                                Components = new List<ChargeComponent>() { new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = _settlementPage.SettlementCategoryName.BuildingChargeBasisCategoryId, CostDistribution = (int)EnumCostDistribution.CostDistribution.PerApartment, CostPerUnit = -a.Saldo, Sum = -a.Saldo } },
                                CreatedDate = DateTime.Today,
                                IsClosed = false,
                                IsDeleted = false,
                                SettlementId = s.SettlementId,
                                OwnerId = a.ApartmentO.OwnerId,
                            };
                            db.Charges.Add(c);
                        }   
                        break;

                    case SettlementMethodsEnum.PER_METERS:
                        foreach (var a in PerMetersCollection)
                        {
                            var c = new Charge()
                            {
                                ApartmentId = a.ApartmentO.ApartmentId,
                                ChargeDate = DateTime.Today,
                                ChargeId = Guid.NewGuid(),
                                Components = new List<ChargeComponent>() { new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = _settlementPage.SettlementCategoryName.BuildingChargeBasisCategoryId, CostDistribution = (int)EnumCostDistribution.CostDistribution.PerApartment, CostPerUnit = -a.Saldo, Sum = -a.Saldo } },
                                CreatedDate = DateTime.Today,
                                IsClosed = false,
                                IsDeleted = false,
                                SettlementId = s.SettlementId,
                                OwnerId = a.ApartmentO.OwnerId,
                            };
                            db.Charges.Add(c);
                        }
                        break;
                                                
                    case SettlementMethodsEnum.GAS:
                        foreach (var a in PerGasCollection)
                        {
                            var c = new Charge()
                            {
                                ApartmentId = a.ApartmentO.ApartmentId,
                                ChargeDate = DateTime.Today,
                                ChargeId = Guid.NewGuid(),
                                Components = new List<ChargeComponent>() { new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = _settlementPage.SettlementCategoryName.BuildingChargeBasisCategoryId, CostDistribution = (int)EnumCostDistribution.CostDistribution.PerApartment, CostPerUnit = -a.Saldo, Sum = -a.Saldo } },
                                CreatedDate = DateTime.Today,
                                IsClosed = false,
                                IsDeleted = false,
                                SettlementId = s.SettlementId,
                                OwnerId = a.ApartmentO.OwnerId,
                            };
                            db.Charges.Add(c);
                        }
                        break;
                }

                db.SaveChanges();
            }
        }

        private void CancelSettlement(object param)
        {
            //var mw = (((((this.Parent as MahApps.Metro.Controls.TransitioningContentControl).Parent as Grid).Parent as DialogHost).Parent as DialogHost).Parent as DialogHost).Parent as Windows.MainWindow;

            var CurrentPage = _settlementPage;
            SwitchPage.SwitchMainPage(CurrentPage, this);
        }

        private bool CanPerformSettlement()
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
