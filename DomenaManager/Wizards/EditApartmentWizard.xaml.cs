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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using LibDataModel;
using MaterialDesignThemes.Wpf;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class EditApartmentWizard : UserControl, INotifyPropertyChanged
    {
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

        private int _apartmentNumber;
        public int ApartmentNumber
        {
            get { return _apartmentNumber; }
            set
            {
                _apartmentNumber = value;
                OnPropertyChanged("ApartmentNumber");
            }
        }

        private string _selectedBuildingAddress;
        public string SelectedBuildingAddress
        {
            get 
            {
                _selectedBuildingAddress = _selectedBuildingName != null ? _selectedBuildingName.GetAddress() : null;
                return _selectedBuildingAddress; 
            }
            set
            {
                _selectedBuildingAddress = value;
                OnPropertyChanged("SelectedBuildingAddress");
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
                _selectedOwnerMailAddress = _selectedOwnerName != null ? _selectedOwnerName.MailAddress : null;
                OnPropertyChanged("SelectedOwnerName");
                OnPropertyChanged("SelectedOwnerMailAddress");
            }
        }

        private string _selectedOwnerMailAddress;
        public string SelectedOwnerMailAddress
        {
            get 
            {                
                return _selectedOwnerMailAddress; 
            }
            set
            {
                _selectedOwnerMailAddress = value;
                OnPropertyChanged("SelectedOwnerMailAddress");
            }
        }

        private string _apartmentArea;
        public string ApartmentArea
        {
            get { return _apartmentArea; }
            set
            {
                if (_apartmentArea != value)
                {
                    _apartmentArea = value;
                    OnPropertyChanged("ApartmentArea");
                }
            }
        }

        private string _additionalArea;
        public string AdditionalArea
        {
            get { return _additionalArea; }
            set
            {
                if (_additionalArea != value)
                {
                    _additionalArea = value;
                    OnPropertyChanged("AdditionalArea");
                }                
            }
        }

        private int _hasWaterMeter;
        public int HasWaterMeter
        {
            get { return _hasWaterMeter; }
            set
            {
                _hasWaterMeter = value;
                OnPropertyChanged("HasWaterMeter");
                OnPropertyChanged("EnableWaterMeterExp");
            }
        }

        public string SelectedBuildingValue { get; set; }
        public string SelectedOwnerValue { get; set; }

        private DateTime _boughtDate;
        public DateTime BoughtDate
        {
            get { return _boughtDate; }
            set
            {
                _boughtDate = value;
                OnPropertyChanged("BoughtDate");
            }
        }

        private DateTime _waterMeterExp;
        public DateTime WaterMeterExp
        {
            get { return _waterMeterExp; }
            set
            {
                _waterMeterExp = value;
                OnPropertyChanged("WaterMeterExp");
            }
        }

        public bool EnableWaterMeterExp
        {
            get { return HasWaterMeter == 0; }            
        }

        public ICommand UpdateAllFieldsCommand
        {
            get
            {
                return new Helpers.RelayCommand(UpdateAllFields, CanUpdateAllFields);
            }
        }

        private void UpdateAllFields(object param)
        {
            Helpers.Validator.IsValid(this);
        }

        private bool CanUpdateAllFields()
        {
            return true;
        }

        public ICommand AddNewBuilding
        {
            get
            {
                return new Helpers.RelayCommand(AddBuilding, CanAddBuilding);
            }
        }

        private async void AddBuilding(object param)
        {
            var ebw = new EditBuildingWizard();
            var result = await DialogHost.Show(ebw, "HelperDialog", ExtendedEBWOpenedEventHandler, ExtendedEBWClosingEventHandler);
        }

        private bool CanAddBuilding()
        {
            return true;
        }

        private void ExtendedEBWOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private async void ExtendedEBWClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {

            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditBuildingWizard);
                //Accept
                if (dc._buildingLocalCopy == null)
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.BuildingName) || string.IsNullOrEmpty(dc.BuildingCity) || string.IsNullOrEmpty(dc.BuildingZipCode) || string.IsNullOrEmpty(dc.BuildingRoadName) || string.IsNullOrEmpty(dc.BuildingRoadNumber)))

                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Add new building

                    var newBuilding = new LibDataModel.Building { BuildingId = Guid.NewGuid(), Name = dc.BuildingName, City = dc.BuildingCity, ZipCode = dc.BuildingZipCode, BuildingNumber = dc.BuildingRoadNumber, RoadName = dc.BuildingRoadName, IsDeleted = false };
                    using (var db = new DB.DomenaDBContext())
                    {
                        db.Buildings.Add(newBuilding);
                        db.SaveChanges();
                    }

                    InitializeBuildingList();
                    SelectedBuildingName = BuildingsNames.Where(x => x.BuildingId.Equals(newBuilding.BuildingId)).FirstOrDefault();
                }
                else
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.BuildingName) || string.IsNullOrEmpty(dc.BuildingCity) || string.IsNullOrEmpty(dc.BuildingZipCode) || string.IsNullOrEmpty(dc.BuildingRoadName) || string.IsNullOrEmpty(dc.BuildingRoadNumber)))

                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Edit building
                    using (var db = new DB.DomenaDBContext())
                    {
                        var q = db.Buildings.Where(x => x.BuildingId.Equals(dc._buildingLocalCopy.BuildingId)).FirstOrDefault();
                        q.BuildingNumber = dc.BuildingRoadNumber;
                        q.City = dc.BuildingCity;
                        q.Name = dc.BuildingName;
                        q.RoadName = dc.BuildingRoadName;
                        q.ZipCode = dc.BuildingZipCode;
                        db.SaveChanges();
                    }

                    InitializeBuildingList();
                    SelectedBuildingName = BuildingsNames.Where(x => x.BuildingId.Equals(dc._buildingLocalCopy.BuildingId)).FirstOrDefault();
                }
            }
            else if (!(bool)eventArgs.Parameter)
            {

                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    //eventArgs.Cancel();
                    var dc = (eventArgs.Session.Content as Wizards.EditBuildingWizard);
                    var result = await DialogHost.Show(dc, "HelperDialog", ExtendedEBWOpenedEventHandler, ExtendedEBWClosingEventHandler);
                }
            }
        }

        public ICommand AddNewOwner
        {
            get
            {
                return new Helpers.RelayCommand(AddOwner, CanAddOwner);
            }
        }

        private async void AddOwner(object param)
        {
            var eow = new EditOwnerWizard();
            var result = await DialogHost.Show(eow, "HelperDialog", ExtendedEOWOpenedEventHandler, ExtendedEOWClosingEventHandler);
        }

        private bool CanAddOwner()
        {
            return true;
        }

        private void ExtendedEOWOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private async void ExtendedEOWClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditOwnerWizard);
                //Accept
                if (dc._ownerLocalCopy == null)
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.OwnerName) || string.IsNullOrEmpty(dc.MailAddress)))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Add new owner
                    var newOwner = new LibDataModel.Owner { OwnerId = Guid.NewGuid(), MailAddress = dc.MailAddress, OwnerName = dc.OwnerName, IsDeleted = false };
                    using (var db = new DB.DomenaDBContext())
                    {
                        db.Owners.Add(newOwner);
                        db.SaveChanges();
                    }
                    InitializeOwnerList();
                    SelectedOwnerName = OwnersNames.Where(x => x.OwnerId.Equals(newOwner.OwnerId)).FirstOrDefault();
                }
                else
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.OwnerName) || string.IsNullOrEmpty(dc.MailAddress)))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Edit Owner
                    using (var db = new DB.DomenaDBContext())
                    {
                        var q = db.Owners.Where(x => x.OwnerId.Equals(dc._ownerLocalCopy.OwnerId)).FirstOrDefault();
                        q.OwnerName = dc.OwnerName;
                        q.MailAddress = dc.MailAddress;
                        db.SaveChanges();
                    }
                    InitializeOwnerList();
                    SelectedOwnerName = OwnersNames.Where(x => x.OwnerId.Equals(dc._ownerLocalCopy.OwnerId)).FirstOrDefault();
                }
            }
            else if (!(bool)eventArgs.Parameter)
            {

                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    //eventArgs.Cancel();
                    var dc = (eventArgs.Session.Content as Wizards.EditOwnerWizard);
                    var result = await DialogHost.Show(dc, "HelperDialog", ExtendedEOWOpenedEventHandler, ExtendedEOWClosingEventHandler);
                }
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

        public Apartment _apartmentLocalCopy;

        public EditApartmentWizard(Apartment SelectedApartment = null)
        {
            if (SelectedApartment != null)
            {
                _apartmentLocalCopy = new Apartment(SelectedApartment);
            }
            InitializeBuildingList();
            InitializeOwnerList();
            InitializeFields();
            DataContext = this;            
            InitializeComponent();            
        }

        private void InitializeBuildingList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                BuildingsNames = new ObservableCollection<Building>(db.Buildings.Where(x => x.IsDeleted == false).ToList());
            }
        }

        private void InitializeOwnerList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                OwnersNames = new ObservableCollection<Owner>(db.Owners.Where(x => x.IsDeleted == false).ToList());
            }
        }

        private void InitializeFields()
        {
            if (_apartmentLocalCopy == null)
            {
                _boughtDate = DateTime.Today;
                _waterMeterExp = DateTime.Today;
                return;
            }

            _boughtDate = _apartmentLocalCopy.BoughtDate;
            _waterMeterExp = _apartmentLocalCopy.WaterMeterExp;
            _additionalArea = _apartmentLocalCopy.AdditionalArea.ToString();
            _apartmentArea = _apartmentLocalCopy.ApartmentArea.ToString();
            _apartmentNumber = _apartmentLocalCopy.ApartmentNumber;
            _hasWaterMeter = _apartmentLocalCopy.HasWaterMeter ? 0 : 1;
            _selectedBuildingName = _buildingsNames.Where(x => x.BuildingId.Equals(_apartmentLocalCopy.BuildingId)).FirstOrDefault();
            _selectedOwnerName = _ownersNames.Where(x => x.OwnerId.Equals(_apartmentLocalCopy.OwnerId)).FirstOrDefault();
            _selectedOwnerMailAddress = _apartmentLocalCopy.CorrespondenceAddress != null ? _apartmentLocalCopy.CorrespondenceAddress : (_selectedOwnerName != null ? _selectedOwnerName.MailAddress : null);
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
