using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LibDataModel;

namespace DomenaManager.Helpers
{
    public class BuildingSummaryDataGrid : INotifyPropertyChanged
    {
        public Building building { get; set; }
        public InvoiceCategory[] categories { get; set; }
        public int year { get; set; }
        public BuildingSummaryDataGridRow[] rows { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class BuildingSummaryDataGridRow
    {
        public string month { get; set; }
        public string[] invoices { get; set; } // [ 0 zł ; 1 zł ]
        public string invoicesSum { get; set; }
        public string payments { get; set; }
        public string saldo { get; set; }
    }    
}
