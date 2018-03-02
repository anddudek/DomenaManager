using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class Payment
    {
        [Key]
        public Guid CostId { get; set; }
        public int CostCategoryId { get; set; }
        public Guid BuildingId { get; set; }
        public int CostDistributionId { get; set; }
        public DateTime PaymentTime { get; set; }
        public string InvoiceNumber { get; set; }
        public string ContractorName { get; set; }
        public DateTime CreatedTime { get; set; }
        public double CostAmount { get; set; }
    }
}
