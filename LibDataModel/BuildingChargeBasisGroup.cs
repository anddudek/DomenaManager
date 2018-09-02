using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibDataModel
{
    public class BuildingChargeBasisGroup : INotifyPropertyChanged
    {
        [Key]
        private Guid _buildingChargeBasisGroupId;
        public Guid BuildingChargeBasisGroupId
        {
            get { return _buildingChargeBasisGroupId; }
            set
            {
                if (value != _buildingChargeBasisGroupId)
                {
                    _buildingChargeBasisGroupId = value;
                    OnPropertyChanged("BuildingChargeBasisGroupId");
                }
            }
        }

        private string _buildingChargeBasicGroupName;
        public string BuildingChargeBasicGroupName
        {
            get { return _buildingChargeBasicGroupName; }
            set
            {
                if (value != _buildingChargeBasicGroupName)
                {
                    _buildingChargeBasicGroupName = value;
                    OnPropertyChanged("BuildingChargeBasicGroupName");
                }
            }
        }

        private string _buildingChargeBasicBankAccount;
        public string BuildingChargeBasicBankAccount
        {
            get { return _buildingChargeBasicBankAccount; }
            set
            {
                if (value != _buildingChargeBasicBankAccount)
                {
                    _buildingChargeBasicBankAccount = value;
                    OnPropertyChanged("BuildingChargeBasicBankAccount");
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
