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
        private ObservableCollection<InvoiceVatRate> _vatCollection;
        public ObservableCollection<InvoiceVatRate> VatCollection
        {
            get { return _vatCollection; }
            set
            {
                _vatCollection = value;
                OnPropertyChanged("VatCollection");
            }
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

        public string CostAmount
        {
            get
            {
                decimal v;
                decimal c;
                return decimal.TryParse(CostAmountVariable, out v) && decimal.TryParse(CostAmountConst, out c) ? (v + c).ToString() : "-";
            }
        }
        
        public string CostAmountGross
        {
            get
            {
                decimal v;
                decimal c;
                return decimal.TryParse(CostAmountVariableGross, out v) && decimal.TryParse(CostAmountConstGross, out c) ? (v + c).ToString() : "-";
            }
        }

        private string _costAmountVariable;
        public string CostAmountVariable
        {
            get { return _costAmountVariable; }
            set
            {
                if (value != _costAmountVariable)
                {
                    _costAmountVariable = value;
                    decimal t;
                    if (SelectedVariableVat != null && decimal.TryParse(CostAmountVariable, out t))
                    {
                        _costAmountVariableGross = (Math.Floor(((1 + SelectedVariableVat.Rate / 100) * t) * 100) / 100).ToString();
                        OnPropertyChanged("CostAmountVariableGross");
                    }
                    OnPropertyChanged("CostAmountVariable");
                    OnPropertyChanged("CostAmountGross");
                    OnPropertyChanged("CostAmount");
                }
            }
        }

        private string _costAmountVariableGross;
        public string CostAmountVariableGross
        {
            get { return _costAmountVariableGross; }
            set
            {
                if (value != _costAmountVariableGross)
                {
                    _costAmountVariableGross = value;
                    decimal t;
                    if (SelectedVariableVat != null && decimal.TryParse(value, out t))
                    {
                        _costAmountVariable = (Math.Floor((t / (1 + SelectedVariableVat.Rate / 100)) * 100) / 100).ToString();
                        OnPropertyChanged("CostAmountVariable");
                    }
                    OnPropertyChanged("CostAmountVariableGross");
                    OnPropertyChanged("CostAmountGross");
                    OnPropertyChanged("CostAmount");
                }
            }
        }

        private InvoiceVatRate _selectedVariableVat;
        public InvoiceVatRate SelectedVariableVat
        {
            get { return _selectedVariableVat; }
            set
            {
                _selectedVariableVat = value;
                decimal t;
                if (SelectedVariableVat != null && decimal.TryParse(CostAmountVariable, out t))
                {
                    _costAmountVariableGross = (Math.Floor(((1 + SelectedVariableVat.Rate / 100) * t) * 100) / 100).ToString();
                    OnPropertyChanged("CostAmountVariableGross");
                }
                OnPropertyChanged("SelectedVariableVat");
                OnPropertyChanged("CostAmountGross");
                OnPropertyChanged("CostAmount");
            }
        }

        public string SelectedVariableVatValue { get; set; }

        private string _costAmountConst;
        public string CostAmountConst
        {
            get { return _costAmountConst; }
            set
            {
                if (value != _costAmountConst)
                {
                    _costAmountConst = value;
                    decimal t;
                    if (SelectedConstVat != null && decimal.TryParse(CostAmount, out t))
                    {
                        _costAmountConstGross = (Math.Floor(((1 + SelectedConstVat.Rate / 100) * t) * 100) / 100).ToString();
                        OnPropertyChanged("CostAmountVariableGross");
                    }
                    OnPropertyChanged("CostAmountConst");
                    OnPropertyChanged("CostAmountConst");
                    OnPropertyChanged("CostAmount");
                }
            }
        }

        private string _costAmountConstGross;
        public string CostAmountConstGross
        {
            get { return _costAmountConstGross; }
            set
            {
                if (value != _costAmountConstGross)
                {
                    _costAmountConstGross = value;
                    decimal t;
                    if (SelectedConstVat != null && decimal.TryParse(value, out t))
                    {
                        _costAmountConst = (Math.Floor((t / (1 + SelectedConstVat.Rate / 100)) * 100) / 100).ToString();
                        OnPropertyChanged("CostAmountConst");
                    }
                    OnPropertyChanged("CostAmountConstGross");
                    OnPropertyChanged("CostAmountGross");
                    OnPropertyChanged("CostAmount");
                }
            }
        }

        private InvoiceVatRate _selectedConstVat;
        public InvoiceVatRate SelectedConstVat
        {
            get { return _selectedConstVat; }
            set
            {
                _selectedConstVat = value;
                decimal t;
                if (SelectedConstVat != null && decimal.TryParse(CostAmountConst, out t))
                {
                    _costAmountConstGross = (Math.Floor(((1 + SelectedConstVat.Rate / 100) * t) * 100) / 100).ToString();
                    OnPropertyChanged("CostAmountConstGross");
                }
                OnPropertyChanged("SelectedConstVat");
                OnPropertyChanged("CostAmountGross");
                OnPropertyChanged("CostAmount");
            }
        }

        public string SelectedConstVatValue { get; set; }

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

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    OnPropertyChanged("Title");
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

        private DateTime _invoiceCreatedDate;
        public DateTime InvoiceCreatedDate
        {
            get { return _invoiceCreatedDate; }
            set
            {
                _invoiceCreatedDate = value;
                OnPropertyChanged("InvoiceCreatedDate");
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

        public ICommand SaveCommand
        {
            get { return new RelayCommand(SaveDialog, CanSaveDialog); }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(CancelDialog, CanCancelDialog); }
        }
        
        public ICommand AcceptCommand
        {
            get
            {
                return new Helpers.RelayCommand(AcceptDialog, CanAcceptDialog);
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
                InvoiceCreatedDate = _lic.InvoiceCreatedDate.Date;
                InvoiceNumber = _lic.InvoiceNumber;
                SelectedContractorsName = ContractorsNames.Where(x => x.Name.Equals(_lic.ContractorName)).FirstOrDefault();
                IsSettled = _lic.IsSettled ? SettlementOptions.Where(x => x == "Tak").FirstOrDefault() : SettlementOptions.Where(x => x == "Nie").FirstOrDefault();
                Title = _lic.Title;
                CostAmountVariable = _lic.CostAmountVariable.ToString();
                CostAmountVariableGross = _lic.CostAmountVariableGross.ToString();
                SelectedVariableVat = VatCollection.FirstOrDefault(x => x.Rate == _lic.VariableVat);
                CostAmountConst = _lic.CostAmountConst.ToString();
                CostAmountConstGross = _lic.CostAmountConstGross.ToString();
                SelectedConstVat = VatCollection.FirstOrDefault(x => x.Rate == _lic.ConstVat);
                return;
            }

            InvoiceDate = DateTime.Today;
            InvoiceCreatedDate = DateTime.Today;
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
                VatCollection = new ObservableCollection<InvoiceVatRate>(db.InvoiceVatRates.Where(x => !x.IsDeleted).ToList());
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

        private void SaveDialog(object param)
        {
            //Accept
            if (_lic == null)
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(SelectedBuildingName.Name) || string.IsNullOrEmpty(SelectedCategoryName.CategoryName)))
                {
                    return;
                }
                //Add new invoice
                var newInvoice = new LibDataModel.Invoice
                {
                    BuildingId = SelectedBuildingName.BuildingId,
                    ContractorName = SelectedContractorsValue,
                    CreatedTime = DateTime.Now,
                    InvoiceCategoryId = SelectedCategoryName.CategoryId,
                    InvoiceDate = InvoiceDate.Date,
                    InvoiceId = Guid.NewGuid(),
                    InvoiceNumber = InvoiceNumber,
                    IsDeleted = false,
                    IsSettled = IsSettled == "Tak" ? true : false,
                    InvoiceCreatedDate = InvoiceCreatedDate,
                    CostAmount = decimal.Parse(CostAmount),
                    CostAmountGross = decimal.Parse(CostAmountGross),
                    CostAmountVariable = decimal.Parse(CostAmountVariable),
                    CostAmountVariableGross = decimal.Parse(CostAmountVariableGross),
                    VariableVat = SelectedVariableVat.Rate,
                    CostAmountConst = decimal.Parse(CostAmountConst),
                    CostAmountConstGross = decimal.Parse(CostAmountConstGross),
                    ConstVat = SelectedConstVat.Rate,
                    Title = Title,
                };
                using (var db = new DB.DomenaDBContext())
                {
                    db.Invoices.Add(newInvoice);

                    if (!db.InvoiceContractors.Any(x => x.Name.Equals(SelectedContractorsValue)))
                    {
                        db.InvoiceContractors.Add(new ContractorsName() { Name = SelectedContractorsValue });
                    }

                    db.SaveChanges();
                }
            }
            else
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(SelectedBuildingName.Name) || string.IsNullOrEmpty(SelectedCategoryName.CategoryName)))
                {
                    return;
                }
                //Edit Invoice
                using (var db = new DB.DomenaDBContext())
                {
                    var q = db.Invoices.Where(x => x.InvoiceId.Equals(_lic.InvoiceId)).FirstOrDefault();
                    q.BuildingId = SelectedBuildingName.BuildingId;
                    q.ContractorName = SelectedContractorsValue;
                    q.CostAmount = decimal.Parse(CostAmount);
                    q.CreatedTime = DateTime.Now;
                    q.InvoiceCategoryId = SelectedCategoryName.CategoryId;
                    q.InvoiceDate = InvoiceDate.Date;
                    q.InvoiceCreatedDate = InvoiceCreatedDate.Date;                    
                    q.InvoiceNumber = InvoiceNumber;
                    q.IsSettled = IsSettled == "Tak" ? true : false;
                    q.Title = Title;
                    q.CostAmount = decimal.Parse(CostAmount);
                    q.CostAmountGross = decimal.Parse(CostAmountGross);
                    q.CostAmountVariable = decimal.Parse(CostAmountVariable);
                    q.CostAmountVariableGross = decimal.Parse(CostAmountVariableGross);
                    q.VariableVat = SelectedVariableVat.Rate;
                    q.CostAmountConst = decimal.Parse(CostAmountConst);
                    q.CostAmountConstGross = decimal.Parse(CostAmountConstGross);
                    q.ConstVat = SelectedConstVat.Rate;

                    if (!db.InvoiceContractors.Any(x => x.Name.Equals(SelectedContractorsValue)))
                    {
                        db.InvoiceContractors.Add(new ContractorsName() { Name = SelectedContractorsValue });
                    }

                    db.SaveChanges();
                }
            }
        }

        private bool CanAcceptDialog()
        {
            return true;
        }

        private void AcceptDialog(object param)
        {
            if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(SelectedBuildingName.Name) || string.IsNullOrEmpty(SelectedCategoryName.CategoryName)))
            {
                return;
            }
            SaveDialog(null);
            Helpers.SwitchPage.SwitchMainPage(new Pages.InvoicesPage(), this);
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

        private bool CanSaveDialog()
        {
            return true;
        }

        private void CancelDialog(object param)
        {
            SwitchPage.SwitchMainPage(new Pages.InvoicesPage(), this);
        }

        private bool CanCancelDialog()
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
                        switch (cmd.CommandType)
                        {
                            default:
                                break;
                            case CommandEnum.Add:
                                db.InvoiceCategories.Add(cmd.Item);
                                db.SaveChanges();
                                break;
                            case CommandEnum.Remove:
                                db.InvoiceCategories.Where(x => x.CategoryId.Equals(cmd.Item.CategoryId)).FirstOrDefault().IsDeleted = true;
                                db.SaveChanges();
                                break;
                            case CommandEnum.Update:
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
