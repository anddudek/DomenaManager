using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDataModel;
using System.ComponentModel;

namespace DomenaManager.Helpers.DataGrid
{
    public class InvoiceSettlementDataGrid : Invoice, INotifyPropertyChanged
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    OnPropertyChanged("IsChecked");
                }
            }
        }

        private InvoiceCategory _invoiceCategory;
        public InvoiceCategory InvoiceCategory
        {
            get
            {
                return _invoiceCategory;
            }
            set
            {
                if (value != _invoiceCategory)
                {
                    _invoiceCategory = value;
                    OnPropertyChanged("InvoiceCategory");
                }
            }
        }        

        public InvoiceSettlementDataGrid(Invoice invoice)
        {
            this.BuildingId = invoice.BuildingId;
            this.ConstVat = invoice.ConstVat;
            this.ContractorName = invoice.ContractorName;
            this.CostAmount = invoice.CostAmount;
            this.CostAmountConst = invoice.CostAmountConst;
            this.CostAmountConstGross = invoice.CostAmountConstGross;
            this.CostAmountGross = invoice.CostAmountGross;
            this.CostAmountVariable = invoice.CostAmountVariable;
            this.CostAmountVariableGross = invoice.CostAmountVariableGross;
            this.CreatedTime = invoice.CreatedTime;
            this.InvoiceCategoryId = invoice.InvoiceCategoryId;
            this.InvoiceCreatedDate = invoice.InvoiceCreatedDate;
            this.InvoiceDate = invoice.InvoiceDate;
            this.InvoiceId = invoice.InvoiceId;
            this.InvoiceNumber = invoice.InvoiceNumber;
            this.IsDeleted = invoice.IsDeleted;
            this.IsSettled = invoice.IsSettled;
            this.Title = invoice.Title;
            this.VariableVat = invoice.VariableVat;
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
