﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDataModel;
using System.ComponentModel;

namespace DomenaManager.Helpers
{
    public class SummaryDataGrid 
    {
        public Owner owner { get; set; }
        public Apartment apartment { get; set; }
        public Building building { get; set; }
        public BuildingChargeBasisCategory[] categories { get; set; }
        public int year { get; set; }
        public SummaryDataGridRow[] rows { get; set; }

        /*public static string[] GetMonthsArray()
        {
            return new string[]{
                "Styczeń",
                "Luty",
                "Marzec",
                "Kwiecień",
                "Maj",
                "Czerwiec",
                "Lipiec",
                "Sierpień",
                "Wrzesień",
                "Październik",
                "Listopad",
                "Grudzień"
            };
        }*/
    }

    public class SummaryDataGridRow
    {
        public string month { get; set; }
        public string[] charges { get; set; }
    }
}