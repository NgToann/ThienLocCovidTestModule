using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

using TLCovidTest.Controllers;
using TLCovidTest.Helpers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for TLClinicWindow.xaml
    /// </summary>
    public partial class TLClinicWindow : Window
    {
        private int stateIndex = 0;
        BackgroundWorker bwLoad;

        List<EmployeeModel> employeeList;
        public TLClinicWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            employeeList = new List<EmployeeModel>();

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bwLoad.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
        }
        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            txtCardId.IsEnabled = true;
            SetTxtDefault();
        }

        private void BwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                employeeList = EmployeeController.GetAvailable();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message.ToString());
                }));
            }
        }

        

        private void txtCardId_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            brDisplay.Background = Brushes.WhiteSmoke;
            if (e.Key == Key.Enter)
            {
                // get worker by cardid
                string scanWhat = txtCardId.Text.Trim().ToUpper().ToString();
                var empById = employeeList.FirstOrDefault(f => f.EmployeeCode.Trim().ToUpper() == scanWhat ||
                                                               f.EmployeeID.Trim().ToUpper() == scanWhat);

                if (empById != null)
                {

                }
                else
                {

                }
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
        private void txtCardId_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtCardId.SelectAll();
        }

        private void radNormal_Checked(object sender, RoutedEventArgs e)
        {
            brDisplay.Background = Brushes.LimeGreen;
            this.Foreground = Brushes.Black;
            stateIndex = 0;
        }

        private void radInfected_Checked(object sender, RoutedEventArgs e)
        {
            brDisplay.Background = Brushes.Red;
            this.Foreground = Brushes.White;
            stateIndex = 1;
        }

        private void radSuspected_Checked(object sender, RoutedEventArgs e)
        {
            brDisplay.Background = Brushes.Orange;
            this.Foreground = Brushes.Black;
            stateIndex = 2;
        }

        private void radOthers_Checked(object sender, RoutedEventArgs e)
        {
            brDisplay.Background = Brushes.Yellow;
            this.Foreground = Brushes.Black;
            stateIndex = 3;
        }
        private void SetTxtDefault()
        {
            txtCardId.SelectAll();
            txtCardId.Focus();
        }

        
    }
}
