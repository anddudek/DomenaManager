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
using System.Data.Entity;
using LibDataModel;
using MaterialDesignThemes.Wpf;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class EditApartmentWizard : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<Apartment> ApartmentsCollection;

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
                if (value != _selectedBuildingName)
                {
                    _selectedBuildingName = value;
                    OnPropertyChanged("SelectedBuildingName");
                    OnPropertyChanged("SelectedBuildingAddress");
                    ApartmentNumber = 0;
                    InitializeMeterCollection();
                }
            }
        }

        private int _locatorsAmount;
        public int LocatorsAmount
        {
            get { return _locatorsAmount; }
            set
            {
                if (value != _locatorsAmount)
                {
                    _locatorsAmount = value;
                    OnPropertyChanged("LocatorsAmount");
                }
            }
        }

        private int _apartmentNumber;
        public int ApartmentNumber
        {
            get { return _apartmentNumber; }
            set
            {
                if (value != _apartmentNumber)
                {
                    _apartmentNumber = value;
                    OnPropertyChanged("ApartmentNumber");                    
                }
                if (_apartmentLocalCopy != null)
                {
                    var q = ApartmentsCollection.Where(x => x.BuildingId.Equals(SelectedBuildingName.BuildingId) && !x.IsDeleted && x.SoldDate == null && !x.ApartmentId.Equals(_apartmentLocalCopy.ApartmentId)).Select(x => x.ApartmentNumber).ToList();
                    if (q.Any(x => x == value))
                    {
                        ValidationError verr = new ValidationError(new Helpers.StringToIntValidationRule(), apartmentNumberTB.GetBindingExpression(TextBox.TextProperty));
                        verr.ErrorContent = "Numer już istnieje";
                        Validation.MarkInvalid(apartmentNumberTB.GetBindingExpression(TextBox.TextProperty), verr);
                    }
                }
                else
                {
                    var q = ApartmentsCollection.Where(x => x.BuildingId.Equals(SelectedBuildingName.BuildingId) && !x.IsDeleted && x.SoldDate == null).Select(x => x.ApartmentNumber).ToList();
                    if (q.Any(x => x == value))
                    {
                        ValidationError verr = new ValidationError(new Helpers.StringToIntValidationRule(), apartmentNumberTB.GetBindingExpression(TextBox.TextProperty));
                        verr.ErrorContent = "Numer już istnieje";
                        Validation.MarkInvalid(apartmentNumberTB.GetBindingExpression(TextBox.TextProperty), verr);
                    }
                }
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

        private ObservableCollection<ApartmentMeter> _meterCollection;
        public ObservableCollection<ApartmentMeter> MeterCollection
        {
            get { return _meterCollection; }
            set
            {
                if (value != _meterCollection)
                {
                    _meterCollection = value;
                    OnPropertyChanged("MeterCollection");
                }
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

        public ICommand AddNewOwner
        {
            get
            {
                return new Helpers.RelayCommand(AddOwner, CanAddOwner);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new Helpers.RelayCommand(SaveDialog, CanSaveDialog);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new Helpers.RelayCommand(CancelDialog, CanCancelDialog);
            }
        }

        public ICommand AcceptCommand
        {
            get
            {
                return new Helpers.RelayCommand(AcceptDialog, CanAcceptDialog);
            }
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
            InitializeMeterCollection();
            DataContext = this;            
            InitializeComponent();            
        }

        private void InitializeMeterCollection()
        {
            if (_apartmentLocalCopy != null && SelectedBuildingName != null && _apartmentLocalCopy.BuildingId.Equals(SelectedBuildingName.BuildingId))
            {
                MeterCollection = new ObservableCollection<ApartmentMeter>();
                foreach (var m in _apartmentLocalCopy.MeterCollection)
                {
                    MeterCollection.Add(new ApartmentMeter() { IsDeleted = m.IsDeleted, MeterTypeParent = m.MeterTypeParent, LastMeasure = m.LastMeasure, LegalizationDate = m.LegalizationDate, MeterId = m.MeterId });
                }
            }
            else if (SelectedBuildingName != null)
            {
                for (int i = SelectedBuildingName.MeterCollection.Count - 1; i >= 0; i--)
                {
                    if (!MeterCollection.Any(x => x.MeterTypeParent.MeterId.Equals(SelectedBuildingName.MeterCollection[i].MeterId)) && SelectedBuildingName.MeterCollection[i].IsApartment)
                    {
                        MeterCollection.Add(new ApartmentMeter() { MeterId = Guid.NewGuid(), MeterTypeParent = SelectedBuildingName.MeterCollection[i], IsDeleted = false, LastMeasure = 0, LegalizationDate = DateTime.Today.AddDays(-1) });
                    }
                    else if (SelectedBuildingName.MeterCollection[i].IsApartment)
                    {
                        MeterCollection.FirstOrDefault(x => x.MeterTypeParent.MeterId.Equals(SelectedBuildingName.MeterCollection[i].MeterId)).IsDeleted = false;
                    }
                }
                for (int i = MeterCollection.Count - 1; i >= 0; i--)
                {
                    if (!SelectedBuildingName.MeterCollection.Any(x => x.MeterId.Equals(MeterCollection[i].MeterTypeParent.MeterId)))
                    {
                        MeterCollection.RemoveAt(i);
                    }
                }
            }
            else
            {
                MeterCollection = new ObservableCollection<ApartmentMeter>();
            }
        }

        private void InitializeBuildingList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                BuildingsNames = new ObservableCollection<Building>(db.Buildings.Include(x => x.MeterCollection).Where(x => x.IsDeleted == false).ToList());
                ApartmentsCollection = new ObservableCollection<Apartment>(db.Apartments.Include(x => x.MeterCollection).ToList());
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
            _additionalArea = _apartmentLocalCopy.AdditionalArea.ToString();
            _apartmentArea = _apartmentLocalCopy.ApartmentArea.ToString();
            _apartmentNumber = _apartmentLocalCopy.ApartmentNumber;
            _selectedBuildingName = _buildingsNames.Where(x => x.BuildingId.Equals(_apartmentLocalCopy.BuildingId)).FirstOrDefault();
            _selectedOwnerName = _ownersNames.Where(x => x.OwnerId.Equals(_apartmentLocalCopy.OwnerId)).FirstOrDefault();
            _selectedOwnerMailAddress = _apartmentLocalCopy.CorrespondenceAddress != null ? _apartmentLocalCopy.CorrespondenceAddress : (_selectedOwnerName != null ? _selectedOwnerName.MailAddress : null);
            _locatorsAmount = _apartmentLocalCopy.Locators;
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

        private void UpdateAllFields(object param)
        {
            Helpers.Validator.IsValid(this);
        }

        private bool CanUpdateAllFields()
        {
            return true;
        }

        private void SaveDialog(object param)
        {
            if (_apartmentLocalCopy == null)
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(SelectedBuildingAddress) || string.IsNullOrEmpty(SelectedOwnerMailAddress) || ApartmentNumber <= 0 || double.Parse(AdditionalArea) < 0 || double.Parse(ApartmentArea) <= 0))
                {
                    return;
                }
                //Add new apartment
                using (var db = new DB.DomenaDBContext())
                {
                    var newApartment = new LibDataModel.Apartment { BoughtDate = BoughtDate.Date, ApartmentId = Guid.NewGuid(), BuildingId = SelectedBuildingName.BuildingId, AdditionalArea = double.Parse(AdditionalArea), ApartmentArea = double.Parse(ApartmentArea), IsDeleted = false, SoldDate = null, OwnerId = SelectedOwnerName.OwnerId, CreatedDate = DateTime.Now, ApartmentNumber = ApartmentNumber, MeterCollection = new List<ApartmentMeter>(), Locators = LocatorsAmount };
                    if (!SelectedOwnerMailAddress.Equals(db.Owners.Where(x => x.OwnerId == _apartmentLocalCopy.OwnerId).Select(x => x.MailAddress)))
                    {
                        newApartment.CorrespondenceAddress = SelectedOwnerMailAddress;
                    }
                    else
                    {
                        newApartment.CorrespondenceAddress = null;
                    }
                    var q = MeterCollection.Where(x => !x.IsDeleted);
                    foreach (var m in q)
                    {
                        newApartment.MeterCollection.Add(new ApartmentMeter() { IsDeleted = false, LastMeasure = m.LastMeasure, MeterTypeParent = m.MeterTypeParent, LegalizationDate = m.LegalizationDate, MeterId = m.MeterId });
                        db.Entry(m.MeterTypeParent).State = EntityState.Unchanged;
                    }
                    db.Apartments.Add(newApartment);

                    // Add initial charge
                    var building = db.Buildings.Include(x => x.CostCollection).FirstOrDefault(x => x.BuildingId == newApartment.BuildingId);
                    double percentage = 1 - (BoughtDate.Day / (double)DateTime.DaysInMonth(BoughtDate.Year, BoughtDate.Month));
                    var chargeDate = BoughtDate;

                    while (chargeDate <= DateTime.Today)
                    {
                        var c = new Charge() { ApartmentId = newApartment.ApartmentId, ChargeId = Guid.NewGuid(), IsClosed = false, ChargeDate = chargeDate, CreatedDate = DateTime.Today, SettlementId = Guid.Empty, AutoChargeId = Guid.Empty, OwnerId = newApartment.OwnerId };
                        c.Components = new List<ChargeComponent>();
                        foreach (var costCollection in building.CostCollection)
                        {
                            if (costCollection.BegginingDate > chargeDate || (costCollection.EndingDate.Year > 1901 && costCollection.EndingDate < chargeDate))
                            {
                                continue;
                            }
                            var group = db.GroupName.FirstOrDefault(x => x.BuildingChargeGroupNameId == costCollection.BuildingChargeGroupNameId);
                            var cc = new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = costCollection.BuildingChargeBasisCategoryId, CostDistribution = costCollection.BuildingChargeBasisDistribution, CostPerUnit = costCollection.CostPerUnit, GroupName = group };
                            double units;
                            switch ((EnumCostDistribution.CostDistribution)cc.CostDistribution)
                            {
                                case EnumCostDistribution.CostDistribution.PerApartment:
                                    units = 1;
                                    break;
                                case EnumCostDistribution.CostDistribution.PerApartmentTotalArea:
                                    units = newApartment.AdditionalArea + newApartment.ApartmentArea;
                                    break;
                                case EnumCostDistribution.CostDistribution.PerApartmentArea:
                                    units = newApartment.ApartmentArea;
                                    break;
                                case EnumCostDistribution.CostDistribution.PerAdditionalArea:
                                    units = newApartment.AdditionalArea;
                                    break;
                                case EnumCostDistribution.CostDistribution.PerLocators:
                                    units = newApartment.Locators;
                                    break;
                                default:
                                    units = 0;
                                    break;
                            }
                            cc.Sum = Math.Round(((units * cc.CostPerUnit) * percentage), 2);
                            c.Components.Add(cc);
                            db.Entry(cc.GroupName).State = EntityState.Unchanged;
                        }
                        if (c.Components != null && c.Components.Count > 0)
                        {
                            db.Charges.Add(c);
                        }
                        chargeDate = (new DateTime(chargeDate.Year, chargeDate.Month, 1)).AddMonths(1);
                        percentage = 1;
                    }
                    db.SaveChanges();
                    _apartmentLocalCopy = newApartment;
                }
            }
            else
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(SelectedBuildingAddress) || string.IsNullOrEmpty(SelectedOwnerMailAddress) || ApartmentNumber <= 0 || double.Parse(AdditionalArea) < 0 || double.Parse(ApartmentArea) <= 0))
                {
                    return;
                }
                //Edit Apartment
                using (var db = new DB.DomenaDBContext())
                {
                    var q = db.Apartments.Include(x => x.MeterCollection).Where(x => x.ApartmentId.Equals(_apartmentLocalCopy.ApartmentId)).FirstOrDefault();
                    q.BoughtDate = BoughtDate.Date;
                    q.AdditionalArea = double.Parse(AdditionalArea);
                    q.ApartmentArea = double.Parse(ApartmentArea);
                    q.ApartmentNumber = ApartmentNumber;
                    q.BuildingId = SelectedBuildingName.BuildingId;
                    q.CreatedDate = DateTime.Now;
                    //q.HasWaterMeter = dc.HasWaterMeter == 0;
                    //q.WaterMeterExp = dc.WaterMeterExp.Date;
                    q.OwnerId = SelectedOwnerName.OwnerId;
                    q.Locators = LocatorsAmount;

                    if (!SelectedOwnerMailAddress.Equals(db.Owners.Where(x => x.OwnerId == _apartmentLocalCopy.OwnerId).Select(x => x.MailAddress)))
                    {
                        q.CorrespondenceAddress = SelectedOwnerMailAddress;
                    }
                    else
                    {
                        q.CorrespondenceAddress = null;
                    }

                    var meters = MeterCollection.Where(x => !x.IsDeleted);
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
                            db.MetersHistories.Add(new MetersHistory
                            {
                                MeterHistoryId = Guid.NewGuid(),
                                Apartment = q,
                                Building = db.Buildings.FirstOrDefault(x => x.BuildingId == q.BuildingId),
                                ApartmentMeter = s,
                                MeterType = s.MeterTypeParent,
                                ModifiedDate = DateTime.Now,
                                NewValue = m.LastMeasure,
                                OldValue = s.LastMeasure
                            });
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

        private bool CanSaveDialog()
        {
            return true;
        }

        private void CancelDialog(object param)
        {
            Helpers.SwitchPage.SwitchMainPage(new Pages.ApartmentsPage(), this);
        }

        private bool CanCancelDialog()
        {
            return true;
        }

        private bool CanAcceptDialog()
        {
            return true;
        }

        private void AcceptDialog(object param)
        {
            if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(SelectedBuildingAddress) || string.IsNullOrEmpty(SelectedOwnerMailAddress) || ApartmentNumber <= 0 || double.Parse(AdditionalArea) < 0 || double.Parse(ApartmentArea) <= 0))
            {
                return;
            }
            SaveDialog(null);
            Helpers.SwitchPage.SwitchMainPage(new Pages.ApartmentsPage(), this);
        }

        private void ExtendedEBWOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

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
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.OwnerFirstName) || string.IsNullOrEmpty(dc.OwnerSurname) || string.IsNullOrEmpty(dc.MailAddress)))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Add new owner
                    var newOwner = new LibDataModel.Owner { OwnerId = Guid.NewGuid(), MailAddress = dc.MailAddress, OwnerFirstName = dc.OwnerFirstName, OwnerSurname=dc.OwnerSurname, IsDeleted = false };
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
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.OwnerFirstName) || string.IsNullOrEmpty(dc.OwnerSurname) || string.IsNullOrEmpty(dc.MailAddress)))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Edit Owner
                    using (var db = new DB.DomenaDBContext())
                    {
                        var q = db.Owners.Where(x => x.OwnerId.Equals(dc._ownerLocalCopy.OwnerId)).FirstOrDefault();
                        q.OwnerFirstName = dc.OwnerFirstName;
                        q.OwnerSurname = dc.OwnerSurname;
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
