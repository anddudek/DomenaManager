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
using MaterialDesignThemes.Wpf;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class MasterDataPart : UserControl, INotifyPropertyChanged
    {
        private BuildingMasterData masterData;

        public string BuildingName
        {
            get { return masterData.BuildingName; }
            set
            {
                if (value != masterData.BuildingName)
                {
                    masterData.BuildingName = value;
                    OnPropertyChanged("BuildingName");
                }
            }
        }

        public string BuildingFullName
        {
            get { return masterData.BuildingFullName; }
            set
            {
                if (value != masterData.BuildingFullName)
                {
                    masterData.BuildingFullName = value;
                    OnPropertyChanged("BuildingFullName");
                }
            }
        }

        public string BuildingCity
        {
            get { return masterData.BuildingCity; }
            set
            {
                if (value != masterData.BuildingCity)
                {
                    masterData.BuildingCity = value;
                    OnPropertyChanged("BuildingCity");
                }
            }
        }

        public string BuildingZipCode
        {
            get { return masterData.BuildingZipCode; }
            set
            {
                if (value != masterData.BuildingZipCode)
                {
                    masterData.BuildingZipCode = value;
                    OnPropertyChanged("BuildingZipCode");
                }
            }
        }

        public string BuildingRoadName
        {
            get { return masterData.BuildingRoadName; }
            set
            {
                if (value != masterData.BuildingRoadName)
                {
                    masterData.BuildingRoadName = value;
                    OnPropertyChanged("BuildingRoadName");
                }
            }
        }

        public string BuildingRoadNumber
        {
            get { return masterData.BuildingRoadNumber; }
            set
            {
                if (value != masterData.BuildingRoadNumber)
                {
                    masterData.BuildingRoadNumber = value;
                    OnPropertyChanged("BuildingRoadNumber");
                }
            }
        }

        public MasterDataPart(Building SelectedBuilding = null)
        {
            InitializeComponent();
            DataContext = this;
            masterData = new BuildingMasterData();
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

    public class BuildingMasterData
    {
        public string BuildingName { get; set; }
        public string BuildingFullName { get; set; }
        public string BuildingCity { get; set; }
        public string BuildingZipCode { get; set; }
        public string BuildingRoadName { get; set; }
        public string BuildingRoadNumber { get; set; }
    }
}
