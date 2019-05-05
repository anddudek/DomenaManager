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

namespace DomenaManager.Wizards.SettlementWizard
{
    /// <summary>
    /// Interaction logic for EditBuildingWizard.xaml
    /// </summary>
    public partial class MasterDataPart : UserControl, INotifyPropertyChanged
    {
        public SettlementMasterData masterData;

        private ObservableCollection<Building> _buildings;
        public ObservableCollection<Building> Buildings
        {
            get
            {
                return _buildings;
            }
            set
            {
                if (_buildings != value)
                {
                    _buildings = value;
                    OnPropertyChanged("Buildings");
                }
            }
        }

        public Building SelectedBuilding
        {
            get { return masterData.Building; }
            set
            {
                if (value != masterData.Building)
                {
                    masterData.Building = value;
                    OnPropertyChanged("SelectedBuilding");
                }
            }
        }

        public string SelectedBuildingValue { get; set; }

        public DateTime StartingDate
        {
            get
            {
                return masterData.StartingDate;
            }
            set
            {
                if (value != masterData.StartingDate)
                {
                    masterData.StartingDate = value;
                    OnPropertyChanged("StartingDate");
                }
            }
        }

        public DateTime EndingDate
        {
            get
            {
                return masterData.EndingDate;
            }
            set
            {
                if (value != masterData.EndingDate)
                {
                    masterData.EndingDate = value;
                    OnPropertyChanged("EndingDate");
                }
            }
        }
        
        public SettlementTypeEnum SelectedSummary
        {
            get
            {
                return masterData.SettlementType;
            }
            set
            {
                if (value != masterData.SettlementType)
                {
                    masterData.SettlementType = value;
                    OnPropertyChanged("SelectedSummary");
                }
            }
        }

        public string SelectedSummaryValue { get; set; }

        public MasterDataPart(Building SelectedBuilding = null)
        {
            InitializeComponent();
            DataContext = this;
            InitializeCollections();
            masterData = new SettlementMasterData();
            
        }            
        
        private void InitializeCollections()
        {
            using (var db = new DB.DomenaDBContext())
            {
                Buildings = new ObservableCollection<Building>(db.Buildings.Where(x => !x.IsDeleted).ToList());
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

    public class SettlementMasterData
    {
        public Building Building { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public SettlementTypeEnum SettlementType { get; set; }

        public SettlementMasterData()
        {
            StartingDate = new DateTime(DateTime.Today.Year, 1, 1);
            EndingDate = DateTime.Today;
            SettlementType = SettlementTypeEnum.UNITS;
        }
    }
}
