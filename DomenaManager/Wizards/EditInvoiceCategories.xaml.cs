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
    public partial class EditInvoiceCategories : UserControl, INotifyPropertyChanged
    {
        private string _categoryName;
        public string CategoryName
        {
            get { return _categoryName; }
            set
            {
                if (value != _categoryName)
                {
                    _categoryName = value;
                    LabelError = "";
                    OnPropertyChanged("CategoryName");
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

        private ObservableCollection<InvoiceCategory> _categoryCollection;
        public ObservableCollection<InvoiceCategory> CategoryCollection
        {
            get { return _categoryCollection; }
            set
            {
                if (value != _categoryCollection)
                {
                    _categoryCollection = value;
                    OnPropertyChanged("CategoryCollection");
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
                    CategoryName = value != null ? value.CategoryName : string.Empty;
                    OnPropertyChanged("SelectedInvoiceCategory");
                }
            }
        }

        public ICommand AddCategoryCommand
        {
            get { return new Helpers.RelayCommand(AddCategory, CanAddCategory); }
        }

        public ICommand DeleteCategoryCommand
        {
            get { return new Helpers.RelayCommand(DeleteCategory, CanDeleteCategory); }
        }

        public ICommand ModifyCategoryCommand
        {
            get { return new Helpers.RelayCommand(ModifyCategory, CanModifyCategory); }
        }

        public List<Helpers.CategoryCommand<InvoiceCategory>> commandBuffer;

        public EditInvoiceCategories()
        {
            DataContext = this;
            InitializeComponent();
            commandBuffer = new List<Helpers.CategoryCommand<InvoiceCategory>>();
            using (var db = new DB.DomenaDBContext())
            {
                CategoryCollection = new ObservableCollection<InvoiceCategory>(db.InvoiceCategories.Where(x => !x.IsDeleted).ToList());
            }
        }

        private void AddCategory(object param)
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                LabelError = "Błędna nazwa";
                return;
            }
            var ic = new InvoiceCategory { CategoryName = CategoryName, CategoryId = Guid.NewGuid(), IsDeleted = false };
            CategoryCollection.Add(ic);

            commandBuffer.Add(new Helpers.CategoryCommand<InvoiceCategory> { CommandType = Helpers.CommandEnum.Add, Item = ic });
        }

        private bool CanAddCategory()
        {
            return true;
        }

        private void ModifyCategory(object param)
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                LabelError = "Błędna nazwa";
                return;
            }
            SelectedInvoiceCategory.CategoryName = CategoryName;

            commandBuffer.Add(new Helpers.CategoryCommand<InvoiceCategory> { CommandType = Helpers.CommandEnum.Update, Item = SelectedInvoiceCategory });
        }

        private bool CanModifyCategory()
        {
            return SelectedInvoiceCategory != null;
        }

        private void DeleteCategory(object param)
        {
            commandBuffer.Add(new Helpers.CategoryCommand<InvoiceCategory> { CommandType = Helpers.CommandEnum.Remove, Item = SelectedInvoiceCategory });

            CategoryCollection.Remove(SelectedInvoiceCategory);            
        }

        private bool CanDeleteCategory()
        {
            return SelectedInvoiceCategory != null;
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
