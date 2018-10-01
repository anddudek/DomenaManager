using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
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
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Serilog;
using System.Data.Entity;

namespace DomenaManager.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        private UserControl _currentPage;
        public UserControl CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }
        
        public ICommand SwitchPageCommand
        {
            get
            {
                return new Helpers.RelayCommand(param => SwitchPage(param), CanSwitchPage);
            }
        }

        public ICommand EditCostCategoriesCommand
        {
            get
            {
                return new RelayCommand(EditCostCategories, CanEditCostCategories);
            }
        }

        public ICommand EditInvoiceCategoriesCommand
        {
            get
            {
                return new RelayCommand(EditInvoiceCategories, CanEditInvoiceCategories);
            }
        }

        public MainWindow()
        {
            System.IO.Directory.CreateDirectory(@"C:/DomenaManager/Logs");
            System.IO.Directory.CreateDirectory(@"C:/DomenaManager/Backup");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(@"C:/DomenaManager/Logs/DomenaManager-{Date}.log")
                .CreateLogger();

            // this is the line you really want 
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, args) => CurrentDomainOnUnhandledException(args, Log.Logger);

            // optional: hooking up some more handlers
            // remember that you need to hook up additional handlers when 
            // logging from other dispatchers, shedulers, or applications

            //Application.Dispatcher.UnhandledException +=
            //    (sender, args) => DispatcherOnUnhandledException(args, Log.Logger);

            Application.Current.DispatcherUnhandledException +=
                (sender, args) => CurrentOnDispatcherUnhandledException(args, Log.Logger);

            TaskScheduler.UnobservedTaskException +=
                (sender, args) => TaskSchedulerOnUnobservedTaskException(args, Log.Logger);
                        

            DataContext = this;
            InitializeComponent();

            SwitchPage("Buildings");


            using (var db = new DB.DomenaDBContext())
            {
                string name = @"C:/DomenaManager/Backup/DomenaManagerDB_" + DateTime.Today.ToString("ddMMyyyy") + ".bak";
                if (!System.IO.File.Exists(name))
                {
                    //string cmd = String.Format("BACKUP DATABASE {0} TO DISK = '{1}'", "DBDomena", name);
                    //int backup = db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);
                }
            }
        }

        public void SwitchPage(object PageName)
        {
            switch ((string)PageName)
            {
                default:
                    return;
                case "Buildings":
                    CurrentPage = new Pages.BuildingsPage();                    
                    OnPropertyChanged("CurrentPage");
                    return;
                case "Owners":
                    CurrentPage = new Pages.OwnersPage();
                    OnPropertyChanged("CurrentPage");
                    return;
                case "Apartments":
                    CurrentPage = new Pages.ApartmentsPage();
                    OnPropertyChanged("CurrentPage");
                    return;
                case "Charges":
                    CurrentPage = new Pages.ChargesPage();
                    OnPropertyChanged("CurrentPage");
                    return;
                case "Invoices":
                    CurrentPage = new Pages.InvoicesPage();
                    OnPropertyChanged("CurrentPage");
                    return;
                case "Payments":
                    CurrentPage = new Pages.PaymentsPage();
                    OnPropertyChanged("CurrentPage");
                    return;
                case "Summary":
                    CurrentPage = new Pages.SummaryPage();
                    OnPropertyChanged("CurrentPage");
                    return;
                case "Bindings":
                    CurrentPage = new Pages.BindingsPage();
                    OnPropertyChanged("CurrentPage");
                    return;
                case "Settlement":
                    CurrentPage = new Pages.SettlementPage();
                    OnPropertyChanged("CurrentPage");
                    return;
            }
        }

        public bool CanSwitchPage()
        {
            return true;
        }

        private bool CanEditCostCategories()
        {
            return true;
        }

        private async void EditCostCategories(object obj)
        {
            Wizards.EditCostCategories ecc;            
            ecc = new Wizards.EditCostCategories();
            var result = await DialogHost.Show(ecc, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingCostCategoriesEventHandler);
        }

        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private async void ExtendedClosingCostCategoriesEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {

            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditCostCategories);
                //Accept
                using (var db = new DB.DomenaDBContext())
                {
                    foreach (var cmd in dc.commandBuffer)
                    {
                        switch (cmd.category)
                        {
                            default:
                                break;
                            case CostCategoryEnum.CostCategoryCommandEnum.Add:
                                db.CostCategories.Add(cmd.costItem);
                                db.SaveChanges();
                                break;
                            case CostCategoryEnum.CostCategoryCommandEnum.Remove:
                                db.CostCategories.Where(x => x.BuildingChargeBasisCategoryId.Equals(cmd.costItem.BuildingChargeBasisCategoryId)).FirstOrDefault().IsDeleted = true;
                                db.SaveChanges();
                                break;
                            case CostCategoryEnum.CostCategoryCommandEnum.Update:
                                db.CostCategories.Where(x => x.BuildingChargeBasisCategoryId.Equals(cmd.costItem.BuildingChargeBasisCategoryId)).FirstOrDefault().CategoryName = cmd.costItem.CategoryName;
                                db.SaveChanges();
                                break;
                        }
                    }
                }
            }
            else if (!(bool)eventArgs.Parameter)
            {

                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    var dc = (eventArgs.Session.Content as Wizards.EditCostCategories);
                    var result = await DialogHost.Show(dc, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingCostCategoriesEventHandler);
                }
            }
        }

        private bool CanEditInvoiceCategories()
        {
            return true;
        }

        private async void EditInvoiceCategories(object obj)
        {
            Wizards.EditInvoiceCategories eic;
            eic = new Wizards.EditInvoiceCategories();
            var result = await DialogHost.Show(eic, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingInvoiceCategoriesEventHandler);
        }

        private async void ExtendedClosingInvoiceCategoriesEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {

            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditInvoiceCategories);
                //Accept
                using (var db = new DB.DomenaDBContext())
                {
                    foreach (var cmd in dc.commandBuffer)
                    {
                        switch (cmd.category)
                        {
                            default:
                                break;
                            case CostCategoryEnum.CostCategoryCommandEnum.Add:
                                db.InvoiceCategories.Add(cmd.Item);
                                db.SaveChanges();
                                break;
                            case CostCategoryEnum.CostCategoryCommandEnum.Remove:
                                db.InvoiceCategories.Where(x => x.CategoryId.Equals(cmd.Item.CategoryId)).FirstOrDefault().IsDeleted = true;
                                db.SaveChanges();
                                break;
                            case CostCategoryEnum.CostCategoryCommandEnum.Update:
                                db.InvoiceCategories.Where(x => x.CategoryId.Equals(cmd.Item.CategoryId)).FirstOrDefault().CategoryName = cmd.Item.CategoryName;
                                db.SaveChanges();
                                break;
                        }
                    }
                }
            }
            else if (!(bool)eventArgs.Parameter)
            {

                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    var dc = (eventArgs.Session.Content as Wizards.EditInvoiceCategories);
                    var result = await DialogHost.Show(dc, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingInvoiceCategoriesEventHandler);
                }
            }
        }

        private static void TaskSchedulerOnUnobservedTaskException(UnobservedTaskExceptionEventArgs args, ILogger log)
        {
            log.Error(args.Exception, args.Exception.Message);
            args.SetObserved();
        }

        private static void CurrentOnDispatcherUnhandledException(DispatcherUnhandledExceptionEventArgs args, ILogger log)
        {
            log.Error(args.Exception, args.Exception.Message);
        }

        private static void DispatcherOnUnhandledException(DispatcherUnhandledExceptionEventArgs args, ILogger log)
        {
            log.Error(args.Exception, args.Exception.Message);
        }

        private static void CurrentDomainOnUnhandledException(UnhandledExceptionEventArgs args, ILogger log)
        {
            var exception = args.ExceptionObject as Exception;
            var terminatingMessage = args.IsTerminating ? " The application is terminating." : string.Empty;
            string exceptionMessage;
            if (exception != null && exception.Message != null)
            {
                exceptionMessage = exception.Message;
            }
            else
            {
                exceptionMessage = "An unmanaged exception occured.";
            }
            var message = string.Concat(exceptionMessage, terminatingMessage);
            log.Error(exception, message);
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
