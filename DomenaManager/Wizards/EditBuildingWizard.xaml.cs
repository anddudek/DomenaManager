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

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class EditBuildingWizard : UserControl, INotifyPropertyChanged
    {
        public ICommand AcceptCommand
        {
            get
            {
                return new Helpers.RelayCommand(Accept, CanAccept);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new Helpers.RelayCommand(Cancel, CanCancel);
            }
        }

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

        public EditBuildingWizard(Building SelectedBuilding = null)
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

        private bool CanAccept()
        {
            return
                !(string.IsNullOrEmpty(BuildingName) || string.IsNullOrEmpty(BuildingCity) || string.IsNullOrEmpty(BuildingZipCode) || string.IsNullOrEmpty(BuildingRoadName) || string.IsNullOrEmpty(BuildingRoadNumber));
        }
        private void Accept(object obj)
        {
            if (_buildingLocalCopy == null)
            {
                
                //Add new building
                using (var db = new DB.DomenaDBContext())
                {
                    
                    var newBuilding = new Building { BuildingId = Guid.NewGuid(), Name = BuildingName, City= BuildingCity, ZipCode= BuildingZipCode, BuildingNumber= BuildingRoadNumber, RoadName = BuildingRoadName, IsDeleted=false};
                    db.Buildings.Add(newBuilding);
                    db.SaveChanges();
                }
            }
            else
            {
                //Edit building
                using (var db = new DB.DomenaDBContext())
                {
                    var q = db.Buildings.Where(x => x.BuildingId.Equals(_buildingLocalCopy.BuildingId)).FirstOrDefault();
                    q.BuildingNumber = BuildingRoadNumber;
                    q.City = BuildingCity;
                    q.Name = BuildingName;
                    q.RoadName = BuildingRoadName;
                    q.ZipCode = BuildingZipCode;
                    db.SaveChanges();
                }
            }
            //this.Close();
        }




        private bool CanCancel()
        {
            return true;
        }
        private void Cancel(object obj)
        {
            MessageBoxResult mbr = MessageBox.Show("Czy chcesz przerwać edycję?", "Anulowanie", MessageBoxButton.YesNo);
            if (mbr == MessageBoxResult.Yes)
            {
                //this.Close();
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
