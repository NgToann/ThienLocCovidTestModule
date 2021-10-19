using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using TLCovidTest.Controllers;
using TLCovidTest.Helpers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for WorkerPrizeScanWindow.xaml
    /// </summary>
    public partial class WorkerPrizeScanWindow : Window
    {
        public WorkerPrizeScanWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblHeader.Text = String.Format("NHẬN QUÀ NGÀY: {0:dd/MM/yyyy}", DateTime.Now.Date);
            txtCardId.IsEnabled = true;
            SetTxtDefault();
        }

        private void txtCardId_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            brDisplay.DataContext = null;
            brDisplay.Background = Brushes.WhiteSmoke;
            if (e.Key == Key.Enter)
            {
                string findWhat = txtCardId.Text.Trim().ToLower().ToString();
                try
                {
                    var displayInfo = new WorkerPrizeModel();
                    var workerPrizeByCardId = WorkerPrizeController.GetByCardId(findWhat).FirstOrDefault();
                    if(workerPrizeByCardId != null)
                    {
                        if (string.IsNullOrEmpty(workerPrizeByCardId.TimeScan))
                        {
                            brDisplay.Background = Brushes.Green;
                            displayInfo = workerPrizeByCardId;
                            displayInfo.Message = "MỜI LÊN NHẬN QUÀ !";
                            displayInfo.TimeScan = string.Format("{0:HH:mm}", DateTime.Now);
                            WorkerPrizeController.UpdateTimeScan(displayInfo);
                        }
                        else
                        {
                            brDisplay.Background = Brushes.Yellow;
                            displayInfo = workerPrizeByCardId;
                            displayInfo.Message = string.Format("ĐÃ NHẬN QUÀ LÚC: {0} !", displayInfo.TimeScan);
                        }
                    }
                    else
                    {
                        displayInfo.FullName = findWhat;
                        displayInfo.Message = "KHÔNG CÓ TRONG DANH SÁCH !";
                    }

                    brDisplay.DataContext = displayInfo;
                    SetTxtDefault();
                }
                catch
                {
                    MessageBox.Show("Vui Lòng Scan Lại\nPlease Try Again!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    SetTxtDefault();
                    return;
                }
            }

        }
        private void SetTxtDefault()
        {
            txtCardId.SelectAll();
            txtCardId.Focus();
        }
        private void txtCardId_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtCardId.SelectAll();
        }
    }
}
