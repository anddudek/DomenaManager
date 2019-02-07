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
    public partial class EditBuildingWizard : UserControl, INotifyPropertyChanged
    {
        private CountersPart countersView;
        private MasterDataPart masterDataView;
        private InvoicesPart invoicesView;
        private ChargesPart chargesView;

        private UserControl _wizardControl;
        public UserControl WizardControl
        {
            get { return _wizardControl; }
            set
            {
                if (value != _wizardControl)
                {
                    _wizardControl = value;
                    OnPropertyChanged("WizardControl");
                }
            }
        }

        private CurrentPageEnum _currentPage;
        public CurrentPageEnum CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (value != _currentPage)
                {
                    _currentPage = value;
                    OnPropertyChanged("CurrentPage");
                    OnPropertyChanged("MasterDataForeground");
                    OnPropertyChanged("ChargesForeground");
                    OnPropertyChanged("InvoicesForeground");
                    OnPropertyChanged("CountersForeground");
                }
            }
        }

        public Brush MasterDataForeground
        {
            get
            {
                return CurrentPage == CurrentPageEnum.MasterData ? Brushes.Green : Brushes.Gray;
            }
        }

        public Brush ChargesForeground
        {
            get
            {
                return CurrentPage == CurrentPageEnum.Charges ? Brushes.Green : Brushes.Gray;
            }
        }

        public Brush InvoicesForeground
        {
            get
            {
                return CurrentPage == CurrentPageEnum.Invoices ? Brushes.Green : Brushes.Gray;
            }
        }

        public Brush CountersForeground
        {
            get
            {
                return CurrentPage == CurrentPageEnum.Counters ? Brushes.Green : Brushes.Gray;
            }
        }

        public ICommand PreviousCommand
        {
            get { return new RelayCommand(GoPrevious, CanGoPrevious); }
        }

        public ICommand NextCommand
        {
            get { return new RelayCommand(GoNext, CanGoNext); }
        }

        public ICommand AcceptCommand
        {
            get { return new RelayCommand(Accept, CanAccept); }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(Cancel, CanCancel); }
        }

        public EditBuildingWizard(Building SelectedBuilding = null)
        {
            InitializeComponent();
            CurrentPage = CurrentPageEnum.MasterData;
            masterDataView = new MasterDataPart();
            chargesView = new ChargesPart();
            invoicesView = new InvoicesPart();
            countersView = new CountersPart();
            WizardControl = masterDataView;
            DataContext = this;
        }        

        public void GoPrevious(object param)
        {
            switch (CurrentPage)
            {
                default:
                    return;
                case CurrentPageEnum.MasterData:
                    return;
                case CurrentPageEnum.Charges:
                    CurrentPage = CurrentPageEnum.MasterData;
                    WizardControl = masterDataView;
                    return;
                case CurrentPageEnum.Invoices:
                    CurrentPage = CurrentPageEnum.Charges;
                    WizardControl = chargesView;
                    return;
                case CurrentPageEnum.Counters:
                    CurrentPage = CurrentPageEnum.Invoices;
                    WizardControl = invoicesView;
                    return;
            }
        }

        public bool CanGoPrevious()
        {
            return CurrentPage != CurrentPageEnum.MasterData;
        }

        public void GoNext(object param)
        {
            switch (CurrentPage)
            {
                default:
                    return;
                case CurrentPageEnum.MasterData:
                    if (Validator.IsValid(masterDataView))
                    {
                        CurrentPage = CurrentPageEnum.Charges;
                        WizardControl = chargesView;
                        return;
                    }
                    else
                    {
                        Validator.IsValid(masterDataView);
                        return;
                    }
                case CurrentPageEnum.Charges:
                    CurrentPage = CurrentPageEnum.Invoices;
                    WizardControl = invoicesView;
                    return;
                case CurrentPageEnum.Invoices:
                    CurrentPage = CurrentPageEnum.Counters;
                    WizardControl = countersView;
                    return;
                case CurrentPageEnum.Counters:
                    return;
            }
        }

        public bool CanGoNext()
        {
            return CurrentPage != CurrentPageEnum.Counters;
        }

        public void Accept(object param)
        {

        }

        public bool CanAccept()
        {
            return CurrentPage == CurrentPageEnum.Counters;
        }

        public void Cancel(object param)
        {
            SwitchPage.SwitchMainPage(new Pages.BuildingsPage(), this);
        }

        public bool CanCancel()
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

    public enum CurrentPageEnum
    {
        MasterData = 0,
        Charges = 1,
        Invoices = 2,
        Counters = 3,
    }
}
