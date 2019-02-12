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
    public partial class CountersPart : UserControl, INotifyPropertyChanged
    {
        private CountersData countersData { get; set; }
        
        private string _meterName;
        public string MeterName
        {
            get { return _meterName; }
            set
            {
                if (value != _meterName)
                {
                    _meterName = value;
                    OnPropertyChanged("MeterName");
                }
            }
        }

        private double _lastMeasure;
        public double LastMeasure
        {
            get { return _lastMeasure; }
            set
            {
                if (value != _lastMeasure)
                {
                    _lastMeasure = value;
                    OnPropertyChanged("LastMeasure");
                }
            }
        }

        private bool _isBuilding;
        public bool IsBuilding
        {
            get { return _isBuilding; }
            set
            {
                if (value != _isBuilding)
                {
                    _isBuilding = value;
                    OnPropertyChanged("IsBuilding");
                }
            }
        }

        private bool _isApartment;
        public bool IsApartment
        {
            get { return _isApartment; }
            set
            {
                if (value != _isApartment)
                {
                    _isApartment = value;
                    OnPropertyChanged("IsApartment");
                }
            }
        }
                
        public ObservableCollection<MeterType> MetersCollection
        {
            get { return countersData.MetersCollection; }
            set
            {
                if (value != countersData.MetersCollection)
                {
                    countersData.MetersCollection = value;
                    OnPropertyChanged("MetersCollection");
                }
            }
        }

        private MeterType _selectedMeter;
        public MeterType SelectedMeter
        {
            get { return _selectedMeter; }
            set
            {
                if (value != _selectedMeter)
                {
                    _selectedMeter = value;
                    OnPropertyChanged("SelectedMeter");
                    if (value != null)
                    {
                        MeterName = value.Name;
                        LastMeasure = value.LastMeasure;
                        IsApartment = value.IsApartment;
                        IsBuilding = value.IsBuilding;
                    }
                }
            }
        }

        public ICommand AddMeter
        {
            get
            {
                return new Helpers.RelayCommand(AddNewMeter, CanAddNewMeter);
            }
        }

        public ICommand ModifySelectedMeter
        {
            get
            {
                return new Helpers.RelayCommand(ModifyMeter, CanModifyMeter);
            }
        }

        public ICommand DeleteSelectedMeter
        {
            get
            {
                return new Helpers.RelayCommand(DeleteMeter, CanDeleteMeter);
            }
        }

        public CountersPart(Building SelectedBuilding = null)
        {
            InitializeComponent();
            DataContext = this;
            countersData = new CountersData(SelectedBuilding);
        }

        private void AddNewMeter(object param)
        {
            MeterType mt = new MeterType() { Name = MeterName, IsDeleted = false, MeterId = Guid.NewGuid(), LastMeasure = LastMeasure, IsApartment = IsApartment, IsBuilding = IsBuilding };
            MetersCollection.Add(mt);
        }

        private bool CanAddNewMeter()
        {
            return !String.IsNullOrWhiteSpace(MeterName);
        }

        private void ModifyMeter(object param)
        {
            SelectedMeter.Name = MeterName;
            SelectedMeter.LastMeasure = LastMeasure;
            SelectedMeter.IsApartment = IsApartment;
            SelectedMeter.IsBuilding = IsBuilding;
        }

        private bool CanModifyMeter()
        {
            return SelectedMeter != null;
        }

        private void DeleteMeter(object param)
        {
            MetersCollection.Remove(SelectedMeter);
        }

        private bool CanDeleteMeter()
        {
            return SelectedMeter != null;
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

    public class CountersData
    {
        public ObservableCollection<MeterType> MetersCollection { get; set; }

        public CountersData(Building b)
        {
            MetersCollection = b != null ? new ObservableCollection<MeterType>(b.MeterCollection) : new ObservableCollection<MeterType>();           
        }
    }
}
