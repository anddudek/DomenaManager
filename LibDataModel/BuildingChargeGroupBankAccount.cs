using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibDataModel
{
    public class BuildingChargeGroupBankAccount : INotifyPropertyChanged
    {
        private BuildingChargeGroupName _groupName;
        private string _bankAccount;

        [Key]
        public Guid BuildingChargeGroupBankAccountId { get; set; }
        public BuildingChargeGroupName GroupName
        {
            get { return _groupName; }
            set
            {
                if (value != _groupName)
                {
                    _groupName = value;
                    OnPropertyChanged("GroupName");
                }
            }
        }
        public string BankAccount
        {
            get { return _bankAccount; }
            set
            {
                if (value != _bankAccount)
                {
                    _bankAccount = value;
                    OnPropertyChanged("BankAccount");
                }
            }
        }
        public bool IsDeleted { get; set; }
        public Building Building { get; set; }

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
