﻿using System;
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

        private List<Apartment> Apartments;

        private List<Payment> Payments;

        private List<Charge> Charges;

        private List<Owner> Owners;

        public Visibility IsSettlementPerMeter { get; set; }

        public Visibility IsSettlementPerArea { get; set; }

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
                Apartments =(db.Apartments.Include(x => x.MeterCollection.Select(y => y.MeterTypeParent)).Where(x => !x.IsDeleted).ToList());
                Charges = (db.Charges.Include(x => x.Components).Where(x => !x.IsDeleted).ToList());
                Owners = (db.Owners.Where(x => !x.IsDeleted).ToList());
            }
        }

        private void PopulateData()
        {
            SelectedBuilding = _settlementPage.SelectedBuildingName.Name;
            InvoiceSum = Math.Ceiling(_settlementPage.SettledInvoices.Select(x => x.CostAmount).DefaultIfEmpty(0).Sum() * 100) / 100;
            ApartmentsAmount = _settlementPage.ApartmentCollection.Where(x => !x.IsDeleted && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)).Count();
            SettlementCattegory = _settlementPage.SettlementCategoryName.CategoryName;

            if (_settlementPage.SettlementMethod == SettlementMethodsEnum.PER_APARTMENT)
            {
                var notRounded = InvoiceSum / ApartmentsAmount;
                SettlePerApartment = (Math.Ceiling(notRounded * 100)) / 100;
                PerApartmentCollection = new ObservableCollection<ApartamentMeterDataGrid>();
                foreach (var a in Apartments.Where(x => !x.IsDeleted && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)))
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

                foreach (var a in Apartments.Where(x => !x.IsDeleted && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)))
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
                var ap = Apartments.Where(x => !x.IsDeleted && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId));
                int notValidMetersCount = ap.Where(x => x.MeterCollection.FirstOrDefault(y => !y.IsDeleted && y.MeterTypeParent.MeterId.Equals(_settlementPage.SelectedMeterName.MeterId)).LegalizationDate <= DateTime.Now).Count();

                MainMeterDiff = _settlementPage.MainMeterDiff;
                ApartmentDiffSum = _settlementPage.ApartmentMetersCollection.Where(x => x.IsMeterLegalized).Select(x => x.MeterDifference).DefaultIfEmpty(0).Sum() + _settlementPage.NoMeterConstantAdjustment * notValidMetersCount;
                ApartmentsAmount = _settlementPage.ApartmentCollection.Where(x => !x.IsDeleted && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)).Count();
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
                var GJsum = _settlementPage.ApartmentGasMetersCollection.Where(x => x.IsMeterLegalized && x.Meter.MeterId.Equals(_settlementPage.HeatMeterName.MeterId)).Select(x => x.MeterDifference).DefaultIfEmpty(0).Sum();
                var warmWaterCubicMeterSum = _settlementPage.ApartmentGasMetersCollection.Where(x => x.IsMeterLegalized && x.Meter.MeterId.Equals(_settlementPage.WarmWaterMeterName.MeterId)).Select(x => x.MeterDifference).DefaultIfEmpty(0).Sum();

                var notValidHeatMetersCount = _settlementPage.ApartmentGasMetersCollection.Where(x => !x.IsMeterLegalized && x.Meter.MeterId.Equals(_settlementPage.HeatMeterName.MeterId)).Count();
                var notValidWarmWaterMetersCount = _settlementPage.ApartmentGasMetersCollection.Where(x => !x.IsMeterLegalized && x.Meter.MeterId.Equals(_settlementPage.WarmWaterMeterName.MeterId)).Count();

                var adjustedInvoiceSum = InvoiceSum - (notValidHeatMetersCount * _settlementPage.NoHeatMeterConstantCharge + notValidWarmWaterMetersCount * _settlementPage.HeatWaterConstantCharge);
                
                double gasCubicMeterCost;
                if (_settlementPage.GasUnitCostAuto && _settlementPage.GasMeterDiff != 0)
                {
                    gasCubicMeterCost = adjustedInvoiceSum / _settlementPage.GasMeterDiff;
                }
                else
                {
                    gasCubicMeterCost = _settlementPage.GasUnitCost;
                }

                var waterHeatCost = gasCubicMeterCost * _settlementPage.GasNeededToHeatWater;
                var waterHeatTotalCost = waterHeatCost * warmWaterCubicMeterSum;
                var heatTotalCost = adjustedInvoiceSum - waterHeatTotalCost;

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
                                Components = new List<ChargeComponent>() { new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = _settlementPage.SelectedChargeCategoryName.BuildingChargeBasisCategoryId, CostDistribution = (int)EnumCostDistribution.CostDistribution.PerApartment, CostPerUnit = -a.Saldo, Sum = -a.Saldo } },
                                CreatedDate = DateTime.Today,
                                IsClosed = false,
                                IsDeleted = false,
                                SettlementId = s.SettlementId
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
                                Components = new List<ChargeComponent>() { new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = _settlementPage.SelectedChargeCategoryName.BuildingChargeBasisCategoryId, CostDistribution = (int)EnumCostDistribution.CostDistribution.PerApartment, CostPerUnit = -a.Saldo, Sum = -a.Saldo } },
                                CreatedDate = DateTime.Today,
                                IsClosed = false,
                                IsDeleted = false,
                                SettlementId = s.SettlementId
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
                                Components = new List<ChargeComponent>() { new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = _settlementPage.SelectedChargeCategoryName.BuildingChargeBasisCategoryId, CostDistribution = (int)EnumCostDistribution.CostDistribution.PerApartment, CostPerUnit = -a.Saldo, Sum = -a.Saldo } },
                                CreatedDate = DateTime.Today,
                                IsClosed = false,
                                IsDeleted = false,
                                SettlementId = s.SettlementId
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
            var mw = (((((this.Parent as MahApps.Metro.Controls.TransitioningContentControl).Parent as Grid).Parent as DialogHost).Parent as DialogHost).Parent as DialogHost).Parent as Windows.MainWindow;

            mw.CurrentPage = _settlementPage;
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
