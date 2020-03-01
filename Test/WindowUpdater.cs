using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Test
{
    //Class that helps update a window (text / listbox etc.) in WPF.
    public static class WindowUpdater
    {
        public static void UpdateTextBlock(TextBlock textBlock, string message)
        {
            textBlock.Text = message;
        }
        public static  void UpdateTextBox(TextBox textBox, string message)
        {
            textBox.Text = message;
        }
        public static void RefreshListBox<T>(ListBox listBoxToRefresh, List<T> listToShow)
        {
            listBoxToRefresh.ItemsSource = null;
            listBoxToRefresh.ItemsSource = listToShow;
        }
        public static void RefreshComboBox<T>(ComboBox comboBoxToRefresh, List<T> listToShow)
        {
            comboBoxToRefresh.ItemsSource = null;
            comboBoxToRefresh.ItemsSource = listToShow;
        }
    }
}
