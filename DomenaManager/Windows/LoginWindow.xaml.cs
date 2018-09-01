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
using System.Windows.Shapes;
using System.ComponentModel;

namespace DomenaManager.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private string _loginText;
        public string LoginText
        {
            get
            {
                return _loginText;
            }
            set
            {
                _loginText = value;
                OnPropertyChanged("LoginText");
            }
        }

        private void Cancel(object param)
        {
            this.Close();
        }
        private bool CanCancel()
        {
            return true;
        }

        public ICommand CancelCommand 
        { 
            get 
            { 
                return new Helpers.RelayCommand(Cancel, CanCancel); 
            } 
        }

        private void Login(object pb)
        {
            if (LoginText.ToUpper() == "DOMENA" && ((PasswordBox)pb).Password == "domena")
            {
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
        }
        private bool CanLogin()
        {
            return true;
        }

        public ICommand LoginCommand 
        { 
            get 
            { 
                return new Helpers.RelayCommand(param => Login(param), CanLogin); 
            } 
        }
        
        public LoginWindow()
        {
            DataContext = this;
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
