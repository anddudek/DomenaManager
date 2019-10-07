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
using LibDataModel;
using MaterialDesignThemes.Wpf;
using System.Data.Entity;
using DomenaManager.Helpers;


namespace DomenaManager.Wizards
{
   /// <summary>
   /// Interaction logic for EditOwnerWizard.xaml
   /// </summary>
   public partial class SellApartmentWizard : UserControl, INotifyPropertyChanged
   {
      private string _ownerFirstName;
      public string OwnerFirstName
      {
         get { return _ownerFirstName; }
         set
         {
            _ownerFirstName = value;
            OnPropertyChanged("OwnerFirstName");
         }
      }

      private string _ownerSurame;
      public string OwnerSurname
      {
         get { return _ownerSurame; }
         set
         {
            _ownerSurame = value;
            OnPropertyChanged("OwnerSurname");
         }
      }

      private Building _selectedBuildingName;
      public Building SelectedBuildingName
      {
         get { return _selectedBuildingName; }
         set
         {
            _selectedBuildingName = value;
            OnPropertyChanged("SelectedBuildingName");
         }
      }

      public string SelectedBuildingValue { get; set; }


      private ObservableCollection<Building> _buildingsNames;
      public ObservableCollection<Building> BuildingsNames
      {
         get { return _buildingsNames; }
         set
         {
            _buildingsNames = value;
            OnPropertyChanged("BuildingsNames");
         }
      }

      private Apartment _selectedApartmentName;
      public Apartment SelectedApartmentName
      {
         get { return _selectedApartmentName; }
         set
         {
            _selectedApartmentName = value;
            OnPropertyChanged("SelectedApartmentName");
            UpdateChargesList();
         }
      }

      public string SelectedApartmentValue { get; set; }


      private ObservableCollection<Apartment> _apartmentNames;
      public ObservableCollection<Apartment> ApartmentNames
      {
         get { return _apartmentNames; }
         set
         {
            _apartmentNames = value;
            OnPropertyChanged("ApartmentNames");
         }
      }

      private Owner _buyerName;
      public Owner BuyerName
      {
         get { return _buyerName; }
         set
         {
            _buyerName = value;
            OnPropertyChanged("BuyerName");
         }
      }

      public string BuyerValue { get; set; }


      private ObservableCollection<Owner> _buyersList;
      public ObservableCollection<Owner> BuyersList
      {
         get { return _buyersList; }
         set
         {
            _buyersList = value;
            OnPropertyChanged("BuyersList");
         }
      }

      private int _locatorsAmount;
      public int LocatorsAmount
      {
         get { return _locatorsAmount; }
         set
         {
            if (value != _locatorsAmount)
            {
               _locatorsAmount = value;
               OnPropertyChanged("LocatorsAmount");
            }
         }
      }

      private DateTime _sellDate;
      public DateTime SellDate
      {
         get { return _sellDate; }
         set
         {
            _sellDate = value;
            OnPropertyChanged("SellDate");
         }
      }

      private ObservableCollection<ApartmentSellChargeDG> _chargesCollection;
      public ObservableCollection<ApartmentSellChargeDG> ChargesCollection
      {
         get
         {
            return _chargesCollection;
         }
         set
         {
            if (value != _chargesCollection)
            {
               _chargesCollection = value;
               OnPropertyChanged("ChargesCollection");
            }
         }
      }

      public ICommand UpdateAllFieldsCommand
      {
         get
         {
            return new Helpers.RelayCommand(UpdateAllFields, CanUpdateAllFields);
         }
      }

      public ICommand SaveCommand
      {
         get { return new RelayCommand(SaveDialog, CanSaveDialog); }
      }

      public ICommand CancelCommand
      {
         get { return new RelayCommand(CancelDialog, CanCancelDialog); }
      }

      public ICommand AcceptCommand
      {
         get
         {
            return new Helpers.RelayCommand(AcceptDialog, CanAcceptDialog);
         }
      }

      public SellApartmentWizard(ApartmentDataGrid SelectedApartment = null)
      {
         DataContext = this;
         InitializeComponent();
         SellDate = DateTime.Today;
         using (var db = new DB.DomenaDBContext())
         {
            BuildingsNames = new ObservableCollection<Building>(db.Buildings.ToList());
            ApartmentNames = new ObservableCollection<Apartment>(db.Apartments.ToList());
            BuyersList = new ObservableCollection<Owner>(db.Owners.ToList());
         }
         SelectedApartmentName = ApartmentNames.FirstOrDefault(x => x.ApartmentId == SelectedApartment.ApartmentId);
         SelectedBuildingName = BuildingsNames.FirstOrDefault(x => x.BuildingId == SelectedApartmentName.BuildingId);
         var owner = BuyersList.FirstOrDefault(x => x.OwnerId == SelectedApartmentName.OwnerId);
         OwnerFirstName = owner.OwnerName;
         OwnerSurname = owner.OwnerSurname;
         LocatorsAmount = 1;
      }

