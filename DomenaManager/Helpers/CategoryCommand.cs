using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomenaManager.Helpers
{
    public class CategoryCommand<T>
    {
        public CommandEnum CommandType { get; set; }
        public T Item { get; set; }
    }
}
