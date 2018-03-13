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

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for OwnersPage.xaml
    /// </summary>
    public partial class ChargesPage : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<ApartmentDataGrid> _apartments;
        public ObservableCollection<ApartmentDataGrid> Apartments
        {
            get { return _apartments; }
            set
            {
                _apartments = value;
                OnPropertyChanged("Apartments");
            }
        }

        private ApartmentDataGrid _selectedApartment;
        public ApartmentDataGrid SelectedApartment
        {
            get { return _selectedApartment; }
            set
            {
                _selectedApartment = value;
                OnPropertyChanged("SelectedApartment");                
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
                    ICollectionView cvApartments = (CollectionView)CollectionViewSource.GetDefaultView(Apartments);
                    if (value)
                    {
                        cvApartments.GroupDescriptions.Add(new PropertyGroupDescription("BuildingName"));
                    }
                    else
                    {
                        cvApartments.GroupDescriptions.Remove(cvApartments.GroupDescriptions.Cast<PropertyGroupDescription>().Where(x => x.PropertyName == "BuildingName").FirstOrDefault());
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
                    ICollectionView cvApartments = (CollectionView)CollectionViewSource.GetDefaultView(Apartments);
                    if (value)
                    {
                        cvApartments.GroupDescriptions.Add(new PropertyGroupDescription("ApartmentOwner"));
                    }
                    else
                    {
                        cvApartments.GroupDescriptions.Remove(cvApartments.GroupDescriptions.Cast<PropertyGroupDescription>().Where(x => x.PropertyName == "ApartmentOwner").FirstOrDefault());
                    }
                    _groupByApartment = value;
                    OnPropertyChanged("GroupByApartment");
                }
            }
        }

        public ICommand AddApartment
        {
            get { return new Helpers.RelayCommand(Add, CanAdd); }
        }

        private bool CanAdd()
        {
            return true;
        }

        private void Add(object param)
        {
            using (var db = new DB.DomenaDBContext())
            {
                foreach (var a in db.Apartments.Where(x => db.Buildings.Include(b => b.CostCollection).Where(y => y.BuildingId.Equals(x.BuildingId)).FirstOrDefault().CostCollection.Count>0 ))
                {
                    
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
                OnPropertyChanged("SelectedOwnerMailAddress");
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
                OnPropertyChanged("SelectedBuildingAddress");
            }
        }

        public ICommand FilterCommand
        {
            get
            {
                return new Helpers.RelayCommand(Filter, CanFilter);
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
            InitializeComponent();
            GroupByBuilding = true;
        }

        public void InitializeCollection()
        {
            Apartments = new ObservableCollection<ApartmentDataGrid>();
            using (var db = new DB.DomenaDBContext())
            {
                _buildingsNames = new ObservableCollection<Building>(db.Buildings.ToList());
                _ownersNames = new ObservableCollection<Owner>(db.Owners.ToList());

                var q = db.Apartments.Where(x => x.IsDeleted == false);
                foreach (var apar in q)
                {
                    var a = new ApartmentDataGrid
                    {
                        BuildingName = db.Buildings.Where(x => x.BuildingId == apar.BuildingId).FirstOrDefault().Name,
                        BulidingAddress = db.Buildings.Where(x => x.BuildingId == apar.BuildingId).FirstOrDefault().GetAddress(),
                        ApartmentId = apar.ApartmentId,
                        ApartmentNumber = apar.ApartmentNumber,
                        ApartmentArea = apar.ApartmentArea,
                        ApartmentAdditionalArea = apar.AdditionalArea,
                        ApartmentTotalArea = apar.ApartmentArea + apar.AdditionalArea,
                        ApartmentOwner = db.Owners.Where(x => x.OwnerId == apar.OwnerId).FirstOrDefault().OwnerName,
                        ApartmentPercentageDistribution = 
                        (100 * (apar.ApartmentArea + apar.AdditionalArea) / 
                        db.Apartments
                        .Where(x => x.BuildingId == apar.BuildingId && !x.IsDeleted)
                        .Select(x=>x.AdditionalArea + x.ApartmentArea)
                        .Sum()).ToString("0.00") + " %",
                        HasWaterMeter = apar.HasWaterMeter,
                        BoughtDate = apar.BoughtDate,
                        WaterMeterExp = apar.WaterMeterExp,
                        ApartmentOwnerAddress = db.Owners.Where(x => x.OwnerId == apar.OwnerId).FirstOrDefault().MailAddress,

                        
                    };                   
                    Apartments.Add(a);
                }

                foreach (var apartment in Apartments)
                {
                    apartment.Balance = 0;
                    //TODO
                }
            }
        }       
                
        private void Filter(object param)
        {
            using (var db = new DB.DomenaDBContext())
            {
                Apartments.Clear();
                IQueryable<Apartment> q = db.Apartments.Where(x => x.IsDeleted == false);
                if (SelectedBuildingName != null)
                {
                    q = q.Where(x => x.BuildingId == SelectedBuildingName.BuildingId);                    
                }

                if (SelectedOwnerName != null)
                {
                    q = q.Where(x => x.OwnerId == SelectedOwnerName.OwnerId);
                }


                foreach (var apar in q)
                {
                    var a = new ApartmentDataGrid
                    {
                        BuildingName = db.Buildings.Where(x => x.BuildingId == apar.BuildingId).FirstOrDefault().Name,
                        BulidingAddress = db.Buildings.Where(x => x.BuildingId == apar.BuildingId).FirstOrDefault().GetAddress(),
                        ApartmentId = apar.ApartmentId,
                        ApartmentNumber = apar.ApartmentNumber,
                        ApartmentArea = apar.ApartmentArea,
                        ApartmentAdditionalArea = apar.AdditionalArea,
                        ApartmentTotalArea = apar.ApartmentArea + apar.AdditionalArea,
                        ApartmentOwner = db.Owners.Where(x => x.OwnerId == apar.OwnerId).FirstOrDefault().OwnerName,
                        HasWaterMeter = apar.HasWaterMeter,
                        BoughtDate = apar.BoughtDate,
                        ApartmentOwnerAddress = db.Owners.Where(x => x.OwnerId == apar.OwnerId).FirstOrDefault().MailAddress,
                        WaterMeterExp = apar.WaterMeterExp,
                                                
                    };
                    Apartments.Add(a);
                }

                foreach (var apartment in Apartments)
                {
                    apartment.Balance = 0;
                    //TODO
                }
            }
        }

        private bool CanFilter()
        {
            return true;
        }

        private void ClearFilter(object param)
        {
            SelectedOwnerName = null;
            SelectedBuildingName = null;
            Filter(null);
        }

        private bool CanClearFilter()
        {
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