      private void SaveDialog(object param)
      {
         var newApartment = new Apartment(SelectedApartmentName);
         newApartment.ApartmentId = Guid.NewGuid();
         newApartment.Locators = LocatorsAmount;
         newApartment.OwnerId = BuyerName.OwnerId;
         newApartment.CorrespondenceAddress = null;
         newApartment.BoughtDate = SellDate;
         var meters = new List<ApartmentMeter>();
         foreach (var meter in newApartment.MeterCollection)
         {
            var m = new ApartmentMeter()
            {
               IsDeleted = meter.IsDeleted,
               LastMeasure = meter.LastMeasure,
               LegalizationDate = meter.LegalizationDate,
               MeterId = Guid.NewGuid(),
               MeterTypeParent = meter.MeterTypeParent,
            };
         }

         using (var db = new DB.DomenaDBContext())
         {
            db.Apartments.Add(newApartment);
            var soldApartment = db.Apartments.FirstOrDefault(x => x.ApartmentId == SelectedApartmentName.ApartmentId);
            soldApartment.OnSellCreatedApartmentGuid = newApartment.ApartmentId;
            soldApartment.SoldDate = SellDate;

            var dateToCompare = new DateTime(SellDate.Year, SellDate.Month, 1);
            var charges = db.Charges.Include(x => x.Components).Where(x => x.ApartmentId == SelectedApartmentName.ApartmentId && x.ChargeDate >= dateToCompare && !x.IsDeleted);
            // Charges after sold date to new apartment
            foreach (var charge in charges)
            {
               charge.ApartmentId = newApartment.ApartmentId;
               charge.OwnerId = newApartment.OwnerId;
            }
            charges = charges.Where(x => x.ChargeDate.Month == SellDate.Month && x.ChargeDate.Year == SellDate.Year);
            var ratio = (double)SellDate.Day / (double)DateTime.DaysInMonth(SellDate.Year, SellDate.Month);

            // Charges in month of sale split to 2
            foreach (var charge in charges)
            {
               var soldApartmentCharge = new Charge()
               {
                  ApartmentId = soldApartment.ApartmentId,
                  AutoChargeId = charge.AutoChargeId,
                  ChargeDate = charge.ChargeDate,
                  ChargeId = Guid.NewGuid(),
                  CreatedDate = DateTime.Today,
                  IsClosed = charge.IsClosed,
                  IsDeleted = charge.IsDeleted,
                  OwnerId = soldApartment.OwnerId,
                  SettlementId = charge.SettlementId,
                  Components = new List<ChargeComponent>(),
               };

               foreach (var cc in charge.Components)
               {
                  var chargeComponent = new ChargeComponent()
                  {
                     ChargeComponentId = Guid.NewGuid(),
                     CostCategoryId = cc.CostCategoryId,
                     CostDistribution = cc.CostDistribution,
                     CostPerUnit = cc.CostPerUnit,
                     GroupName = cc.GroupName,
                     Sum = Math.Round(cc.Sum * Convert.ToDecimal(ratio), 0),
                  };
                  soldApartmentCharge.Components.Add(chargeComponent);
                  cc.Sum -= chargeComponent.Sum;
               }

               db.Charges.Add(soldApartmentCharge);
            }
            db.SaveChanges();
         }

         SwitchPage.SwitchMainPage(new Pages.ApartmentsPage(), this);
      }

      private void UpdateChargesList()
      {
         if (SellDate == null || SelectedApartmentName == null)
         {
            return;
         }

         using (var db = new DB.DomenaDBContext())
         {
            var ltDay = new DateTime(SellDate.Year, SellDate.Month, DateTime.DaysInMonth(SellDate.Year, SellDate.Month));
            var gtDay = new DateTime(SellDate.Year, SellDate.Month, 1);

            var q = db.Charges.Include(x => x.Components).Include(x => x.Components.Select(y => y.GroupName)).Where(x => x.ApartmentId == SelectedApartmentName.ApartmentId &&
               x.ChargeDate >= gtDay &&
               x.ChargeDate <= ltDay);

            ChargesCollection = new ObservableCollection<ApartmentSellChargeDG>();

            foreach (var ch in q)
            {
               ChargesCollection.Add(
                  new ApartmentSellChargeDG()
                  {
                     Charge = ch,
                     TotalCost = ch.Components.Where(x => x.GroupName.BuildingChargeGroupNameId != BuildingChargeGroupName.RepairFundGroupId).Select(x => x.Sum).DefaultIfEmpty(0).Sum(),
                     SettlementType = ApartmentSellChargeSettlementType.SPLIT_BY_SELL_DATE,
                  }
                  );
            }
         }
      }

      private bool CanSaveDialog()
      {
         return true;
      }

      private void CancelDialog(object param)
      {
         SwitchPage.SwitchMainPage(new Pages.ApartmentsPage(), this);
      }

      private bool CanCancelDialog()
      {
         return true;
      }

      private bool CanAcceptDialog()
      {
         return true;
      }

      private void AcceptDialog(object param)
      {
         SaveDialog(null);
         SwitchPage.SwitchMainPage(new Pages.ApartmentsPage(), this);
      }

      private void UpdateAllFields(object param)
      {
         Helpers.Validator.IsValid(this);
      }

      private bool CanUpdateAllFields()
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
