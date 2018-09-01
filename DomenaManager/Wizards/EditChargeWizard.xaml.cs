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
    public partial class EditChargeWizard : UserControl, INotifyPropertyChanged
    {
        #region Bindings

        public Charge _charge;

        public bool CanEditApartment { get; set; }

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

        public ObservableCollection<BuildingChargeBasisCategory> Categories { get; set; }

        #endregion

        #region Commands

        public ICommand AddNewCategory
        {
            get { return new RelayCommand(AddNew, CanAddNew); }
        }

        public ICommand UpdateAllFieldsCommand
        {
            get { return new Helpers.RelayCommand(UpdateAllFields, CanUpdateAllFields); }
        }

        public ICommand AddNewChargeCommand
        {
            get { return new Helpers.RelayCommand(AddNewCharge, CanAddNewCharge); }
        }

        public ICommand ModifySelectedChargeCommand
        {
            get { return new RelayCommand(ModifySelectedCharge, CanModifySelectedCharge); }
        }

        public ICommand DeleteSelectedChargeCommand
        {
            get { return new RelayCommand(DeleteSelectedCharge, CanDeleteSelectedCharge); }
        }

        public ICommand SaveCommand
        {
            get { return new RelayCommand(SaveDialog, CanSaveDialog); }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(CancelDialog, CanCancelDialog); }
        }

        #endregion

        public EditChargeWizard(ChargeDataGrid charge)
        {
            _charge = charge;
            if (_charge == null)
            {
                CanEditApartment = true;
                ChargeDate = DateTime.Today;
            }
            else
                CanEditApartment = false;
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

        private void ModifySelectedCharge(object param)
        {
            if (SelectedChargeComponent != null && SelectedCategoryName != null && SelectedUnitName != null)
            {
                SelectedChargeComponent.CostCategoryId = SelectedCategoryName.BuildingChargeBasisCategoryId;
                SelectedChargeComponent.CostDistribution = SelectedUnitName.EnumValue;
                double uc;
                if (!double.TryParse(UnitCost, out uc))
                    return;
                SelectedChargeComponent.CostPerUnit = uc;
                double cs;
                if (!double.TryParse(ChargeSum, out cs))
                    return;
                SelectedChargeComponent.Sum = cs;
            }
        }

        private bool CanModifySelectedCharge()
        {
            return (SelectedChargeComponent != null && UnitCost != null && ChargeSum != null);
        }

        private void AddNewCharge(object param)
        {
            if (SelectedCategoryName != null && SelectedUnitName != null)
            {
                double uc;
                if (!double.TryParse(UnitCost, out uc))
                    return;
                double cs;
                if (!double.TryParse(ChargeSum, out cs))
                    return;
                ChargeComponents.Add(new ChargeComponent() { ChargeComponentId = Guid.NewGuid(), CostCategoryId = SelectedCategoryName.BuildingChargeBasisCategoryId, CostDistribution = SelectedUnitName.EnumValue, CostPerUnit = uc, Sum = cs });
            }
        }

        private bool CanAddNewCharge()
        {
            return (UnitCost != null && ChargeSum != null);
        }

        private void DeleteSelectedCharge(object param)
        {
            if (SelectedChargeComponent != null)
            {
                ChargeComponents.Remove(SelectedChargeComponent);
            }
        }

        private bool CanDeleteSelectedCharge()
        {
            return SelectedChargeComponent != null;
        }

        private void SaveDialog(object param)
        {
            if (_charge == null)
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(SelectedBuildingValue) || string.IsNullOrEmpty(SelectedApartmentNumberValue)))
                {
                    return;
                }
                //Add new apartment
                using (var db = new DB.DomenaDBContext())
                {
                    var newCharge = new LibDataModel.Charge();
                    newCharge.SettlementId = Guid.Empty;
                    newCharge.ChargeId = Guid.NewGuid();
                    newCharge.ApartmentId = db.Apartments.FirstOrDefault(x => x.BuildingId.Equals(SelectedBuilding.BuildingId) && x.ApartmentNumber.Equals(SelectedApartmentNumber)).ApartmentId;
                    newCharge.CreatedDate = DateTime.Today;
                    newCharge.ChargeDate = ChargeDate;
                    newCharge.IsClosed = ChargeStatus == "Otwarte" ? false : true;
                    newCharge.IsDeleted = false;

                    newCharge.Components = new List<ChargeComponent>();
                    foreach (var cc in ChargeComponents)
                    {
                        newCharge.Components.Add(cc);
                    }

                    db.Charges.Add(newCharge);
                    db.SaveChanges();
                }
            }
            else
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(SelectedBuildingValue) || string.IsNullOrEmpty(SelectedApartmentNumberValue)))
                {
                    return;
                }
                //Edit Apartment
                using (var db = new DB.DomenaDBContext())
                {
                    var q = db.Charges.Include(x => x.Components).Where(x => x.ChargeId.Equals(_charge.ChargeId)).FirstOrDefault();
                    q.IsClosed = ChargeStatus == "Otwarte" ? false : true;
                    q.ChargeDate = ChargeDate;
                    //q.Components = new List<ChargeComponent>();
                    foreach (var cc in ChargeComponents)
                    {
                        //q.Components.Add(cc);
                        if (q.Components.Any(x => x.ChargeComponentId.Equals(cc.ChargeComponentId)))
                        {
                            var comp = q.Components.FirstOrDefault(x => x.ChargeComponentId.Equals(cc.ChargeComponentId));
                            comp.CostCategoryId = cc.CostCategoryId;
                            comp.CostDistribution = cc.CostDistribution;
                            comp.CostPerUnit = cc.CostPerUnit;
                            comp.Sum = cc.Sum;
                        }
                        else
                        {
                            q.Components.Add(cc);
                        }
                    }

                    for (int i = q.Components.Count - 1; i >= 0; i--)
                    {
                        if (!ChargeComponents.Any(x => x.ChargeComponentId.Equals(q.Components[i].ChargeComponentId)))
                        {
                            q.Components.RemoveAt(i);
                        }
                    }

                    db.SaveChanges();
                }
            }
            SwitchPage.SwitchMainPage(new Pages.ChargesPage(), this);
        }

        private bool CanSaveDialog()
        {
            return true;
        }

        private void CancelDialog(object param)
        {
            SwitchPage.SwitchMainPage(new Pages.ChargesPage(), this);
        }

        private bool CanCancelDialog()
        {
            return true;
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

        private void CalculateSum()
        { 
            double uc;
            if (!double.TryParse(UnitCost, out uc) || SelectedCategoryName == null || SelectedUnitName == null || SelectedApartmentNumberValue == null || SelectedBuilding == null)
                return;
            double units;
            switch ((EnumCostDistribution.CostDistribution)SelectedUnitName.EnumValue)
            {
                case EnumCostDistribution.CostDistribution.PerApartment:
                    units = 1;
                    break;
                case EnumCostDistribution.CostDistribution.PerMeasurement:
                    var a = ApartmentsCollection.FirstOrDefault(x => x.BuildingId.Equals(SelectedBuilding.BuildingId) && x.ApartmentNumber.Equals(SelectedApartmentNumber));
                    units = a.AdditionalArea + a.ApartmentArea;
                    break;
                case EnumCostDistribution.CostDistribution.PerLocators:
                    var ap = ApartmentsCollection.FirstOrDefault(x => x.BuildingId.Equals(SelectedBuilding.BuildingId) && x.ApartmentNumber.Equals(SelectedApartmentNumber));
                    units = ap.Locators;
                    break;
                default:
                    units = 0;
                    break;
            }
            ChargeSum = (units * uc).ToString();
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
                InitializeCategoriesList();
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
