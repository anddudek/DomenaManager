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
using LibDataModel;
using MaterialDesignThemes.Wpf;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class EditApartmentWizard : UserControl, INotifyPropertyChanged
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

        private int _apartmentNumber;
        public int ApartmentNumber
        {
            get { return _apartmentNumber; }
            set
            {
                _apartmentNumber = value;
                OnPropertyChanged("ApartmentNumber");
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

        private ObservableCollection<Owner> _ownersNames;
        public ObservableCollection<Owner> OwnersNames
        {
            get { return _ownersNames; }
            set
            {
                _ownersNames = value;
                OnPropertyChanged("OwnersNames");
            }
        }

        private Owner _selectedOwnerName;
        public Owner SelectedOwnerName
        {
            get { return _selectedOwnerName; }
            set
            {
                _selectedOwnerName = value;
                OnPropertyChanged("SelectedOwnerName");
                OnPropertyChanged("SelectedOwnerMailAddress");
            }
        }

        private string _selectedOwnerMailAddress;
        public string SelectedOwnerMailAddress
        {
            get 
            {
                _selectedOwnerMailAddress = _selectedOwnerName != null ? _selectedOwnerName.MailAddress : null;
                return _selectedOwnerMailAddress; 
            }
            set
            {
                _selectedOwnerMailAddress = value;
                OnPropertyChanged("SelectedOwnerMailAddress");
            }
        }

        private string _apartmentArea;
        public string ApartmentArea
        {
            get { return _apartmentArea; }
            set
            {
                if (_apartmentArea != value)
                {
                    _apartmentArea = value;
                    OnPropertyChanged("ApartmentArea");
                }
            }
        }

        private string _additionalArea;
        public string AdditionalArea
        {
            get { return _additionalArea; }
            set
            {
                if (_additionalArea != value)
                {
                    _additionalArea = value;
                    OnPropertyChanged("AdditionalArea");
                }                
            }
        }

        private int _hasWaterMeter;
        public int HasWaterMeter
        {
            get { return _hasWaterMeter; }
            set
            {
                _hasWaterMeter = value;
                OnPropertyChanged("HasWaterMeter");
            }
        }

        public string SelectedBuildingValue { get; set; }
        public string SelectedOwnerValue { get; set; }

        private DateTime _boughtDate;
        public DateTime BoughtDate
        {
            get { return _boughtDate; }
            set
            {
                _boughtDate = value;
                OnPropertyChanged("BoughtDate");
            }
        }

        public ICommand UpdateAllFieldsCommand
        {
            get
            {
                return new Helpers.RelayCommand(UpdateAllFields, CanUpdateAllFields);
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

        public Apartment _apartmentLocalCopy;

        public EditApartmentWizard(Apartment SelectedApartment = null)
        {
            if (SelectedApartment != null)
            {
                _apartmentLocalCopy = new Apartment(SelectedApartment);
            }
            InitializeList();
            DataContext = this;            
            InitializeComponent();            
        }

        private void InitializeList()
        {
            using (var db = new DB.DomenaDBContext())
            {
                _buildingsNames = new ObservableCollection<Building>(db.Buildings.ToList());
                _ownersNames = new ObservableCollection<Owner>(db.Owners.ToList());
            }            

            if (_apartmentLocalCopy == null)
            {
                _boughtDate = DateTime.Today;
                return;
            }

            _boughtDate = _apartmentLocalCopy.BoughtDate;
            _additionalArea = _apartmentLocalCopy.AdditionalArea.ToString();
            _apartmentArea = _apartmentLocalCopy.ApartmentArea.ToString();
            _apartmentNumber = _apartmentLocalCopy.ApartmentNumber;
            _hasWaterMeter = _apartmentLocalCopy.HasWaterMeter ? 0 : 1;
            _selectedBuildingName = _buildingsNames.Where(x => x.BuildingId.Equals(_apartmentLocalCopy.BuildingId)).FirstOrDefault();
            _selectedOwnerName = _ownersNames.Where(x => x.OwnerId.Equals(_apartmentLocalCopy.OwnerId)).FirstOrDefault();
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
