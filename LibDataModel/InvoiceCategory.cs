using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibDataModel
{
    public class InvoiceCategory : INotifyPropertyChanged
    {
        private string _categoryName;

        [Key]
        public Guid CategoryId { get; set; }
        public string CategoryName
        {
            get { return _categoryName; }
            set
            {
                if (value != _categoryName)
                {
                    _categoryName = value;
                    OnPropertyChanged("CategoryName");
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

        public static Guid RepairFundInvoiceCategoryId
        {
            get
            {
                return new Guid("00000000-0000-0000-1000-100000000000");
            }
        }
    }
}
