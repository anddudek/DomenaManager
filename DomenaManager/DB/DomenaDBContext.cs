﻿using System;
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
        public DbSet<CostCategory> CostCategories { get; set; }
        public DbSet<CostDistributionType> CostDistributionTypes { get; set; }
        public DbSet<Cost> Costs { get; set; }
    }
}
