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

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditInvoiceCategories.xaml
    /// </summary>
    public partial class EditInvoiceVatRates : UserControl, INotifyPropertyChanged
    {
        private double _itemName;
        public double ItemName
        {
            get { return _itemName; }
            set
            {
                if (value != _itemName)
                {
                    _itemName = value;
                    LabelError = "";
                    OnPropertyChanged("ItemName");
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

        private ObservableCollection<InvoiceVatRate> _itemsCollection;
        public ObservableCollection<InvoiceVatRate> ItemsCollection
        {
            get { return _itemsCollection; }
            set
            {
                if (value != _itemsCollection)
                {
                    _itemsCollection = value;
                    OnPropertyChanged("ItemsCollection");
                }
            }
        }

        private InvoiceVatRate _selectedItem;
        public InvoiceVatRate SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (value != _selectedItem)
                {
                    _selectedItem = value;                    
                    ItemName = value != null ? value.Rate : 0;
                    OnPropertyChanged("SelectedItem");
                }
            }
        }

        public ICommand AddCommand
        {
            get { return new Helpers.RelayCommand(Add, CanAdd); }
        }

        public ICommand DeleteCommand
        {
            get { return new Helpers.RelayCommand(Delete, CanDelete); }
        }

        public ICommand ModifyCommand
        {
            get { return new Helpers.RelayCommand(Modify, CanModify); }
        }

        public List<Helpers.CategoryCommand<InvoiceVatRate>> commandBuffer;

        public EditInvoiceVatRates()
        {
            DataContext = this;
            InitializeComponent();
            commandBuffer = new List<Helpers.CategoryCommand<InvoiceVatRate>>();
            using (var db = new DB.DomenaDBContext())
            {
                ItemsCollection = new ObservableCollection<InvoiceVatRate>(db.InvoiceVatRates.Where(x => !x.IsDeleted).ToList());
            }
        }

        private void Add(object param)
        {
            
            var ic = new InvoiceVatRate { Rate = ItemName, InvoiceVatRateId = Guid.NewGuid(), IsDeleted = false };
            ItemsCollection.Add(ic);

            commandBuffer.Add(new Helpers.CategoryCommand<InvoiceVatRate> { CommandType = Helpers.CommandEnum.Add, Item = ic });
        }

        private bool CanAdd()
        {
            return true;
        }

        private void Modify(object param)
        {            
            SelectedItem.Rate = ItemName;
            commandBuffer.Add(new Helpers.CategoryCommand<InvoiceVatRate> { CommandType = Helpers.CommandEnum.Update, Item = SelectedItem });
        }

        private bool CanModify()
        {
            return SelectedItem != null;
        }

        private void Delete(object param)
        {
            commandBuffer.Add(new Helpers.CategoryCommand<InvoiceVatRate> { CommandType = Helpers.CommandEnum.Remove, Item = SelectedItem });
            ItemsCollection.Remove(SelectedItem);            
        }

        private bool CanDelete()
        {
            return SelectedItem != null;
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
