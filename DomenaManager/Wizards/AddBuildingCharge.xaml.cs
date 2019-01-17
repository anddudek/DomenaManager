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
using DomenaManager.Helpers;
using System.Data.Entity;
using MaterialDesignThemes.Wpf;
using System.IO;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditChargeWizard.xaml
    /// </summary>
    public partial class AddBuildingCharge : UserControl, INotifyPropertyChanged
    {
        #region Bindings       

        private ObservableCollection<Building> _buildingsCollection;
        public ObservableCollection<Building> BuildingsCollection
        {
            get { return _buildingsCollection; }
            set
            {
                if (value != _buildingsCollection)
                {
                    _buildingsCollection = value;
                    OnPropertyChanged("BuildingsCollection");
                }
            }
        }

        private Building _selectedBuilding;
        public Building SelectedBuilding
        {
            get { return _selectedBuilding; }
            set
            {
                if (value != _selectedBuilding)
                {
                    _selectedBuilding = value;
                    OnPropertyChanged("SelectedBuilding");
                }
            }
        }

        public string SelectedBuildingValue { get; set; }    

        private DateTime _chargeDate;
        public DateTime ChargeDate
        {
            get { return _chargeDate; }
            set
            {
                if (value != _chargeDate)
                {
                    _chargeDate = value;
                    OnPropertyChanged("ChargeDate");
                }
            }
        }

        #endregion

        #region Commands
        
        public ICommand SaveCommand
        {
            get { return new RelayCommand(SaveDialog, CanSaveDialog); }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(CancelDialog, CanCancelDialog); }
        }

        #endregion

        public AddBuildingCharge()
        {
            DataContext = this;
            ChargeDate = DateTime.Today;
            InitializeComponent();
            InitializeBuildingsList();
        }

        #region Functions

        private void InitializeBuildingsList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                BuildingsCollection = new ObservableCollection<Building>(db.Buildings.ToList());
            }
        }

        private void SaveDialog(object param)
        {            
            if (!IsValid(this as DependencyObject) || SelectedBuilding == null || ChargeDate == null)
            {
                return;
            }

            var charges = new List<Charge>();

            using (var db = new DB.DomenaDBContext())
            {
                var apartments = db.Apartments.Where(x => !x.IsDeleted && x.SoldDate == null && x.BuildingId == SelectedBuilding.BuildingId).ToList();

                foreach (var ap in apartments)
                {
                    var newCharge = new LibDataModel.Charge();
                    newCharge.SettlementId = Guid.Empty;
                    newCharge.ChargeId = Guid.NewGuid();
                    newCharge.ApartmentId = ap.ApartmentId;
                    newCharge.CreatedDate = DateTime.Today;
                    newCharge.ChargeDate = ChargeDate;
                    newCharge.IsClosed = false;
                    newCharge.IsDeleted = false;
                    newCharge.OwnerId = ap.OwnerId;

                    db.Charges.Add(newCharge);
                    charges.Add(newCharge);
                }
                db.SaveChanges();
            }

            foreach (var c in charges)
            {
                ChargesOperations.RecalculateCharge(c);
            }

            SwitchPage.SwitchMainPage(new Pages.ChargesPage(), this);
        }

        private bool CanSaveDialog()
        {
            return SelectedBuilding != null && ChargeDate != null;
        }

        private void CancelDialog(object param)
        {
            SwitchPage.SwitchMainPage(new Pages.ChargesPage(), this);
        }

        private bool CanCancelDialog()
        {
            return true;
        }

        #endregion
        
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
