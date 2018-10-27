using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibDataModel
{
    public class Owner
    {
        [Key]
        public Guid OwnerId { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerSurname {get; set;}
        public string MailAddress { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped]
        public string OwnerName
        {
            get
            {
                return this.OwnerFirstName + " " + this.OwnerSurname;
            }
        }

        public Owner()
        {
            this.OwnerId = Guid.NewGuid();
            this.OwnerFirstName = null;
            this.OwnerSurname = null;
            this.MailAddress = null;
            this.IsDeleted = false;
        }

        public Owner(Owner CopySource)
        {
            this.OwnerId = CopySource.OwnerId;
            this.OwnerFirstName = CopySource.OwnerFirstName;
            this.OwnerSurname = CopySource.OwnerSurname;
            this.MailAddress = CopySource.MailAddress;
            this.IsDeleted = CopySource.IsDeleted;
        }
    }
}
