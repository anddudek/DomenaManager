using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace DomenaManager.Helpers
{
    public static class SwitchPage
    {
        public static void SwitchMainPage(UserControl page, UserControl sender)
        {
            if (!(sender.Parent == null))
            {
                var mw = ((((((sender).Parent as MahApps.Metro.Controls.TransitioningContentControl).Parent as Grid).Parent as DialogHost).Parent as DialogHost).Parent as DialogHost).Parent as Windows.MainWindow;
                mw.CurrentPage = page;
            }
        }
    }
}
