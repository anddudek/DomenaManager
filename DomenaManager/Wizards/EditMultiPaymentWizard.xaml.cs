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
    public partial class EditMultiPaymentWizard : UserControl, INotifyPropertyChanged
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

        private ObservableCollection<MultiPaymentDataGrid> _paymentsList;
        public ObservableCollection<MultiPaymentDataGrid> PaymentsList
        {
            get { return _paymentsList; }
            set
            {
                if (value != _paymentsList)
                {
                    _paymentsList = value;
                    OnPropertyChanged("PaymentsList");
                }
            }
        }

        private MultiPaymentDataGrid _selectedPayment;
        public MultiPaymentDataGrid SelectedPayment
        {
            get { return _selectedPayment; }
            set
            {
                if (value != _selectedPayment)
                {
                    _selectedPayment = value;
                    OnPropertyChanged("SelectedPayment");
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
        
        private string _amountError;
        public string AmountError
        {
            get { return _amountError; }
            set
            {
                if (value != _amountError)
                {
                    _amountError = value;
                    OnPropertyChanged("AmountError");
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
                        _selectedOwner = _ownersOC.Where(x => x.OwnerId.Equals(SelectedApartmentNumber.OwnerId)).FirstOrDefault();
                        OwnerMailAddress = _selectedOwner.OwnerName + Environment.NewLine;
                        OwnerMailAddress += SelectedApartmentNumber.CorrespondenceAddress == null ? _selectedOwner.MailAddress : SelectedApartmentNumber.CorrespondenceAddress;
                    }
                }
            }
        }

        private Owner _selectedOwner;

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
                    AmountError = string.Empty;
                    OnPropertyChanged("PaymentAmount");
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
            get { return new Helpers.RelayCommand(AcceptDialog, CanAcceptDialog); }
        }

        public ICommand AddPaymentCommand
        {
            get { return new Helpers.RelayCommand(AddPayment, CanAddPayment); }
        }

        public ICommand UpdatePaymentCommand
        {
            get { return new Helpers.RelayCommand(UpdatePayment, CanUpdatePayment); }
        }

        public ICommand DeletePaymentCommand
        {
            get { return new Helpers.RelayCommand(DeletePayment, CanDeletePayment); }
        }

        public EditMultiPaymentWizard()
        {
            DataContext = this;
            InitializeComponent();
            InitializeBuildingList();
            InitializeApartmentsNumbers();
            CanEdit = true;
            PaymentRegistrationDate = DateTime.Today;
            PaymentsList = new ObservableCollection<MultiPaymentDataGrid>();
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
            
            if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(this.SelectedBuildingValue) || string.IsNullOrEmpty(this.SelectedApartmentNumberValue) || !isAmountValid))
            {
                return;
            }
            //Add new payment
            using (var db = new DB.DomenaDBContext())
            {
                foreach (var p in PaymentsList)
                {
                    var newPayment = new Payment() { IsDeleted = false, ApartmentId = p.Apartment.ApartmentId, PaymentAddDate = p.PaymentAddDate, PaymentAmount = p.PaymentAmount, PaymentId = Guid.NewGuid(), PaymentRegistrationDate = p.PaymentRegistrationDate, ChargeGroup = p.ChargeGroup };
                    db.Payments.Add(newPayment);
                    db.Entry(newPayment.ChargeGroup).State = EntityState.Unchanged;
                }
                db.SaveChanges();
            }
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
            if (!IsValid(this as DependencyObject))
            {
                return;
            }
            SaveDialog(null);
            Helpers.SwitchPage.SwitchMainPage(new Pages.PaymentsPage(), this);
        }

        private bool CanAddPayment()
        {
            return PaymentAmount != null && SelectedApartmentNumber != null && SelectedBuildingName != null && SelectedGroupName != null && PaymentRegistrationDate != null;
        }

        private void AddPayment(object param)
        {
            decimal amount;
            bool isAmountValid = decimal.TryParse(this.PaymentAmount, out amount);
            if (!isAmountValid || !IsValid(this as DependencyObject))
            {
                AmountError = "Niepoprawna kwota";
                return;
            }

            var mpdg = new MultiPaymentDataGrid()
            {
                PaymentId = null,
                Apartment = SelectedApartmentNumber,
                Building = SelectedBuildingName,
                ChargeGroup = SelectedGroupName,
                Owner = _selectedOwner,
                PaymentAddDate = DateTime.Now,
                PaymentAmount = amount,
                PaymentRegistrationDate = PaymentRegistrationDate,
            };
            PaymentsList.Add(mpdg);
        }

        private bool CanDeletePayment()
        {
            return SelectedPayment != null;
        }

        private void DeletePayment(object param)
        {
            PaymentsList.Remove(SelectedPayment);
        }

        private bool CanUpdatePayment()
        {
            return CanAddPayment() && SelectedPayment != null;
        }

        private void UpdatePayment(object param)
        {
            decimal amount;
            bool isAmountValid = decimal.TryParse(this.PaymentAmount, out amount);
            if (!isAmountValid || !IsValid(this as DependencyObject))
            {
                AmountError = "Niepoprawna kwota";
                return;
            }
            SelectedPayment.Apartment = SelectedApartmentNumber;
            SelectedPayment.Building = SelectedBuildingName;
            SelectedPayment.ChargeGroup = SelectedGroupName;
            SelectedPayment.Owner = _selectedOwner;
            SelectedPayment.PaymentAddDate = DateTime.Now;
            SelectedPayment.PaymentAmount = amount;
            SelectedPayment.PaymentRegistrationDate = PaymentRegistrationDate;            
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
