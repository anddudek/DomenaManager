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
                    }
                    OnPropertyChanged("SelectedCost");
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

                foreach (var c in SelectedBuilding.CostCollection)
                {
                    var clv = new Helpers.CostListView { BegginingDate = c.BegginingDate.Date, EndingDate = c.EndingDate.Date, Cost = c.CostPerUnit, CostUnit =  UnitsNames.Where(x => x.EnumValue == c.CostDistribution).FirstOrDefault(), CategoryName = CategoriesNames.Where(x => x.CostCategoryId.Equals(c.CostCategoryId)).FirstOrDefault().CategoryName };
                    CostCollection.Add(clv);
                }
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
            cvCostCollection.SortDescriptions.Add(new SortDescription("BegginingDate", ListSortDirection.Ascending));
        }

        public ICommand DeleteSelectedCost
        {
            get
            {
                return new Helpers.RelayCommand(DeleteCost, CanDeleteCost);
            }
        }

        private void DeleteCost(object param)
        {
            CostCollection.Remove(SelectedCost);
        }

        private bool CanDeleteCost()
        {
            return SelectedCost != null;
        }

        public ICommand ModifySelectedCost
        {
            get
            {
                return new Helpers.RelayCommand(ModifyCost, CanModifyCost);
            }
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
            var q = CostCollection.Where(x => x.BegginingDate.Date.CompareTo(CostBeggining.Date) == 0 && x.CategoryName == SelectedCategoryValue).Count();
            if (q > 0)
            {
                LabelError = "Istnieje już rekord z taką samą kategorią i datą";
                return;
            }
            LabelError = null;
            var endingDate = new DateTime(1900, 01, 01);

            // Add to the end 
            var last = CostCollection.Where(x => Helpers.DateTimeOperations.IsDateNull(x.EndingDate)).FirstOrDefault();

            if (last != null && last.BegginingDate.Date < CostBeggining.Date)
            {
                last.EndingDate = CostBeggining.AddDays(-1);
            }
            else
            {
                var before = CostCollection.Where(x => x.BegginingDate.Date < CostBeggining.Date && x.EndingDate.Date >= CostBeggining.Date).FirstOrDefault();
                // Add in the middle of collection
                if (before != null)
                {
                    endingDate = before.EndingDate.Date;
                    before.EndingDate = CostBeggining.Date.AddDays(-1);
                }
                else
                {
                    // Add at the beggining of collection
                    var first = CostCollection.OrderBy(x => x.BegginingDate).FirstOrDefault();
                    if (first != null && first.BegginingDate.Date > CostBeggining.Date)
                    {
                        endingDate = first.BegginingDate.Date.AddDays(-1);
                    }
                    else
                    {
                        if (CostCollection.Where(x => x.CategoryName == SelectedCategoryValue).Count() > 0)
                        {
                            LabelError = "Błąd dodawania kosztu";
                            return;
                        }
                    }
                }
            }

            var c = new Helpers.CostListView() { BegginingDate = CostBeggining, CategoryName = SelectedCategoryValue, Cost = uc, CostUnit = SelectedUnitName, EndingDate = endingDate };

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
