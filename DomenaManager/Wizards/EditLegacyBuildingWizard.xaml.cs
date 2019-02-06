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
    public partial class EditLegacyBuildingWizard : UserControl, INotifyPropertyChanged
    {
        #region Building data

        private string _buildingName;
        public string BuildingName
        {
            get { return _buildingName; }
            set
            {
                _buildingName = value;
                OnPropertyChanged("BuildingName");
            }
        }

        private string _buildingCity;
        public string BuildingCity
        {
            get { return _buildingCity; }
            set
            {
                _buildingCity = value;
                OnPropertyChanged("BuildingCity");
            }
        }

        private string _buildingZipCode;
        public string BuildingZipCode
        {
            get { return _buildingZipCode; }
            set
            {
                _buildingZipCode = value;
                OnPropertyChanged("BuildingZipCode");
            }
        }

        private string _buildingRoadName;
        public string BuildingRoadName
        {
            get { return _buildingRoadName; }
            set
            {
                _buildingRoadName = value;
                OnPropertyChanged("BuildingRoadName");
            }
        }

        private string _buildingRoadNumber;
        public string BuildingRoadNumber
        {
            get { return _buildingRoadNumber; }
            set
            {
                _buildingRoadNumber = value;
                OnPropertyChanged("BuildingRoadNumber");
            }
        }

        #endregion

        public string SelectedCategoryValue { get; set; }
        public string SelectedUnitValue { get; set; }

        private ObservableCollection<BuildingChargeBasisCategory> _categoriesNames;
        public ObservableCollection<BuildingChargeBasisCategory> CategoriesNames
        {
            get { return _categoriesNames; }
            set
            {
                if (value != _categoriesNames)
                {
                    _categoriesNames = value;
                    OnPropertyChanged("CategoriesNames");
                }
            }
        }

        private BuildingChargeBasisCategory _selectedCategoryName;
        public BuildingChargeBasisCategory SelectedCategoryName
        {
            get { return _selectedCategoryName; }
            set
            {
                if (value != _selectedCategoryName)
                {
                    _selectedCategoryName = value;
                    OnPropertyChanged("SelectedCategoryName");
                }
            }
        }

        private string _unitCost;
        public string UnitCost
        {
            get { return _unitCost; }
            set
            {
                if (value != _unitCost)
                {
                    _unitCost = value;
                    OnPropertyChanged("UnitCost");
                }
            }
        }

        private DateTime _costBeggining;
        public DateTime CostBeggining
        {
            get { return _costBeggining; }
            set
            {
                if (value != _costBeggining)
                {
                    _costBeggining = value;
                    OnPropertyChanged("CostBeggining");
                }
            }
        }

        private string _labelError;
        public string LabelError
        {
            get { return _labelError; }
            set
            {
                if (value != _labelError)
                {
                    _labelError = value;
                    OnPropertyChanged("LabelError");
                }
            }
        }

        private ObservableCollection<Helpers.CostDistributionCollectionItem> _unitsNames;
        public ObservableCollection<Helpers.CostDistributionCollectionItem> UnitsNames
        {
            get { return _unitsNames; }
            set
            {
                if (value != _unitsNames)
                {
                    _unitsNames = value;
                    OnPropertyChanged("UnitsNames");
                }
            }
        }

        private Helpers.CostDistributionCollectionItem _selectedUnitName;
        public Helpers.CostDistributionCollectionItem SelectedUnitName
        {
            get { return _selectedUnitName; }
            set
            {
                if (value != _selectedUnitName)
                {
                    _selectedUnitName = value;
                    OnPropertyChanged("SelectedUnitName");
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

        public string SelectedGroupValue { get; set; }

        private ObservableCollection<Helpers.CostListView> _costCollection;
        public ObservableCollection<Helpers.CostListView> CostCollection
        {
            get { return _costCollection; }
            set
            {
                if (value != _costCollection)
                {
                    _costCollection = value;
                    OnPropertyChanged("CostCollection");
                }
            }
        }

        private Helpers.CostListView _selectedCost;
        public Helpers.CostListView SelectedCost
        {
            get { return _selectedCost; }
            set
            {
                if (value != _selectedCost)
                {
                    _selectedCost = value;
                    if (value != null)
                    {
                        SelectedCategoryName = CategoriesNames.Where(x => x.CategoryName == value.CategoryName).FirstOrDefault();
                        SelectedUnitName = UnitsNames.Where(x => x == value.CostUnit).FirstOrDefault();
                        UnitCost = value.Cost.ToString();
                        CostBeggining = value.BegginingDate.Date;
                        SelectedGroupName = GroupNames.Where(x => x == value.CostGroup).FirstOrDefault();
                    }
                    OnPropertyChanged("SelectedCost");
                }
            }
        }

        private string _meterName;
        public string MeterName
        {
            get { return _meterName; }
            set
            {
                if (value != _meterName)
                {
                    _meterName = value;
                    OnPropertyChanged("MeterName");
                }
            }
        }

        private double _lastMeasure;
        public double LastMeasure
        {
            get { return _lastMeasure; }
            set
            {
                if (value != _lastMeasure)
                {
                    _lastMeasure = value;
                    OnPropertyChanged("LastMeasure");
                }
            }
        }

        private bool _isBuilding;
        public bool IsBuilding
        {
            get { return _isBuilding; }
            set
            {
                if (value != _isBuilding)
                {
                    _isBuilding = value;
                    OnPropertyChanged("IsBuilding");
                }
            }
        }

        private bool _isApartment;
        public bool IsApartment
        {
            get { return _isApartment; }
            set
            {
                if (value != _isApartment)
                {
                    _isApartment = value;
                    OnPropertyChanged("IsApartment");
                }
            }
        }

        private ObservableCollection<MeterType> _metersCollection;
        public ObservableCollection<MeterType> MetersCollection
        {
            get { return _metersCollection; }
            set
            {
                if (value != _metersCollection)
                {
                    _metersCollection = value;
                    OnPropertyChanged("MetersCollection");
                }
            }
        }

        private MeterType _selectedMeter;
        public MeterType SelectedMeter
        {
            get { return _selectedMeter; }
            set
            {
                if (value != _selectedMeter)
                {
                    _selectedMeter = value;
                    OnPropertyChanged("SelectedMeter");
                    if (value != null)
                    {
                        MeterName = value.Name;
                        LastMeasure = value.LastMeasure;
                        IsApartment = value.IsApartment;
                        IsBuilding = value.IsBuilding;
                    }
                }
            }
        }

        private string _bankAccount;
        public string BankAccount
        {
            get { return _bankAccount; }
            set
            {
                if (value != _bankAccount)
                {
                    _bankAccount = value;
                }
                OnPropertyChanged("BankAccount");
            }
        }

        private ObservableCollection<BuildingChargeGroupBankAccount> _groupBankAccounts;
        public ObservableCollection<BuildingChargeGroupBankAccount> GroupBankAccounts
        {
            get { return _groupBankAccounts; }
            set
            {
                if (value != _groupBankAccounts)
                {
                    _groupBankAccounts = value;
                    OnPropertyChanged("GroupBankAccounts");
                }
            }
        }

        private BuildingChargeGroupBankAccount _selectedGroupBankAccount;
        public BuildingChargeGroupBankAccount SelectedGroupBankAccount
        {
            get { return _selectedGroupBankAccount; }
            set
            {
                if (value != _selectedGroupBankAccount)
                {
                    _selectedGroupBankAccount = value;
                    OnPropertyChanged("SelectedGroupBankAccount");
                    if (value != null)
                    {
                        BankAccount = value.BankAccount;
                        SelectedGroupName = GroupNames.FirstOrDefault(x => x.BuildingChargeGroupNameId == value.GroupName.BuildingChargeGroupNameId);
                    }
                }
            }
        }

        private List<BuildingChargeGroupBankAccount> GroupBankAccountsTotal;

        public Building _buildingLocalCopy;

        public ICommand DeleteSelectedCost
        {
            get
            {
                return new Helpers.RelayCommand(DeleteCost, CanDeleteCost);
            }
        }

        public ICommand ModifySelectedCost
        {
            get
            {
                return new Helpers.RelayCommand(ModifyCost, CanModifyCost);
            }
        }

        public ICommand AddCost
        {
            get
            {
                return new Helpers.RelayCommand(AddNewCost, CanAddCost);
            }
        }

        public ICommand UpdateAllFieldsCommand
        {
            get
            {
                return new Helpers.RelayCommand(UpdateAllFields, CanUpdateAllFields);
            }
        }

        public ICommand AddNewCategory
        {
            get
            {
                return new Helpers.RelayCommand(AddNewCat, CanAddNewCat);
            }
        }

        public ICommand AddNewGroup
        {
            get
            {
                return new Helpers.RelayCommand(AddNewGrp, CanAddNewGroup);
            }
        }

        public ICommand AddMeter
        {
            get
            {
                return new Helpers.RelayCommand(AddNewMeter, CanAddNewMeter);
            }
        }

        public ICommand ModifySelectedMeter
        {
            get
            {
                return new Helpers.RelayCommand(ModifyMeter, CanModifyMeter);
            }
        }

        public ICommand DeleteSelectedMeter
        {
            get
            {
                return new Helpers.RelayCommand(DeleteMeter, CanDeleteMeter);
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

        public ICommand AddGroupBankAccountCommand
        {
            get
            {
                return new Helpers.RelayCommand(AddGroupBankAccount, CanAddGroupBankAccount);
            }
        }

        public ICommand ModifyGroupBankAccountCommand
        {
            get
            {
                return new Helpers.RelayCommand(ModifyGroupBankAccount, CanModifyGroupBankAccount);
            }
        }

        public ICommand DeleteGroupBankAccountCommand
        {
            get
            {
                return new Helpers.RelayCommand(DeleteGroupBankAccount, CanDeleteGroupBankAccount);
            }
        }

        public EditLegacyBuildingWizard(Building SelectedBuilding = null)
        {
            InitializeComponent();
            DataContext = this;

            MetersCollection = new ObservableCollection<MeterType>();
            InitializeCategoriesList();
            InitializeUnitsList();
            InitializeCostCollection();
            CostBeggining = DateTime.Today;
            if (SelectedBuilding != null)
            {
                _buildingLocalCopy = new Building(SelectedBuilding);

                BuildingName = SelectedBuilding.Name;
                BuildingCity = SelectedBuilding.City;
                BuildingZipCode = SelectedBuilding.ZipCode;
                BuildingRoadName = SelectedBuilding.RoadName;
                BuildingRoadNumber = SelectedBuilding.BuildingNumber;

                foreach (var c in SelectedBuilding.CostCollection)
                {
                    var clv = new Helpers.CostListView { BegginingDate = c.BegginingDate.Date, EndingDate = c.EndingDate.Date, Cost = c.CostPerUnit, CostUnit =  UnitsNames.Where(x => x.EnumValue == c.BuildingChargeBasisDistribution).FirstOrDefault(), CategoryName = CategoriesNames.Where(x => x.BuildingChargeBasisCategoryId.Equals(c.BuildingChargeBasisCategoryId)).FirstOrDefault().CategoryName, CostGroup = GroupNames.Where(x => x.BuildingChargeGroupNameId == c.BuildingChargeGroupNameId).FirstOrDefault() };
                    CostCollection.Add(clv);
                }
                MetersCollection = new ObservableCollection<MeterType>(SelectedBuilding.MeterCollection);
            }
            if (_buildingLocalCopy != null)
                GroupBankAccounts = new ObservableCollection<BuildingChargeGroupBankAccount>(GroupBankAccountsTotal.Where(x => !x.IsDeleted && x.Building.BuildingId == _buildingLocalCopy.BuildingId).ToList());
            else
                GroupBankAccounts = new ObservableCollection<BuildingChargeGroupBankAccount>();
        }

        private void InitializeCategoriesList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                CategoriesNames = new ObservableCollection<BuildingChargeBasisCategory>(db.CostCategories.Where(x => !x.IsDeleted).ToList());
                GroupNames = new ObservableCollection<BuildingChargeGroupName>(db.GroupName.Where(x => !x.IsDeleted).ToList());
                GroupBankAccountsTotal = new List<BuildingChargeGroupBankAccount>(db.BuildingChargeGroupBankAccounts.Include(x => x.GroupName).Include(x => x.Building).ToList());                
            }
        }

        private void InitializeUnitsList()
        {
            var values = (EnumCostDistribution.CostDistribution[])Enum.GetValues(typeof(EnumCostDistribution.CostDistribution));
            UnitsNames = new ObservableCollection<Helpers.CostDistributionCollectionItem>();
            foreach (var v in values)
            {
                var cdci = new Helpers.CostDistributionCollectionItem(v);
                UnitsNames.Add(cdci);
            }
        }

        public void InitializeCostCollection()
        {
            CostCollection = new ObservableCollection<Helpers.CostListView>();
            ICollectionView cvCostCollection = (CollectionView)CollectionViewSource.GetDefaultView(CostCollection);
            cvCostCollection.GroupDescriptions.Add(new PropertyGroupDescription("CategoryName"));
            cvCostCollection.SortDescriptions.Add(new SortDescription("BegginingDate", ListSortDirection.Ascending));
        }

        private void DeleteCost(object param)
        {
            CostCollection.Remove(SelectedCost);
            CalculateCostsDates();
        }

        private bool CanDeleteCost()
        {
            return SelectedCost != null;
        }

        private void ModifyCost(object param)
        {
            DeleteCost(null);
            AddNewCost(null);
        }

        private bool CanModifyCost()
        {
            return SelectedCost != null;
        }

        private void AddNewCost(object param)
        {
            if (SelectedCategoryName == null)
            {
                LabelError = "Wybierz kategorię";
                return;
            }
            if (SelectedUnitName == null)
            {
                LabelError = "Wybierz jednostkę";
                return;
            }
            double uc;
            if (!double.TryParse(UnitCost, out uc) && uc <= 0)
            {
                LabelError = "Podaj poprawny koszt";
                return;
            }
            if (CostBeggining == null)
            {
                LabelError = "Podaj poprawną datę początku obowiązywania";
                return;
            }
            if (SelectedGroupName == null)
            {
                LabelError = "Wybierz grupę";
                return;
            }
            var q = CostCollection.Where(x => x.BegginingDate.Date.CompareTo(CostBeggining.Date) == 0 && x.CategoryName == SelectedCategoryValue && x.CostGroup == SelectedGroupName).Count();
            if (q > 0)
            {
                LabelError = "Istnieje już rekord z taką samą kategorią i datą";
                return;
            }
            LabelError = null;
            var endingDate = new DateTime(1900, 01, 01);
            
            var c = new Helpers.CostListView() { BegginingDate = CostBeggining, CategoryName = SelectedCategoryValue, Cost = uc, CostUnit = SelectedUnitName, EndingDate = endingDate, CostGroup = SelectedGroupName };

            CostCollection.Add(c);
            CalculateCostsDates();
        }

        private void CalculateCostsDates()
        {
            var categories = CostCollection.GroupBy(x => x.CategoryName).Select(c => c.First()).Select(x => x.CategoryName);
            foreach (var cat in categories)
            {
                Helpers.CostListView[] costs = CostCollection.Where(x => x.CategoryName.Equals(cat)).OrderBy(x => x.BegginingDate).ToArray();
                costs[costs.Length - 1].EndingDate = new DateTime(1900, 1, 1);
                for (int i = 0; i < costs.Length - 1; i++)
                {
                    costs[i].EndingDate = costs[i + 1].BegginingDate.AddDays(-1);
                }
            }
        }

        private bool CanAddCost()
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

        private async void AddNewCat(object param)
        {
            var ecc = new Wizards.EditCostCategories();
            var result = await DialogHost.Show(ecc, "HelperDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
        }

        private bool CanAddNewGroup()
        {
            return true;
        }

        private async void AddNewGrp(object param)
        {
            var ecn = new Wizards.EditGroupNames();
            var result = await DialogHost.Show(ecn, "HelperDialog", ExtendedOpenedEventHandler, ExtendedClosingEcnEventHandler);
        }

        private bool CanAddNewCat()
        {
            return true;
        }

        private void AddNewMeter(object param)
        {
            MeterType mt = new MeterType() { Name = MeterName, IsDeleted = false, MeterId = Guid.NewGuid(), LastMeasure= LastMeasure, IsApartment = IsApartment, IsBuilding = IsBuilding};
            MetersCollection.Add(mt);
        }

        private bool CanAddNewMeter()
        {
            return !String.IsNullOrWhiteSpace(MeterName);
        }

        private void ModifyMeter(object param)
        {
            SelectedMeter.Name = MeterName;
            SelectedMeter.LastMeasure = LastMeasure;
            SelectedMeter.IsApartment = IsApartment;
            SelectedMeter.IsBuilding = IsBuilding;
        }

        private bool CanModifyMeter()
        {
            return SelectedMeter != null;
        }

        private void DeleteMeter(object param)
        {
            MetersCollection.Remove(SelectedMeter);
        }

        private bool CanDeleteMeter()
        {
            return SelectedMeter != null;
        }

        private void AddGroupBankAccount(object param)
        {
            if (SelectedGroupName != null && BankAccount != null && IsBankAccountValid(BankAccount) && !GroupBankAccounts.Any(x => x.GroupName.BuildingChargeGroupNameId == SelectedGroupName.BuildingChargeGroupNameId))
            {
                GroupBankAccounts.Add(new BuildingChargeGroupBankAccount() { BuildingChargeGroupBankAccountId = Guid.NewGuid(), BankAccount = BankAccount, GroupName = SelectedGroupName, IsDeleted = false });
            }
        }

        private bool CanAddGroupBankAccount()
        {
            return (SelectedGroupName != null && BankAccount != null && IsBankAccountValid(BankAccount)); 
        }

        private void ModifyGroupBankAccount(object param)
        {
            if (SelectedGroupName != null && BankAccount != null && SelectedGroupBankAccount != null && IsBankAccountValid(BankAccount))
            {
                SelectedGroupBankAccount.BankAccount = BankAccount;
                SelectedGroupBankAccount.GroupName = SelectedGroupName;
            }
        }

        private bool CanModifyGroupBankAccount()
        {
            return (SelectedGroupName != null && BankAccount != null && SelectedGroupBankAccount != null && IsBankAccountValid(BankAccount));
        }

        private void DeleteGroupBankAccount(object param)
        {
            GroupBankAccounts.Remove(SelectedGroupBankAccount);
        }

        private bool CanDeleteGroupBankAccount()
        {
            return (SelectedGroupBankAccount != null);
        }

        private bool IsBankAccountValid(string bankAccount)
        {
            string account = bankAccount.Replace(" ", "");
            if (account.Length != 26)
                return false;
            if (account.Any(x => !char.IsDigit(x)))
                return false;
            account = account.Substring(2, account.Length - 2) + "2521" + account.Substring(0, 2);
            int checksum = int.Parse(account.Substring(0, 1));
            for (int i = 1; i < account.Length; i++)
            {
                int v = int.Parse(account.Substring(i, 1));
                checksum *= 10;
                checksum += v;
                checksum %= 97;
            }
            return checksum == 1;
        }

        private void SaveDialog(object param)
        {
            if (_buildingLocalCopy == null)
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(BuildingName) || string.IsNullOrEmpty(BuildingCity) || string.IsNullOrEmpty(BuildingZipCode) || string.IsNullOrEmpty(BuildingRoadName) || string.IsNullOrEmpty(BuildingRoadNumber)))
                {
                    return;
                }
                //Add new building
                using (var db = new DB.DomenaDBContext())
                {
                    var newBuilding = new LibDataModel.Building { BuildingId = Guid.NewGuid(), Name = BuildingName, City = BuildingCity, ZipCode = BuildingZipCode, BuildingNumber = BuildingRoadNumber, RoadName = BuildingRoadName, IsDeleted = false };
                    List<LibDataModel.BuildingChargeBasis> costs = new List<LibDataModel.BuildingChargeBasis>();
                    foreach (var c in CostCollection)
                    {
                        var catId = db.CostCategories.Where(x => x.CategoryName.Equals(c.CategoryName)).FirstOrDefault().BuildingChargeBasisCategoryId;
                        var cost = new LibDataModel.BuildingChargeBasis { BuildingChargeBasisId = Guid.NewGuid(), BegginingDate = c.BegginingDate.Date, EndingDate = c.EndingDate.Date, CostPerUnit = c.Cost, BuildingChargeBasisDistribution = c.CostUnit.EnumValue, BuildingChargeBasisCategoryId = catId, BuildingChargeGroupNameId = c.CostGroup.BuildingChargeGroupNameId };
                        costs.Add(cost);
                    }
                    newBuilding.CostCollection = costs;
                    foreach (var m in MetersCollection)
                    {
                        newBuilding.MeterCollection.Add(m);
                    }
                    db.Buildings.Add(newBuilding);
                    foreach (var g in GroupBankAccounts)
                    {
                        g.Building = newBuilding;
                        db.Entry(g.GroupName).State = EntityState.Unchanged;
                        db.BuildingChargeGroupBankAccounts.Add(g);
                    }
                    _buildingLocalCopy = newBuilding;
                    db.SaveChanges();
                }
            }
            else
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(BuildingName) || string.IsNullOrEmpty(BuildingCity) || string.IsNullOrEmpty(BuildingZipCode) || string.IsNullOrEmpty(BuildingRoadName) || string.IsNullOrEmpty(BuildingRoadNumber)))

                {
                    return;
                }
                //Edit building
                using (var db = new DB.DomenaDBContext())
                {
                    var q = db.Buildings.Include(x => x.CostCollection).Include(x => x.MeterCollection).Where(x => x.BuildingId.Equals(_buildingLocalCopy.BuildingId)).FirstOrDefault();
                    q.BuildingNumber = BuildingRoadNumber;
                    q.City = BuildingCity;
                    q.Name = BuildingName;
                    q.RoadName = BuildingRoadName;
                    q.ZipCode = BuildingZipCode;
                    //q.CostCollection.RemoveAll(x => true);

                    List<LibDataModel.BuildingChargeBasis> costs = new List<LibDataModel.BuildingChargeBasis>();
                    foreach (var c in CostCollection)
                    {
                        var catId = db.CostCategories.Where(x => x.CategoryName.Equals(c.CategoryName)).FirstOrDefault().BuildingChargeBasisCategoryId;
                        var cost = new LibDataModel.BuildingChargeBasis { BuildingChargeBasisId = Guid.NewGuid(), BegginingDate = c.BegginingDate.Date, EndingDate = c.EndingDate.Date, CostPerUnit = c.Cost, BuildingChargeBasisDistribution = c.CostUnit.EnumValue, BuildingChargeBasisCategoryId = catId, BuildingChargeGroupNameId = c.CostGroup.BuildingChargeGroupNameId };
                        costs.Add(cost);
                    }
                    q.CostCollection = costs;

                    //Add new
                    foreach (var m in MetersCollection)
                    {
                        if (!q.MeterCollection.Any(x => x.MeterId.Equals(m.MeterId)))
                        {
                            q.MeterCollection.Add(m);
                        }
                    }
                    //Remove necessary
                    for (int i = q.MeterCollection.Count - 1; i >= 0; i--)
                    {
                        if (!MetersCollection.Any(x => x.MeterId.Equals(q.MeterCollection[i].MeterId)))
                        {
                            q.MeterCollection.RemoveAt(i);
                        }
                        else
                        {
                            // Change names
                            q.MeterCollection[i].Name = MetersCollection.FirstOrDefault(x => x.MeterId.Equals(q.MeterCollection[i].MeterId)).Name;
                            if (q.MeterCollection[i].LastMeasure != MetersCollection.FirstOrDefault(x => x.MeterId.Equals(q.MeterCollection[i].MeterId)).LastMeasure)
                            {
                                var nm = new MetersHistory
                                {
                                    MeterHistoryId = Guid.NewGuid(),
                                    Apartment = null,
                                    ApartmentMeter = null,
                                    Building = q,
                                    MeterType = q.MeterCollection[i],
                                    ModifiedDate = DateTime.Now,
                                    NewValue = q.MeterCollection[i].LastMeasure,
                                    OldValue = MetersCollection.FirstOrDefault(x => x.MeterId.Equals(q.MeterCollection[i].MeterId)).LastMeasure,
                                };
                                db.MetersHistories.Add(nm);
                                db.Entry(nm.Building).State = EntityState.Unchanged;

                                q.MeterCollection[i].LastMeasure = MetersCollection.FirstOrDefault(x => x.MeterId.Equals(q.MeterCollection[i].MeterId)).LastMeasure;
                            }
                            q.MeterCollection[i].IsBuilding = MetersCollection.FirstOrDefault(x => x.MeterId.Equals(q.MeterCollection[i].MeterId)).IsBuilding;
                            q.MeterCollection[i].IsApartment = MetersCollection.FirstOrDefault(x => x.MeterId.Equals(q.MeterCollection[i].MeterId)).IsApartment;
                        }
                    }

                    var buildingBankAddresses = db.BuildingChargeGroupBankAccounts.Where(x => x.Building.BuildingId == q.BuildingId).ToList();
                    
                    //Add new
                    foreach (var bba in GroupBankAccounts)
                    {
                        if (!buildingBankAddresses.Any(x => x.BuildingChargeGroupBankAccountId == bba.BuildingChargeGroupBankAccountId))
                        {
                            bba.Building = q;
                            db.BuildingChargeGroupBankAccounts.Add(bba);
                            db.Entry(bba.GroupName).State = EntityState.Unchanged;
                            db.Entry(bba.Building).State = EntityState.Unchanged;
                        }
                    }
                    //Remove necessary
                    for (int i = buildingBankAddresses.Count - 1; i >= 0; i--)
                    {
                        if (!GroupBankAccounts.Any(x => x.BuildingChargeGroupBankAccountId.Equals(buildingBankAddresses[i].BuildingChargeGroupBankAccountId)))
                        {
                            buildingBankAddresses[i].IsDeleted = true;
                        }
                        else
                        {
                            // Change names
                            buildingBankAddresses[i].BankAccount = GroupBankAccounts.FirstOrDefault(x => x.BuildingChargeGroupBankAccountId == buildingBankAddresses[i].BuildingChargeGroupBankAccountId).BankAccount;
                            buildingBankAddresses[i].GroupName = GroupBankAccounts.FirstOrDefault(x => x.BuildingChargeGroupBankAccountId == buildingBankAddresses[i].BuildingChargeGroupBankAccountId).GroupName;
                            
                            //db.Entry(buildingBankAddresses[i].Building).State = EntityState.Unchanged;
                            db.Entry(buildingBankAddresses[i].GroupName).State = EntityState.Unchanged;
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
            Helpers.SwitchPage.SwitchMainPage(new Pages.BuildingsPage(), this);
        }

        private bool CanAcceptDialog()
        {
            return true;
        }

        private void AcceptDialog(object param)
        {
            if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(BuildingName) || string.IsNullOrEmpty(BuildingCity) || string.IsNullOrEmpty(BuildingZipCode) || string.IsNullOrEmpty(BuildingRoadName) || string.IsNullOrEmpty(BuildingRoadNumber)))
            {
                return;
            }
            SaveDialog(null);
            Helpers.SwitchPage.SwitchMainPage(new Pages.BuildingsPage(), this);
        }

        private bool CanCancelDialog()
        {
            return true;
        }
        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private async void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditCostCategories);
                //Accept
                using (var db = new DB.DomenaDBContext())
                {
                    foreach (var cmd in dc.commandBuffer)
                    {
                        switch (cmd.CommandType)
                        {
                            default:
                                break;
                            case Helpers.CommandEnum.Add:
                                db.CostCategories.Add(cmd.Item);
                                db.SaveChanges();
                                break;
                            case Helpers.CommandEnum.Remove:
                                db.CostCategories.Where(x => x.BuildingChargeBasisCategoryId.Equals(cmd.Item.BuildingChargeBasisCategoryId)).FirstOrDefault().IsDeleted = true;
                                db.SaveChanges();
                                break;
                            case Helpers.CommandEnum.Update:
                                db.CostCategories.Where(x => x.BuildingChargeBasisCategoryId.Equals(cmd.Item.BuildingChargeBasisCategoryId)).FirstOrDefault().CategoryName = cmd.Item.CategoryName;
                                db.SaveChanges();
                                break;
                        }
                    }
                }
            }
            else if (!(bool)eventArgs.Parameter)
            {

                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    //eventArgs.Cancel();
                    var dc = (eventArgs.Session.Content as Wizards.EditCostCategories);
                    var result = await DialogHost.Show(dc, "HelperDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
                }
            }
            InitializeCategoriesList();
        }


        private async void ExtendedClosingEcnEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditGroupNames);
                //Accept
                using (var db = new DB.DomenaDBContext())
                {
                    foreach (var cmd in dc.commandBuffer)
                    {
                        switch (cmd.CommandType)
                        {
                            default:
                                break;
                            case Helpers.CommandEnum.Add:
                                db.GroupName.Add(cmd.Item);
                                db.SaveChanges();
                                break;
                            case Helpers.CommandEnum.Remove:
                                db.GroupName.Where(x => x.BuildingChargeGroupNameId.Equals(cmd.Item.BuildingChargeGroupNameId)).FirstOrDefault().IsDeleted = true;
                                db.SaveChanges();
                                break;
                            case Helpers.CommandEnum.Update:
                                db.GroupName.Where(x => x.BuildingChargeGroupNameId.Equals(cmd.Item.BuildingChargeGroupNameId)).FirstOrDefault().GroupName = cmd.Item.GroupName;
                                db.SaveChanges();
                                break;
                        }
                    }
                }
            }
            else if (!(bool)eventArgs.Parameter)
            {

                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    var dc = (eventArgs.Session.Content as Wizards.EditGroupNames);
                    var result = await DialogHost.Show(dc, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEcnEventHandler);
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
