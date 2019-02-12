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
using DomenaManager.Helpers;
using MaterialDesignThemes.Wpf;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class ChargesPart : UserControl, INotifyPropertyChanged
    {
        private ChargesData chargeData { get; set; }

        public ObservableCollection<BuildingChargeGroupBankAccount> GroupBankAccounts
        {
            get { return chargeData.GroupBankAccounts; }
            set
            {
                if (value != chargeData.GroupBankAccounts)
                {
                    chargeData.GroupBankAccounts = value;
                    OnPropertyChanged("GroupBankAccounts");
                }
            }
        }

        public ObservableCollection<CostListView> CostCollection
        {
            get { return chargeData.CostCollection; }
            set
            {
                if (value != chargeData.CostCollection)
                {
                    chargeData.CostCollection = value;
                    OnPropertyChanged("CostCollection");
                }
            }
        }

        private CostListView _selectedCost;
        public CostListView SelectedCost
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
                        SelectedUnitName = UnitsNames.Where(x => x.EnumValue == value.CostUnit.EnumValue).FirstOrDefault();
                        UnitCost = value.Cost.ToString();
                        CostBeggining = value.BegginingDate.Date;
                        SelectedGroupName = GroupNames.Where(x => x.BuildingChargeGroupNameId == value.CostGroup.BuildingChargeGroupNameId).FirstOrDefault();
                    }
                    OnPropertyChanged("SelectedCost");
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

        private ObservableCollection<CostDistributionCollectionItem> _unitsNames;
        public ObservableCollection<CostDistributionCollectionItem> UnitsNames
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

        private CostDistributionCollectionItem _selectedUnitName;
        public CostDistributionCollectionItem SelectedUnitName
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

        public string SelectedGroupValue { get; set; }

        public ChargesPart(Building SelectedBuilding = null)
        {
            InitializeComponent();
            DataContext = this;
            OnLoad();
            chargeData = new ChargesData(SelectedBuilding);
            AfterInitialLoad();
            CostBeggining = DateTime.Today.AddYears(-3);
        }              

        private void OnLoad()
        {
            InitializeCategoriesList();

            var values = (CostDistribution[])Enum.GetValues(typeof(CostDistribution));
            UnitsNames = new ObservableCollection<Helpers.CostDistributionCollectionItem>();
            foreach (var v in values)
            {
                var cdci = new Helpers.CostDistributionCollectionItem(v);
                UnitsNames.Add(cdci);
            }
        }

        private void AfterInitialLoad()
        {
            ICollectionView cvCostCollection = (CollectionView)CollectionViewSource.GetDefaultView(CostCollection);
            cvCostCollection.GroupDescriptions.Add(new PropertyGroupDescription("CategoryName"));
            cvCostCollection.SortDescriptions.Add(new SortDescription("BegginingDate", ListSortDirection.Ascending));
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

    public class ChargesData
    {
        public ObservableCollection<BuildingChargeGroupBankAccount> GroupBankAccounts { get; set; }
        public ObservableCollection<CostListView> CostCollection { get; set; }

        public ChargesData(Building b)
        {
            ObservableCollection<BuildingChargeBasisCategory> CategoriesNames;
            ObservableCollection<BuildingChargeGroupName> GroupNames;
            using (var db = new DB.DomenaDBContext())
            {
                GroupBankAccounts = b != null ? new ObservableCollection<BuildingChargeGroupBankAccount>(db.BuildingChargeGroupBankAccounts.Where(x => !x.IsDeleted && x.Building.BuildingId == b.BuildingId).ToList()) : new ObservableCollection<BuildingChargeGroupBankAccount>();
                CategoriesNames = new ObservableCollection<BuildingChargeBasisCategory>(db.CostCategories.Where(x => !x.IsDeleted).ToList());
                GroupNames = new ObservableCollection<BuildingChargeGroupName>(db.GroupName.Where(x => !x.IsDeleted).ToList());
            }

            var values = (CostDistribution[])Enum.GetValues(typeof(CostDistribution));
            var UnitsNames = new ObservableCollection<Helpers.CostDistributionCollectionItem>();
            foreach (var v in values)
            {
                var cdci = new Helpers.CostDistributionCollectionItem(v);
                UnitsNames.Add(cdci);
            }

            CostCollection = new ObservableCollection<CostListView>();
            if (b != null)
            {
                foreach (var c in b.CostCollection)
                {
                    var clv = new Helpers.CostListView { BegginingDate = c.BegginingDate.Date, EndingDate = c.EndingDate.Date, Cost = c.CostPerUnit, CostUnit = UnitsNames.Where(x => x.EnumValue == c.BuildingChargeBasisDistribution).FirstOrDefault(), CategoryName = CategoriesNames.Where(x => x.BuildingChargeBasisCategoryId.Equals(c.BuildingChargeBasisCategoryId)).FirstOrDefault().CategoryName, CostGroup = GroupNames.Where(x => x.BuildingChargeGroupNameId == c.BuildingChargeGroupNameId).FirstOrDefault() };
                    CostCollection.Add(clv);
                }
            }
        }
    }
}
