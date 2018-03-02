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
    /// Interaction logic for EditCostCategories.xaml
    /// </summary>
    public partial class EditCostCategories : UserControl, INotifyPropertyChanged
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

        private ObservableCollection<CostCategory> _categoryCollection;
        public ObservableCollection<CostCategory> CategoryCollection
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

        private CostCategory _selectedCostCategory;
        public CostCategory SelectedCostCategory
        {
            get { return _selectedCostCategory; }
            set
            {
                if (value != _selectedCostCategory)
                {
                    _selectedCostCategory = value;                    
                    CategoryName = value != null ? value.CategoryName : string.Empty;
                    OnPropertyChanged("SelectedCostCategory");
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

        public EditCostCategories()
        {
            DataContext = this;
            InitializeComponent();
            using (var db = new DB.DomenaDBContext())
            {
                CategoryCollection = new ObservableCollection<CostCategory>(db.CostCategories.Where(x => !x.IsDeleted).ToList());
            }
        }

        private void AddCategory(object param)
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                LabelError = "Błędna nazwa";
                return;
            }
            var cc = new CostCategory { CategoryName = CategoryName, CostCategoryId = Guid.NewGuid(), IsDeleted = false };
            CategoryCollection.Add(cc);
            //TODO
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
            SelectedCostCategory.CategoryName = CategoryName;
            //TODO
        }

        private bool CanModifyCategory()
        {
            return SelectedCostCategory != null;
        }

        private void DeleteCategory(object param)
        {
            CategoryCollection.Remove(SelectedCostCategory);
            //TODO
        }

        private bool CanDeleteCategory()
        {
            return SelectedCostCategory != null;
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
