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
using LibDataModel;

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

        private string _bindingName;
        public string BindingName
        {
            get { return _bindingName; }
            set
            {
                if (value != _bindingName)
                {
                    _bindingName = value;
                    OnPropertyChanged("BindingName");
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

        public ObservableCollection<Apartment> Apartments;

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
                    if (value != null)
                    {
                        BindingName = value.Name;                        
                    }
                    AvailableCV.Refresh();
                }
            }
        }

        private ICollectionView _availableCV;
        public ICollectionView AvailableCV
        {
            get
            {
                return _availableCV;
            }
            set
            {
                if (value != _availableCV)
                {
                    _availableCV = value;
                    OnPropertyChanged("AvailableCV");
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
            InitializeAvailableApartmentsCollection();
        }

        public void InitializeCollection()
        {
            BindingsList = new ObservableCollection<ApartmentBinding>();
            using (var db = new DB.DomenaDBContext())
            {
                Apartments = new ObservableCollection<Apartment>(db.Apartments.Where(x => !x.IsDeleted && x.SoldDate == null).ToList());
                foreach (var b in db.Bindings.Where(x => !x.IsDeleted))
                {
                    var ab = new ApartmentBinding() { BindingId = b.BindingId, Name = b.Name };
                    ab.BoundApartments = new ObservableCollection<BindingDataGrid>();
                    var apartments = db.Apartments.Where(x => x.BindingParent.Equals(b.BindingId) && !x.IsDeleted && x.SoldDate == null);
                    foreach (var a in apartments)
                    {
                        var bdg = new BindingDataGrid();
                        bdg.apartment = a;
                        bdg.building = db.Buildings.FirstOrDefault(x => x.BuildingId.Equals(a.BuildingId));
                        bdg.owner = db.Owners.FirstOrDefault(x => x.OwnerId.Equals(a.OwnerId));
                        ab.BoundApartments.Add(bdg);
                    }
                    BindingsList.Add(ab);
                }
            }
        }

        public void InitializeAvailableApartmentsCollection()
        {
            AvailableApartments = new ObservableCollection<BindingDataGrid>();
            using (var db = new DB.DomenaDBContext())
            {
                Apartments = new ObservableCollection<Apartment>(db.Apartments.Where(x => !x.IsDeleted && x.SoldDate == null).ToList());

                foreach (var a in Apartments)
                {
                    var bdg = new BindingDataGrid() { apartment = a, building = db.Buildings.FirstOrDefault(x => x.BuildingId.Equals(a.BuildingId)), owner = db.Owners.FirstOrDefault(x => x.OwnerId.Equals(a.OwnerId)) };
                    AvailableApartments.Add(bdg);
                }
            }

            FilterAvailableApartments();
        }

        public void FilterAvailableApartments()
        {
            AvailableCV = (CollectionView)CollectionViewSource.GetDefaultView(AvailableApartments);
            AvailableCV.SortDescriptions.Add(new SortDescription("building.Name", ListSortDirection.Ascending));
            AvailableCV.Filter = FilterCollection;

            AvailableCV.Refresh();
        }

        private bool FilterCollection(object item)
        {
            var bdg = item as BindingDataGrid;
            if (SelectedBinding != null && SelectedBinding.BoundApartments != null && 
                SelectedBinding.BoundApartments.Any(x => x.apartment.ApartmentId.Equals(bdg.apartment.ApartmentId)))
            {
                return false;
            }

            return true;
        }

        private bool CanAddBinding()
        {
            return !IsEditMode;
        }

        private void AddBinding(object obj)
        {
            IsEditMode = true;
            ApartmentBinding newBind = new ApartmentBinding() { BindingId = Guid.NewGuid(), Name = "Nowa grupa", BoundApartments = new ObservableCollection<BindingDataGrid>() };
            BindingsList.Add(newBind);
            SelectedBinding = newBind;
        }

        private bool CanAccept()
        {
            return IsEditMode ? true : SelectedBinding != null;
        }

        private void Accept(object param)
        {
            if (IsEditMode) // Accept
            {
                IsEditMode = false;
                using (var db = new DB.DomenaDBContext())
                {
                    var bind = db.Bindings.FirstOrDefault(x => !x.IsDeleted && x.BindingId.Equals(SelectedBinding.BindingId));
                    if (bind != null) // edit
                    {
                        SelectedBinding.Name = BindingName;
                        bind.Name = SelectedBinding.Name;
                        foreach (var ap in db.Apartments)
                        {
                            if (ap.BindingParent.Equals(SelectedBinding.BindingId))
                            {
                                ap.BindingParent = Guid.Empty;
                            }
                            if (SelectedBinding.BoundApartments.Any(x => x.apartment.ApartmentId.Equals(ap.ApartmentId)))
                            {
                                ap.BindingParent = SelectedBinding.BindingId;
                            }
                        }
                    }
                    else // add new
                    {
                        SelectedBinding.Name = BindingName;
                        db.Bindings.Add(new BindingParent() { BindingId = SelectedBinding.BindingId, IsDeleted = SelectedBinding.IsDeleted, Name = BindingName });
                        foreach (var ap in db.Apartments)
                        {
                            if (ap.BindingParent.Equals(SelectedBinding.BindingId))
                            {
                                ap.BindingParent = Guid.Empty;
                            }
                            if (SelectedBinding.BoundApartments.Any(x => x.apartment.ApartmentId.Equals(ap.ApartmentId)))
                            {
                                ap.BindingParent = SelectedBinding.BindingId;
                            }
                        }
                    }
                    db.SaveChanges();                                        
                }
            }
            else // Edit
            {
                IsEditMode = true;
            }
        }

        private bool CanDeleteBinding()
        {
            return IsEditMode ? true : SelectedBinding != null;
        }

        private void DeleteBinding(object param)
        {
            if (IsEditMode) // cancel
            {
                IsEditMode = false;
                InitializeCollection();
                InitializeAvailableApartmentsCollection();
            }
            else // delete
            {
                using (var db = new DB.DomenaDBContext())
                {
                    var bind = db.Bindings.FirstOrDefault(x => !x.IsDeleted && x.BindingId.Equals(SelectedBinding.BindingId));
                    if (bind != null) // delete
                    {
                        bind.IsDeleted = true;

                        var ap = db.Apartments.Where(x => x.BindingParent.Equals(SelectedBinding.BindingId));
                        foreach (var a in ap)
                        {
                            a.BindingParent = Guid.Empty;
                        }

                        BindingsList.Remove(SelectedBinding);                        
                    }
                    db.SaveChanges();
                }
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
