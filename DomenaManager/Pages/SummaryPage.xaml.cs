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
using System.Globalization;

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class SummaryPage : UserControl, INotifyPropertyChanged
    {
        private DataGrid _summaryDG;
        public DataGrid SummaryDG
        {
            get
            {
                return _summaryDG;
            }
            set
            {
                if (value != _summaryDG)
                {
                    _summaryDG = value;
                    OnPropertyChanged("SummaryDG");
                }
            }
        }

        private SummaryDataGrid _selectedSummary;
        public SummaryDataGrid SelectedSummary
        {
            get { return _selectedSummary; }
            set
            {
                if (value != _selectedSummary)
                {
                    _selectedSummary = value;
                    OnPropertyChanged("SelectedSummary");
                }
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
            }
        }

        private ObservableCollection<Apartment> ApartmentsList { get; set; }

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

        private ObservableCollection<Owner> OwnersList { get; set; }

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
                    if (_selectedApartmentNumber != null)
                    {                        
                        owner = OwnersList.FirstOrDefault(x => x.OwnerId.Equals(_selectedApartmentNumber.OwnerId));
                        SelectedOwner = owner.OwnerName;
                    }
                }
            }
        }

        private Owner owner { get; set; }

        private string _selectedOwner;
        public string SelectedOwner
        {
            get { return _selectedOwner; }
            set
            {
                if (value != _selectedOwner)
                {
                    _selectedOwner = value;
                    OnPropertyChanged("SelectedOwner");
                }
            }
        }

        private string _selectedYear;
        public string SelectedYear 
        {
            get
            {
                return _selectedYear;
            }
            set
            {
                if (value != _selectedYear)
                {
                    _selectedYear = value;
                    OnPropertyChanged("SelectedYear");
                }
            }
        }

        public ICommand FilterCommand
        {
            get { return new RelayCommand(Filter, CanFilter); }
        }

        public ICommand PreviousYearCommand
        {
            get { return new RelayCommand(SwitchPreviousYear, CanSwitchYear); }
        }

        public ICommand NextYearCommand
        {
            get { return new RelayCommand(SwitchNextYear, CanSwitchYear); }
        }

        public ICommand SaveToPdfCommand
        {
            get { return new RelayCommand(ExportPDF, CanExportPDF); }
        }

        public SummaryPage()
        {
            DataContext = this;
            InitializeComponent();
            InitializeLists();
            InitializeApartmentsNumbers();
        }

        private void PrepareData(Apartment apartment, int year)
        {
            var a = new DataGrid()
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true,
            };
            var col = new DataGridTextColumn();
            col.Header = "Miesiąc";
            col.Binding = new Binding("month");
            a.Columns.Add(col);
            var sdg = new SummaryDataGrid();

            using (var db = new DB.DomenaDBContext())
            {
                var bu = db.Buildings.FirstOrDefault(x => x.BuildingId.Equals(apartment.BuildingId));
                var ow = db.Owners.FirstOrDefault(x => x.OwnerId.Equals(apartment.OwnerId));
                sdg.apartment = apartment;
                sdg.building = bu;
                sdg.owner = ow;
                sdg.year = year;
                sdg.rows = new SummaryDataGridRow[14];

                var charges = db.Charges.Include(x => x.Components).Where(x => x.ApartmentId.Equals(apartment.ApartmentId) && x.ChargeDate.Year.Equals(sdg.year));
                var cat = db.Charges.Include(c => c.Components).Where(x => x.ApartmentId.Equals(apartment.ApartmentId) && !x.IsDeleted && x.ChargeDate.Year == year).Select(x => x.Components);
                List<ChargeComponent> allComponents = new List<ChargeComponent>();
                foreach (var c in cat)
                {
                    allComponents.AddRange(c);
                }
                var uniqueCategories = allComponents.GroupBy(x => x.CostCategoryId).Select(x => x.FirstOrDefault()).Select(x => x.CostCategoryId);
                sdg.categories = db.CostCategories.Where(p => uniqueCategories.Any(g => g.Equals(p.BuildingChargeBasisCategoryId))).ToArray();

                sdg.rows[0] = new SummaryDataGridRow()
                {
                    month = "Zeszły rok",
                    charges = new string[sdg.categories.Length + 2],                
                };
                for (int k = 0; k < sdg.categories.Length + 1; k++)
                {
                    sdg.rows[0].charges[k] = "-";
                }
                double lastYearSaldo = Payments.CalculateSaldo(year-1, apartment);
                sdg.rows[0].chargesSum = "-";
                sdg.rows[0].charges[sdg.categories.Length + 1] = lastYearSaldo.ToString() + " zł"; // lastYear

                double yearSum = 0;
                for (int i = 1; i < 13; i++)//months
                {
                    sdg.rows[i] = new SummaryDataGridRow();
                    sdg.rows[i].month = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(new DateTime(2000, i, 1).ToString("MMMM"));
                    sdg.rows[i].charges = new string[sdg.categories.Length + 2];
                    double allCat = 0;
                    for (int j = 0; j < sdg.categories.Length; j++)//each categories
                    {
                        double sum = 0;
                        foreach (var c in charges.Where(x => x.ChargeDate.Month.Equals(i)))
                        {
                            foreach (var cc in c.Components)
                            {
                                if (cc.CostCategoryId.Equals(sdg.categories[j].BuildingChargeBasisCategoryId))
                                {
                                    sum += cc.Sum;
                                }
                            }
                        }
                        allCat += sum;
                        sdg.rows[i].charges[j] = sum.ToString() + " zł";
                    }
                    double thisMonthPayments = db.Payments.Where(x => x.ApartmentId.Equals(apartment.ApartmentId) && x.PaymentRegistrationDate.Year == year && x.PaymentRegistrationDate.Month == i).Select(x => x.PaymentAmount).DefaultIfEmpty(0).Sum();
                    sdg.rows[i].chargesSum = allCat.ToString() + " zł";
                    sdg.rows[i].charges[sdg.categories.Length] = thisMonthPayments.ToString() + " zł";
                    sdg.rows[i].charges[sdg.categories.Length + 1] = (thisMonthPayments - allCat).ToString() + " zł"; //SALDO
                    yearSum += thisMonthPayments - allCat;
                }
                sdg.rows[sdg.rows.Length - 1] = new SummaryDataGridRow()
                {
                    month = "Razem",
                    charges = new string[sdg.categories.Length + 2],
                };
                for (int k = 0; k < sdg.categories.Length + 1; k++)
                {
                    sdg.rows[sdg.rows.Length - 1].charges[k] = "-";
                }
                sdg.rows[sdg.rows.Length - 1].charges[sdg.categories.Length + 1] = (Payments.CalculateSaldo(year, apartment)).ToString() + " zł"; //AllYears year
                sdg.rows[sdg.rows.Length - 1].chargesSum = "-";
            }
            for (int i = 0; i < sdg.categories.Length; i++)
            {
                var ncol = new DataGridTextColumn();
                ncol.Header = sdg.categories[i].CategoryName;
                ncol.Binding = new Binding("charges[" + i + "]");
                a.Columns.Add(ncol);
            }
            var sepColumn = new DataGridTextColumn();
            sepColumn.MaxWidth = 3;
            sepColumn.MinWidth = 3;
            sepColumn.Width = 3;            
            sepColumn.CellStyle = new Style(typeof(DataGridCell));
            sepColumn.CellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightGray)));
            a.Columns.Add(sepColumn);
            var sumCol = new DataGridTextColumn();
            sumCol.Header = "Razem";
            sumCol.Binding = new Binding("chargesSum");
            a.Columns.Add(sumCol);
            var paym = new DataGridTextColumn();
            paym.Header = "Wpłaty";
            paym.Binding = new Binding("charges[" + (sdg.categories.Length) + "]");
            a.Columns.Add(paym);
            var saldo = new DataGridTextColumn();
            saldo.Header = "Saldo";
            saldo.Binding = new Binding("charges[" + (sdg.categories.Length + 1) + "]");
            a.Columns.Add(saldo);
            a.ItemsSource = sdg.rows;            
            SummaryDG = a;
            SelectedYear = sdg.year.ToString();
            SelectedSummary = sdg;            
        }

        private void Filter(object param)
        {
            int y;
            if (int.TryParse(SelectedYear, out y))
                PrepareData(SelectedApartmentNumber, y);
        }

        private bool CanFilter()
        {
            int y;
            bool tp = int.TryParse(SelectedYear, out y);
            return SelectedBuildingName != null && SelectedApartmentNumber != null && tp && y > 2000 && y <= DateTime.Now.Year;
        }

        private void ExportPDF(object param)
        {
            int y;
            if (int.TryParse(SelectedYear, out y))
                PDFOperations.PrepareSingleYearSummary(SummaryDG, y, SelectedApartmentNumber, owner, SelectedBuildingName, true);
        }

        private bool CanExportPDF()
        {
            return SummaryDG != null;
        }

        private void SwitchPreviousYear(object param)
        {
            SelectedYear = (SelectedSummary.year - 1).ToString();
            PrepareData(SelectedSummary.apartment, SelectedSummary.year - 1);
        }

        private void SwitchNextYear(object param)
        {
            SelectedYear = (SelectedSummary.year + 1).ToString();
            PrepareData(SelectedSummary.apartment, SelectedSummary.year + 1);
        }

        private bool CanSwitchYear()
        {
            return SelectedSummary != null;
        }

        private void InitializeApartmentsNumbers()
        {
            if (SelectedBuildingName != null)
            {
                var a = SelectedBuildingName.BuildingId;
                var b = ApartmentsList.Where(x => x.BuildingId.Equals(SelectedBuildingName.BuildingId)).ToList();
                var c = b.Distinct().ToList();
                ApartmentsNumbers = new ObservableCollection<Apartment>(c.Where(x => x.BuildingId.Equals(SelectedBuildingName.BuildingId)).OrderBy(x => x.ApartmentNumber).ToList());
            }
        }

        private void InitializeLists()
        {
            using (var db = new DB.DomenaDBContext())
            {
                BuildingsNames = new ObservableCollection<Building>(db.Buildings.ToList());
                ApartmentsList = new ObservableCollection<Apartment>(db.Apartments.ToList());
                OwnersList = new ObservableCollection<Owner>(db.Owners.ToList());
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
