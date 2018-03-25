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

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditChargeWizard.xaml
    /// </summary>
    public partial class EditChargeWizard : UserControl, INotifyPropertyChanged
    {
        #region Bindings

        private Guid ChargeId { get; set; }

        private string _buildingName;
        public string BuildingName
        {
            get { return _buildingName; }
            set
            {
                if (value != _buildingName)
                {
                    _buildingName = value;
                    OnPropertyChanged("BuildingName");
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
            }
        }

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

        private ObservableCollection<CostCategory> _categoriesNames;
        public ObservableCollection<CostCategory> CategoriesNames
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

        private CostCategory _selectedCategoryName;
        public CostCategory SelectedCategoryName
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
                    OnPropertyChanged("SelectedChargeComponent");
                }
            }
        }

        public ObservableCollection<CostCategory> Categories { get; set; }

        #endregion

        #region Commands

        #endregion

        public EditChargeWizard(ChargeDataGrid charge)
        {
            DataContext = this;
            InitializeComponent();
            InitializeChargeStatusCollection();
            ChargeId = charge.ChargeId;
            LoadCharge(charge);
            InitializeCategoriesList();
            InitializeUnitsList();
            InitializeChargeComponents();
            InitializeCategories();
        }

        #region Functions

        private void InitializeChargeStatusCollection()
        {
            ChargeStatusCollection = new List<string>() { "Otwarte", "Zamknięte" };
        }

        private void LoadCharge(ChargeDataGrid charge)
        {
            BuildingName = charge.Building.Name;
            ApartmentNumber = charge.Apartment.ApartmentNumber;
            OwnerName = charge.Owner.OwnerName + Environment.NewLine + charge.Owner.MailAddress;
            ChargeStatus = charge.IsClosed ? ChargeStatusCollection[1] : ChargeStatusCollection[0];
        }

        private void InitializeCategoriesList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                CategoriesNames = new ObservableCollection<CostCategory>(db.CostCategories.Where(x => !x.IsDeleted).ToList());

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

        private void InitializeChargeComponents()
        {
            using (var db = new DB.DomenaDBContext())
            {
                ChargeComponents = new ObservableCollection<ChargeComponent>(db.Charges.Include(c => c.Components).Where(x => x.ChargeId.Equals(ChargeId)).FirstOrDefault().Components.ToList());
            }            
        }

        private void InitializeCategories()
        {
            using (var db = new DB.DomenaDBContext())
            {
                Categories = new ObservableCollection<CostCategory>(db.CostCategories.ToList());
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
