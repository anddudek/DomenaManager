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
using System.Data.Objects.SqlClient;

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
                //if (value != null)
                    //OpenDrawer();   
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

        public ICommand ExpandApartmentCommand
        {
            get
            {
                return new RelayCommand(ExpandApartment, CanExpandApartment);
            }
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

        public ICommand ClearFilterCommand
        {
            get
            {
                return new Helpers.RelayCommand(ClearFilter, CanClearFilter);
            }
        }

        public ICommand ShowChargesCommand
        {
            get
            {
                return new RelayCommand(ShowCharges, CanShowCharges);
            }
        }

        public ICommand ShowPaymentsCommand
        {
            get
            {
                return new RelayCommand(ShowPayments, CanShowPayments);
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
                var q = db.Apartments.Where(x => x.IsDeleted == false);
                InitializeApartments(q);
            }
        }

        private void InitializeApartments(IQueryable<Apartment> q)
        {
            using (var db = new DB.DomenaDBContext())
            {
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
                        .Select(x => x.AdditionalArea + x.ApartmentArea)
                        .Sum()).ToString("0.00") + " %",
                        BoughtDate = apar.BoughtDate,
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
                        },


                    };
                    a.Balance = Payments.CalculateSaldo(DateTime.Today.Year, db.Apartments.FirstOrDefault(x => x.ApartmentId.Equals(a.ApartmentId)));
                    a.CostHistory = new ObservableCollection<string>(db.Charges.Include(x => x.Components).Where(x => x.ApartmentId.Equals(a.ApartmentId) && !x.IsDeleted).OrderByDescending(x => x.ChargeDate).Take(5).ToList().Select(x => new { costConc = x.ChargeDate.ToString("dd-MM-yyyy") + " : " + x.Components.Select(y => y.Sum).DefaultIfEmpty(0).Sum() + " zł" }).Select(x => x.costConc).ToList());
                    a.PaymentHistory = new ObservableCollection<string>(db.Payments.Where(x => !x.IsDeleted && x.ApartmentId.Equals(a.ApartmentId)).OrderByDescending(x => x.PaymentRegistrationDate).Take(5).ToList().Select(x => new { paymConc = x.PaymentRegistrationDate.ToString("dd-MM-yyyy") + " : " + x.PaymentAmount + " zł" }).Select(x => x.paymConc).ToList());
                    Apartments.Add(a);
                }
            }
        }

        private void OpenDrawer()
        {
            DrawerHost.OpenDrawerCommand.Execute(Dock.Bottom, this.DH);
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

        private void ExpandApartment(object param)
        {
            OpenDrawer();
        }

        private bool CanExpandApartment()
        {
            return SelectedApartment != null;
        }

        private void ShowCharges(object param)
        {
            var mw = (((((this.Parent as MahApps.Metro.Controls.TransitioningContentControl).Parent as Grid).Parent as DialogHost).Parent as DialogHost).Parent as DialogHost).Parent as Windows.MainWindow;
            using (var db = new DB.DomenaDBContext())
            {
                mw.CurrentPage = new ChargesPage(db.Apartments.FirstOrDefault(x => x.ApartmentId.Equals(SelectedApartment.ApartmentId)));
            }
        }

        private bool CanShowCharges()
        {
            return SelectedApartment != null;
        }

        private void ShowPayments(object param)
        {
            var mw = (((((this.Parent as MahApps.Metro.Controls.TransitioningContentControl).Parent as Grid).Parent as DialogHost).Parent as DialogHost).Parent as DialogHost).Parent as Windows.MainWindow;
            using (var db = new DB.DomenaDBContext())
            {
                mw.CurrentPage = new PaymentsPage(db.Apartments.FirstOrDefault(x => x.ApartmentId.Equals(SelectedApartment.ApartmentId)));
            }
        }

        private bool CanShowPayments()
        {
            return SelectedApartment != null;
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
                
                InitializeApartments(q);
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
                var sa = db.Apartments.Include(x => x.MeterCollection.Select(y => y.MeterTypeParent)).Where(x => x.ApartmentId.Equals(SelectedApartment.ApartmentId)).FirstOrDefault();
                var b = db.Buildings.Include(x => x.MeterCollection).FirstOrDefault(x => x.BuildingId.Equals(sa.BuildingId));
                foreach (var m in b.MeterCollection)
                {
                    if (!sa.MeterCollection.Any(x => x.MeterTypeParent.MeterId.Equals(m.MeterId)))
                    {
                        sa.MeterCollection.Add(new ApartmentMeter() { MeterId = Guid.NewGuid(), MeterTypeParent = m, IsDeleted = false, LastMeasure = 0, LegalizationDate = DateTime.Today.AddDays(-1) });
                    }
                    else
                    {
                        sa.MeterCollection.FirstOrDefault(x => x.MeterTypeParent.MeterId.Equals(m.MeterId)).IsDeleted = false;
                    }
                }
                foreach (var m in sa.MeterCollection)
                {
                    if (!b.MeterCollection.Any(x => x.MeterId.Equals(m.MeterTypeParent.MeterId)))
                    {
                        m.IsDeleted = true;
                    }
                }
                eaw = new Wizards.EditApartmentWizard(sa);
            }

            var result = await DialogHost.Show(eaw, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
        }

        private bool CanEditApartment()
        {
            return SelectedApartment != null;
        }

        public async void DeleteApartment(object param)
        {
            bool ynResult = await Helpers.YNMsg.Show("Czy chcesz usunąć lokal " + SelectedApartment.BuildingName + " " + SelectedApartment.ApartmentNumber + "?");
            if (ynResult)
            {
                using (var db = new DB.DomenaDBContext())
                {
                    var sa = db.Apartments.Where(x => x.ApartmentId.Equals(SelectedApartment.ApartmentId)).FirstOrDefault();
                    sa.IsDeleted = true;
                    db.SaveChanges();
                }
            }
            InitializeCollection();
            
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
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.SelectedBuildingAddress) || string.IsNullOrEmpty(dc.SelectedOwnerMailAddress) || dc.ApartmentNumber <= 0 || double.Parse(dc.AdditionalArea) < 0 || double.Parse(dc.ApartmentArea) <= 0))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Add new apartment
                    using (var db = new DB.DomenaDBContext())
                    {
                        var newApartment = new LibDataModel.Apartment { BoughtDate = dc.BoughtDate.Date, ApartmentId = Guid.NewGuid(), BuildingId = dc.SelectedBuildingName.BuildingId, AdditionalArea = double.Parse(dc.AdditionalArea), ApartmentArea = double.Parse(dc.ApartmentArea), IsDeleted=false, OwnerId = dc.SelectedOwnerName.OwnerId, CreatedDate = DateTime.Now, ApartmentNumber = dc.ApartmentNumber, MeterCollection = new List<ApartmentMeter>() };
                        if (!dc.SelectedOwnerMailAddress.Equals(db.Owners.Where(x => x.OwnerId == dc._apartmentLocalCopy.OwnerId).Select(x => x.MailAddress)))
                        {
                            newApartment.CorrespondenceAddress = dc.SelectedOwnerMailAddress;
                        }
                        else
                        {
                            newApartment.CorrespondenceAddress = null;
                        }
                        var q = dc.MeterCollection.Where(x => !x.IsDeleted);
                        foreach (var m in q)
                        {
                            newApartment.MeterCollection.Add(new ApartmentMeter() { IsDeleted = false, LastMeasure = m.LastMeasure, MeterTypeParent = m.MeterTypeParent, LegalizationDate = m.LegalizationDate, MeterId = m.MeterId });
                        }
                        db.Apartments.Add(newApartment);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.SelectedBuildingAddress) || string.IsNullOrEmpty(dc.SelectedOwnerMailAddress) || dc.ApartmentNumber <= 0 || double.Parse(dc.AdditionalArea) < 0 || double.Parse(dc.ApartmentArea) <= 0))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Edit Apartment
                    using (var db = new DB.DomenaDBContext())
                    {
                        var q = db.Apartments.Include(x => x.MeterCollection).Where(x => x.ApartmentId.Equals(dc._apartmentLocalCopy.ApartmentId)).FirstOrDefault();
                        q.BoughtDate = dc.BoughtDate.Date;
                        q.AdditionalArea = double.Parse(dc.AdditionalArea);
                        q.ApartmentArea = double.Parse(dc.ApartmentArea);
                        q.ApartmentNumber = dc.ApartmentNumber;
                        q.BuildingId = dc.SelectedBuildingName.BuildingId;
                        q.CreatedDate = DateTime.Now;
                        //q.HasWaterMeter = dc.HasWaterMeter == 0;
                        //q.WaterMeterExp = dc.WaterMeterExp.Date;
                        q.OwnerId = dc.SelectedOwnerName.OwnerId;

                        if (!dc.SelectedOwnerMailAddress.Equals(db.Owners.Where(x => x.OwnerId == dc._apartmentLocalCopy.OwnerId).Select(x => x.MailAddress)))
                        {
                            q.CorrespondenceAddress = dc.SelectedOwnerMailAddress;
                        }
                        else
                        {
                            q.CorrespondenceAddress = null;
                        }

                        var meters = dc.MeterCollection.Where(x => !x.IsDeleted);
                        foreach (var m in meters)
                        {
                            if (!q.MeterCollection.Any(x => x.MeterId.Equals(m.MeterId)))
                            {
                                q.MeterCollection.Add(m);
                                //var a = db.Buildings.SelectMany(x => x.MeterCollection).FirstOrDefault(x => x.MeterId.Equals(m.MeterTypeParent.MeterId));
                                db.Entry(m.MeterTypeParent).State = EntityState.Unchanged;
                            }
                            else
                            {
                                var s = q.MeterCollection.FirstOrDefault(x => x.MeterId.Equals(m.MeterId));
                                s.LegalizationDate = m.LegalizationDate;
                                s.LastMeasure = m.LastMeasure;
                            }
                        }
                        foreach (var m in q.MeterCollection)
                        {
                            if (!meters.Any(x => x.MeterId.Equals(m.MeterId)))
                            {
                                m.IsDeleted = true;
                            }
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
