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
    public partial class AnalysisPage : UserControl, INotifyPropertyChanged
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

        private string _ownerName;
        public string OwnerName
        {
            get
            {
                return _ownerName;
            }
            set
            {
                if (value != _ownerName)
                {
                    _ownerName = value;
                    OnPropertyChanged("OwnerName");
                }
            }
        }

        private string _buildingFullName;
        public string BuildingFullName
        {
            get
            {
                return _buildingFullName;
            }
            set
            {
                if (value != _buildingFullName)
                {
                    _buildingFullName = value;
                    OnPropertyChanged("BuildingFullName");
                }
            }
        }

        private string _apartmentNumber;
        public string ApartmentNumber
        {
            get
            {
                return _apartmentNumber;
            }
            set
            {
                if (value != _apartmentNumber)
                {
                    _apartmentNumber = value;
                    OnPropertyChanged("ApartmentNumber");
                }
            }
        }

        private string _locators;
        public string Locators
        {
            get
            {
                return _locators;
            }
            set
            {
                if (value != _locators)
                {
                    _locators = value;
                    OnPropertyChanged("Locators");
                }
            }
        }

        private string _ownedPercentage;
        public string OwnedPercentage
        {
            get
            {
                return _ownedPercentage;
            }
            set
            {
                if (value != _ownedPercentage)
                {
                    _ownedPercentage = value;
                    OnPropertyChanged("OwnedPercentage");
                }
            }
        }

        private string _apartmentTotalArea;
        public string ApartmentTotalArea
        {
            get
            {
                return _apartmentTotalArea;
            }
            set
            {
                if (value != _apartmentTotalArea)
                {
                    _apartmentTotalArea = value;
                    OnPropertyChanged("ApartmentTotalArea");
                }
            }
        }

        private ObservableCollection<AnalysisDataGrid> _costsCollection;
        public ObservableCollection<AnalysisDataGrid> CostsCollection
        {
            get
            {
                return _costsCollection;
            }
            set
            {
                if (value != _costsCollection)
                {
                    _costsCollection = value;
                    OnPropertyChanged("CostsCollection");
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

        public AnalysisPage()
        {
            DataContext = this;
            InitializeComponent();
            InitializeLists();
            InitializeApartmentsNumbers();
        }

        private void PrepareData(Apartment apartment, int year)
        {
            var owner = OwnersList.FirstOrDefault(x => x.OwnerId == apartment.OwnerId);
            var building = BuildingsNames.FirstOrDefault(x => x.BuildingId == apartment.BuildingId);

            OwnerName = owner.OwnerName;
            BuildingFullName = building.FullName;
            ApartmentNumber = apartment.ApartmentNumber.ToString();
            Locators = apartment.Locators.ToString();
            ApartmentTotalArea = (apartment.AdditionalArea + apartment.ApartmentArea).ToString() + " m2";

            using (var db = new DB.DomenaDBContext())
            {
                var apArea = db.Apartments.Where(x => x.BuildingId == apartment.BuildingId && !x.IsDeleted).Select(x => x.ApartmentArea).DefaultIfEmpty(0).Sum();
                var addArea = db.Apartments.Where(x => x.BuildingId == apartment.BuildingId && !x.IsDeleted).Select(x => x.AdditionalArea).DefaultIfEmpty(0).Sum();
                OwnedPercentage = (Math.Floor(((apartment.AdditionalArea + apartment.ApartmentArea) / (apArea + addArea) * 100) * 100) / 100).ToString() + "%";

                var groups = db.Invoices.Where(x => !x.IsDeleted && x.BuildingId == apartment.BuildingId).Select(x => x.InvoiceCategoryId).Distinct().ToList();

                var dataGridGroups = new List<InvoiceCategory>();
                var settleableGroups = db.BuildingInvoceBindings.Include(x => x.Building).Include(x => x.InvoiceCategory).Where(x => !x.IsDeleted && x.Building.BuildingId == building.BuildingId).ToList();

                foreach (var g in groups)
                {
                    if (settleableGroups.Any(x => x.InvoiceCategory.CategoryId == g))
                    {
                        dataGridGroups.Add(db.InvoiceCategories.FirstOrDefault(x => x.CategoryId == g));
                    }
                }

                int i = 1;
                CostsCollection = new ObservableCollection<AnalysisDataGrid>();

                //
                var buildingTotalArea = apArea + addArea;
                var apartmentsCount = db.Apartments.Where(x => !x.IsDeleted && x.BuildingId == apartment.BuildingId).Count();
                var totalLocators = db.Apartments.Where(x => !x.IsDeleted && x.BuildingId == apartment.BuildingId).Select(x => x.Locators).DefaultIfEmpty(0).Sum();

                foreach (var inv in dataGridGroups)
                {
                    var cc = new AnalysisDataGrid()
                    {
                        Id = i.ToString(),
                        Group = inv.CategoryName,
                        TotalCost = Math.Floor(db.Invoices.Where(x => x.InvoiceDate.Year == year && !x.IsDeleted && x.BuildingId == apartment.BuildingId && x.InvoiceCategoryId == inv.CategoryId).Select(x => x.CostAmountGross).DefaultIfEmpty(0).Sum() * 100) / 100,
                    };
                    double scale;
                    switch (settleableGroups.FirstOrDefault(x => x.InvoiceCategory.CategoryId == inv.CategoryId).Distribution)
                    {
                        default:
                            break;
                        case CostDistribution.PerAdditionalArea:
                            scale = (apartment.AdditionalArea / addArea);
                            cc.ApartmentCost = Math.Floor(100 * (cc.TotalCost * scale)) / 100;
                            break;
                        case CostDistribution.PerApartment:
                            scale = (1 / apartmentsCount);
                            cc.ApartmentCost = Math.Floor(100 * (cc.TotalCost * scale)) / 100;
                            break;
                        case CostDistribution.PerApartmentArea:
                            scale = (apartment.ApartmentArea / apArea);
                            cc.ApartmentCost = Math.Floor(100 * (cc.TotalCost * scale)) / 100;
                            break;
                        case CostDistribution.PerApartmentTotalArea:
                            scale = ((apartment.ApartmentArea + apartment.AdditionalArea) / buildingTotalArea);
                            cc.ApartmentCost = Math.Floor(100 * (cc.TotalCost * scale)) / 100;
                            break;
                        case CostDistribution.PerLocators:
                            scale = (apartment.Locators / totalLocators);
                            cc.ApartmentCost = Math.Floor(100 * (cc.TotalCost * scale)) / 100;
                            break;
                    }
                    CostsCollection.Add(cc);
                    i++;
                }
                CostsCollection.Add(new AnalysisDataGrid()
                {
                    Id = i.ToString(),
                    Group = "Razem: ",
                    TotalCost = CostsCollection.Sum(x => x.TotalCost),
                    ApartmentCost = CostsCollection.Sum(x => x.ApartmentCost),
                });
            }
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
