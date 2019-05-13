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
using DomenaManager.Helpers.DGSummary;
using MaterialDesignThemes.Wpf;
using Serilog;

namespace DomenaManager.Wizards.SettlementWizard
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class SummaryPart : UserControl, INotifyPropertyChanged
    {
        public SummaryData SummaryData { get; set; }

        private DataGrid _unitSummaryDG;
        public DataGrid UnitSummaryDG
        {
            get
            {
                return _unitSummaryDG;
            }
            set
            {
                if (value != _unitSummaryDG)
                {
                    _unitSummaryDG = value;
                    OnPropertyChanged("UnitSummaryDG");
                }
            }
        }

        private ObservableCollection<DGUnitSummary> _DGUnitSummaries;
        public ObservableCollection<DGUnitSummary> DGUnitSummaries
        {
            get
            {
                return _DGUnitSummaries;
            }
            set
            {
                if (value != _DGUnitSummaries)
                {
                    _DGUnitSummaries = value;
                    OnPropertyChanged("DGUnitSummaries");
                }
            }
        }
        
        public Visibility IsMutualVisible
        {
            get
            {
                return (SummaryData?.SettlementData?.IsMutualSettlement).HasValue && (SummaryData?.SettlementData?.IsMutualSettlement).Value ?
                    Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility IsMutualNotVisible
        {
            get
            {
                return (SummaryData?.SettlementData?.IsMutualSettlement).HasValue && (SummaryData?.SettlementData?.IsMutualSettlement).Value ?
                    Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string MutualSummaryType
        {
            get
            {
                return (SummaryData?.SettlementData?.MutualSummaryType).HasValue && (SummaryData?.SettlementData?.MutualSummaryType).Value != null ?
                    SummaryData.SettlementData.MutualSummaryType.Description() : null;
            }
        }

        public string VarSummaryType
        {
            get
            {
                return (SummaryData?.SettlementData?.VarSummaryType).HasValue && (SummaryData?.SettlementData?.VarSummaryType).Value != null ?
                    SummaryData.SettlementData.VarSummaryType.Description() : null;
            }
        }

        public string ConstSummaryType
        {
            get
            {
                return (SummaryData?.SettlementData?.ConstSummaryType).HasValue && (SummaryData?.SettlementData?.ConstSummaryType).Value != null ?
                    SummaryData.SettlementData.ConstSummaryType.Description() : null;
            }
        }

        public string MutualUnitsCount
        {
            get
            {
                if ((SummaryData?.SettlementData?.MutualSummaryType).HasValue && (SummaryData?.SettlementData?.MutualSummaryType).Value != null && SummaryData?.SettlementData?.InvoiceData?.MasterData?.Building != null)
                {
                    using (var db = new DB.DomenaDBContext())
                    {
                        var apartments = db.Apartments.Where(x => !x.IsDeleted && x.BuildingId == SummaryData.SettlementData.InvoiceData.MasterData.Building.BuildingId);
                        switch (SummaryData.SettlementData.MutualSummaryType)
                        {
                            case SettlementUnitType.ADDITIONAL_AREA:
                                return apartments.Select(x => x.AdditionalArea).DefaultIfEmpty(0).Sum().ToString() + " m2";
                            case SettlementUnitType.APARTMENT_AREA:
                                return apartments.Select(x => x.ApartmentArea).DefaultIfEmpty(0).Sum().ToString() + " m2";
                            case SettlementUnitType.LOCATORS:
                                return apartments.Select(x => x.Locators).DefaultIfEmpty(0).Sum().ToString() + " lokatorów";
                            case SettlementUnitType.PER_APARTMENT:
                                return apartments.Count().ToString();
                            case SettlementUnitType.TOTAL_AREA:
                                return (apartments.Select(x => x.AdditionalArea).DefaultIfEmpty(0).Sum() +
                                    apartments.Select(x => x.ApartmentArea).DefaultIfEmpty(0).Sum()).ToString() + " m2";
                        }
                    }
                }
                return null;
            }
        }

        public string VarUnitsCount
        {
            get
            {
                if ((SummaryData?.SettlementData?.VarSummaryType).HasValue && (SummaryData?.SettlementData?.VarSummaryType).Value != null && SummaryData?.SettlementData?.InvoiceData?.MasterData?.Building != null)
                {
                    using (var db = new DB.DomenaDBContext())
                    {
                        var apartments = db.Apartments.Where(x => !x.IsDeleted && x.BuildingId == SummaryData.SettlementData.InvoiceData.MasterData.Building.BuildingId);
                        switch (SummaryData.SettlementData.VarSummaryType)
                        {
                            case SettlementUnitType.ADDITIONAL_AREA:
                                return apartments.Select(x => x.AdditionalArea).DefaultIfEmpty(0).Sum().ToString() + " m2";
                            case SettlementUnitType.APARTMENT_AREA:
                                return apartments.Select(x => x.ApartmentArea).DefaultIfEmpty(0).Sum().ToString() + " m2";
                            case SettlementUnitType.LOCATORS:
                                return apartments.Select(x => x.Locators).DefaultIfEmpty(0).Sum().ToString() + " lokatorów";
                            case SettlementUnitType.PER_APARTMENT:
                                return apartments.Count().ToString();
                            case SettlementUnitType.TOTAL_AREA:
                                return (apartments.Select(x => x.AdditionalArea).DefaultIfEmpty(0).Sum() +
                                    apartments.Select(x => x.ApartmentArea).DefaultIfEmpty(0).Sum()).ToString() + " m2";
                        }
                    }
                }
                return null;
            }
        }

        public string ConstUnitsCount
        {
            get
            {
                if ((SummaryData?.SettlementData?.ConstSummaryType).HasValue && (SummaryData?.SettlementData?.ConstSummaryType).Value != null && SummaryData?.SettlementData?.InvoiceData?.MasterData?.Building != null)
                {
                    using (var db = new DB.DomenaDBContext())
                    {
                        var apartments = db.Apartments.Where(x => !x.IsDeleted && x.BuildingId == SummaryData.SettlementData.InvoiceData.MasterData.Building.BuildingId);
                        switch (SummaryData.SettlementData.ConstSummaryType)
                        {
                            case SettlementUnitType.ADDITIONAL_AREA:
                                return apartments.Select(x => x.AdditionalArea).DefaultIfEmpty(0).Sum().ToString() + " m2";
                            case SettlementUnitType.APARTMENT_AREA:
                                return apartments.Select(x => x.ApartmentArea).DefaultIfEmpty(0).Sum().ToString() + " m2";
                            case SettlementUnitType.LOCATORS:
                                return apartments.Select(x => x.Locators).DefaultIfEmpty(0).Sum().ToString() + " lokatorów";
                            case SettlementUnitType.PER_APARTMENT:
                                return apartments.Count().ToString();
                            case SettlementUnitType.TOTAL_AREA:
                                return (apartments.Select(x => x.AdditionalArea).DefaultIfEmpty(0).Sum() +
                                    apartments.Select(x => x.ApartmentArea).DefaultIfEmpty(0).Sum()).ToString() + " m2";
                        }
                    }
                }
                return null;
            }
        }

        public string SelectedSummaryType
        {
            get
            {
                return SummaryData?.SettlementData?.InvoiceData?.MasterData?.SettlementType.Description();
            }
        }

        public Visibility IsUnitSettle
        {
            get
            {
                if (SummaryData?.SettlementData?.InvoiceData?.MasterData?.SettlementType == SettlementTypeEnum.UNITS)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public string IsMutual
        {
            get
            {
                return (SummaryData?.SettlementData?.IsMutualSettlement).HasValue && (SummaryData?.SettlementData?.IsMutualSettlement).Value ? "Tak" : "Nie";
            }
        }

        private ObservableCollection<BuildingChargeBasisCategory> _categories;
        public ObservableCollection<BuildingChargeBasisCategory> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                if (value != _categories)
                {
                    _categories = value;
                    OnPropertyChanged("Categories");
                }
            }
        }

        private BuildingChargeBasisCategory _selectedCategory;
        public BuildingChargeBasisCategory SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                if (value != _selectedCategory)
                {
                    _selectedCategory = value;
                    SummaryData.SelectedCategory = value;
                    OnPropertyChanged("SelectedCategory");
                }
            }
        }

        public SummaryPart(Building SelectedBuilding = null)
        {
            InitializeComponent();
            DataContext = this;
        }       
        
        public void InitializeView()
        {
            InitializeUnitDG();
            using (var db = new DB.DomenaDBContext())
            {
                Categories = new ObservableCollection<BuildingChargeBasisCategory>(db.CostCategories.Where(x => !x.IsDeleted).ToList());
            }
            OnPropertyChanged("");
        }

        private void InitializeUnitDG()
        {
            try
            {
                var dg = new DataGrid()
                {
                    AutoGenerateColumns = false,
                    CanUserAddRows = false,
                    CanUserDeleteRows = false,
                    IsReadOnly = true,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    FrozenColumnCount = 1,
                };

                var col = new DataGridTextColumn();
                col.Header = "Nr mieszkania";
                col.Binding = new Binding("Apartment.ApartmentNumber");
                col.SortDirection = ListSortDirection.Ascending;
                dg.Columns.Add(col);
                col = new DataGridTextColumn();
                col.Header = "Właściciel";
                col.Binding = new Binding("Owner.OwnerName");
                dg.Columns.Add(col);

                switch ((SummaryData?.SettlementData?.InvoiceData?.MasterData?.SettlementType).Value)
                {
                    case (SettlementTypeEnum.UNITS):
                        bool? isMutual = SummaryData?.SettlementData?.IsMutualSettlement;
                        if (isMutual.HasValue)
                        {
                            PrepareMutualDataGrid(isMutual.Value, dg);
                        }
                        break;
                    case SettlementTypeEnum.WATER:
                        break;
                }

                dg.ItemsSource = DGUnitSummaries;
                col = new DataGridTextColumn();
                col.Header = "Koszt całkowity";
                col.Binding = new Binding("TotalCost");
                col.Binding.StringFormat = "{0} zł";
                dg.Columns.Add(col);
                UnitSummaryDG = dg;
                OnPropertyChanged("DGUnitSummaries");
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd tworzenia rozliczenia. Sprawdź poprawność danych na poprzednich zakładkach");
                Log.Error(e, "Error during InitializeUnitDG");
            }

        }

        private void PrepareMutualDataGrid(bool IsMutual, DataGrid dg)
        {
            if (IsMutual)
            {
                var col = new DataGridTextColumn();
                col.Header = "Ilość jednostek";
                col.Binding = new Binding("MutualUnits");
                dg.Columns.Add(col);
                col = new DataGridTextColumn();
                col.Header = "Koszt jednostkowy";
                col.Binding = new Binding("MutualUnitCost");
                col.Binding.StringFormat = "{0:#.00} zł";
                dg.Columns.Add(col);
                col = new DataGridTextColumn();
                col.Header = "Koszty razem";
                col.Binding = new Binding("MutualCost");
                col.Binding.StringFormat = "{0} zł";
                dg.Columns.Add(col);

                DGUnitSummaries = new ObservableCollection<DGUnitSummary>();
                using (var db = new DB.DomenaDBContext())
                {
                    foreach (var a in db.Apartments.Where(x => !x.IsDeleted && x.BuildingId == SummaryData.SettlementData.InvoiceData.MasterData.Building.BuildingId))
                    {
                        DGUnitSummary dgu = new DGUnitSummary()
                        {
                            Apartment = a,
                            MutualUnitCost = SummaryData.SettlementData.MutualUnitCost,
                            Owner = db.Owners.FirstOrDefault(o => o.OwnerId == a.OwnerId),
                        };
                        switch (SummaryData.SettlementData.MutualSummaryType)
                        {
                            case SettlementUnitType.ADDITIONAL_AREA:
                                dgu.MutualUnits = a.AdditionalArea;
                                dgu.MutualCost = Decimal.Ceiling(100 * SummaryData.SettlementData.MutualUnitCost * Convert.ToDecimal(a.AdditionalArea)) / 100;
                                dgu.TotalCost = dgu.MutualCost;
                                break;
                            case SettlementUnitType.APARTMENT_AREA:
                                dgu.MutualUnits = a.ApartmentArea;
                                dgu.MutualCost = Decimal.Ceiling(100 * SummaryData.SettlementData.MutualUnitCost * Convert.ToDecimal(a.ApartmentArea)) / 100;
                                dgu.TotalCost = dgu.MutualCost;
                                break;
                            case SettlementUnitType.LOCATORS:
                                dgu.MutualUnits = a.Locators;
                                dgu.MutualCost = Decimal.Ceiling(100 * SummaryData.SettlementData.MutualUnitCost * Convert.ToDecimal(a.Locators)) / 100;
                                dgu.TotalCost = dgu.MutualCost;
                                break;
                            case SettlementUnitType.PER_APARTMENT:
                                dgu.MutualUnits = 1;
                                dgu.MutualCost = Decimal.Ceiling(100 * SummaryData.SettlementData.MutualUnitCost) / 100;
                                dgu.TotalCost = dgu.MutualCost;
                                break;
                            case SettlementUnitType.TOTAL_AREA:
                                dgu.MutualUnits = a.AdditionalArea + a.ApartmentArea;
                                dgu.MutualCost = Decimal.Ceiling(100 * SummaryData.SettlementData.MutualUnitCost * Convert.ToDecimal(a.AdditionalArea + a.ApartmentArea)) / 100;
                                dgu.TotalCost = dgu.MutualCost;
                                break;
                        }
                        DGUnitSummaries.Add(dgu);
                    }
                }
            }
            else
            {
                var col = new DataGridTextColumn();
                col.Header = "KS - Ilość jednostek";
                col.Binding = new Binding("ConstUnits");
                dg.Columns.Add(col);
                col = new DataGridTextColumn();
                col.Header = "KS - Koszt jednostkowy";
                col.Binding = new Binding("ConstUnitCost");
                col.Binding.StringFormat = "{0:#.00} zł";
                dg.Columns.Add(col);
                col = new DataGridTextColumn();
                col.Header = "Koszty stałe razem";
                col.Binding = new Binding("ConstCost");
                col.Binding.StringFormat = "{0} zł";
                dg.Columns.Add(col);
                col = new DataGridTextColumn();
                col.Header = "KZ - Ilość jednostek";
                col.Binding = new Binding("VarUnits");
                dg.Columns.Add(col);
                col = new DataGridTextColumn();
                col.Header = "KZ - Koszt jednostkowy";
                col.Binding = new Binding("VarUnitCost");
                col.Binding.StringFormat = "{0:#.00} zł";
                dg.Columns.Add(col);
                col = new DataGridTextColumn();
                col.Header = "Koszty zmienne razem";
                col.Binding = new Binding("VarCost");
                col.Binding.StringFormat = "{0} zł";
                dg.Columns.Add(col);

                DGUnitSummaries = new ObservableCollection<DGUnitSummary>();
                using (var db = new DB.DomenaDBContext())
                {
                    foreach (var a in db.Apartments.Where(x => !x.IsDeleted && x.BuildingId == SummaryData.SettlementData.InvoiceData.MasterData.Building.BuildingId))
                    {
                        DGUnitSummary dgu = new DGUnitSummary()
                        {
                            Apartment = a,
                            VarUnitCost = SummaryData.SettlementData.VarUnitCost,
                            ConstUnitCost = SummaryData.SettlementData.ConstUnitCost,
                            Owner = db.Owners.FirstOrDefault(o => o.OwnerId == a.OwnerId),
                            TotalCost = 0,
                        };
                        switch (SummaryData.SettlementData.VarSummaryType)
                        {
                            case SettlementUnitType.ADDITIONAL_AREA:
                                dgu.VarUnits = a.AdditionalArea;
                                dgu.VarCost = Decimal.Ceiling(100 * SummaryData.SettlementData.VarUnitCost * Convert.ToDecimal(a.AdditionalArea)) / 100;
                                dgu.TotalCost += dgu.VarCost;
                                break;
                            case SettlementUnitType.APARTMENT_AREA:
                                dgu.VarUnits = a.ApartmentArea;
                                dgu.VarCost = Decimal.Ceiling(100 * SummaryData.SettlementData.VarUnitCost * Convert.ToDecimal(a.ApartmentArea)) / 100;
                                dgu.TotalCost += dgu.VarCost;
                                break;
                            case SettlementUnitType.LOCATORS:
                                dgu.VarUnits = a.Locators;
                                dgu.VarCost = Decimal.Ceiling(100 * SummaryData.SettlementData.VarUnitCost * Convert.ToDecimal(a.Locators)) / 100;
                                dgu.TotalCost += dgu.VarCost;
                                break;
                            case SettlementUnitType.PER_APARTMENT:
                                dgu.VarUnits = 1;
                                dgu.VarCost = Decimal.Ceiling(100 * SummaryData.SettlementData.VarUnitCost) / 100;
                                dgu.TotalCost += dgu.VarCost;
                                break;
                            case SettlementUnitType.TOTAL_AREA:
                                dgu.VarUnits = a.AdditionalArea + a.ApartmentArea;
                                dgu.VarCost = Decimal.Ceiling(100 * SummaryData.SettlementData.VarUnitCost * Convert.ToDecimal(a.AdditionalArea + a.ApartmentArea)) / 100;
                                dgu.TotalCost += dgu.VarCost;
                                break;
                        }
                        switch (SummaryData.SettlementData.ConstSummaryType)
                        {
                            case SettlementUnitType.ADDITIONAL_AREA:
                                dgu.ConstUnits = a.AdditionalArea;
                                dgu.ConstCost = Decimal.Ceiling(100 * SummaryData.SettlementData.ConstUnitCost * Convert.ToDecimal(a.AdditionalArea)) / 100;
                                dgu.TotalCost += dgu.ConstCost;
                                break;
                            case SettlementUnitType.APARTMENT_AREA:
                                dgu.ConstUnits = a.ApartmentArea;
                                dgu.ConstCost = Decimal.Ceiling(100 * SummaryData.SettlementData.ConstUnitCost * Convert.ToDecimal(a.ApartmentArea)) / 100;
                                dgu.TotalCost += dgu.ConstCost;
                                break;
                            case SettlementUnitType.LOCATORS:
                                dgu.ConstUnits = a.Locators;
                                dgu.ConstCost = Decimal.Ceiling(100 * SummaryData.SettlementData.ConstUnitCost * Convert.ToDecimal(a.Locators)) / 100;
                                dgu.TotalCost += dgu.ConstCost;
                                break;
                            case SettlementUnitType.PER_APARTMENT:
                                dgu.ConstUnits = 1;
                                dgu.ConstCost = Decimal.Ceiling(100 * SummaryData.SettlementData.ConstUnitCost) / 100;
                                dgu.TotalCost += dgu.ConstCost;
                                break;
                            case SettlementUnitType.TOTAL_AREA:
                                dgu.ConstUnits = a.AdditionalArea + a.ApartmentArea;
                                dgu.ConstCost = Decimal.Ceiling(100 * SummaryData.SettlementData.ConstUnitCost * Convert.ToDecimal(a.AdditionalArea + a.ApartmentArea)) / 100;
                                dgu.TotalCost += dgu.ConstCost;
                                break;
                        }
                        DGUnitSummaries.Add(dgu);
                    }
                }
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

    public class SummaryData
    {
        public SettlementData SettlementData { get; set; }
        public BuildingChargeBasisCategory SelectedCategory { get; set; }
    }
}
