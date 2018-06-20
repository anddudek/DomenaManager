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

        public SummaryPage()
        {
            DataContext = this;
            InitializeComponent();

            var sdg = new SummaryDataGrid();
            using (var db = new DB.DomenaDBContext())
            {
                var ap = db.Apartments.FirstOrDefault();
                var bu = db.Buildings.FirstOrDefault(x => x.BuildingId.Equals(ap.BuildingId));
                var ow = db.Owners.FirstOrDefault(x => x.OwnerId.Equals(ap.OwnerId));
                sdg.apartment = ap;
                sdg.building = bu;
                sdg.owner = ow;
                sdg.year = DateTime.Today.Year;
                sdg.categories = db.CostCategories.ToArray();
                sdg.rows = new SummaryDataGridRow[12];
                                
                for (int i = 0; i < 12; i++)
                {
                    sdg.rows[i] = new SummaryDataGridRow();
                    sdg.rows[i].month = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(new DateTime(2000, i+1, 1).ToString("MMMM"));
                    sdg.rows[i].charges = new string[sdg.categories.Length];
                    for (int j = 0; j < sdg.categories.Length; j++)
                    {
                        sdg.rows[i].charges[j] = (j + i).ToString();
                    }
                }                
            }
            var a = new DataGrid();
            a.AutoGenerateColumns = false;
            var col = new DataGridTextColumn();
            col.Header = "Miesiąc";
            col.Binding = new Binding("month");
            a.Columns.Add(col);
            for (int i = 0; i < sdg.categories.Length; i++)
            {
                var ncol = new DataGridTextColumn();
                ncol.Header = sdg.categories[i].CategoryName;
                ncol.Binding = new Binding("charges[" + i + "]");
                a.Columns.Add(ncol);
            }
            a.ItemsSource = sdg.rows;
            SummaryDG = a;
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
