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
    public partial class ApartmentsPage : UserControl, INotifyPropertyChanged
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
                if (value != null)
                    OpenDrawer();   
            }
        }

        async Task PutTaskDelay()
        {
            await Task.Delay(300);
        }

        private async void OpenDrawer()
        {
            // Wait till all events in bg that can takes focus away from drawer (thus cancel it) finishes
            await PutTaskDelay();
            DrawerHost.OpenDrawerCommand.Execute(Dock.Bottom, this.DH);
            
        }
        
        public ICommand AddApartmentCommand
        {
            get
            {
                return new Helpers.RelayCommand(AddApartment, CanAddApartment);
            }
        }

        public ICommand EditApartmentCommand
        {
            get
            {
                return new Helpers.RelayCommand(EditApartment, CanEditApartment);
            }
        }

        public ICommand DeleteApartmentCommand
        {
            get
            {
                return new Helpers.RelayCommand(DeleteApartment, CanDeleteApartment);
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

        private bool _groupByOwner;
        public bool GroupByOwner
        {
            get { return _groupByOwner; }
            set
            {
                if (value != _groupByOwner)
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
                    _groupByOwner = value;
                    OnPropertyChanged("GroupByOwner");
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

        public ApartmentsPage()
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

                        ApartmentAreaSeries = new SeriesCollection
                        {
                            new PieSeries
                            {
                                Title = "Powierzchnia mieszkania (m2)", Values = new ChartValues<ObservableValue> {new ObservableValue(apar.ApartmentArea)},
                                LabelPoint=chartPoint => string.Format("{0} m2 ({1:P})", chartPoint.Y, chartPoint.Participation)
                            },

                            new PieSeries
                            {
                                Title = "Powierzchnie przynależne (m2)", Values = new ChartValues<ObservableValue> {new ObservableValue(apar.AdditionalArea)},
                                LabelPoint=chartPoint => string.Format("{0} m2 ({1:P})", chartPoint.Y, chartPoint.Participation)
                            }
                        },

                        BuildingAreaSeries = new SeriesCollection
                        {
                            new PieSeries
                            {
                                Title = "Całkowita powierzchnia (m2)", Values = new ChartValues<ObservableValue> {new ObservableValue(apar.ApartmentArea + apar.AdditionalArea) },
                                LabelPoint=chartPoint => string.Format("{0} m2 ({1:P})", chartPoint.Y, chartPoint.Participation)
                            },

                            new PieSeries
                            {
                                Title = "Reszta budynku (m2)", Values = new ChartValues<ObservableValue>
                                {
                                    new ObservableValue(
                                    db.Apartments.Where(x => x.BuildingId==apar.BuildingId && x.ApartmentId != apar.ApartmentId && x.IsDeleted==false).Select(x => x.ApartmentArea).DefaultIfEmpty(0).Sum() +
                                    db.Apartments.Where(x => x.BuildingId==apar.BuildingId && x.ApartmentId != apar.ApartmentId && x.IsDeleted==false).Select(x => x.AdditionalArea).DefaultIfEmpty(0).Sum()
                                    )
                                },
                                LabelPoint=chartPoint => string.Format("{0} m2 ({1:P})", chartPoint.Y, chartPoint.Participation)
                            }
                        }
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

        private async void AddApartment(object param)
        {
            Wizards.EditApartmentWizard eaw = new Wizards.EditApartmentWizard();

            var result = await DialogHost.Show(eaw, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
        }

        private bool CanAddApartment()
        {
            return true;
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

                        ApartmentAreaSeries = new SeriesCollection
                        {
                            new PieSeries
                            {
                                Title = "Powierzchnia mieszkania (m2)", Values = new ChartValues<ObservableValue> {new ObservableValue(apar.ApartmentArea)},
                                LabelPoint=chartPoint => string.Format("{0} m2 ({1:P})", chartPoint.Y, chartPoint.Participation)
                            },

                            new PieSeries
                            {
                                Title = "Powierzchnie przynależne (m2)", Values = new ChartValues<ObservableValue> {new ObservableValue(apar.AdditionalArea)},
                                LabelPoint=chartPoint => string.Format("{0} m2 ({1:P})", chartPoint.Y, chartPoint.Participation)
                            }
                        },

                        BuildingAreaSeries = new SeriesCollection
                        {
                            new PieSeries
                            {
                                Title = "Całkowita powierzchnia (m2)", Values = new ChartValues<ObservableValue> {new ObservableValue(apar.ApartmentArea + apar.AdditionalArea)},
                                LabelPoint=chartPoint => string.Format("{0} m2 ({1:P})", chartPoint.Y, chartPoint.Participation)
                            },

                            new PieSeries
                            {
                                Title = "Reszta budynku (m2)", Values = new ChartValues<ObservableValue>
                                {
                                    new ObservableValue(
                                    db.Apartments.Where(x => x.BuildingId==apar.BuildingId && x.ApartmentId != apar.ApartmentId && x.IsDeleted==false).Select(x => x.ApartmentArea).DefaultIfEmpty(0).Sum() +
                                    db.Apartments.Where(x => x.BuildingId==apar.BuildingId && x.ApartmentId != apar.ApartmentId && x.IsDeleted==false).Select(x => x.AdditionalArea).DefaultIfEmpty(0).Sum()
                                    )
                                },
                                LabelPoint=chartPoint => string.Format("{0} m2 ({1:P})", chartPoint.Y, chartPoint.Participation)
                            }
                        }
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

        private async void EditApartment(object param)
        {
            Wizards.EditApartmentWizard eaw;
            using (var db = new DB.DomenaDBContext())
            {
                var sa = db.Apartments.Where(x => x.ApartmentId.Equals(SelectedApartment.ApartmentId)).FirstOrDefault();
                eaw = new Wizards.EditApartmentWizard(sa);
            }

            var result = await DialogHost.Show(eaw, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
        }

        private bool CanEditApartment()
        {
            return SelectedApartment != null;
        }

        public void DeleteApartment(object param)
        {
            using (var db = new DB.DomenaDBContext())
            {
                var sa = db.Apartments.Where(x => x.ApartmentId.Equals(SelectedApartment.ApartmentId)).FirstOrDefault();
                sa.IsDeleted = true;
                db.SaveChanges();
            }
        }

        private bool CanDeleteApartment()
        {
            return SelectedApartment != null;
        }

        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private async void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            
            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditApartmentWizard);
                //Accept
                if (dc._apartmentLocalCopy == null)
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.SelectedBuildingAddress) || string.IsNullOrEmpty(dc.SelectedOwnerMailAddress) || dc.ApartmentNumber <= 0 || double.Parse(dc.AdditionalArea) <= 0 || double.Parse(dc.ApartmentArea) <= 0))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Add new apartment
                    using (var db = new DB.DomenaDBContext())
                    {
                        var newApartment = new LibDataModel.Apartment { BoughtDate = dc.BoughtDate, ApartmentId = Guid.NewGuid(), BuildingId = dc.SelectedBuildingName.BuildingId, AdditionalArea = double.Parse(dc.AdditionalArea), ApartmentArea = double.Parse(dc.ApartmentArea), HasWaterMeter = dc.HasWaterMeter == 1, IsDeleted=false, OwnerId = dc.SelectedOwnerName.OwnerId, CreatedDate = DateTime.Now, ApartmentNumber = dc.ApartmentNumber };
                        if (!dc.SelectedOwnerMailAddress.Equals(db.Owners.Where(x => x.OwnerId == dc._apartmentLocalCopy.OwnerId).Select(x => x.MailAddress)))
                        {
                            newApartment.CorrespondenceAddress = dc.SelectedOwnerMailAddress;
                        }
                        else
                        {
                            newApartment.CorrespondenceAddress = null;
                        }
                        db.Apartments.Add(newApartment);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.SelectedBuildingAddress) || string.IsNullOrEmpty(dc.SelectedOwnerMailAddress) || dc.ApartmentNumber <= 0 || double.Parse(dc.AdditionalArea) <= 0 || double.Parse(dc.ApartmentArea) <= 0))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Edit Apartment
                    using (var db = new DB.DomenaDBContext())
                    {
                        var q = db.Apartments.Where(x => x.ApartmentId.Equals(dc._apartmentLocalCopy.ApartmentId)).FirstOrDefault();
                        q.BoughtDate = dc.BoughtDate;
                        q.AdditionalArea = double.Parse(dc.AdditionalArea);
                        q.ApartmentArea = double.Parse(dc.ApartmentArea);
                        q.ApartmentNumber = dc.ApartmentNumber;
                        q.BuildingId = dc.SelectedBuildingName.BuildingId;
                        q.CreatedDate = DateTime.Now;
                        q.HasWaterMeter = dc.HasWaterMeter == 0;
                        q.OwnerId = dc.SelectedOwnerName.OwnerId;

                        if (!dc.SelectedOwnerMailAddress.Equals(db.Owners.Where(x => x.OwnerId == dc._apartmentLocalCopy.OwnerId).Select(x => x.MailAddress)))
                        {
                            q.CorrespondenceAddress = dc.SelectedOwnerMailAddress;
                        }
                        else
                        {
                            q.CorrespondenceAddress = null;
                        }

                        db.SaveChanges();
                    }
                }
            }
            else if (!(bool)eventArgs.Parameter)
            {

                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    //eventArgs.Cancel();
                    var dc = (eventArgs.Session.Content as Wizards.EditApartmentWizard);
                    var result = await DialogHost.Show(dc, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
                }
            }
            InitializeCollection();
            DrawerHost.CloseDrawerCommand.Execute(Dock.Bottom, this.DH);            
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
