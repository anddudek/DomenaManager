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
    public partial class SummaryPart : UserControl, INotifyPropertyChanged
    {
        public SummaryData SummaryData { get; set; }

        public string SelectedSummaryType
        {
            get
            {
                return SummaryData?.SettlementData?.InvoiceData?.MasterData?.SettlementType.Description();
            }
        }

        public Visibility IsUnitSettle
        {
            get
            {
                if (SummaryData?.SettlementData?.InvoiceData?.MasterData?.SettlementType == SettlementTypeEnum.UNITS)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public string IsMutual
        {
            get
            {
                return (SummaryData?.SettlementData?.IsMutualSettlement).HasValue && (SummaryData?.SettlementData?.IsMutualSettlement).Value ? "Tak" : "Nie";
            }
        }
        
        public SummaryPart(Building SelectedBuilding = null)
        {
            InitializeComponent();
            DataContext = this;
        }       
        
        public void InitializeView()
        {
            OnPropertyChanged("");
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

    public class SummaryData
    {
        public SettlementData SettlementData { get; set; }
    }
}
