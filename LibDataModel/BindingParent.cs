using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class BindingParent
    {
        [Key]
        public Guid BindingId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
