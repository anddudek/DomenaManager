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
using System.Data.Entity;
using MaterialDesignThemes.Wpf;

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for BuildingsPage.xaml
    /// </summary>
    public partial class BuildingsPage : UserControl, INotifyPropertyChanged
    {
        #region Bindings

        private ObservableCollection<Helpers.BuildingDataGrid> _buildings;
        public ObservableCollection<Helpers.BuildingDataGrid> Buildings
        {
            get { return _buildings; }
            set
            { 
                _buildings = value;
                OnPropertyChanged("Buildings");
            }
        }

        private Helpers.BuildingDataGrid _selectedBuilding;
        public Helpers.BuildingDataGrid SelectedBuilding
        {
            get { return _selectedBuilding; }
            set
            {
                _selectedBuilding = value;
                OnPropertyChanged("SelectedBuilding");
            }
        }

        public ICommand AddBuildingCommand
        {
            get
            {
                return new Helpers.RelayCommand(AddBuilding, CanAddBuilding);
            }
        }

        public ICommand EditBuildingCommand
        {
            get
            {
                return new Helpers.RelayCommand(EditBuilding, CanEditBuilding);
            }
        }

        public ICommand DeleteBuildingCommand
        {
            get
            {
                return new Helpers.RelayCommand(DeleteBuilding, CanDeleteBuilding);
            }
        }

        #endregion

        public BuildingsPage()
        {
            DataContext = this;
            InitializeComponent();
            InitializeCollection();
            
        }

        public void InitializeCollection()
        {
            Buildings = new ObservableCollection<Helpers.BuildingDataGrid>();
            using (var db = new DB.DomenaDBContext())
            {
                var q = db.Buildings.Include(x => x.CostCollection).Where(x => x.IsDeleted == false);
                foreach (var build in q)
                {
                    var b = new Helpers.BuildingDataGrid { Name = build.Name, ApartmentsCount = db.Apartments.Where(x => !x.IsDeleted && x.BuildingId.Equals(build.BuildingId)).Count() };
                    b.BuildingId = build.BuildingId;
                    b.Address = build.GetAddress();
                    Buildings.Add(b);
                }

                foreach (var b in Buildings)
                {
                    b.CostsList = new List<Helpers.BuildingDescriptionListView>();
                    //var costs = db.Costs.Where(x => x.BuildingId == b.BuildingId && (DbFunctions.TruncateTime(DbFunctions.AddMonths(DateTime.Now, -1))).Value.Month == DbFunctions.TruncateTime(x.PaymentTime).Value.Month && (DbFunctions.TruncateTime(DbFunctions.AddMonths(DateTime.Now, -1))).Value.Year == DbFunctions.TruncateTime(x.PaymentTime).Value.Year);

                    //foreach (var c in costs)
                    //{
                        //b.CostsList.Add(new Helpers.BuildingDescriptionListView { Category = db.CostCategories.Where(x => x.CostCategoryId == c.CostCategoryId).FirstOrDefault().CategoryName, CostString = c.CostAmount + " zł", DateString = c.PaymentTime.ToString("yyyy-MM-dd") });
                    //}
                    if (b.CostsList.Count == 0)
                    {
                        b.CostsList.Add(new Helpers.BuildingDescriptionListView { Category = "Brak kosztów" });
                    }
                }
            }
        }

        private bool CanAddBuilding()
        {
            return true;
        }

        private async void AddBuilding(object obj)
        {
            Wizards.EditBuildingWizard ebw = new Wizards.EditBuildingWizard();

            var result = await DialogHost.Show(ebw, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
        }

        private bool CanEditBuilding()
        {
            return (SelectedBuilding != null);
        }

        private async void EditBuilding(object obj)
        {
            Wizards.EditBuildingWizard ebw;
            using (var db = new DB.DomenaDBContext())
            {
                var sb = db.Buildings.Include(x => x.CostCollection).Where(x => x.Name.Equals(SelectedBuilding.Name)).FirstOrDefault();
                ebw = new Wizards.EditBuildingWizard(sb);
            }

            var result = await DialogHost.Show(ebw, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
        }

        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {
            
        }

        private async void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            
            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditBuildingWizard);
                //Accept
                if (dc._buildingLocalCopy == null)
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.BuildingName) || string.IsNullOrEmpty(dc.BuildingCity) || string.IsNullOrEmpty(dc.BuildingZipCode) || string.IsNullOrEmpty(dc.BuildingRoadName) || string.IsNullOrEmpty(dc.BuildingRoadNumber)))
        
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Add new building
                    using (var db = new DB.DomenaDBContext())
                    {
                        var newBuilding = new LibDataModel.Building { BuildingId = Guid.NewGuid(), Name = dc.BuildingName, City = dc.BuildingCity, ZipCode = dc.BuildingZipCode, BuildingNumber = dc.BuildingRoadNumber, RoadName = dc.BuildingRoadName, IsDeleted = false };
                        List<LibDataModel.Cost> costs = new List<LibDataModel.Cost>();
                        foreach (var c in dc.CostCollection)
                        {
                            var catId = db.CostCategories.Where(x => x.CategoryName.Equals(c.CategoryName)).FirstOrDefault().CostCategoryId;
                            var cost = new LibDataModel.Cost { CostId = Guid.NewGuid(), BegginingDate = c.BegginingDate.Date, EndingDate = c.EndingDate.Date, CostPerUnit = c.Cost, CostDistribution = c.CostUnit.EnumValue, CostCategoryId=catId };
                            costs.Add(cost);
                        }
                        newBuilding.CostCollection = costs;
                        db.Buildings.Add(newBuilding);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.BuildingName) || string.IsNullOrEmpty(dc.BuildingCity) || string.IsNullOrEmpty(dc.BuildingZipCode) || string.IsNullOrEmpty(dc.BuildingRoadName) || string.IsNullOrEmpty(dc.BuildingRoadNumber)))

                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Edit building
                    using (var db = new DB.DomenaDBContext())
                    {
                        var q = db.Buildings.Where(x => x.BuildingId.Equals(dc._buildingLocalCopy.BuildingId)).FirstOrDefault();
                        q.BuildingNumber = dc.BuildingRoadNumber;
                        q.City = dc.BuildingCity;
                        q.Name = dc.BuildingName;
                        q.RoadName = dc.BuildingRoadName;
                        q.ZipCode = dc.BuildingZipCode;

                        List<LibDataModel.Cost> costs = new List<LibDataModel.Cost>();
                        foreach (var c in dc.CostCollection)
                        {
                            var catId = db.CostCategories.Where(x => x.CategoryName.Equals(c.CategoryName)).FirstOrDefault().CostCategoryId;
                            var cost = new LibDataModel.Cost { CostId = Guid.NewGuid(), BegginingDate = c.BegginingDate.Date, EndingDate = c.EndingDate.Date, CostPerUnit = c.Cost, CostDistribution = c.CostUnit.EnumValue, CostCategoryId = catId };
                            costs.Add(cost);
                        }
                        q.CostCollection = costs;

                        db.SaveChanges();
                    }
                }
            }
            else if (!(bool)eventArgs.Parameter)
            {
                
                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    //eventArgs.Cancel();
                    var dc = (eventArgs.Session.Content as Wizards.EditBuildingWizard);
                    var result = await DialogHost.Show(dc, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
                }
            }
            InitializeCollection();
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

        private bool CanDeleteBuilding()
        {
            return (SelectedBuilding != null);
        }

        private async void DeleteBuilding(object obj)
        {
            bool ynResult = await Helpers.YNMsg.Show("Czy chcesz usunąć budynek " + SelectedBuilding.Name + "?");
            if (ynResult)
            {
                using (var db = new DB.DomenaDBContext())
                {
                    db.Buildings.Where(x => x.BuildingId.Equals(SelectedBuilding.BuildingId)).FirstOrDefault().IsDeleted = true;
                    db.SaveChanges();
                }
            }
            InitializeCollection();
            
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
