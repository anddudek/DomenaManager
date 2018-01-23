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
        public class MetroButtonExpand : Button
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

            public static readonly DependencyProperty MDTIconProperty = DependencyProperty.Register("MDTIcon", typeof(PackIconKind), typeof(MetroButtonExpand), new PropertyMetadata(null));

            public static readonly DependencyProperty MDTTextProperty = DependencyProperty.Register("MDTText", typeof(string), typeof(MetroButtonExpand), new PropertyMetadata(null));

            static MetroButtonExpand()
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroButtonExpand), new FrameworkPropertyMetadata(typeof(MetroButtonExpand)));
            }

        }
    
}
