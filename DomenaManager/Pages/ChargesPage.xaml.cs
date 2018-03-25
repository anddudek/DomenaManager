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

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for OwnersPage.xaml
    /// </summary>
    public partial class ChargesPage : UserControl, INotifyPropertyChanged
    {
        private IList _testSelected;
        public IList TestSelected
        {
            get { return _testSelected; }
            set
            {
                if (value != _testSelected)
                {
                    _testSelected = value;
                    OnPropertyChanged("TestSelected");
                }
            }
        }

        public ObservableCollection<CostCategory> Categories { get; set; }

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

        public ICommand EditApartmentCommand
        {
            get { return new Helpers.RelayCommand(Edit, CanEdit); }
        }

        private bool CanEdit()
        {
            return true;
        }

        private void Edit(object param)
        {
            var a = SelectedCharge;
            var b = TestSelected;
        }

        public ICommand AddApartmentCommand
        {
            get { return new Helpers.RelayCommand(Add, CanAdd); }
        }

        private bool CanAdd()
        {
            return true;
        }

        private void Add(object param)
        {
            Charges = new ObservableCollection<ChargeDataGrid>();
            using (var db = new DB.DomenaDBContext())
            {
                var q = db.Buildings.Include(b => b.CostCollection);
                foreach (var a in db.Apartments.Where(x => q.Where(y => y.BuildingId.Equals(x.BuildingId)).FirstOrDefault().CostCollection.Count>0 ))
                {
                    var c = new Charge() { ApartmentId = a.ApartmentId, ChargeId = Guid.NewGuid(), IsClosed = false, CreatedTime = DateTime.Today };
                    c.Components = new List<ChargeComponent>();
                    foreach (var costCollection in db.Buildings.Include(b=>b.CostCollection).Where(x => x.BuildingId.Equals(a.BuildingId)).FirstOrDefault().CostCollection)
                    {
                        var cc = new ChargeComponent() {ChargeComponentId = Guid.NewGuid(), CostCategoryId = costCollection.CostCategoryId, CostDistribution = costCollection.CostDistribution, CostPerUnit = costCollection.CostPerUnit };
                        double units;
                        switch ((EnumCostDistribution.CostDistribution)cc.CostDistribution)
                        {
                            case EnumCostDistribution.CostDistribution.PerApartment:
                                units = 1;
                                break;
                            case EnumCostDistribution.CostDistribution.PerMeasurement:
                                units = a.AdditionalArea + a.ApartmentArea;
                                break;
                            default:
                                units = 0;
                                break;
                        }
                        cc.Sum = units * cc.CostPerUnit;
                        c.Components.Add(cc);
                        db.Charges.Add(c);
                        var cdg = new ChargeDataGrid(c);
                        Charges.Add(cdg);
                    }    
                }
                db.SaveChanges();
            }
        }

        public ICommand ShowChargeDetails
        {
            get { return new Helpers.RelayCommand(ShowDetails, CanShowDetails); }
        }

        private bool CanShowDetails()
        {
            return true;
        }

        private async void ShowDetails(object param)
        {
            Wizards.EditChargeWizard ecw;
            
                ecw = new Wizards.EditChargeWizard(SelectedCharge);
            

            var result = await DialogHost.Show(ecw, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
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
        
        public ICommand ClearFilterCommand
        {
            get
            {
                return new Helpers.RelayCommand(ClearFilter, CanClearFilter);
            }
        }

        public ChargesPage()
        {
            DataContext = this;
            InitializeCollection();
            InitializeCategories();
            InitializeLists();
            InitializeApartmentsNumbers();
            TestSelected = new List<ChargeDataGrid>();
            InitializeComponent();
            GroupByBuilding = true;
        }

        private void InitializeCollection()
        {
            Charges = new ObservableCollection<ChargeDataGrid>();
            using (var db = new DB.DomenaDBContext())
            {
                var qa = db.Charges.Include(c => c.Components).Where(x => x.IsClosed).FirstOrDefault();
                var q = db.Charges.Include(x => x.Components);
                foreach (var ch in q)
                {
                    var cdg = new ChargeDataGrid(ch);
                    Charges.Add(cdg); 
                }
            }
            
            ChargesCV = (CollectionView)CollectionViewSource.GetDefaultView(Charges);
            ChargesCV.SortDescriptions.Add(new SortDescription("CreatedTime", ListSortDirection.Ascending)); 
            ChargesCV.Filter = FilterCollection;

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
                Categories = new ObservableCollection<CostCategory>(db.CostCategories.ToList());
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

        private async void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {

        }

        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

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
