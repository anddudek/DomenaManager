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
using System.IO;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditChargeWizard.xaml
    /// </summary>
    public partial class PreviewChargeWizard : UserControl, INotifyPropertyChanged
    {
        #region Bindings

        public Charge _charge;
                
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
                    OwnerName = ow.OwnerName +Environment.NewLine + ow.MailAddress;
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
        
        public string ComponentsSum
        {
            get
            {
                if (ChargeComponents != null && ChargeComponents.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    var groups = ChargeComponents.Select(x => x.GroupName).Distinct();
                    foreach (var g in groups)
                    {
                        sb.Append("Razem: ");
                        sb.Append(g.GroupName);
                        sb.Append(": ");
                        sb.Append(ChargeComponents.Where(x => x.GroupName.BuildingChargeGroupNameId == g.BuildingChargeGroupNameId).Select(x => x.Sum).DefaultIfEmpty(0).Sum());
                        sb.Append(" zł");
                        sb.Append(Environment.NewLine);
                    }
                    return sb.ToString();
                }
                else
                {
                    return "";
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
                    CalculateSum();
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
                    CalculateSum();
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

        public BuildingChargeGroupName _selectedGroupName;
        public BuildingChargeGroupName SelectedGroupName
        {
            get
            {
                return _selectedGroupName;
            }
            set
            {
                if (value != _selectedGroupName)
                {
                    _selectedGroupName = value;
                    OnPropertyChanged("SelectedGroupName");
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
                    OnPropertyChanged("SelectedChargeComponent");
                    if (value != null)
                    {
                        SelectedCategoryName = CategoriesNames.Where(x => x.BuildingChargeBasisCategoryId.Equals(_selectedChargeComponent.CostCategoryId)).FirstOrDefault();
                        SelectedUnitName = UnitsNames.Where(x => x.EnumValue.Equals(_selectedChargeComponent.CostDistribution)).FirstOrDefault();
                        UnitCost = _selectedChargeComponent.CostPerUnit.ToString();
                        ChargeSum = _selectedChargeComponent.Sum.ToString();
                        SelectedGroupName = GroupNames.Where(x => x.BuildingChargeGroupNameId.Equals(_selectedChargeComponent.GroupName.BuildingChargeGroupNameId)).FirstOrDefault();
                    }
                }
            }
        }

        private DateTime _chargeDate;
        public DateTime ChargeDate
        {
            get { return _chargeDate; }
            set
            {
                if (value != _chargeDate)
                {
                    _chargeDate = value;
                    OnPropertyChanged("ChargeDate");
                }
            }
        }

        public string SelectedGroupNameValue { get; set; }

        public ObservableCollection<BuildingChargeBasisCategory> Categories { get; set; }
        public ObservableCollection<BuildingChargeGroupName> Groups { get; set; }

        #endregion

        #region Commands       

        public ICommand UpdateAllFieldsCommand
        {
            get { return new Helpers.RelayCommand(UpdateAllFields, CanUpdateAllFields); }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(CancelDialog, CanCancelDialog); }
        }

        #endregion

        public PreviewChargeWizard(ChargeDataGrid charge)
        {
            _charge = charge;
            if (_charge == null)
            {
                ChargeDate = DateTime.Today;
            }
            else
            
            DataContext = this;
            InitializeComponent();
            InitializeChargeStatusCollection();
            InitializeBuildingsList();
            InitializeCategoriesList();
            InitializeUnitsList();
            InitializeCategories();
            LoadCharge(charge);
            InitializeChargeComponents(charge);      
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
                ChargeDate = charge.ChargeDate;
                SelectedBuilding = BuildingsCollection.FirstOrDefault(x => x.BuildingId.Equals(charge.Building.BuildingId));
                //ApartmentNumber = charge.Apartment.ApartmentNumber;
                SelectedApartmentNumber = ApartmentsNumbersCollection.FirstOrDefault(x => x.Equals(charge.Apartment.ApartmentNumber));
                OwnerName = charge.Owner.OwnerName + Environment.NewLine + charge.Owner.MailAddress;
            }
        }

        private void InitializeCategoriesList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                CategoriesNames = new ObservableCollection<BuildingChargeBasisCategory>(db.CostCategories.Where(x => !x.IsDeleted).ToList());
                Groups = new ObservableCollection<BuildingChargeGroupName>(db.GroupName.Where(x => !x.IsDeleted).ToList());
                GroupNames = Groups;
            }
        }

        private void InitializeUnitsList()
        {
            var values = (CostDistribution[])Enum.GetValues(typeof(CostDistribution));
            UnitsNames = new ObservableCollection<Helpers.CostDistributionCollectionItem>();
            foreach (var v in values)
            {
                var cdci = new Helpers.CostDistributionCollectionItem(v);
                UnitsNames.Add(cdci);
            }
        }

        private void InitializeChargeComponents(ChargeDataGrid charge)
        {
            if (charge != null)
            {
                ChargeComponents = new ObservableCollection<ChargeComponent>(charge.Components);            
            }
            else ChargeComponents = new ObservableCollection<ChargeComponent>();
            OnPropertyChanged("ComponentsSum");
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

        private void UpdateAllFields(object param)
        {
            Helpers.Validator.IsValid(this);
        }

        private bool CanUpdateAllFields()
        {
            return true;
        }

        private void CancelDialog(object param)
        {
            SwitchPage.SwitchMainPage(new Pages.MonthlyChargesPage(), this);
        }

        private bool CanCancelDialog()
        {
            return true;
        }

        private void UpdateApartmentsNumbers()
        {
            if (SelectedBuilding != null)
            {
                ApartmentsNumbersCollection = new ObservableCollection<int>(ApartmentsCollection.Where(x => x.BuildingId.Equals(SelectedBuilding.BuildingId)).Select(x => x.ApartmentNumber).ToList().OrderBy(x => x));
                SelectedApartmentNumberValue = null;
            }
            else
            {
                ApartmentsNumbersCollection = new ObservableCollection<int>();
                SelectedApartmentNumberValue = null;
            }
        }

        private void CalculateSum()
        { 
            double uc;
            if (!double.TryParse(UnitCost, out uc) || SelectedCategoryName == null || SelectedUnitName == null || SelectedApartmentNumberValue == null || SelectedBuilding == null)
                return;
            double units;
            var a = ApartmentsCollection.FirstOrDefault(x => x.BuildingId.Equals(SelectedBuilding.BuildingId) && x.ApartmentNumber.Equals(SelectedApartmentNumber));
            switch ((CostDistribution)SelectedUnitName.EnumValue)
            {
                case CostDistribution.PerApartment:
                    units = 1;
                    break;
                case CostDistribution.PerApartmentTotalArea:                    
                    units = a.AdditionalArea + a.ApartmentArea;
                    break;
                case CostDistribution.PerApartmentArea:
                    units = a.ApartmentArea;
                    break;
                case CostDistribution.PerAdditionalArea:
                    units = a.AdditionalArea;
                    break;
                case CostDistribution.PerLocators:
                    var ap = ApartmentsCollection.FirstOrDefault(x => x.BuildingId.Equals(SelectedBuilding.BuildingId) && x.ApartmentNumber.Equals(SelectedApartmentNumber));
                    units = ap.Locators;
                    break;
                default:
                    units = 0;
                    break;
            }
            ChargeSum = (Math.Round((units * uc), 2)).ToString();
        }
        
        #endregion
        
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
