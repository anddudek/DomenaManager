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

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for BuildingsPage.xaml
    /// </summary>
    public partial class BindingsPage : UserControl, INotifyPropertyChanged
    {
        #region Bindings

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                if (value != _isEditMode)
                {
                    _isEditMode = value;
                    OnPropertyChanged("IsEditMode");
                    OnPropertyChanged("AcceptText");
                    OnPropertyChanged("AcceptIcon");
                    OnPropertyChanged("CancelText");
                    OnPropertyChanged("CancelIcon");
                }
            }
        }
        
        public string AcceptText
        {
            get { return IsEditMode  ? "Zapisz" : "Edytuj"; }
        }

        public PackIconKind AcceptIcon
        {
            get { return IsEditMode ? PackIconKind.ClipboardCheck : PackIconKind.Pencil; }
        }

        public string CancelText
        {
            get { return IsEditMode ? "Anuluj" : "Usuń"; }
        }

        public PackIconKind CancelIcon
        {
            get { return IsEditMode ? PackIconKind.CloseCircle : PackIconKind.DeleteForever; }
        }

        private ObservableCollection<BindingDataGrid> _availableApartments;
        public ObservableCollection<BindingDataGrid> AvailableApartments
        {
            get { return _availableApartments; }
            set
            { 
                if (value != _availableApartments)
                {
                    _availableApartments = value;
                    OnPropertyChanged("AvailableApartments");
                }
            }
        }

        private ObservableCollection<ApartmentBinding> _bindingsList;
        public ObservableCollection<ApartmentBinding> BindingsList
        {
            get { return _bindingsList; }
            set
            {
                if (value != _bindingsList)
                {
                    _bindingsList = value;
                    OnPropertyChanged("BindingsList");
                }
            }
        }

        private ApartmentBinding _selectedBinding;
        public ApartmentBinding SelectedBinding
        {
            get { return _selectedBinding; }
            set
            {
                if (value != _selectedBinding)
                {
                    _selectedBinding = value;
                    OnPropertyChanged("SelectedBinding");
                }
            }
        }

        public ICommand AddBindingCommand
        {
            get
            {
                return new Helpers.RelayCommand(AddBinding, CanAddBinding);
            }
        }

        public ICommand ButtonCommand
        {
            get
            {
                return new Helpers.RelayCommand(Accept, CanAccept);
            }
        }

        public ICommand DeleteBindingCommand
        {
            get
            {
                return new Helpers.RelayCommand(DeleteBinding, CanDeleteBinding);
            }
        }

        #endregion

        public BindingsPage()
        {
            DataContext = this;
            InitializeComponent();
            InitializeCollection();
            
        }

        public void InitializeCollection()
        {
            
        }

        private bool CanAddBinding()
        {
            return !IsEditMode;
        }

        private void AddBinding(object obj)
        {

        }

        private bool CanAccept()
        {
            return true;
        }

        private void Accept(object param)
        {
            
        }

        private bool CanDeleteBinding()
        {
            return true;
        }

        private void DeleteBinding(object param)
        {
            
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
}
