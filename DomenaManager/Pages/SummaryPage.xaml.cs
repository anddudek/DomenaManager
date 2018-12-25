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

        public ICommand MaximizeCommand
        {
            get { return new RelayCommand(Maximize, CanMaximize); }
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
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                FrozenColumnCount = 1,
            };
            var cellStyle = new Style(typeof(DataGridCell));
            cellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#87B04D"))));
            ControlTemplate ct = new ControlTemplate(typeof(DataGridCell));
            string template =
        "<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' TargetType =\"DataGridCell\">" +
             "<Grid Background = \"{TemplateBinding Background}\"> " +
                "<ContentPresenter HorizontalAlignment = \"Center\" VerticalAlignment = \"Center\"/>" +  
             "</Grid>" +
        "</ControlTemplate>";
            cellStyle.Setters.Add(new Setter(DataGridCell.TemplateProperty, (ControlTemplate)System.Windows.Markup.XamlReader.Parse(template)));

            var cellCenteredStyle = new Style(typeof(DataGridCell));
            cellCenteredStyle.Setters.Add(new Setter(DataGridCell.TemplateProperty, (ControlTemplate)System.Windows.Markup.XamlReader.Parse(template)));

            var headerStyle = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
            headerStyle.Setters.Add(new Setter(FontSizeProperty, 18.0));
            headerStyle.Setters.Add(new Setter(BackgroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#B4B4B4"))));
            string templateHdr =
        "<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' TargetType =\"DataGridColumnHeader\">" +
             "<Grid Background = \"{TemplateBinding Background}\"> " +
                "<ContentPresenter HorizontalAlignment = \"Center\" VerticalAlignment = \"Center\"/>" +
             "</Grid>" +
        "</ControlTemplate>";
            headerStyle.Setters.Add(new Setter(System.Windows.Controls.Primitives.DataGridColumnHeader.TemplateProperty, (ControlTemplate)System.Windows.Markup.XamlReader.Parse(templateHdr)));

            var col = new DataGridTextColumn();
            col.Header = "Miesiąc";
            col.Binding = new Binding("month");
            //col.HeaderStyle = headerStyle;
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

                var charges = db.Charges.Include(x => x.Components).Include(x => x.Components.Select(y => y.GroupName)).Where(x => !x.IsDeleted && x.ApartmentId.Equals(apartment.ApartmentId) && x.ChargeDate.Year.Equals(sdg.year));
                var componentsList = new List<ChargeComponent>();
                foreach (var charge in charges)
                {
                    componentsList.AddRange(charge.Components);
                }
                var groups = componentsList.Select(x => x.GroupName);

                var payments = db.Payments.Include(x => x.ChargeGroup).Where(x => !x.IsDeleted && x.ApartmentId == apartment.ApartmentId && x.PaymentRegistrationDate.Year == sdg.year);
                var paymentGroups = payments.Select(x => x.ChargeGroup);

                var allGroups = new List<BuildingChargeGroupName>();
                allGroups.AddRange(groups);
                allGroups.AddRange(paymentGroups);
                var distinctGroups = allGroups.Distinct();

                var uniqueCategories = componentsList.GroupBy(x => x.CostCategoryId).Select(x => x.FirstOrDefault()).Select(x => x.CostCategoryId);
                sdg.categories = db.CostCategories.Where(p => !p.IsDeleted && uniqueCategories.Any(g => g.Equals(p.BuildingChargeBasisCategoryId))).ToArray();
                var rowArrayLength = sdg.categories.Length + distinctGroups.Count() * 2;
                var columnsCount = rowArrayLength + 2;

                // Last year row

                sdg.rows[0] = new SummaryDataGridRow()
                {
                    month = "Zeszły rok",
                    charges = new string[rowArrayLength],
                };
                for (int k = 0; k < rowArrayLength; k++)
                {
                    sdg.rows[0].charges[k] = "-";
                }
                double lastYearSaldo = Payments.CalculateSaldo(year - 1, apartment);
                sdg.rows[0].chargesSum = lastYearSaldo.ToString() + " zł"; // lastYear

                for (int i = 1; i < 13; i++)//months
                {
                    var thisMonthComponents = new List<ChargeComponent>();
                    charges.Where(x => x.ChargeDate.Month == i).ToList().ForEach(x => thisMonthComponents.AddRange(x.Components));

                    sdg.rows[i] = new SummaryDataGridRow();
                    sdg.rows[i].month = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(new DateTime(2000, i, 1).ToString("MMMM"));
                    sdg.rows[i].charges = new string[rowArrayLength];
                    double currentMonthSum = 0;
                    int iterator = 0;

                    foreach (var g in distinctGroups)
                    {
                        if (g != null)
                        {
                            // iterujemy po kazdej grupie a w srodku po kazdej kategorii + Wplty w grupie + suma
                            var categoriesInGroup = componentsList.Where(x => x.GroupName!=null && x.GroupName.BuildingChargeGroupNameId == g.BuildingChargeGroupNameId).GroupBy(x => x.CostCategoryId).Select(x => x.FirstOrDefault()).Select(x => x.CostCategoryId);
                            double groupSum = 0;
                            foreach (var cat in categoriesInGroup)
                            {
                                var currentComponets = thisMonthComponents.Where(x => x.CostCategoryId == cat && x.GroupName.BuildingChargeGroupNameId == g.BuildingChargeGroupNameId);
                                groupSum += currentComponets.Sum(x => x.Sum);
                                sdg.rows[i].charges[iterator] = currentComponets.Sum(x => x.Sum).ToString() + " zł";

                                if (a.Columns.Count < columnsCount - 1)
                                {
                                    var catCol = new DataGridTextColumn();
                                    catCol.Header = db.CostCategories.FirstOrDefault(x => x.BuildingChargeBasisCategoryId == cat).CategoryName;
                                    catCol.Binding = new Binding("charges[" + iterator + "]");
                                    catCol.CellStyle = cellCenteredStyle;
                                    a.Columns.Add(catCol);
                                }
                                iterator++;
                            }
                            //Wplaty
                            var groupPayments = payments.Where(x => x.PaymentRegistrationDate.Month == i && x.ChargeGroup.BuildingChargeGroupNameId == g.BuildingChargeGroupNameId).Select(x => x.PaymentAmount).DefaultIfEmpty(0).Sum();
                            sdg.rows[i].charges[iterator] = groupPayments.ToString() + " zł";

                            if (a.Columns.Count < columnsCount - 1)
                            {
                                var paymCol = new DataGridTextColumn();
                                paymCol.Header = "Wpłaty";
                                paymCol.Binding = new Binding("charges[" + iterator + "]");
                                paymCol.CellStyle = cellCenteredStyle;
                                a.Columns.Add(paymCol);
                            }
                            iterator++;

                            //Suma
                            sdg.rows[i].charges[iterator] = (groupSum).ToString() + " zł";//(groupPayments - groupSum).ToString() + " zł";
                            currentMonthSum += (groupPayments - groupSum);
                            var groupSumCol = new DataGridTextColumn();

                            if (a.Columns.Count < columnsCount - 1)
                            {
                                groupSumCol.CellStyle = cellStyle;
                                groupSumCol.Header = "Razem - " + g.GroupName;
                                groupSumCol.Binding = new Binding("charges[" + iterator + "]");
                                a.Columns.Add(groupSumCol);
                            }
                            iterator++;
                        }
                    }
                    sdg.rows[i].chargesSum = currentMonthSum.ToString() + " zł";
                }

                // Summary row
                sdg.rows[sdg.rows.Length - 1] = new SummaryDataGridRow()
                {
                    month = "Razem",
                    charges = new string[rowArrayLength],
                };
                for (int k = 0; k < rowArrayLength; k++)
                {
                    sdg.rows[sdg.rows.Length - 1].charges[k] = "-";
                }
                //sdg.rows[sdg.rows.Length - 1].charges[sdg.rows[sdg.rows.Length - 1].charges.Length - 1] = (Payments.CalculateSaldo(year, apartment)).ToString() + " zł"; //AllYears year
                sdg.rows[sdg.rows.Length - 1].chargesSum = (Payments.CalculateSaldo(year, apartment)).ToString() + " zł";

                var sumCol = new DataGridTextColumn();
                sumCol.Header = "Razem";
                sumCol.Binding = new Binding("chargesSum");
                sumCol.CellStyle = cellStyle;
                a.Columns.Add(sumCol);                
            }
            a.ItemsSource = sdg.rows;
            //a.ColumnHeaderStyle = headerStyle;
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
                PDFOperations.PrepareSingleYearSummary(y, SelectedApartmentNumber, owner, SelectedBuildingName, true);
        }

        private bool CanExportPDF()
        {
            return SummaryDG != null;
        }

        private void Maximize(object param)
        {
            Windows.MaximizeSummaryWindow msw = new Windows.MaximizeSummaryWindow();
            msw.SummaryDG = SummaryDG;
            msw.Show();
        }

        private bool CanMaximize()
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
                ApartmentsNumbers = new ObservableCollection<Apartment>(c.Where(x => x.BuildingId.Equals(SelectedBuildingName.BuildingId) && !x.IsDeleted).OrderBy(x => x.ApartmentNumber).ToList());
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
