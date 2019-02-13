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
using System.Data.Entity;
using LibDataModel;
using DomenaManager.Helpers;
using MaterialDesignThemes.Wpf;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class InvoicesPart : UserControl, INotifyPropertyChanged
    {
        private InvoiceData _invoiceData { get; set; }

        private ObservableCollection<CostDistributionCollectionItem> _distributionTypes;
        public ObservableCollection<CostDistributionCollectionItem> DistributionTypes
        {
            get { return _distributionTypes; }
            set
            {
                if (value != _distributionTypes)
                {
                    _distributionTypes = value;
                    OnPropertyChanged("DistributionTypes");
                }
            }
        }

        private CostDistributionCollectionItem _selectedDistributionType;
        public CostDistributionCollectionItem SelectedDistributionType
        {
            get { return _selectedDistributionType; }
            set
            {
                if (value != _selectedDistributionType)
                {
                    _selectedDistributionType = value;
                    OnPropertyChanged("SelectedDistributionType");
                } 
            }
        }

        private ObservableCollection<InvoiceCategory> _invoiceCategories;
        public ObservableCollection<InvoiceCategory> InvoiceCategories
        {
            get { return _invoiceCategories; }
            set
            {
                if (value != _invoiceCategories)
                {
                    _invoiceCategories = value;
                    OnPropertyChanged("InvoiceCategories");
                }
            }
        }

        private InvoiceCategory _selectedInvoiceCategory;
        public InvoiceCategory SelectedInvoiceCategory
        {
            get { return _selectedInvoiceCategory; }
            set
            {
                if (value != _selectedInvoiceCategory)
                {
                    _selectedInvoiceCategory = value;
                    OnPropertyChanged("SelectedInvoiceCategory");
                }
            }
        }

        public ObservableCollection<BuildingInvoiceBinding> BuildingInvoiceBindings
        {
            get { return _invoiceData.BuildingInvoiceBinding; }
            set
            {
                if (value != _invoiceData.BuildingInvoiceBinding)
                {
                    _invoiceData.BuildingInvoiceBinding = value;
                    OnPropertyChanged("BuildingInvoiceBindings");
                }
            }
        }

        private BuildingInvoiceBinding _selectedBuildingInvoiceBinding;
        public BuildingInvoiceBinding SelectedBuildingInvoiceBinding
        {
            get { return _selectedBuildingInvoiceBinding; }
            set
            {
                if (value != _selectedBuildingInvoiceBinding)
                {
                    _selectedBuildingInvoiceBinding = value;
                    OnPropertyChanged("SelectedBuildingInvoiceBinding");
                }
            }
        }

        public string SelectedUnitValue { get; set; }

        public string SelectedInvoiceCategoryValue { get; set; }
                
        public ICommand AddBuildingInvoiceBindingCommand
        {
            get { return new RelayCommand(AddBuildingInvoiceBinding, CanAddBuildingInvoiceBinding); }
        }

        public ICommand DeleteBuildingInvoiceBindingCommand
        {
            get { return new RelayCommand(DeleteBuildingInvoiceBinding, CanDeleteBuildingInvoiceBinding); }
        }

        public InvoicesPart(Building SelectedBuilding = null)
        {
            InitializeComponent();
            DataContext = this;
            InitializeInvoicesPart();
            _invoiceData = new InvoiceData(SelectedBuilding);
        }

        public void InitializeInvoicesPart()
        {
            var values = (CostDistribution[])Enum.GetValues(typeof(CostDistribution));
            DistributionTypes = new ObservableCollection<CostDistributionCollectionItem>();
            foreach (var v in values)
            {
                var cdci = new CostDistributionCollectionItem(v);
                DistributionTypes.Add(cdci);
            }
            using (var db = new DB.DomenaDBContext())
            {
                InvoiceCategories = new ObservableCollection<InvoiceCategory>(db.InvoiceCategories.Where(x => !x.IsDeleted).ToList());
            }
        }

        private void AddBuildingInvoiceBinding(object param)
        {
            BuildingInvoiceBindings.Add(new BuildingInvoiceBinding()
            {
                BindingId = Guid.NewGuid(),
                Building = null,
                Distribution = (CostDistribution)SelectedDistributionType.EnumValue,
                InvoiceCategory = SelectedInvoiceCategory,
                IsDeleted = false,
            });
        }

        private bool CanAddBuildingInvoiceBinding()
        {
            return SelectedInvoiceCategory != null && SelectedDistributionType != null;
        }

        private void DeleteBuildingInvoiceBinding(object param)
        {
            BuildingInvoiceBindings.Remove(SelectedBuildingInvoiceBinding);
        }

        private bool CanDeleteBuildingInvoiceBinding()
        {
            return SelectedBuildingInvoiceBinding != null;
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

    public class InvoiceData
    {
        public ObservableCollection<BuildingInvoiceBinding> BuildingInvoiceBinding { get; set; }

        public InvoiceData(Building b)
        {
            using (var db = new DB.DomenaDBContext())
            {
                BuildingInvoiceBinding = new ObservableCollection<BuildingInvoiceBinding>(db.BuildingInvoceBindings.Include(x => x.InvoiceCategory).Where(x => !x.IsDeleted && x.Building.BuildingId == b.BuildingId).ToList());
            }
        }
    }
}
