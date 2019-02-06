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

        public ICommand ShowRecordDetails
        {
            get { return new Helpers.RelayCommand(ShowDetails, CanShowDetails); }
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
                var q = db.Buildings.Include(x => x.CostCollection).Include(x => x.MeterCollection).Where(x => x.IsDeleted == false);
                foreach (var build in q)
                {
                    var b = new Helpers.BuildingDataGrid { Name = build.Name, ApartmentsCount = db.Apartments.Where(x => !x.IsDeleted && x.SoldDate == null && x.BuildingId.Equals(build.BuildingId)).Count() };
                    b.BuildingId = build.BuildingId;
                    b.Address = build.GetAddress();
                    Buildings.Add(b);
                }

                foreach (var b in Buildings)
                {
                    b.CostsList = new List<Helpers.BuildingDescriptionListView>();

                    var invoices = db.Invoices.Where(x => !x.IsDeleted && x.BuildingId.Equals(b.BuildingId)).OrderByDescending(x => x.InvoiceDate).Take(5);
                    foreach (var inv in invoices)
                    {
                        b.CostsList.Add(new Helpers.BuildingDescriptionListView()
                        {
                            Category = db.InvoiceCategories.FirstOrDefault(x => x.CategoryId.Equals(inv.InvoiceCategoryId)).CategoryName,
                            CostString = inv.CostAmount + " zł",
                            DateString = inv.InvoiceDate.ToString("dd-MM-yyyy")
                        });
                    }
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

        private void AddBuilding(object obj)
        {
            Helpers.SwitchPage.SwitchMainPage(new Wizards.EditBuildingWizard(), this);
        }

        private bool CanEditBuilding()
        {
            return (SelectedBuilding != null);
        }

        private void EditBuilding(object obj)
        {
            Wizards.EditBuildingWizard ebw;
            using (var db = new DB.DomenaDBContext())
            {
                var sb = db.Buildings.Include(x => x.CostCollection).Include(x => x.MeterCollection).Where(x => x.Name.Equals(SelectedBuilding.Name)).FirstOrDefault();
                ebw = new Wizards.EditBuildingWizard(sb);
            }
            Helpers.SwitchPage.SwitchMainPage(ebw, this);
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

        private bool CanShowDetails()
        {
            return CanEditBuilding();
        }

        private void ShowDetails(object param)
        {
            EditBuilding(null);
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
