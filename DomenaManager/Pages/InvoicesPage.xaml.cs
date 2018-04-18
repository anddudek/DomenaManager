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
using DomenaManager.Helpers;
using System.Windows.Threading;
using LibDataModel;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Collections;

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for OwnersPage.xaml
    /// </summary>
    public partial class InvoicesPage : UserControl, INotifyPropertyChanged
    {
        private IList _selectedInvoicesList;
        public IList SelectedInvoicesList
        {
            get { return _selectedInvoicesList; }
            set
            {
                if (value != _selectedInvoicesList)
                {
                    _selectedInvoicesList = value;
                    OnPropertyChanged("SelectedInvoicesList");
                }
            }
        }

        public ObservableCollection<InvoiceCategory> Categories { get; set; }

        private ObservableCollection<InvoiceDataGrid> _invoices;
        public ObservableCollection<InvoiceDataGrid> Invoices
        {
            get { return _invoices; }
            set
            {
                _invoices = value;
                OnPropertyChanged("Invoices");
            }
        }

        private ICollectionView _invoicesCV;
        public ICollectionView InvoicesCV
        {
            get
            {
                return _invoicesCV;
            }
            set
            {
                if (value != _invoicesCV)
                {
                    _invoicesCV = value;
                    OnPropertyChanged("InvoicesCV");
                }
            }
        }

        private InvoiceDataGrid _selectedInvoice;
        public InvoiceDataGrid SelectedInvoice
        {
            get { return _selectedInvoice; }
            set
            {
                _selectedInvoice = value;
                OnPropertyChanged("SelectedInvoice");                
            }
        }

        private bool _groupByBuilding;
        public bool GroupByBuilding
        {
            get { return _groupByBuilding; }
            set
            { 
                
                    ICollectionView cvInvoices = (CollectionView)CollectionViewSource.GetDefaultView(Invoices);
                    if (value)
                    {
                        cvInvoices.GroupDescriptions.Add(new PropertyGroupDescription("Building.Name")); 
                    }
                    else
                    {
                        cvInvoices.GroupDescriptions.Remove(cvInvoices.GroupDescriptions.Cast<PropertyGroupDescription>().Where(x => x.PropertyName == "Building.Name").FirstOrDefault());
                    }
                    _groupByBuilding = value;
                    OnPropertyChanged("GroupByBuilding");
                
            }
        }

        public ICommand DeleteInvoiceCommand
        {
            get { return new Helpers.RelayCommand(Delete, CanDelete);}
        }

        private bool CanDelete()
        {
            return SelectedInvoice != null;
        }

        private void Delete(object param)
        {
            using (var db = new DB.DomenaDBContext())
            {
                db.Invoices.Where(x => x.InvoiceId.Equals(SelectedInvoice.InvoiceId)).FirstOrDefault().IsDeleted = true;
                db.SaveChanges();
            }
            InitializeCollection();
        } 

        public ICommand EditInvoiceCommand
        {
            get { return new Helpers.RelayCommand(Edit, CanEdit); }
        }

        private bool CanEdit()
        {
            return SelectedInvoice != null;
        }

        private async void Edit(object param)
        {
            if (SelectedInvoice != null)
            {
                var eiw = new Wizards.EditInvoiceWizard(SelectedInvoice);
                var result = await DialogHost.Show(eiw, "RootDialog", ExtendedEIWOpenedEventHandler, ExtendedEIWClosingEventHandler);
            }
        }

        public ICommand AddInvoiceCommand
        {
            get { return new Helpers.RelayCommand(Add, CanAdd); }
        }

        private bool CanAdd()
        {
            return true;
        }

        private async void Add(object param)
        {
            var eiw = new Wizards.EditInvoiceWizard();
            var result = await DialogHost.Show(eiw, "RootDialog", ExtendedEIWOpenedEventHandler, ExtendedEIWClosingEventHandler);
        }

        public ICommand ShowInvoiceDetails
        {
            get { return new Helpers.RelayCommand(ShowDetails, CanShowDetails); }
        }

        private bool CanShowDetails()
        {
            return SelectedInvoice != null;
        }

        private async void ShowDetails(object param)
        {
            var eiw = new Wizards.EditInvoiceWizard(SelectedInvoice);
            var result = await DialogHost.Show(eiw, "RootDialog", ExtendedEIWOpenedEventHandler, ExtendedEIWClosingEventHandler);
        }

        private ObservableCollection<Building> _buildingsNames;
        public ObservableCollection<Building> BuildingsNames
        {
            get { return _buildingsNames; }
            set
            {
                _buildingsNames = value;
                OnPropertyChanged("BuildingsNames");
            }
        }

        private Building _selectedBuildingName;
        public Building SelectedBuildingName
        {
            get { return _selectedBuildingName; }
            set
            {
                _selectedBuildingName = value;
                OnPropertyChanged("SelectedBuildingName");
                InvoicesCV.Refresh();
            }
        }

        private ObservableCollection<int> _apartmentsNumbers;
        public ObservableCollection<int> ApartmentsNumbers
        {
            get { return _apartmentsNumbers; }
            set
            {
                if (value != _apartmentsNumbers)
                {
                    _apartmentsNumbers = value;
                    OnPropertyChanged("ApartmentsNumbers");
                }
            }
        }
        
        public ICommand ClearFilterCommand
        {
            get
            {
                return new Helpers.RelayCommand(ClearFilter, CanClearFilter);
            }
        }

        public InvoicesPage()
        {
            DataContext = this;
            InitializeCollection();
            InitializeCategories();
            InitializeLists();
            SelectedInvoicesList = new List<InvoiceDataGrid>();
            InitializeComponent();
            GroupByBuilding = true;
        }

        private void InitializeCollection()
        {
            Invoices = new ObservableCollection<InvoiceDataGrid>();
            using (var db = new DB.DomenaDBContext())
            {
                var q = db.Invoices.Where(x => x.IsDeleted == false);
                foreach (var inv in q)
                {
                    var idg = new InvoiceDataGrid(inv);
                    Invoices.Add(idg); 
                }
            }
            
            InvoicesCV = (CollectionView)CollectionViewSource.GetDefaultView(Invoices);
            InvoicesCV.SortDescriptions.Add(new SortDescription("CreatedTime", ListSortDirection.Ascending));
            InvoicesCV.Filter = FilterCollection;

            GroupByBuilding = GroupByBuilding;
        }

        private void InitializeLists()
        {
            using (var db = new DB.DomenaDBContext())
            {
                _buildingsNames = new ObservableCollection<Building>(db.Buildings.ToList());
            }
        }

        private void InitializeCategories()
        {            
            using (var db = new DB.DomenaDBContext())
            {
                Categories = new ObservableCollection<InvoiceCategory>(db.InvoiceCategories.ToList());
            }
        }

        private void ClearFilter(object param)
        {
            SelectedBuildingName = null;
            InvoicesCV.Refresh();
        }

        private bool CanClearFilter()
        {
            return true;
        }

        private bool FilterCollection(object item)
        {
            var idg = item as InvoiceDataGrid;
            if (SelectedBuildingName != null && !idg.Building.BuildingId.Equals(SelectedBuildingName.BuildingId))
            {
                return false;
            }
            
            return true;
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

        private async void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {

        }

        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private void ExtendedEIWOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private async void ExtendedEIWClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            
            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditInvoiceWizard);
                //Accept
                if (dc._lic == null)
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.SelectedBuildingName.Name) || string.IsNullOrEmpty(dc.SelectedCategoryName.CategoryName)))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Add new invoice
                    var newInvoice = new LibDataModel.Invoice { BuildingId = dc.SelectedBuildingName.BuildingId, ContractorName = dc.SelectedContractorsValue, CostAmount = double.Parse(dc.CostAmount), CreatedTime = DateTime.Now, InvoiceCategoryId = dc.SelectedCategoryName.CategoryId, InvoiceDate = dc.InvoiceDate.Date, InvoiceId = Guid.NewGuid(), InvoiceNumber = dc.InvoiceNumber, IsDeleted = false };
                    using (var db = new DB.DomenaDBContext())
                    {
                        db.Invoices.Add(newInvoice);

                        if (!db.InvoiceContractors.Any(x => x.Name.Equals(dc.SelectedContractorsValue)))
                        {
                            db.InvoiceContractors.Add(new ContractorsName() { Name = dc.SelectedContractorsValue });
                        }

                        db.SaveChanges();
                    }
                    InitializeCollection();
                }
                else
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.SelectedBuildingName.Name) || string.IsNullOrEmpty(dc.SelectedCategoryName.CategoryName)))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Edit Invoice
                    using (var db = new DB.DomenaDBContext())
                    {
                        var q = db.Invoices.Where(x => x.InvoiceId.Equals(dc._lic.InvoiceId)).FirstOrDefault();
                        q.BuildingId = dc.SelectedBuildingName.BuildingId;
                        q.ContractorName = dc.SelectedContractorsValue;
                        q.CostAmount = double.Parse(dc.CostAmount);
                        q.CreatedTime = DateTime.Now;
                        q.InvoiceCategoryId = dc.SelectedCategoryName.CategoryId;
                        q.InvoiceDate = dc.InvoiceDate.Date;
                        q.InvoiceNumber = dc.InvoiceNumber;

                        if (!db.InvoiceContractors.Any(x => x.Name.Equals(dc.SelectedContractorsValue)))
                        {
                            db.InvoiceContractors.Add(new ContractorsName() { Name = dc.SelectedContractorsValue });
                        }
                        
                        db.SaveChanges();
                    }
                    InitializeCollection();
                }
            }
            else if (!(bool)eventArgs.Parameter)
            {

                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    //eventArgs.Cancel();
                    var dc = (eventArgs.Session.Content as Wizards.EditInvoiceWizard);
                    var result = await DialogHost.Show(dc, "HelperDialog", ExtendedEIWOpenedEventHandler, ExtendedEIWClosingEventHandler);
                }
            }
             
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
