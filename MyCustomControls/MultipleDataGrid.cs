using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections;
using System.Windows;

namespace MyCustomControls
{
    public class MultipleDataGrid : DataGrid
    {
        public MultipleDataGrid()
        {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }

        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.SelectedItemsList = this.SelectedItems;
            this.SelectedItemsList.Clear();
            foreach (var item in this.SelectedItems)
            {
                this.SelectedItemsList.Add(item);
            }
        }
        #region SelectedItemsList

        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
                DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(MultipleDataGrid), new PropertyMetadata(null));

        #endregion
    }
}
