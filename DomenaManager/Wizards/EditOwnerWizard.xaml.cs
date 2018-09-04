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
using DomenaManager.Helpers;

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditOwnerWizard.xaml
    /// </summary>
    public partial class EditOwnerWizard : UserControl, INotifyPropertyChanged
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

        private string _mailAddress;
        public string MailAddress
        {
            get { return _mailAddress; }
            set
            {
                _mailAddress = value;
                OnPropertyChanged("MailAddress");
            }
        }

        private ObservableCollection<ApartmentListView> _apartmentsOwned;
        public ObservableCollection<ApartmentListView> ApartmentsOwned
        {
            get
            {  
                if (_apartmentsOwned == null)
                {
                    _apartmentsOwned = new ObservableCollection<ApartmentListView>();
                }
                return _apartmentsOwned;
            }
            set
            {
                _apartmentsOwned = value;
                OnPropertyChanged("ApartmentsOwned");
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

        public Owner _ownerLocalCopy;

        public EditOwnerWizard(Owner SelectedOwner = null)
        {
            DataContext = this;
            InitializeComponent();
            if (SelectedOwner != null)
            {
                _ownerLocalCopy = new Owner(SelectedOwner);

                OwnerFirstName = SelectedOwner.OwnerFirstName;
                OwnerSurname = SelectedOwner.OwnerSurname;
                MailAddress = SelectedOwner.MailAddress;
                using (var db = new DB.DomenaDBContext())
                {
                    var q = db.Apartments.Where(x => x.OwnerId == SelectedOwner.OwnerId);
                    foreach (var a in q)
                    {
                        var apartment = new ApartmentListView();
                        apartment.BuildingName = db.Buildings.Where(x => x.BuildingId == a.BuildingId).Select(x => x.Name).FirstOrDefault();
                        apartment.ApartmentNumber = a.ApartmentNumber;
                        ApartmentsOwned.Add(apartment);
                    }
                }
            }
        }

        private void SaveDialog(object param)
        {
            //Accept
            if (this._ownerLocalCopy == null)
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(this.OwnerFirstName) || string.IsNullOrEmpty(this.OwnerSurname) || string.IsNullOrEmpty(this.MailAddress)))
                {
                    return;
                }
                //Add new owner
                using (var db = new DB.DomenaDBContext())
                {
                    var newOwner = new LibDataModel.Owner { OwnerId = Guid.NewGuid(), MailAddress = this.MailAddress, OwnerFirstName = this.OwnerFirstName, OwnerSurname=this.OwnerSurname, IsDeleted = false };
                    db.Owners.Add(newOwner);
                    db.SaveChanges();
                }
            }
            else
            {
                if (!IsValid(this as DependencyObject) || (string.IsNullOrEmpty(this.OwnerFirstName) || string.IsNullOrEmpty(this.OwnerSurname) || string.IsNullOrEmpty(this.MailAddress)))
                {
                    return;
                }
                //Edit Owner
                using (var db = new DB.DomenaDBContext())
                {
                    var q = db.Owners.Where(x => x.OwnerId.Equals(this._ownerLocalCopy.OwnerId)).FirstOrDefault();
                    q.OwnerFirstName = this.OwnerFirstName;
                    q.OwnerSurname = this.OwnerSurname;
                    q.MailAddress = this.MailAddress;
                    db.SaveChanges();
                }
            }
            SwitchPage.SwitchMainPage(new Pages.OwnersPage(), this);
        }

        private bool CanSaveDialog()
        {
            return true;
        }

        private void CancelDialog(object param)
        {
            SwitchPage.SwitchMainPage(new Pages.OwnersPage(), this);
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
           SwitchPage.SwitchMainPage(new Pages.OwnersPage(), this);
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
