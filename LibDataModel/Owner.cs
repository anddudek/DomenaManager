using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibDataModel
{
    public class Owner
    {
        [Key]
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string MailAddress { get; set; }
        public bool IsDeleted { get; set; }
    }
}
