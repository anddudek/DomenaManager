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
using System.Collections;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for OwnersPage.xaml
    /// </summary>
    public partial class MonthlyChargesPage : UserControl, INotifyPropertyChanged
    {
        public string OpenCloseButtonText
        {
            get { return SelectedCharge == null || !SelectedCharge.IsClosed ? "Zamknij" : "Otwórz"; }
        }

        public PackIconKind OpenCloseButtonIcon
        {
            get { return SelectedCharge == null || !SelectedCharge.IsClosed ? PackIconKind.CloseCircle : PackIconKind.CheckCircle; }
        }

        private IList _selectedChargesList;
        public IList SelectedChargesList
        {
            get { return _selectedChargesList; }
            set
            {
                if (value != _selectedChargesList)
                {
                    _selectedChargesList = value;
                    OnPropertyChanged("SelectedChargesList");
                }
            }
        }

        public ObservableCollection<BuildingChargeBasisCategory> Categories { get; set; }

        private ObservableCollection<ChargeDataGrid> _charges;
        public ObservableCollection<ChargeDataGrid> Charges
        {
            get { return _charges; }
            set
            {
                _charges = value;
                OnPropertyChanged("Charges");
            }
        }

        private ICollectionView _chargesCV;
        public ICollectionView ChargesCV
        {
            get
            {
                return _chargesCV;
            }
            set
            {
                if (value != _chargesCV)
                {
                    _chargesCV = value;
                    OnPropertyChanged("ChargesCV");
                }
            }
        }

        private ChargeDataGrid _selectedCharge;
        public ChargeDataGrid SelectedCharge
        {
            get { return _selectedCharge; }
            set
            {
                _selectedCharge = value;
                OnPropertyChanged("SelectedCharge");
                OnPropertyChanged("OpenCloseButtonText");
                OnPropertyChanged("OpenCloseButtonIcon");
            }
        }

        private bool _groupByBuilding;
        public bool GroupByBuilding
        {
            get { return _groupByBuilding; }
            set
            { 
                if (value != _groupByBuilding)
                {
                    ICollectionView cvCharges = (CollectionView)CollectionViewSource.GetDefaultView(Charges);
                    if (value)
                    {
                        cvCharges.GroupDescriptions.Add(new PropertyGroupDescription("Building.Name")); //nameof(Building.BUildingName)
                    }
                    else
                    {
                        cvCharges.GroupDescriptions.Remove(cvCharges.GroupDescriptions.Cast<PropertyGroupDescription>().Where(x => x.PropertyName == "Building.Name").FirstOrDefault());
                    }
                    _groupByBuilding = value;
                    OnPropertyChanged("GroupByBuilding");
                }
            }
        }

        private bool _groupByApartment;
        public bool GroupByApartment
        {
            get { return _groupByApartment; }
            set
            {
                if (value != _groupByApartment)
                {
                    ICollectionView cvCharges = (CollectionView)CollectionViewSource.GetDefaultView(Charges);
                    if (value)
                    {
                        cvCharges.GroupDescriptions.Add(new PropertyGroupDescription("Apartment.ApartmentNumber"));
                    }
                    else
                    {
                        cvCharges.GroupDescriptions.Remove(cvCharges.GroupDescriptions.Cast<PropertyGroupDescription>().Where(x => x.PropertyName == "Apartment.ApartmentNumber").FirstOrDefault());
                    }
                    _groupByApartment = value;
                    OnPropertyChanged("GroupByApartment");
                }
            }
        }

        private bool _showClosed;
        public bool ShowClosed
        {
            get { return _showClosed; }
            set
            {
                if (value != _showClosed)
                {
                    _showClosed = value;
                    ChargesCV.Refresh();
                    OnPropertyChanged("ShowClosed");
                }
            }
        }

        private ObservableCollection<Owner> _ownersNames;
        public ObservableCollection<Owner> OwnersNames
        {
            get { return _ownersNames; }
            set
            {
                _ownersNames = value;
                OnPropertyChanged("OwnersNames");
            }
        }

        private Owner _selectedOwnerName;
        public Owner SelectedOwnerName
        {
            get { return _selectedOwnerName; }
            set
            {
                _selectedOwnerName = value;
                OnPropertyChanged("SelectedOwnerName");
                ChargesCV.Refresh();
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
                ChargesCV.Refresh();
            }
        }

        private ObservableCollection<int> _apartmentsNumbers;
        public ObservableCollection<int> ApartmentsNumbers
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

        private int? _selectedApartmentNumber;
        public int? SelectedApartmentNumber
        {
            get { return _selectedApartmentNumber; }
            set
            {
                if (value != _selectedApartmentNumber)
                {
                    _selectedApartmentNumber = value;
                    OnPropertyChanged("SelectedApartmentNumber");
                    ChargesCV.Refresh();
                }
            }
        }

        public ICommand ShowChargeDetails
        {
            get { return new Helpers.RelayCommand(ShowDetails, CanShowDetails); }
        }

        public ICommand ClearFilterCommand
        {
            get { return new Helpers.RelayCommand(ClearFilter, CanClearFilter); }
        }

        public ICommand PreparePdfCommand
        {
            get { return new Helpers.RelayCommand(PreparePdf, CanPreparePdf); }
        }

        public MonthlyChargesPage()
        {
            DataContext = this;
            InitializeCollection();
            InitializeCategories();
            InitializeLists();
            InitializeApartmentsNumbers();
            SelectedChargesList = new List<ChargeDataGrid>();
            InitializeComponent();
            GroupByBuilding = true;
        }

        private void InitializeCollection()
        {
            Charges = new ObservableCollection<ChargeDataGrid>();
            using (var db = new DB.DomenaDBContext())
            {
                foreach (var apartment in db.Apartments.Where(x => !x.IsDeleted))
                {
                    var c = Helpers.ChargesOperations.CreateMonthlyCharge(apartment);
                    var cdg = new ChargeDataGrid(c);
                    Charges.Add(cdg);
                }
            }
            
            ChargesCV = (CollectionView)CollectionViewSource.GetDefaultView(Charges);
            ChargesCV.SortDescriptions.Add(new SortDescription("Apartment.ApartmentNumber", ListSortDirection.Ascending)); 
            ChargesCV.Filter = FilterCollection;

            if (GroupByApartment)
            {
                ChargesCV.GroupDescriptions.Add(new PropertyGroupDescription("Apartment.ApartmentNumber"));
            }
            if (GroupByBuilding)
            {
                ChargesCV.GroupDescriptions.Add(new PropertyGroupDescription("Building.Name"));
            }
            ChargesCV.Refresh();
        }

        private void InitializeLists()
        {
            using (var db = new DB.DomenaDBContext())
            {
                _buildingsNames = new ObservableCollection<Building>(db.Buildings.ToList());
                _ownersNames = new ObservableCollection<Owner>(db.Owners.ToList());
            }
        }

        private void InitializeCategories()
        {            
            using (var db = new DB.DomenaDBContext())
            {
                Categories = new ObservableCollection<BuildingChargeBasisCategory>(db.CostCategories.ToList());
            }
        }

        private void InitializeApartmentsNumbers()
        {            
            if (Charges != null && SelectedBuildingName != null)
            {
                var a = SelectedBuildingName.BuildingId;
                var b = Charges.Where(x => x.Building.BuildingId.Equals(SelectedBuildingName.BuildingId)).ToList();
                var c = b.Select(x => x.Apartment.ApartmentNumber).ToList();
                var d = c.Distinct().ToList();
                ApartmentsNumbers = new ObservableCollection<int>(Charges.Where(x => x.Building.BuildingId.Equals(SelectedBuildingName.BuildingId)).Select(x => x.Apartment.ApartmentNumber).Distinct().OrderBy(x => x).ToList());
            }
        }

        private bool CanShowDetails()
        {
            return true;
        }

        private async void ShowDetails(object param)
        {
            if (SelectedCharge != null)
            {
                Wizards.PreviewChargeWizard pcw;

                pcw = new Wizards.PreviewChargeWizard(SelectedCharge);

                SwitchPage.SwitchMainPage(pcw, this);
            }
        }

        private bool CanPreparePdf()
        {
            return SelectedCharge != null && !SelectedCharge.IsClosed;
        }

        private void PreparePdf(object param)
        {
            /*Document doc = PDFOperations.CreateTemplate();
            PDFOperations.AddTitle(doc, "Naliczenie z dnia: " + SelectedCharge.ChargeDate.ToString("dd-MM-yyyy"));
            PDFOperations.AddChargeTable(doc, SelectedCharge);*/

            //PDFOperations.PrepareSingleChargeReport(SelectedCharge);
            if (SelectedChargesList.Count > 1)
            {
                foreach (var sc in SelectedChargesList)
                {
                    PDFOperations.PrepareSingleChargeReport((ChargeDataGrid)sc, true);
                }
            }
            else if (SelectedCharge != null)
            {
                PDFOperations.PrepareSingleChargeReport(SelectedCharge, false);
            }
            //doc.Save("test.pdf");
        }

        private void ClearFilter(object param)
        {
            SelectedOwnerName = null;
            SelectedBuildingName = null;
            SelectedApartmentNumber = null;
            ChargesCV.Refresh();
        }

        private bool CanClearFilter()
        {
            return true;
        }

        private bool FilterCollection(object item)
        {
            var cdg = item as ChargeDataGrid;
            if (SelectedBuildingName != null && !cdg.Building.BuildingId.Equals(SelectedBuildingName.BuildingId))
            {
                return false;
            }
            if (SelectedApartmentNumber != null && !cdg.Apartment.ApartmentNumber.Equals(SelectedApartmentNumber))
            {
                return false;
            }
            if (SelectedOwnerName != null && !cdg.Owner.OwnerId.Equals(SelectedOwnerName.OwnerId))
            {
                return false;
            }
            if (!ShowClosed && cdg.IsClosed)
            {
                return false;
            }
            
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
