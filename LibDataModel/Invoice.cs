using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibDataModel
{
    public class Invoice : INotifyPropertyChanged
    {
        [Key]
        public Guid InvoiceId { get; set; }
        private Guid _invoiceCategoryId;
        public Guid InvoiceCategoryId
        {
            get { return _invoiceCategoryId; }
            set
            {
                if (value != _invoiceCategoryId)
                {
                    _invoiceCategoryId = value;
                    OnPropertyChanged("InvoiceCategoryId");
                }
            }
        }
        public Guid BuildingId { get; set; }
        private DateTime _invoiceDate;
        public DateTime InvoiceDate
        {
            get { return _invoiceDate; }
            set
            {
                if (value != _invoiceDate)
                {
                    _invoiceDate = value;
                    OnPropertyChanged("InvoiceDate");
                }
            }
        }
        private string _invoiceNumber;
        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set
            {
                if (value != _invoiceNumber)
                {
                    _invoiceNumber = value;
                    OnPropertyChanged("InvoiceNumber");
                }
            }
        }
        private string _contractorName;
        public string ContractorName
        {
            get { return _contractorName; }
            set
            {
                if (value != _contractorName)
                {
                    _contractorName = value;
                    OnPropertyChanged("ContractorName");
                }
            }
        }
        public DateTime CreatedTime { get; set; }
        private double _costAmount;
        public double CostAmount
        {
            get { return _costAmount; }
            set
            {
                if (value != _costAmount)
                {
                    _costAmount = value;
                    OnPropertyChanged("CostAmount");
                }
            }
        }

        private bool _isSettled;
        public bool IsSettled
        {
            get { return _isSettled; }
            set
            {
                if (value != _isSettled)
                {
                    _isSettled = value;
                    OnPropertyChanged("IsSettled");
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
    }
}
