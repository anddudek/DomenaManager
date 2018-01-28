using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomenaManager.Wizards;
using MaterialDesignThemes.Wpf;

namespace DomenaManager.Helpers
{
    public static class YNMsg
    {
        public static async Task<bool> Show(string Msg)
        {
            YNMsgBox yn = new YNMsgBox(Msg);
            var result = await DialogHost.Show(yn, "MsgBoxDialog");
            return (bool)result;
        }

        public static async Task<bool> Show(string Msg, string YesBtn, string NoBtn)
        {
            YNMsgBox yn = new YNMsgBox(Msg, YesBtn, NoBtn);
            var result = await DialogHost.Show(yn, "MsgBoxDialog");
            return (bool)result;
        }
    }
}
