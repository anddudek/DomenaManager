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
                    var clv = new Helpers.CostListView { BegginingDate = c.BegginingDate.Date, EndingDate = c.EndingDate.Date, Cost = c.CostPerUnit, CostUnit =  UnitsNames.Where(x => x.EnumValue == c.BuildingChargeBasisDistribution).FirstOrDefault(), CategoryName = CategoriesNames.Where(x => x.BuildingChargeBasisCategoryId.Equals(c.BuildingChargeBasisCategoryId)).FirstOrDefault().CategoryName };
                    CostCollection.Add(clv);
                }
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
            var q = CostCollection.Where(x => x.BegginingDate.Date.CompareTo(CostBeggining.Date) == 0 && x.CategoryName == SelectedCategoryValue).Count();
            if (q > 0)
            {
                LabelError = "Istnieje już rekord z taką samą kategorią i datą";
                return;
            }
            LabelError = null;
            var endingDate = new DateTime(1900, 01, 01);

            /*
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
            }*/

            var c = new Helpers.CostListView() { BegginingDate = CostBeggining, CategoryName = SelectedCategoryValue, Cost = uc, CostUnit = SelectedUnitName, EndingDate = endingDate };

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

        private bool CanAddNewCat()
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
