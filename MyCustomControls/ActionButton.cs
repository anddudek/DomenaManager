﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace MyCustomControls
{
    public class ActionButton : Button
    {
        public PackIconKind MDTIcon
        {
            get
            {
                return (PackIconKind)GetValue(MDTIconProperty);
            }
            set
            {
                SetValue(MDTIconProperty, value);
            }
        }

        public string MDTText
        {
            get
            {
                return (string)GetValue(MDTTextProperty);
            }
            set
            {
                SetValue(MDTTextProperty, value);
            }
        }

        public static readonly DependencyProperty MDTIconProperty = DependencyProperty.Register("MDTIcon", typeof(PackIconKind), typeof(ActionButton), new PropertyMetadata(null));

        public static readonly DependencyProperty MDTTextProperty = DependencyProperty.Register("MDTText", typeof(string), typeof(ActionButton), new PropertyMetadata(null));

        static ActionButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ActionButton), new FrameworkPropertyMetadata(typeof(ActionButton)));
        }
    }
}
