using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace DomenaManager.Wizards
{
   /// <summary>
   /// Interaction logic for EditSettings.xaml
   /// </summary>
   public partial class EditSettings : UserControl, INotifyPropertyChanged
   {
      public EditSettings()
      {
         InitializeComponent();
         DataContext = this;
      }

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
}
