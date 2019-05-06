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
        
        public SummaryPart(Building SelectedBuilding = null)
        {
            InitializeComponent();
            DataContext = this;
        }       
        
        public void InitializeView()
        {
            InitializeUnitDG();
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

                if ((SummaryData?.SettlementData?.IsMutualSettlement).HasValue && (SummaryData?.SettlementData?.IsMutualSettlement).Value)
                {
                    col = new DataGridTextColumn();
                    col.Header = "Ilość jednostek";
                    col.Binding = new Binding("MutualUnits");
                    dg.Columns.Add(col);
                    col = new DataGridTextColumn();
                    col.Header = "Koszt jednostkowy";
                    col.Binding = new Binding("MutualUnitCost");
                    dg.Columns.Add(col);
                    col = new DataGridTextColumn();
                    col.Header = "Koszt całkowity";
                    col.Binding = new Binding("MutualCost");
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
                                    dgu.MutualCost = SummaryData.SettlementData.MutualUnitCost * Convert.ToDecimal(a.AdditionalArea);
                                    break;
                                case SettlementUnitType.APARTMENT_AREA:
                                    dgu.MutualUnits = a.ApartmentArea;
                                    dgu.MutualCost = SummaryData.SettlementData.MutualUnitCost * Convert.ToDecimal(a.ApartmentArea);
                                    break;
                                case SettlementUnitType.LOCATORS:
                                    dgu.MutualUnits = a.Locators;
                                    dgu.MutualCost = SummaryData.SettlementData.MutualUnitCost * Convert.ToDecimal(a.Locators);
                                    break;
                                case SettlementUnitType.PER_APARTMENT:
                                    dgu.MutualUnits = 1;
                                    dgu.MutualCost = SummaryData.SettlementData.MutualUnitCost;
                                    break;
                                case SettlementUnitType.TOTAL_AREA:
                                    dgu.MutualUnits = a.AdditionalArea + a.ApartmentArea;
                                    dgu.MutualCost = SummaryData.SettlementData.MutualUnitCost * Convert.ToDecimal(a.AdditionalArea + a.ApartmentArea);
                                    break;
                            }
                            DGUnitSummaries.Add(dgu);
                        }
                    }
                }

                dg.ItemsSource = DGUnitSummaries;
                UnitSummaryDG = dg;
                OnPropertyChanged("DGUnitSummaries");
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd tworzenia rozliczenia. Sprawdź poprawność danych na poprzednich zakładkach");
                Log.Error(e, "Error during InitializeUnitDG");
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
    }
}
