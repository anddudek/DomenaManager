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
using DomenaManager.Helpers.DataGrid;
using System.Collections.Specialized;

namespace DomenaManager.Wizards.SettlementWizard
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class InvoicesPart : UserControl, INotifyPropertyChanged
    {
        public InvoiceData InvoiceData { get; set; }

        public InvoicesPart()
        {
            InitializeComponent();
            DataContext = this;
            InvoiceData = new InvoiceData();
        }

        public int SelectedCount
        {
            get
            {
                return (InvoiceData != null && InvoiceData.Invoices != null) ? InvoiceData.Invoices.Count(x => x.IsChecked) : 0;
            }
        }

        public decimal ConstantSum
        {
            get
            {
                return (InvoiceData != null && InvoiceData.Invoices != null) ? InvoiceData.Invoices.Where(x => x.IsChecked).Select(x => x.CostAmountConstGross).DefaultIfEmpty(0).Sum() : 0;
            }
        }

        public decimal VariableSum
        {
            get
            {
                return (InvoiceData != null && InvoiceData.Invoices != null) ? InvoiceData.Invoices.Where(x => x.IsChecked).Select(x => x.CostAmountVariableGross).DefaultIfEmpty(0).Sum() : 0;
            }
        }

        public decimal TotalSum
        {
            get
            {
                return (InvoiceData != null && InvoiceData.Invoices != null) ? InvoiceData.Invoices.Where(x => x.IsChecked).Select(x => x.CostAmountGross).DefaultIfEmpty(0).Sum() : 0;
            }
        }

        public void InitializeInvoicesPart()
        {            
            using (var db = new DB.DomenaDBContext())
            {
                var q = db.Invoices.Where(x => !x.IsDeleted &&
                x.BuildingId == InvoiceData.MasterData.Building.BuildingId &&
                x.InvoiceDate >= InvoiceData.MasterData.StartingDate &&
                x.InvoiceDate <= InvoiceData.MasterData.EndingDate).ToList();

                InvoiceData.Invoices = new ObservableCollection<InvoiceSettlementDataGrid>();

                foreach (var invoice in q)
                {
                    var isdg = new InvoiceSettlementDataGrid(invoice);
                    isdg.InvoiceCategory = db.InvoiceCategories.FirstOrDefault(x => x.CategoryId == isdg.InvoiceCategoryId);
                    isdg.IsChecked = false;
                    InvoiceData.Invoices.Add(isdg);
                    isdg.PropertyChanged += PropertyChangedEventHandler;
                }
                
                OnPropertyChanged("InvoiceData");
            }
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

        private void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("SelectedCount");
            OnPropertyChanged("ConstantSum");
            OnPropertyChanged("VariableSum");
            OnPropertyChanged("TotalSum");
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("SelectedCount");
        }
    }

    public class InvoiceData
    {
        public ObservableCollection<InvoiceSettlementDataGrid> Invoices { get; set; }
        public SettlementMasterData MasterData { get; set; }
    }
}
