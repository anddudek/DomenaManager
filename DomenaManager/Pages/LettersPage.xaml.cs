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
    public partial class LettersPage : UserControl, INotifyPropertyChanged
    {
        #region Bindings

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (value != _selectedDate)
                {
                    _selectedDate = value;
                    OnPropertyChanged("SelectedDate");
                }
            }
        }

        private List<string> _letterValues;
        public List<string> LetterValues
        {
            get { return _letterValues; }
            set
            {
                if (value != _letterValues)
                {
                    _letterValues = value;
                    OnPropertyChanged("LetterValues");
                }
            }
        }

        private string _selectedLetterType;
        public string SelectedLetterType
        {
            get { return _selectedLetterType; }
            set
            {
                if (value != _selectedLetterType)
                {
                    _selectedLetterType = value;
                    OnPropertyChanged("SelectedLetterType");
                }
            }
        }

        private string _selectedLetterValue;
        public string SelectedLetterValue
        {
            get { return _selectedLetterValue; }
            set
            {
                if (value != _selectedLetterValue)
                {
                    _selectedLetterValue = value;
                    OnPropertyChanged("SelectedLetterValue");
                }
            }
        }

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

        private ObservableCollection<BindingDataGrid> _selectedApartments;
        public ObservableCollection<BindingDataGrid> SelectedApartments
        {
            get { return _selectedApartments; }
            set
            {
                if (value != _selectedApartments)
                {
                    _selectedApartments = value;
                    OnPropertyChanged("SelectedApartments");
                }
            }
        }

        public ICommand CreatePdfCommand
        {
            get { return new Helpers.RelayCommand(CreatePDF, CanCreatePDF); }
        }

        #endregion

        private const string YearSettlement = "Zestawienie roczne";
        private const string MonthlySummary = "Miesięczne naliczenie";

        public LettersPage()
        {
            DataContext = this;
            InitializeComponent();
            InitializeCollection();
            InitializeAvailableApartmentsCollection();
        }

        public void InitializeCollection()
        {
            SelectedDate = DateTime.Today;
            LetterValues = new List<string>() { YearSettlement, MonthlySummary };
        }

        public void InitializeAvailableApartmentsCollection()
        {
            SelectedApartments = new ObservableCollection<BindingDataGrid>();
            using (var db = new DB.DomenaDBContext())
            {
                AvailableApartments = new ObservableCollection<BindingDataGrid>();
                foreach (var ap in db.Apartments.Where(x => !x.IsDeleted))
                {
                    BindingDataGrid bdg = new BindingDataGrid();
                    bdg.apartment = ap;
                    bdg.building = db.Buildings.FirstOrDefault(x => x.BuildingId.Equals(ap.BuildingId));
                    bdg.owner = db.Owners.FirstOrDefault(x => x.OwnerId.Equals(ap.OwnerId));
                    AvailableApartments.Add(bdg);
                }
            }
        }
        
        private bool CanCreatePDF()
        {
            return SelectedApartments != null && SelectedApartments.Count > 0 ;
        }

        private void CreatePDF(object obj)
        {
            if (SelectedLetterValue == MonthlySummary)
            {
                List<ChargeDataGrid> selectedCharges = new List<ChargeDataGrid>();
                using (var db = new DB.DomenaDBContext())
                {
                    foreach (var c in db.Charges.Include(x => x.Components))
                    {
                        if (!c.IsDeleted && 
                            c.ChargeDate.Month == SelectedDate.Month && 
                            c.ChargeDate.Year == SelectedDate.Year &&
                            AvailableApartments.Any(x => x.apartment.ApartmentId.Equals(c.ApartmentId)))
                        {
                            selectedCharges.Add(new ChargeDataGrid(c));
                        }
                    }
                }

                foreach(var s in selectedCharges)
                {
                    PDFOperations.PrepareSingleChargeReport(s, true);
                }               
            }
            else if (SelectedLetterValue == YearSettlement)
            {

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
