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
using MaterialDesignThemes.Wpf;
using DomenaManager.Helpers;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditInvoiceWizard.xaml
    /// </summary>
    public partial class EditInvoiceWizard : UserControl, INotifyPropertyChanged
    {
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
                OnPropertyChanged("SelectedBuildingAddress");
            }
        }

        public string SelectedBuildingValue { get; set; }

        private ObservableCollection<InvoiceCategory> _categoriesNames;
        public ObservableCollection<InvoiceCategory> CategoriesNames
        {
            get { return _categoriesNames; }
            set
            {
                _categoriesNames = value;
                OnPropertyChanged("CategoriesNames");
            }
        }

        private InvoiceCategory _selectedCategoryName;
        public InvoiceCategory SelectedCategoryName
        {
            get { return _selectedCategoryName; }
            set
            {
                _selectedCategoryName = value;
                OnPropertyChanged("SelectedCategoryName");
            }
        }

        public string SelectedCategoryValue { get; set; }

        private ObservableCollection<ContractorsName> _contractorsNames;
        public ObservableCollection<ContractorsName> ContractorsNames
        {
            get { return _contractorsNames; }
            set
            {
                _contractorsNames = value;
                OnPropertyChanged("ContractorsNames");
            }
        }

        private ContractorsName _selectedContractorsName;
        public ContractorsName SelectedContractorsName
        {
            get { return _selectedContractorsName; }
            set
            {
                _selectedContractorsName = value;
                OnPropertyChanged("SelectedContractorsName");
            }
        }

        public string SelectedContractorsValue { get; set; }

        private string _costAmount;
        public string CostAmount
        {
            get { return _costAmount; }
            set
            {
                if (value != _costAmount)
                {
                    _costAmount = value;
                    OnPropertyChanged("CostAmount");
                }
            }
        }

        private string _invoiceNumber;
        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set
            {
                if (value != _invoiceNumber)
                {
                    _invoiceNumber = value;
                    OnPropertyChanged("InvoiceNumber");
                }
            }
        }

        private string _selectedBuildingAddress;
        public string SelectedBuildingAddress
        {
            get
            {
                _selectedBuildingAddress = _selectedBuildingName != null ? _selectedBuildingName.GetAddress() : null;
                return _selectedBuildingAddress;
            }
            set
            {
                _selectedBuildingAddress = value;
                OnPropertyChanged("SelectedBuildingAddress");
            }
        }

        private List<string> _settlementOptions;
        public List<string> SettlementOptions
        {
            get
            {
                if (_settlementOptions == null)
                    _settlementOptions =  new List<string>() { "Tak", "Nie" };
                return _settlementOptions;
            }
            set { return; }
        }

        private string _isSettled;
        public string IsSettled
        {
            get { return _isSettled; }
            set
            {
                if (value != null)
                {
                    _isSettled = value;
                    OnPropertyChanged("IsSettled");
                }
            }
        }

        public string SettlementOptionsValue { get; set; }

        private DateTime _invoiceDate;
        public DateTime InvoiceDate
        {
            get { return _invoiceDate; }
            set
            {
                _invoiceDate = value;
                OnPropertyChanged("InvoiceDate");
            }
        }

        public ICommand UpdateAllFieldsCommand
        {
            get
            {
                return new Helpers.RelayCommand(UpdateAllFields, CanUpdateAllFields);
            }
        }

        public ICommand AddCategoryCommand
        {
            get
            {
                return new Helpers.RelayCommand(AddCategory, CanAddCategory);
            }
        }

        public Invoice _lic;

        public EditInvoiceWizard(Invoice invoice = null)
        {
            DataContext = this;
            InitializeComponent();
            InitializeBuildingsList();
            InitializeCategoriesList();
            InitializeContractorsList();

            if (invoice != null)
            {
                _lic = invoice;
                SelectedBuildingName = BuildingsNames.Where(x => x.BuildingId.Equals(_lic.BuildingId)).FirstOrDefault();
                SelectedCategoryName = CategoriesNames.Where(x => x.CategoryId.Equals(_lic.InvoiceCategoryId)).FirstOrDefault();
                InvoiceDate = _lic.InvoiceDate.Date;
                InvoiceNumber = _lic.InvoiceNumber;
                CostAmount = _lic.CostAmount.ToString();
                SelectedContractorsName = ContractorsNames.Where(x => x.Name.Equals(_lic.ContractorName)).FirstOrDefault();
                IsSettled = _lic.IsSettled ? SettlementOptions.Where(x => x == "Tak").FirstOrDefault() : SettlementOptions.Where(x => x == "Nie").FirstOrDefault();
            }

            InvoiceDate = DateTime.Today;
        }

        private void InitializeBuildingsList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                BuildingsNames = new ObservableCollection<Building>(db.Buildings.Where(x => x.IsDeleted == false).ToList());
            }
        }

        private void InitializeCategoriesList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                CategoriesNames = new ObservableCollection<InvoiceCategory>(db.InvoiceCategories.Where(x => x.IsDeleted == false).ToList());
            }
        }

        private void InitializeContractorsList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                ContractorsNames = new ObservableCollection<ContractorsName>(db.InvoiceContractors.ToList());
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

        private async void AddCategory(object param)
        {
            Wizards.EditInvoiceCategories eic;
            eic = new Wizards.EditInvoiceCategories();
            var result = await DialogHost.Show(eic, "HelperDialog", ExtendedOpenedEventHandler, ExtendedClosingInvoiceCategoriesEventHandler);
        }

        private bool CanAddCategory()
        {
            return true;
        }

        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private async void ExtendedClosingInvoiceCategoriesEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {

            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditInvoiceCategories);
                //Accept
                using (var db = new DB.DomenaDBContext())
                {
                    foreach (var cmd in dc.commandBuffer)
                    {
                        switch (cmd.category)
                        {
                            default:
                                break;
                            case CostCategoryEnum.CostCategoryCommandEnum.Add:
                                db.InvoiceCategories.Add(cmd.Item);
                                db.SaveChanges();
                                break;
                            case CostCategoryEnum.CostCategoryCommandEnum.Remove:
                                db.InvoiceCategories.Where(x => x.CategoryId.Equals(cmd.Item.CategoryId)).FirstOrDefault().IsDeleted = true;
                                db.SaveChanges();
                                break;
                            case CostCategoryEnum.CostCategoryCommandEnum.Update:
                                db.InvoiceCategories.Where(x => x.CategoryId.Equals(cmd.Item.CategoryId)).FirstOrDefault().CategoryName = cmd.Item.CategoryName;
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
                    var dc = (eventArgs.Session.Content as Wizards.EditInvoiceCategories);
                    var result = await DialogHost.Show(dc, "HelperDialog", ExtendedOpenedEventHandler, ExtendedClosingInvoiceCategoriesEventHandler);
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
