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
using DomenaManager.Helpers;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for SettlementSummary.xaml
    /// </summary>
    public partial class SettlementSummaryPage : UserControl, INotifyPropertyChanged
    {
        public Visibility IsSettlementPerApartment { get; set; }

        private string _selectedBuilding;
        public string SelectedBuilding
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

        private double _invoiceSum;
        public double InvoiceSum
        {
            get { return _invoiceSum; }
            set
            {
                if (value != _invoiceSum)
                {
                    _invoiceSum = value;
                    OnPropertyChanged("InvoiceSum");
                }
            }
        }

        private int _apartmentsAmount;
        public int ApartmentsAmount
        {
            get { return _apartmentsAmount; }
            set
            {
                if (value != _apartmentsAmount)
                {
                    _apartmentsAmount = value;
                    OnPropertyChanged("ApartmentsAmount");
                }
            }
        }

        private string _settlementCattegory;
        public string SettlementCattegory
        {
            get { return _settlementCattegory; }
            set
            {
                if (value != _settlementCattegory)
                {
                    _settlementCattegory = value;
                    OnPropertyChanged("SettlementCattegory");
                }
            }
        }

        private double _settlePerApartment;
        public double SettlePerApartment
        {
            get { return _settlePerApartment; }
            set
            {
                if (value != _settlePerApartment)
                {
                    _settlePerApartment = value;
                    OnPropertyChanged("SettlePerApartment");
                }
            }
        }

        private ObservableCollection<ApartamentMeterDataGrid> _perApartmentCollection;
        public ObservableCollection<ApartamentMeterDataGrid> PerApartmentCollection
        {
            get { return _perApartmentCollection; }
            set
            {
                if (value != _perApartmentCollection)
                {
                    _perApartmentCollection = value;
                    OnPropertyChanged("PerApartmentCollection");
                }
            }
        }

        public Visibility IsSettlementPerMeter { get; set; }

        public Visibility IsSettlementPerArea { get; set; }

        public ICommand SettlementProceed
        {
            get { return new RelayCommand(PerformSettlement, CanPerformSettlement); }
        }

        public ICommand SettlementBack
        {
            get { return new RelayCommand(CancelSettlement, CanPerformSettlement); }
        }
        private SettlementPage _settlementPage;

        public SettlementSummaryPage(SettlementPage settlementPage)
        {
            _settlementPage = settlementPage;
            IsSettlementPerApartment = Visibility.Collapsed;
            IsSettlementPerMeter = Visibility.Collapsed;
            IsSettlementPerArea = Visibility.Collapsed;  
            switch (settlementPage.SettlementMethod)
            {
                default:
                    break;
                case SettlementMethodsEnum.PER_APARTMENT:
                    IsSettlementPerApartment = System.Windows.Visibility.Visible;
                    break;
                case SettlementMethodsEnum.PER_AREA:
                    IsSettlementPerArea = System.Windows.Visibility.Visible;
                    break;
                case SettlementMethodsEnum.PER_METERS:
                    IsSettlementPerMeter = System.Windows.Visibility.Visible;
                    break;
            }
            DataContext = this;
            PopulateData();
            InitializeComponent();
        }

        private void PopulateData()
        {
            SelectedBuilding = _settlementPage.SelectedBuildingName.Name;
            InvoiceSum = _settlementPage.SettledInvoices.Select(x => x.CostAmount).DefaultIfEmpty(0).Sum();
            ApartmentsAmount = _settlementPage.ApartmentCollection.Where(x => !x.IsDeleted && x.BuildingId.Equals(_settlementPage.SelectedBuildingName.BuildingId)).Count();
            SettlementCattegory = _settlementPage.SettlementCategoryName.CategoryName;
            var notRounded = InvoiceSum / ApartmentsAmount;
            SettlePerApartment = (Math.Ceiling(notRounded * 100)) / 100;
            //PerApartmentCollection = _settlementPage.ApartmentMetersCollection;
        }

        private void PerformSettlement(object param)
        {

        }

        private void CancelSettlement(object param)
        {
            var mw = (((((this.Parent as MahApps.Metro.Controls.TransitioningContentControl).Parent as Grid).Parent as DialogHost).Parent as DialogHost).Parent as DialogHost).Parent as Windows.MainWindow;

            mw.CurrentPage = _settlementPage;
        }

        private bool CanPerformSettlement()
        {
            return true;
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
