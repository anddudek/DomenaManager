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
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

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
            DataContext = this;
            InitializeComponent();

            SwitchPage("Buildings");


            using (var db = new DB.DomenaDBContext())
            {
                /*
                //db.Buildings.Add(new LibDataModel.Building { Name = "ELO", BuildingId = Guid.NewGuid(), BuildingNumber = "7", City = "Świątniki", RoadName = "Kreta", ZipCode="32-040" });
                //db.Buildings.Add(new LibDataModel.Building { Name = "Grunwaldzka 61", BuildingId = Guid.NewGuid(), BuildingNumber = "61", City = "Jelenia Góra", RoadName = "Grunwaldzka", ZipCode = "58-500" });
                db.CostCategories.Add(new LibDataModel.CostCategory { CategoryName = "Prąd", CostCategoryId = Guid.NewGuid() });
                db.CostCategories.Add(new LibDataModel.CostCategory { CategoryName = "Woda i ścieki", CostCategoryId = Guid.NewGuid() });
                db.CostCategories.Add(new LibDataModel.CostCategory { CategoryName = "Utrzymanie czystości", CostCategoryId = Guid.NewGuid() });
                db.CostCategories.Add(new LibDataModel.CostCategory { CategoryName = "Teren zewnętrzny", CostCategoryId = Guid.NewGuid() });
                db.CostCategories.Add(new LibDataModel.CostCategory { CategoryName = "Ubezpieczenie", CostCategoryId = Guid.NewGuid() });
                db.CostCategories.Add(new LibDataModel.CostCategory { CategoryName = "Remonty", CostCategoryId = Guid.NewGuid() });
                db.CostCategories.Add(new LibDataModel.CostCategory { CategoryName = "Naprawy i konserwacje", CostCategoryId = Guid.NewGuid() });
                db.CostCategories.Add(new LibDataModel.CostCategory { CategoryName = "Przeglądy", CostCategoryId = Guid.NewGuid() });
                db.CostCategories.Add(new LibDataModel.CostCategory { CategoryName = "Wynagrodzenie zarządcy", CostCategoryId = Guid.NewGuid() });
                db.CostCategories.Add(new LibDataModel.CostCategory { CategoryName = "Inne", CostCategoryId = Guid.NewGuid() });
                //db.CostDistributionTypes.Add(new LibDataModel.CostDistributionType { CostDistributionId = Guid.NewGuid(), CostDistributionName = "Od lokalu" });
                //db.CostDistributionTypes.Add(new LibDataModel.CostDistributionType { CostDistributionId = Guid.NewGuid(), CostDistributionName = "Od powierzchni" });
                
                var GrunwGuid = db.Buildings.Where(x => x.BuildingNumber == "61").FirstOrDefault().BuildingId;
                var ubez = db.CostCategories.Where(x => x.CategoryName == "Ubezpieczenie").FirstOrDefault().CostCategoryId;
                var domena = db.CostCategories.Where(x => x.CategoryName == "Wynagrodzenie zarządcy").FirstOrDefault().CostCategoryId;
                var woda = db.CostCategories.Where(x => x.CategoryName == "Woda i ścieki").FirstOrDefault().CostCategoryId;
                var dist = db.CostDistributionTypes.Where(x => x.CostDistributionName == "Od powierzchni").FirstOrDefault().CostDistributionId;
                var lok = db.CostDistributionTypes.Where(x => x.CostDistributionName == "Od lokalu").FirstOrDefault().CostDistributionId;
                db.Costs.Add(new LibDataModel.Cost { BuildingId=GrunwGuid, ContractorName="Allianz ubezpieczenia", CostAmount=429, CostCategoryId=ubez, CostDistributionId=dist, CostId=Guid.NewGuid(), CreatedTime=DateTime.Now, InvoiceNumber="Polisa nr 6356947", PaymentTime= new DateTime(2017,11,22)});
                db.Costs.Add(new LibDataModel.Cost { BuildingId = GrunwGuid, ContractorName = "ZN Domena", CostAmount = 255, CostCategoryId = domena, CostDistributionId = lok, CostId = Guid.NewGuid(), CreatedTime = DateTime.Now, InvoiceNumber = "Faktura nr 1/2017", PaymentTime = new DateTime(2017, 11, 12) });
                db.Costs.Add(new LibDataModel.Cost { BuildingId = GrunwGuid, ContractorName = "Wodnik", CostAmount = 105.78, CostCategoryId = woda, CostDistributionId = lok, CostId = Guid.NewGuid(), CreatedTime = DateTime.Now, InvoiceNumber = "Faktura nr 89219/2017", PaymentTime = new DateTime(2017, 11, 29) });
                db.Costs.Add(new LibDataModel.Cost { BuildingId = GrunwGuid, ContractorName = "ZN Domena", CostAmount = 255, CostCategoryId = domena, CostDistributionId = lok, CostId = Guid.NewGuid(), CreatedTime = DateTime.Now, InvoiceNumber = "Faktura nr 3/2017", PaymentTime = new DateTime(2017, 12, 2) });
                */
                //db.Owners.Add(new LibDataModel.Owner { OwnerId = Guid.NewGuid(), IsDeleted = false, MailAddress = "ul. Krzaczasta 5, /r/n 30-389 Kraków", OwnerName="Dominik Biegański" });
                
                //db.Apartments.Add(new LibDataModel.Apartment { ApartmentId=Guid.NewGuid(), AdditionalArea = 15, ApartmentArea=45, ApartmentNumber = 7, CreatedDate = DateTime.Today, HasWaterMeter=false, IsDeleted= false, OwnerId = Guid.Parse("2FE5BADA-1FF5-4F01-81B5-F4A7470B5DDC"), BuildingId=Guid.Parse("CDBBEEDB-EC2F-49E1-9B74-71AAB9ED2102")  });
                //db.Invoices.Add(new LibDataModel.Invoice() { BuildingId = db.Buildings.FirstOrDefault().BuildingId, ContractorName = "Wodnik", CostAmount = 100, CreatedTime = DateTime.Today, InvoiceCategoryId = Guid.NewGuid(), InvoiceDate = DateTime.Today.AddDays(-1), InvoiceId = Guid.NewGuid(), InvoiceNumber = "ASB01/2018" });
                
                //db.SaveChanges();
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
                                db.CostCategories.Where(x => x.CostCategoryId.Equals(cmd.costItem.CostCategoryId)).FirstOrDefault().IsDeleted = true;
                                db.SaveChanges();
                                break;
                            case CostCategoryEnum.CostCategoryCommandEnum.Update:
                                db.CostCategories.Where(x => x.CostCategoryId.Equals(cmd.costItem.CostCategoryId)).FirstOrDefault().CategoryName = cmd.costItem.CategoryName;
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
