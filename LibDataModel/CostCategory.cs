using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class CostCategory
    {
        [Key]
        public Guid CostCategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
