using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class Invoice
    {
        [Key]
        public Guid InvoiceId { get; set; }
        public Guid InvoiceCategoryId { get; set; }
        public Guid BuildingId { get; set; }        
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string ContractorName { get; set; }
        public DateTime CreatedTime { get; set; }
        public double CostAmount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
