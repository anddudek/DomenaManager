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

        public Owner()
        {
            this.OwnerId = Guid.NewGuid();
            this.OwnerName = null;
            this.MailAddress = null;
            this.IsDeleted = false;
        }

        public Owner(Owner CopySource)
        {
            this.OwnerId = CopySource.OwnerId;
            this.OwnerName = CopySource.OwnerName;
            this.MailAddress = CopySource.MailAddress;
            this.IsDeleted = CopySource.IsDeleted;
        }
    }
}
