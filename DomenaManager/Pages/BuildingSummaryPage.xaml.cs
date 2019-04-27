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
    public partial class BuildingSummaryPage : UserControl, INotifyPropertyChanged
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

        private BuildingSummaryDataGrid _selectedSummary;
        public BuildingSummaryDataGrid SelectedSummary
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
                OnPropertyChanged("SelectedBuildingName");
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

        public BuildingSummaryPage()
        {
            DataContext = this;
            InitializeComponent();
            InitializeLists();
        }

        private void PrepareData(Building building, int year)
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
            var sdg = new BuildingSummaryDataGrid();

            

            using (var db = new DB.DomenaDBContext())
            {       
                sdg.building = building;
                sdg.year = year;
                sdg.rows = new BuildingSummaryDataGridRow[14];

                var apartments = db.Apartments.Where(x => x.BuildingId == building.BuildingId && !x.IsDeleted).Select(x => x.ApartmentId).ToList();
                var payments = db.Payments.Where(x => apartments.Contains(x.ApartmentId) && x.PaymentRegistrationDate.Year <= year && !x.IsDeleted);

                var invoices = db.Invoices.Where(x => !x.IsDeleted && x.InvoiceDate.Year == year && x.BuildingId == building.BuildingId);
                var invoiceCategories = db.InvoiceCategories.ToList();
                var distinctCategories = invoices.Select(x => x.InvoiceCategoryId).Distinct();
                
                var rowArrayLength = distinctCategories.Count();
                var columnsCount = rowArrayLength + 4;

                // Last year row

                sdg.rows[0] = new BuildingSummaryDataGridRow()
                {
                    month = "Zeszły rok",
                    invoices = new string[rowArrayLength],
                };
                for (int k = 0; k < rowArrayLength; k++)
                {
                    sdg.rows[0].invoices[k] = "-";
                }
                decimal lastYearSaldo = Payments.CalculateBuildingSaldo(year - 1, building);
                sdg.rows[0].invoicesSum = "-";
                sdg.rows[0].payments = "-";
                sdg.rows[0].saldo = lastYearSaldo.ToString() + " zł"; // lastYear

                for (int i = 1; i < 13; i++)//months
                {
                    sdg.rows[i] = new BuildingSummaryDataGridRow();
                    sdg.rows[i].month = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(new DateTime(2000, i, 1).ToString("MMMM"));
                    sdg.rows[i].invoices = new string[rowArrayLength];
                    int iterator = 0;
                    decimal invoicesSum = 0;

                    foreach (var g in distinctCategories)
                    {
                        if (g != null)
                        {
                            if (i == 1)
                            {
                                var catCol = new DataGridTextColumn();
                                catCol.Header = invoiceCategories.FirstOrDefault(x => x.CategoryId == g).CategoryName;
                                catCol.Binding = new Binding("invoices[" + iterator + "]");
                                catCol.CellStyle = cellCenteredStyle;
                                a.Columns.Add(catCol);
                            }

                            var sum = invoices.Where(x => x.InvoiceCategoryId == g && x.InvoiceDate.Month == i).Select(x => x.CostAmountGross).DefaultIfEmpty(0).Sum();
                            invoicesSum += sum;
                            sdg.rows[i].invoices[iterator] = sum.ToString() + " zł";                             
                            iterator++;
                        }
                    }

                    if (i == 1)
                    {
                        var catCol = new DataGridTextColumn();
                        catCol.Header = "Suma kosztów";
                        catCol.Binding = new Binding("invoicesSum");
                        catCol.CellStyle = cellStyle;
                        a.Columns.Add(catCol);

                        catCol = new DataGridTextColumn();
                        catCol.Header = "Wpłaty";
                        catCol.Binding = new Binding("payments");
                        catCol.CellStyle = cellStyle;
                        a.Columns.Add(catCol);

                        catCol = new DataGridTextColumn();
                        catCol.Header = "Saldo";
                        catCol.Binding = new Binding("saldo");
                        catCol.CellStyle = cellStyle;
                        a.Columns.Add(catCol);
                    }

                    sdg.rows[i].invoicesSum = invoicesSum.ToString() + " zł";
                    decimal paym = payments.Where(x => x.PaymentRegistrationDate.Month == i).Select(x => x.PaymentAmount).DefaultIfEmpty(0).Sum();
                    sdg.rows[i].payments = paym.ToString() + " zł";
                    sdg.rows[i].saldo = (paym - invoicesSum).ToString() + " zł";
                }

                // Summary row
                sdg.rows[sdg.rows.Length - 1] = new BuildingSummaryDataGridRow()
                {
                    month = "Razem",
                    invoices = new string[rowArrayLength],
                };
                for (int k = 0; k < rowArrayLength; k++)
                {
                    sdg.rows[sdg.rows.Length - 1].invoices[k] = "-";
                }
                sdg.rows[sdg.rows.Length - 1].invoicesSum = (invoices.Select(x => x.CostAmountGross).DefaultIfEmpty(0).Sum()).ToString() + " zł";
                sdg.rows[sdg.rows.Length - 1].payments = (payments.Select(x => x.PaymentAmount).DefaultIfEmpty(0).Sum()).ToString() + " zł";
                sdg.rows[sdg.rows.Length - 1].saldo = (Payments.CalculateBuildingSaldo(year, building)).ToString() + " zł";
            }
            a.ItemsSource = sdg.rows;
            SummaryDG = a;
            SelectedYear = sdg.year.ToString();
            SelectedSummary = sdg;    
            
        }

        private void Filter(object param)
        {
            int y;
            if (int.TryParse(SelectedYear, out y))
                PrepareData(SelectedBuildingName, y);
        }

        private bool CanFilter()
        {
            int y;
            bool tp = int.TryParse(SelectedYear, out y);
            return SelectedBuildingName != null && tp && y > 2000 && y <= DateTime.Now.Year;
        }

        private void ExportPDF(object param)
        {
            /*int y;
            if (int.TryParse(SelectedYear, out y))
                PDFOperations.PrepareSingleYearSummary(y, SelectedApartmentNumber, owner, SelectedBuildingName, true);*/
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
            PrepareData(SelectedBuildingName, SelectedSummary.year - 1);
        }

        private void SwitchNextYear(object param)
        {
            SelectedYear = (SelectedSummary.year + 1).ToString();
            PrepareData(SelectedBuildingName, SelectedSummary.year + 1);
        }

        private bool CanSwitchYear()
        {
            return SelectedSummary != null;
        }

        private void InitializeLists()
        {
            using (var db = new DB.DomenaDBContext())
            {
                BuildingsNames = new ObservableCollection<Building>(db.Buildings.ToList());
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
