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
    public partial class EditSettlementWizard : UserControl, INotifyPropertyChanged
    {
        private SettlementWizard.SettlementPart settlementView;
        private SettlementWizard.MasterDataPart masterDataView;
        private SettlementWizard.InvoicesPart invoicesView;
        private SettlementWizard.SummaryPart summaryView;

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

        private CurrentSettlementPageEnum _currentPage;
        public CurrentSettlementPageEnum CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (value != _currentPage)
                {
                    _currentPage = value;
                    OnPropertyChanged("CurrentPage");
                    OnPropertyChanged("MasterDataForeground");
                    OnPropertyChanged("InvoicesForeground");
                    OnPropertyChanged("SettlementForeground");
                    OnPropertyChanged("SummaryForeground");
                }
            }
        }

        public Brush MasterDataForeground
        {
            get
            {
                return CurrentPage == CurrentSettlementPageEnum.MasterData ? Brushes.Green : Brushes.Gray;
            }
        }

        public Brush SettlementForeground
        {
            get
            {
                return CurrentPage == CurrentSettlementPageEnum.Settlement ? Brushes.Green : Brushes.Gray;
            }
        }

        public Brush InvoicesForeground
        {
            get
            {
                return CurrentPage == CurrentSettlementPageEnum.Invoices ? Brushes.Green : Brushes.Gray;
            }
        }

        public Brush SummaryForeground
        {
            get
            {
                return CurrentPage == CurrentSettlementPageEnum.Summary ? Brushes.Green : Brushes.Gray;
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

        private Building _localBuildingCopy;

        public EditSettlementWizard(Building SelectedBuilding = null)
        {
            InitializeComponent();
            _localBuildingCopy = SelectedBuilding;
            CurrentPage = CurrentSettlementPageEnum.MasterData;
            masterDataView = new SettlementWizard.MasterDataPart(_localBuildingCopy);
            summaryView = new SettlementWizard.SummaryPart(_localBuildingCopy);
            invoicesView = new SettlementWizard.InvoicesPart();
            settlementView = new SettlementWizard.SettlementPart();
            WizardControl = masterDataView;
            DataContext = this;
        }        

        public void GoPrevious(object param)
        {
            switch (CurrentPage)
            {
                default:
                    return;
                case CurrentSettlementPageEnum.Invoices:
                    CurrentPage = CurrentSettlementPageEnum.MasterData;
                    WizardControl = masterDataView;
                    return;
                case CurrentSettlementPageEnum.Settlement:
                    CurrentPage = CurrentSettlementPageEnum.Invoices;
                    WizardControl = invoicesView;
                    return;
                case CurrentSettlementPageEnum.Summary:
                    CurrentPage = CurrentSettlementPageEnum.Settlement;
                    WizardControl = settlementView;
                    return;
            }
        }

        public bool CanGoPrevious()
        {
            return CurrentPage != CurrentSettlementPageEnum.MasterData;
        }

        public void GoNext(object param)
        {
            switch (CurrentPage)
            {
                default:
                    return;
                case CurrentSettlementPageEnum.MasterData:
                    if (Validator.IsValid(masterDataView))
                    {
                        CurrentPage = CurrentSettlementPageEnum.Invoices;
                        WizardControl = invoicesView;
                        invoicesView.InvoiceData.MasterData = masterDataView.masterData;
                        invoicesView.InitializeInvoicesPart();
                        return;
                    }
                    else
                    {
                        Validator.IsValid(masterDataView);
                        return;
                    }
                case CurrentSettlementPageEnum.Invoices:
                    CurrentPage = CurrentSettlementPageEnum.Settlement;
                    settlementView.SettlementData = new SettlementWizard.SettlementData();
                    settlementView.SettlementData.InvoiceData = invoicesView.InvoiceData;
                    settlementView.InitializeView();
                    WizardControl = settlementView;
                    return;
                case CurrentSettlementPageEnum.Settlement:
                    settlementView.PackViewResult();
                    CurrentPage = CurrentSettlementPageEnum.Summary;
                    WizardControl = summaryView;
                    summaryView.SummaryData = new SettlementWizard.SummaryData();
                    summaryView.SummaryData.SettlementData = settlementView.SettlementData;
                    summaryView.InitializeView();
                    return;
            }
        }

        public bool CanGoNext()
        {
            return CurrentPage != CurrentSettlementPageEnum.Summary;
        }

        public void Accept(object param)
        {

            SwitchPage.SwitchMainPage(new Pages.BuildingsPage(), this);
        }

        public bool CanAccept()
        {
            return CurrentPage == CurrentSettlementPageEnum.Summary;
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

    public enum CurrentSettlementPageEnum
    {
        MasterData = 0,
        Invoices = 1,
        Settlement = 2,
        Summary = 3,
    }
}
