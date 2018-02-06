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
using LibDataModel;
using MaterialDesignThemes.Wpf;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class EditApartmentWizard : UserControl, INotifyPropertyChanged
    {
        private string _buildingName;
        public string BuildingName
        {
            get { return _buildingName; }
            set
            {
                _buildingName = value;
                OnPropertyChanged("BuildingName");
            }
        }

        private string _buildingCity;
        public string BuildingCity
        {
            get { return _buildingCity; }
            set
            {
                _buildingCity = value;
                OnPropertyChanged("BuildingCity");
            }
        }

        private string _buildingZipCode;
        public string BuildingZipCode
        {
            get { return _buildingZipCode; }
            set
            {
                _buildingZipCode = value;
                OnPropertyChanged("BuildingZipCode");
            }
        }

        private string _buildingRoadName;
        public string BuildingRoadName
        {
            get { return _buildingRoadName; }
            set
            {
                _buildingRoadName = value;
                OnPropertyChanged("BuildingRoadName");
            }
        }

        private string _buildingRoadNumber;
        public string BuildingRoadNumber
        {
            get { return _buildingRoadNumber; }
            set
            {
                _buildingRoadNumber = value;
                OnPropertyChanged("BuildingRoadNumber");
            }
        }

        public Building _buildingLocalCopy;

        public EditApartmentWizard(Building SelectedBuilding = null)
        {
            DataContext = this;
            InitializeComponent();
            if (SelectedBuilding != null)
            {
                _buildingLocalCopy = new Building(SelectedBuilding);

                BuildingName = SelectedBuilding.Name;
                BuildingCity = SelectedBuilding.City;
                BuildingZipCode = SelectedBuilding.ZipCode;
                BuildingRoadName = SelectedBuilding.RoadName;
                BuildingRoadNumber = SelectedBuilding.BuildingNumber;
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
