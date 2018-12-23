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
using LibDataModel;
using System.Data.Entity;
using MaterialDesignThemes.Wpf;
using DomenaManager.Helpers;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditPaymentWizard.xaml
    /// </summary>
    public partial class EditPaymentWizard : UserControl, INotifyPropertyChanged
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
                InitializeApartmentsNumbers();
                InitializeGroupNames();
            }
        }

        private ObservableCollection<Apartment> _apartmentsOC { get; set; }

        private ObservableCollection<Owner> _ownersOC { get; set; }

        private ObservableCollection<Charge> _chargesOC { get; set; }

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

        private ObservableCollection<BuildingChargeGroupName> _groupNames;
        public ObservableCollection<BuildingChargeGroupName> GroupNames
        {
            get { return _groupNames; }
            set
            {
                if (value != _groupNames)
                {
                    _groupNames = value;
                    OnPropertyChanged("GroupNames");
                }
            }
        }

        private BuildingChargeGroupName _selectedGroupName;
        public BuildingChargeGroupName SelectedGroupName
        {
            get { return _selectedGroupName; }
            set
            {
                if (value != _selectedGroupName)
                {
                    _selectedGroupName = value;
                    OnPropertyChanged("SelectedGroupName");
                }
            }
        }

        public string SelectedGroupNameValue { get; set; }

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

                    if (SelectedApartmentNumber != null)
                    {
                        OwnerMailAddress = _ownersOC.Where(x => x.OwnerId.Equals(SelectedApartmentNumber.OwnerId)).FirstOrDefault().OwnerName + Environment.NewLine;
                        OwnerMailAddress += SelectedApartmentNumber.CorrespondenceAddress == null ? _ownersOC.Where(x => x.OwnerId.Equals(SelectedApartmentNumber.OwnerId)).FirstOrDefault().MailAddress : SelectedApartmentNumber.CorrespondenceAddress;
                        var charge = _chargesOC.Where(x => x.ApartmentId.Equals(SelectedApartmentNumber.ApartmentId)).OrderByDescending(x => x.ChargeDate).FirstOrDefault();
                        LastChargeAmount = charge != null ? charge.Components.Sum(x => x.Sum).ToString() + " zł" : "brak";
                    }
                }
            }
        }

        private string _ownerMailAddress;

        public string OwnerMailAddress
        {
            get { return _ownerMailAddress; }
            set
            {
                if (value != _ownerMailAddress)
                {
                    _ownerMailAddress = value;
                    OnPropertyChanged("OwnerMailAddress");
                }
            }
        }

        public string SelectedApartmentNumberValue { get; set; }

        public string SelectedBuildingValue { get; set; }

        private string _paymentAmount;        
        public string PaymentAmount
        {
            get { return _paymentAmount; }
            set
            {
                if (value != _paymentAmount)
                {
                    _paymentAmount = value;
                    OnPropertyChanged("PaymentAmount");
                }
            }
        }

        private string _lastChargeAmount;
        public string LastChargeAmount
        {
            get { return _lastChargeAmount; }
            set
            {
                if (value != _lastChargeAmount)
                {
                    _lastChargeAmount = value;
                    OnPropertyChanged("LastChargeAmount");
                }
            }
        }

        private DateTime _paymentRegistrationDate;

        public DateTime PaymentRegistrationDate
        {
            get { return _paymentRegistrationDate; }
            set
            {
                if (value != _paymentRegistrationDate)
                {
                    _paymentRegistrationDate = value;
                    OnPropertyChanged("PaymentRegistrationDate");
                }
            }
        }

        private bool _canEdit;
        public bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                if (value != _canEdit)
                {
                    _canEdit = value;
                    OnPropertyChanged("CanEdit");
                }
            }
        }

        public Payment _lpc;

        private List<BuildingChargeGroupName> _groupNamesDB;

        public ICommand SaveCommand
        {
            get { return new RelayCommand(SaveDialog, CanSaveDialog); }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(CancelDialog, CanCancelDialog); }
        }

        public ICommand AcceptCommand
        {
            get
            {
                return new Helpers.RelayCommand(AcceptDialog, CanAcceptDialog);
            }
        }

        public EditPaymentWizard(Payment _payment = null)
        {
            DataContext = this;
            InitializeComponent();
            InitializeBuildingList();
            InitializeApartmentsNumbers();
            _lpc = _payment;
            if (_payment != null)
            {
                CanEdit = false;
                PaymentRegistrationDate = _payment.PaymentRegistrationDate;
                PaymentAmount = _payment.PaymentAmount.ToString();
                SelectedBuildingName = BuildingsNames.FirstOrDefault(x => x.BuildingId.Equals( _apartmentsOC.FirstOrDefault(a => a.ApartmentId.Equals(_payment.ApartmentId)).BuildingId ));
                SelectedApartmentNumber = ApartmentsNumbers.FirstOrDefault(x => x.ApartmentId.Equals(_payment.ApartmentId));
                OwnerMailAddress = _ownersOC.FirstOrDefault(x => x.OwnerId.Equals(SelectedApartmentNumber.OwnerId)).MailAddress;
                SelectedGroupName = _groupNamesDB.FirstOrDefault(x => x.BuildingChargeGroupNameId == _payment.ChargeGroup.BuildingChargeGroupNameId);
                return;
            }
            CanEdit = true;
            PaymentRegistrationDate = DateTime.Today;
        }

        private void InitializeGroupNames()
        {
            GroupNames = new ObservableCollection<BuildingChargeGroupName>();
            if (SelectedBuildingName != null && SelectedBuildingName.CostCollection != null)
            {
                foreach (var c in SelectedBuildingName.CostCollection)
                {
                    if (!GroupNames.Any(x => x.BuildingChargeGroupNameId == c.BuildingChargeGroupNameId))
                        GroupNames.Add(_groupNamesDB.FirstOrDefault(x => x.BuildingChargeGroupNameId == c.BuildingChargeGroupNameId));
                }                
            }
        }

        private void InitializeApartmentsNumbers()
        {
            using (var db = new DB.DomenaDBContext())
            {
                if (SelectedBuildingName == null)
                {
                    ApartmentsNumbers = new ObservableCollection<Apartment>();
                }
                else
                {
                    ApartmentsNumbers = new ObservableCollection<Apartment>(_apartmentsOC.Where(x => x.BuildingId.Equals(SelectedBuildingName.BuildingId)).OrderBy(x => x.ApartmentNumber).ToList());
                }
            }
            OnPropertyChanged("ApartmentsNumbers");
        }

        private void InitializeBuildingList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                BuildingsNames = new ObservableCollection<Building>(db.Buildings.Include(x => x.CostCollection).Where(x => x.IsDeleted == false).ToList());
                _apartmentsOC = new ObservableCollection<Apartment>(db.Apartments.Where(x => !x.IsDeleted && x.SoldDate == null).ToList());
                _ownersOC = new ObservableCollection<Owner>(db.Owners.ToList());
                _chargesOC = new ObservableCollection<Charge>(db.Charges.Include(x => x.Components).Where(x => !x.IsDeleted).ToList());
                _groupNamesDB = new List<BuildingChargeGroupName>(db.GroupName.ToList());
            }
        }

        private void SaveDialog(object param)
        {
            if (!IsValid(this as DependencyObject))
            {
                return;
            }
            double amount;
            bool isAmountValid = double.TryParse(this.PaymentAmount, out amount);
            //Accept
            if (this._lpc == null)
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(this.SelectedBuildingValue) || string.IsNullOrEmpty(this.SelectedApartmentNumberValue) || !isAmountValid))
                {
                    return;
                }
                //Add new payment
                using (var db = new DB.DomenaDBContext())
                {
                    var newPayment = new Payment() { IsDeleted = false, ApartmentId = this.SelectedApartmentNumber.ApartmentId, PaymentAddDate = DateTime.Today, PaymentAmount = amount, PaymentId = Guid.NewGuid(), PaymentRegistrationDate = this.PaymentRegistrationDate, ChargeGroup = SelectedGroupName };
                    db.Payments.Add(newPayment);
                    db.Entry(newPayment.ChargeGroup).State = EntityState.Unchanged;
                    db.SaveChanges();
                }
            }
            else
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(this.SelectedBuildingValue) || string.IsNullOrEmpty(this.SelectedApartmentNumberValue) || !isAmountValid))
                {
                    return;
                }
                //Edit payment
                using (var db = new DB.DomenaDBContext())
                {
                    var q = db.Payments.Where(x => x.PaymentId.Equals(this._lpc.PaymentId)).FirstOrDefault();
                    q.ChargeGroup = SelectedGroupName;
                    q.PaymentAddDate = DateTime.Today;
                    q.PaymentRegistrationDate = this.PaymentRegistrationDate;
                    q.PaymentAmount = amount;
                    db.Entry(q.ChargeGroup).State = EntityState.Unchanged;
                    db.SaveChanges();
                }
            }
            SwitchPage.SwitchMainPage(new Pages.PaymentsPage(), this);
        }

        private bool CanSaveDialog()
        {
            return (SelectedApartmentNumber != null && SelectedBuildingName != null && SelectedGroupName != null && !String.IsNullOrWhiteSpace(PaymentAmount));
        }

        private void CancelDialog(object param)
        {
            SwitchPage.SwitchMainPage(new Pages.PaymentsPage(), this);
        }

        private bool CanCancelDialog()
        {
            return true;
        }

        private bool CanAcceptDialog()
        {
            return (SelectedApartmentNumber != null && SelectedBuildingName != null && SelectedGroupName != null && !String.IsNullOrWhiteSpace(PaymentAmount));
        }

        private void AcceptDialog(object param)
        {
            SaveDialog(null);
            Helpers.SwitchPage.SwitchMainPage(new Pages.PaymentsPage(), this);
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
