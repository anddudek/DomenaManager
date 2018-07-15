using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class Settlement
    {
        [Key]
        public Guid SettlementId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
