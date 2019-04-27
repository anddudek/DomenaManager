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
        private DateTime _invoiceCreatedDate;
        public DateTime InvoiceCreatedDate
        {
            get { return _invoiceCreatedDate; }
            set
            {
                if (value != _invoiceCreatedDate)
                {
                    _invoiceCreatedDate = value;
                    OnPropertyChanged("InvoiceCreatedDate");
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
        private decimal _costAmount;
        public decimal CostAmount
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

        private decimal _variableVat;
        public decimal VariableVat
        {
            get { return _variableVat; }
            set
            {
                if (value != _variableVat)
                {
                    _variableVat = value;
                    OnPropertyChanged("VariableVat");
                }
            }
        }

        private decimal _constVat;
        public decimal ConstVat
        {
            get { return _constVat; }
            set
            {
                if (value != _constVat)
                {
                    _constVat = value;
                    OnPropertyChanged("ConstVat");
                }
            }
        }

        private decimal _costAmountGross;
        public decimal CostAmountGross
        {
            get { return _costAmountGross; }
            set
            {
                if (value != _costAmountGross)
                {
                    _costAmountGross = value;
                    OnPropertyChanged("CostAmountGross");
                }
            }
        }

        private decimal _costAmountVariable;
        public decimal CostAmountVariable
        {
            get { return _costAmountVariable; }
            set
            {
                if (value != _costAmountVariable)
                {
                    _costAmountVariable = value;
                    OnPropertyChanged("CostAmountVariable");
                }
            }
        }

        private decimal _costAmountVariableGross;
        public decimal CostAmountVariableGross
        {
            get { return _costAmountVariableGross; }
            set
            {
                if (value != _costAmountVariableGross)
                {
                    _costAmountVariableGross = value;
                    OnPropertyChanged("CostAmountVariableGross");
                }
            }
        }

        private decimal _costAmountConst;
        public decimal CostAmountConst
        {
            get { return _costAmountConst; }
            set
            {
                if (value != _costAmountConst)
                {
                    _costAmountConst = value;
                    OnPropertyChanged("CostAmountConst");
                }
            }
        }

        private decimal _costAmountConstGross;
        public decimal CostAmountConstGross
        {
            get { return _costAmountConstGross; }
            set
            {
                if (value != _costAmountConstGross)
                {
                    _costAmountConstGross = value;
                    OnPropertyChanged("CostAmountConstGross");
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

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    OnPropertyChanged("Title");
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
