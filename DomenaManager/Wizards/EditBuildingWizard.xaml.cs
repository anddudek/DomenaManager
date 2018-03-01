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
    public partial class EditBuildingWizard : UserControl, INotifyPropertyChanged
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
        
        public Building _buildingLocalCopy;

        public EditBuildingWizard(Building SelectedBuilding = null)
        {
            DataContext = this;
            InitializeComponent();

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
            }
        }

        public void InitializeCategoriesList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                CategoriesNames = new ObservableCollection<CostCategory>(db.CostCategories.ToList());               
               
            }
        }

        public void InitializeUnitsList()
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
        }

        public ICommand AddCost
        {
            get
            {
                return new Helpers.RelayCommand(AddNewCost, CanAddCost);
            }
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
            LabelError = null;
            var c = new Helpers.CostListView() { BegginingDate = CostBeggining, CategoryName = SelectedCategoryValue, Cost = uc, CostUnit = SelectedUnitName };
            CostCollection.Add(c);
        }

        private bool CanAddCost()
        {
            return true;
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
