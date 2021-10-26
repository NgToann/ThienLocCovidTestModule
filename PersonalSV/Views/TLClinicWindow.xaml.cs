using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for TLClinicWindow.xaml
    /// </summary>
    public partial class TLClinicWindow : Window
    {
        public TLClinicWindow()
        {
            InitializeComponent();
        }

        private void txtCardId_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void txtCardId_PreviewKeyUp(object sender, KeyEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void radNormal_Checked(object sender, RoutedEventArgs e)
        {
            brDisplay.Background = Brushes.Green;
        }

        private void radInfected_Checked(object sender, RoutedEventArgs e)
        {
            brDisplay.Background = Brushes.Red;
        }

        private void radSuspected_Checked(object sender, RoutedEventArgs e)
        {
            brDisplay.Background = Brushes.Orange;
        }

        private void radOthers_Checked(object sender, RoutedEventArgs e)
        {
            brDisplay.Background = Brushes.Yellow;
        }
    }
}
