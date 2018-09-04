using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
    public class OwnerDataGrid
    {
        public Guid OwnerId { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Address { get; set; }
        public int ApartmentsCount { get; set; }
        public List<OwnerDescriptionListView> ApartmensList { get; set; }

        public string OwnerName()
        {
            return this.FirstName + " " + this.SurName;
        }
    }
}
