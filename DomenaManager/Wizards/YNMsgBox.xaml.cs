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
    /// Interaction logic for YNMsgBox.xaml
    /// </summary>
    public partial class YNMsgBox : UserControl, INotifyPropertyChanged
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        private string _yesTitle;
        public string YesTitle
        {
            get { return _yesTitle; }
            set
            {
                _yesTitle = value;
                OnPropertyChanged("YesTitle");
            }
        }

        private string _noTitle;
        public string NoTitle
        {
            get { return _noTitle; }
            set
            {
                _noTitle = value;
                OnPropertyChanged("NoTitle");
            }
        }

        public YNMsgBox(string BoxMessage)
        {
            DataContext = this;
            _yesTitle = "Tak";
            _noTitle = "Nie";
            _message = BoxMessage;
            InitializeComponent();
        }

        public YNMsgBox(string BoxMessage, string YesButtonTitle, string NoButtonTitle)
        {
            DataContext = this;
            _yesTitle = YesButtonTitle;
            _noTitle = NoButtonTitle;
            _message = BoxMessage;
            InitializeComponent();
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
