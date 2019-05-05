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

namespace DomenaManager.Wizards.SettlementWizard
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class SettlementPart : UserControl, INotifyPropertyChanged
    {
        public SettlementData SettlementData { get; set; }

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
                return SettlementData.InvoiceData.MasterData.SettlementType == SettlementTypeEnum.UNITS ? Visibility.Visible : Visibility.Hidden;
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
                return (decimal.Floor(100 * TotalSum / Convert.ToDecimal(CalculateUnitCount(MutualSummaryType)))) / 100;
            }
        }

        public string ConstUnitsCount
        {
            get
            {
                return CalculateUnitCount(ConstSummaryType).ToString() + CalculateUnitSuffix(MutualSummaryType);
            }
        }

        public decimal ConstUnitCost
        {
            get
            {
                if (CalculateUnitCount(ConstSummaryType) == 0)
                    return 0;
                return (decimal.Floor(100 * ConstSum / Convert.ToDecimal(CalculateUnitCount(ConstSummaryType)))) / 100;
            }
        }

        public string VarUnitsCount
        {
            get
            {
                return CalculateUnitCount(VarSummaryType).ToString() + CalculateUnitSuffix(MutualSummaryType);
            }
        }

        public decimal VarUnitCost
        {
            get
            {
                if (CalculateUnitCount(VarSummaryType) == 0)
                    return 0;
                return (decimal.Floor(100 * VarSum / Convert.ToDecimal(CalculateUnitCount(VarSummaryType)))) / 100;
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
                _apartmentsList = db.Apartments.ToList();
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
                        .Sum()
                        );
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
