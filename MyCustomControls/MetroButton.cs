using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace MyCustomControls
{
    public class MetroButton : Button
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

        public static readonly DependencyProperty MDTIconProperty = DependencyProperty.Register("MDTIcon", typeof(PackIconKind), typeof(MetroButton), new PropertyMetadata(null));

        public static readonly DependencyProperty MDTTextProperty = DependencyProperty.Register("MDTText", typeof(string), typeof(MetroButton), new PropertyMetadata(null));

        static MetroButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroButton), new FrameworkPropertyMetadata(typeof(MetroButton)));
        }

    }
}
