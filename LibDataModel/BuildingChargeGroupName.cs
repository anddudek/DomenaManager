using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibDataModel
{
    public class BuildingChargeGroupName : INotifyPropertyChanged
    {
        private string _groupName;

        [Key]
        public Guid BuildingChargeGroupNameId { get; set; }
        public string GroupName
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
        public bool IsDeleted { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static Guid RepairFundGroupId
        {
            get
            {
                return new Guid("00000000-0000-0000-0000-100000000000");
            }
        }       
    }
}
