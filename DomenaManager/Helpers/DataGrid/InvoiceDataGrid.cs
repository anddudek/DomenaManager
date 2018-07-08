using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDataModel;

namespace DomenaManager.Helpers
{
    public class InvoiceDataGrid : Invoice
    {
        public Building Building { get; set; }
        public InvoiceCategory Category { get; set; }

        public InvoiceDataGrid()
        {

        }

        public InvoiceDataGrid(Invoice invoice)
        {
            this.BuildingId = invoice.BuildingId;
            this.ContractorName = invoice.ContractorName;
            this.CostAmount = invoice.CostAmount;
            this.CreatedTime = invoice.CreatedTime;
            this.InvoiceCategoryId = invoice.InvoiceCategoryId;
            this.InvoiceDate = invoice.InvoiceDate;
            this.InvoiceId = invoice.InvoiceId;
            this.InvoiceNumber = invoice.InvoiceNumber;
            this.IsDeleted = invoice.IsDeleted;
            this.IsSettled = invoice.IsSettled;
            
            using (var db = new DB.DomenaDBContext())
            {
                this.Building = db.Buildings.Where(x => x.BuildingId.Equals(invoice.BuildingId)).FirstOrDefault();
                this.Category = db.InvoiceCategories.Where(x => x.CategoryId.Equals(invoice.InvoiceCategoryId)).FirstOrDefault();
            }
        }
    }
}
