using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibDataModel
{
    public class BuildingInvoiceBinding : INotifyPropertyChanged
    {
        [Key]
        public Guid BindingId { get; set; }
        public bool IsDeleted { get; set; }

        private InvoiceCategory _invoiceCategory;
        public InvoiceCategory InvoiceCategory
        {
            get { return _invoiceCategory; }
            set
            {
                if (value != _invoiceCategory)
                {
                    _invoiceCategory = value;
                    OnPropertyChanged("InvoiceCategory");
                }
            }
        }

        private Building _building;
        public Building Building
        {
            get { return _building; }
            set
            {
                if (value != _building)
                {
                    _building = value;
                    OnPropertyChanged("Building");
                }
            }
        }

        private CostDistribution _distribution;
        public CostDistribution Distribution
        {
            get { return _distribution; }
            set
            {
                if (value != _distribution)
                {
                    _distribution = value;
                    OnPropertyChanged("Distribution");
                }
            }
        }

        [NotMapped]
        public string DistributionString
        {
            get { return EnumCostDistribution.CostDistributionToString(Distribution); }
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
