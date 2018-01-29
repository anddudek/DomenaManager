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

namespace DomenaManager.Pages
{
    /// <summary>
    /// Interaction logic for OwnersPage.xaml
    /// </summary>
    public partial class OwnersPage : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<OwnerDataGrid> _owners;
        public ObservableCollection<OwnerDataGrid> Owners
        {
            get { return _owners; }
            set
            {
                _owners = value;
                OnPropertyChanged("Owners");
            }
        }

        private OwnerDataGrid _selectedOwner;
        public OwnerDataGrid SelectedOwner
        {
            get { return _selectedOwner; }
            set
            {
                _selectedOwner = value;
                OnPropertyChanged("SelectedOwner");
            }
        }

        public ICommand AddOwnerCommand
        {
            get
            {
                return new Helpers.RelayCommand(AddOwner, CanAddOwner);
            }
        }

        public ICommand EditOwnerCommand
        {
            get
            {
                return new Helpers.RelayCommand(EditOwner, CanEditOwner);
            }
        }

        public ICommand DeleteOwnerCommand
        {
            get
            {
                return new Helpers.RelayCommand(DeleteOwner, CanDeleteOwner);
            }
        }

        public OwnersPage()
        {
            DataContext = this;
            InitializeComponent();
            InitializeCollection();
        }

        public void InitializeCollection()
        {
            Owners = new ObservableCollection<OwnerDataGrid>();
            using (var db = new DB.DomenaDBContext())
            {
                var q = db.Owners.Where(x => x.IsDeleted == false);
                foreach (var own in q)
                {
                    var o = new OwnerDataGrid {Name = own.OwnerName, Address= own.MailAddress, ApartmentsCount=1 };
                    o.OwnerId = own.OwnerId;                    
                    Owners.Add(o);
                }

                foreach (var owner in Owners)
                {
                    owner.ApartmensList = new List<OwnerDescriptionListView>();
                    var apartments = db.Apartments.Where(x => x.OwnerId == owner.OwnerId);

                    foreach (var a in apartments)
                    {
                        var address = new StringBuilder();
                        var build = db.Buildings.Where(x => x.BuildingId == a.BuildingId).FirstOrDefault();
                        address.Append(build.City);
                        address.Append(" ");
                        address.Append(build.ZipCode);
                        address.Append(", ul. ");
                        address.Append(build.RoadName);
                        address.Append(" ");
                        address.Append(build.BuildingNumber);
                        owner.ApartmensList.Add(new OwnerDescriptionListView { DateString = a.CreatedDate.ToString("yyyy-MM-dd"), BuildingName = build.Name, BuildingAddress= address.ToString()});
                    } 
                    if (owner.ApartmensList.Count == 0)
                    {
                        owner.ApartmensList.Add(new OwnerDescriptionListView { BuildingAddress = "Brak mieszkań" });
                    }
                }
            }
        }

        private async void AddOwner(object param)
        {
            Wizards.EditOwnerWizard eow = new Wizards.EditOwnerWizard();

            var result = await DialogHost.Show(eow, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
        }

        private bool CanAddOwner()
        {
            return true;
        }

        private async void EditOwner(object param)
        {
            Wizards.EditOwnerWizard eow;
            using (var db = new DB.DomenaDBContext())
            {
                var so = db.Owners.Where(x => x.OwnerId.Equals(SelectedOwner.OwnerId)).FirstOrDefault();
                eow = new Wizards.EditOwnerWizard(so);
            }

            var result = await DialogHost.Show(eow, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
        }

        private bool CanEditOwner()
        {
            return SelectedOwner != null;
        }

        public async void DeleteOwner(object param)
        {
            bool ynResult = await Helpers.YNMsg.Show("Czy chcesz usunąć właściciela " + SelectedOwner.Name + "?");
            if (ynResult)
            {
                using (var db = new DB.DomenaDBContext())
                {
                    db.Owners.Where(x => x.OwnerId.Equals(SelectedOwner.OwnerId)).FirstOrDefault().IsDeleted = true;
                    db.SaveChanges();
                }
            }
            InitializeCollection();

        }

        private bool CanDeleteOwner()
        {
            return SelectedOwner != null;
        }

        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {
            
        }

        private async void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                var dc = (eventArgs.Session.Content as Wizards.EditOwnerWizard);
                //Accept
                if (dc._ownerLocalCopy == null)
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.OwnerName) || string.IsNullOrEmpty(dc.MailAddress) ))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Add new owner
                    using (var db = new DB.DomenaDBContext())
                    {
                        var newOwner = new LibDataModel.Owner { OwnerId = Guid.NewGuid(), MailAddress=dc.MailAddress, OwnerName=dc.OwnerName, IsDeleted = false };
                        db.Owners.Add(newOwner);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (!IsValid(dc as DependencyObject) || (string.IsNullOrEmpty(dc.OwnerName) || string.IsNullOrEmpty(dc.MailAddress)))
                    {
                        eventArgs.Cancel();
                        return;
                    }
                    //Edit Owner
                    using (var db = new DB.DomenaDBContext())
                    {
                        var q = db.Owners.Where(x => x.OwnerId.Equals(dc._ownerLocalCopy.OwnerId)).FirstOrDefault();
                        q.OwnerName = dc.OwnerName;
                        q.MailAddress = dc.MailAddress;
                        db.SaveChanges();
                    }
                }
            }
            else if (!(bool)eventArgs.Parameter)
            {

                bool ynResult = await Helpers.YNMsg.Show("Czy chcesz anulować?");
                if (!ynResult)
                {
                    //eventArgs.Cancel();
                    var dc = (eventArgs.Session.Content as Wizards.EditOwnerWizard);
                    var result = await DialogHost.Show(dc, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);
                }
            }
            InitializeCollection();
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
