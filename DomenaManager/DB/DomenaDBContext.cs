using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using LibDataModel;

namespace DomenaManager.DB
{
    class DomenaDBContext : DbContext
    {
        public DomenaDBContext() : base("LocalDB")
        {
        }

        public DbSet<Building> Buildings { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<BuildingChargeBasisCategory> CostCategories { get; set; }     
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Charge> Charges { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceCategory> InvoiceCategories { get; set; }
        public DbSet<ContractorsName> InvoiceContractors { get; set; }
        public DbSet<InvoiceVatRate> InvoiceVatRates { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BindingParent> Bindings { get; set; }        
        public DbSet<Settlement> Settlements { get; set; }
        public DbSet<AutoCharge> AutoCharges { get; set; }
        public DbSet<BuildingChargeGroupName> GroupName { get; set; }        
        public DbSet<MetersHistory> MetersHistories { get; set; }
        public DbSet<BuildingChargeGroupBankAccount> BuildingChargeGroupBankAccounts { get; set; }
        public DbSet<YearSummary> YearSummaries { get; set; }
    }
}
