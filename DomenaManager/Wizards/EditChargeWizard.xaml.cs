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
using DomenaManager.Helpers;
using System.Data.Entity;
using MaterialDesignThemes.Wpf;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditChargeWizard.xaml
    /// </summary>
    public partial class EditChargeWizard : UserControl, INotifyPropertyChanged
    {
        #region Bindings

        private Charge _charge;

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
            }
        }

        private ObservableCollection<Apartment> ApartmentsCollection { get; set; }

        private ObservableCollection<Owner> OwnersCollection { get; set; }

        private ObservableCollection<int> _apartmentsNumbersCollection;
        public ObservableCollection<int> ApartmentsNumbersCollection
        {
            get { return _apartmentsNumbersCollection;  }
            set
            {
                if (value != _apartmentsNumbersCollection)
                {
                    _apartmentsNumbersCollection = value;
                    OnPropertyChanged("ApartmentsNumbersCollection");
                }
            }
        }

        private int _selectedApartmentNumber;
        public int SelectedApartmentNumber
        {
            get { return _selectedApartmentNumber; }
            set
            {
                if (value != _selectedApartmentNumber)
                {
                    _selectedApartmentNumber = value;
                    OnPropertyChanged("SelectedApartmentNumber");
                    var ow = OwnersCollection.FirstOrDefault(o => o.OwnerId.Equals(ApartmentsCollection.FirstOrDefault(x => x.BuildingId.Equals(SelectedBuilding.BuildingId) && x.ApartmentNumber.Equals(SelectedApartmentNumber)).OwnerId));
                    OwnerName = ow.OwnerName + Environment.NewLine + ow.MailAddress;
                }
            }
        }

        public string SelectedApartmentNumberValue { get; set; }

        private List<String> _chargeStatusCollection;
        public List<String> ChargeStatusCollection
        {
            get { return _chargeStatusCollection; }
            set
            {
                if (value != _chargeStatusCollection)
                {
                    _chargeStatusCollection = value;
                    OnPropertyChanged("ChargeStatusCollection");
                }
            }
        }

        public string CurrentChargeStatus { get; set; }

        private string _chargeStatus;
        public string ChargeStatus
        {
            get { return _chargeStatus; }
            set
            {
                if (value != _chargeStatus)
                {
                    _chargeStatus = value;
                    OnPropertyChanged("ChargeStatus");
                }
            }
        }

        private string _ownerName;
        public string OwnerName
        {
            get { return _ownerName; }
            set
            {
                if (value != _ownerName)
                {
                    _ownerName = value;
                    OnPropertyChanged("OwnerName");
                }
            }
        }

        public string SelectedCategoryValue { get; set; }

        private ObservableCollection<Building> _buildingsCollection;
        public ObservableCollection<Building> BuildingsCollection
        {
            get { return _buildingsCollection; }
            set
            {
                if (value != _buildingsCollection)
                {
                    _buildingsCollection = value;
                    OnPropertyChanged("BuildingsCollection");
                }
            }
        }

        private Building _selectedBuilding;
        public Building SelectedBuilding
        {
            get { return _selectedBuilding; }
            set
            {
                if (value != _selectedBuilding)
                {
                    _selectedBuilding = value;
                    OnPropertyChanged("SelectedBuilding");
                    UpdateApartmentsNumbers();
                }
            }
        }

        public string SelectedBuildingValue { get; set; }

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

        public string SelectedUnitValue { get; set; }

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

        private string _chargeSum;
        public string ChargeSum
        {
            get { return _chargeSum; }
            set
            {
                if (value != _chargeSum)
                {
                    _chargeSum = value;
                    OnPropertyChanged("ChargeSum");
                }
            }
        }

        private ObservableCollection<ChargeComponent> _chargeComponents;
        public ObservableCollection<ChargeComponent> ChargeComponents
        {
            get { return _chargeComponents; }
            set
            {
                if (value != _chargeComponents)
                {
                    _chargeComponents = value;
                    OnPropertyChanged("ChargeComponents");
                }
            }
        }

        private ChargeComponent _selectedChargeComponent;
        public ChargeComponent SelectedChargeComponent
        {
            get { return _selectedChargeComponent; }
            set
            {
                if (value != _selectedChargeComponent)
                {
                    _selectedChargeComponent = value;
                    SelectedCategoryName = CategoriesNames.Where(x => x.BuildingChargeBasisCategoryId.Equals(_selectedChargeComponent.CostCategoryId)).FirstOrDefault();
                    SelectedUnitName = UnitsNames.Where(x => x.EnumValue.Equals(_selectedChargeComponent.CostDistribution)).FirstOrDefault();
                    UnitCost = _selectedChargeComponent.CostPerUnit.ToString();
                    ChargeSum = _selectedChargeComponent.Sum.ToString();
                    OnPropertyChanged("SelectedChargeComponent");
                }
            }
        }

        public ICommand AddNewCategory
        {
            get { return new RelayCommand(AddNew, CanAddNew); }
        }

        public ICommand UpdateAllFieldsCommand
        {
            get
            {
                return new Helpers.RelayCommand(UpdateAllFields, CanUpdateAllFields);
            }
        }

        public ObservableCollection<BuildingChargeBasisCategory> Categories { get; set; }

        #endregion

        #region Commands

        #endregion

        public EditChargeWizard(ChargeDataGrid charge)
        {
            _charge = charge;
            InitializeChargeStatusCollection();
            InitializeBuildingsList();
            InitializeCategoriesList();
            InitializeUnitsList();
            InitializeCategories();
            LoadCharge(charge);
            InitializeChargeComponents(charge);
            DataContext = this;
            InitializeComponent();              
        }

        #region Functions

        private void InitializeChargeStatusCollection()
        {
            ChargeStatusCollection = new List<string>() { "Otwarte", "Zamknięte" };
        }

        private void LoadCharge(ChargeDataGrid charge)
        {
            if (charge != null)
            {
                SelectedBuilding = BuildingsCollection.FirstOrDefault(x => x.BuildingId.Equals(charge.Building.BuildingId));
                //ApartmentNumber = charge.Apartment.ApartmentNumber;
                SelectedApartmentNumber = ApartmentsNumbersCollection.FirstOrDefault(x => x.Equals(charge.Apartment.ApartmentNumber));
                OwnerName = charge.Owner.OwnerName + Environment.NewLine + charge.Owner.MailAddress;
                ChargeStatus = charge.IsClosed ? ChargeStatusCollection[1] : ChargeStatusCollection[0];
            }
        }

        private void InitializeCategoriesList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                CategoriesNames = new ObservableCollection<BuildingChargeBasisCategory>(db.CostCategories.Where(x => !x.IsDeleted).ToList());

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

        private void InitializeChargeComponents(Charge charge)
        {
            if (charge != null)
            {
                using (var db = new DB.DomenaDBContext())
                {
                    ChargeComponents = new ObservableCollection<ChargeComponent>(db.Charges.Include(c => c.Components).Where(x => x.ChargeId.Equals(charge.ChargeId)).FirstOrDefault().Components.ToList());
                }
            }
            else ChargeComponents = new ObservableCollection<ChargeComponent>();
        }

        private void InitializeCategories()
        {
            using (var db = new DB.DomenaDBContext())
            {
                Categories = new ObservableCollection<BuildingChargeBasisCategory>(db.CostCategories.ToList());
            }
        }

        private void InitializeBuildingsList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                BuildingsCollection = new ObservableCollection<Building>(db.Buildings.ToList());
                ApartmentsCollection = new ObservableCollection<Apartment>(db.Apartments.ToList());
                OwnersCollection = new ObservableCollection<Owner>(db.Owners.ToList());
                OwnerName = null;
            }
            ApartmentsNumbersCollection = new ObservableCollection<int>();
            SelectedApartmentNumberValue = null;
        }

        private async void AddNew(object param)
        {
            var ecc = new Wizards.EditCostCategories();
            var result = await DialogHost.Show(ecc, "HelperDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
        }

        private bool CanAddNew()
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
                        switch (cmd.category)
                        {
                            default:
                                break;
                            case Helpers.CostCategoryEnum.CostCategoryCommandEnum.Add:
                                db.CostCategories.Add(cmd.costItem);
                                db.SaveChanges();
                                break;
                            case Helpers.CostCategoryEnum.CostCategoryCommandEnum.Remove:
                                db.CostCategories.Where(x => x.BuildingChargeBasisCategoryId.Equals(cmd.costItem.BuildingChargeBasisCategoryId)).FirstOrDefault().IsDeleted = true;
                                db.SaveChanges();
                                break;
                            case Helpers.CostCategoryEnum.CostCategoryCommandEnum.Update:
                                db.CostCategories.Where(x => x.BuildingChargeBasisCategoryId.Equals(cmd.costItem.BuildingChargeBasisCategoryId)).FirstOrDefault().CategoryName = cmd.costItem.CategoryName;
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

        private void UpdateApartmentsNumbers()
        {
            if (SelectedBuilding != null)
            {
                ApartmentsNumbersCollection = new ObservableCollection<int>(ApartmentsCollection.Where(x => x.BuildingId.Equals(SelectedBuilding.BuildingId)).Select(x => x.ApartmentNumber).ToList());                
                SelectedApartmentNumberValue = null;
            }
            else
            {
                ApartmentsNumbersCollection = new ObservableCollection<int>();
                SelectedApartmentNumberValue = null;
            }
        }

        #endregion

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
