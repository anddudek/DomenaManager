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

namespace DomenaManager.Wizards
{
    /// <summary>
    /// Interaction logic for EditInvoiceCategories.xaml
    /// </summary>
    public partial class EditGroupNames : UserControl, INotifyPropertyChanged
    {
        private string _groupName;
        public string GroupName
        {
            get { return _groupName; }
            set
            {
                if (value != _groupName)
                {
                    _groupName = value;
                    LabelError = "";
                    OnPropertyChanged("GroupName");
                }
            }
        }

        private string _labelError;
        public string LabelError
        {
            get { return _labelError; }
            set
            {
                if (value != _labelError)
                {
                    _labelError = value;
                    OnPropertyChanged("LabelError");
                }
            }
        }

        private ObservableCollection<BuildingChargeGroupName> _groupCollection;
        public ObservableCollection<BuildingChargeGroupName> GroupCollection
        {
            get { return _groupCollection; }
            set
            {
                if (value != _groupCollection)
                {
                    _groupCollection = value;
                    OnPropertyChanged("GroupCollection");
                }
            }
        }

        private BuildingChargeGroupName _selectedGroupName;
        public BuildingChargeGroupName SelectedGroupName
        {
            get { return _selectedGroupName; }
            set
            {
                if (value != _selectedGroupName)
                {
                    _selectedGroupName = value;                    
                    GroupName = value != null ? value.GroupName : string.Empty;
                    OnPropertyChanged("SelectedGroupName");
                }
            }
        }

        public ICommand AddGroupCommand
        {
            get { return new Helpers.RelayCommand(AddGroup, CanAddGroup); }
        }

        public ICommand DeleteGroupCommand
        {
            get { return new Helpers.RelayCommand(DeleteGroup, CanDeleteGroup); }
        }

        public ICommand ModifyGroupCommand
        {
            get { return new Helpers.RelayCommand(ModifyGroup, CanModifyGroup); }
        }

        public List<Helpers.CategoryCommand<BuildingChargeGroupName>> commandBuffer;

        public EditGroupNames()
        {
            DataContext = this;
            InitializeComponent();
            commandBuffer = new List<Helpers.CategoryCommand<BuildingChargeGroupName>>();
            using (var db = new DB.DomenaDBContext())
            {
                GroupCollection = new ObservableCollection<BuildingChargeGroupName>(db.GroupName.Where(x => !x.IsDeleted).ToList());
            }
        }

        private void AddGroup(object param)
        {
            if (string.IsNullOrWhiteSpace(GroupName))
            {
                LabelError = "Błędna nazwa";
                return;
            }
            var ic = new BuildingChargeGroupName { GroupName = this.GroupName, BuildingChargeGroupNameId = Guid.NewGuid(), IsDeleted = false };
            GroupCollection.Add(ic);

            commandBuffer.Add(new Helpers.CategoryCommand<BuildingChargeGroupName> { CommandType = Helpers.CommandEnum.Add, Item = ic });
        }

        private bool CanAddGroup()
        {
            return true;
        }

        private void ModifyGroup(object param)
        {
            if (string.IsNullOrWhiteSpace(GroupName))
            {
                LabelError = "Błędna nazwa";
                return;
            }
            SelectedGroupName.GroupName = this.GroupName;

            commandBuffer.Add(new Helpers.CategoryCommand<BuildingChargeGroupName> { CommandType = Helpers.CommandEnum.Update, Item = SelectedGroupName });
        }

        private bool CanModifyGroup()
        {
            return SelectedGroupName != null;
        }

        private void DeleteGroup(object param)
        {
            commandBuffer.Add(new Helpers.CategoryCommand<BuildingChargeGroupName> { CommandType = Helpers.CommandEnum.Remove, Item = SelectedGroupName });

            GroupCollection.Remove(SelectedGroupName);            
        }

        private bool CanDeleteGroup()
        {
            return SelectedGroupName != null;
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
