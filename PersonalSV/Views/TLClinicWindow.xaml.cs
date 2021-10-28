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
        private string stateDisplay = "";
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
                    MessageBox.Show(ex.InnerException.Message.ToString());
                }));
            }
        }

        private void txtCardId_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            brDisplay.Background = Brushes.WhiteSmoke;
            brState.Background = Brushes.WhiteSmoke;

            grWorkerInfo.DataContext = null;
            grPatientInfo.DataContext = null;

            radNormal.IsChecked     = false;
            radInfected.IsChecked   = false;
            radSuspected.IsChecked  = false;
            radOthers.IsChecked     = false;

            if (e.Key == Key.Enter)
            {
                // get worker by cardid
                string scanWhat = txtCardId.Text.Trim().ToUpper().ToString();
                var empById = employeeList.FirstOrDefault(f => f.EmployeeCode.Trim().ToUpper() == scanWhat ||
                                                               f.EmployeeID.Trim().ToUpper() == scanWhat);

                if (empById != null)
                {
                    grWorkerInfo.DataContext = empById;
                    try
                    {
                        var patientListByWorkerId = PatientController.GetByEmpId(empById.EmployeeID);
                        if (patientListByWorkerId.Count() == 0)
                        {
                            radNormal.IsChecked = true;
                            var patientNew = new PatientModel
                            {
                                EmployeeID = empById.EmployeeID,
                                ConfirmBy = "",
                                Remarks = "",
                                ConfirmDate = DateTime.Now.Date,
                                StateDisplay = stateDisplay
                            };
                            grPatientInfo.DataContext = patientNew;
                        }
                        else
                        {
                            var patientLatest = patientListByWorkerId.OrderBy(o => o.ConfirmDate).LastOrDefault();

                            // Refresh UI
                            if (patientLatest.StateIndex == 0)
                                radNormal.IsChecked = true;
                            else if (patientLatest.StateIndex == 1)
                                radInfected.IsChecked = true;
                            else if (patientLatest.StateIndex == 2)
                                radSuspected.IsChecked = true;
                            else if (patientLatest.StateIndex == 3)
                                radOthers.IsChecked = true;

                            patientLatest.StateDisplay = stateDisplay;
                            grPatientInfo.DataContext = patientLatest;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.InnerException.Message.ToString());
                        SetTxtDefault();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Worker Not Found !\nKhông Tìm Thấy!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    SetTxtDefault();
                    return;
                }
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var patientSave = grPatientInfo.DataContext as PatientModel;
            if (patientSave == null)
                return;

            try
            {
                if (PatientController.Insert(patientSave))
                {
                    MessageBox.Show("Sucessfully !\nĐã lưu dữ liệu!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    SetTxtDefault();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message.ToString());
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReloadState(SolidColorBrush bgColor)
        {
            var currentState = grPatientInfo.DataContext as PatientModel;
            if (currentState != null)
            {
                currentState.StateIndex = stateIndex;
                currentState.StateDisplay = stateDisplay;
                currentState.Background = bgColor;
            }
            this.Foreground = bgColor;

            grPatientInfo.DataContext = null;
            grPatientInfo.DataContext = currentState;

            brDisplay.DataContext = null;
            brDisplay.DataContext = currentState;
        }
        
        private void txtCardId_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtCardId.SelectAll();
        }

        private void radNormal_Checked(object sender, RoutedEventArgs e)
        {
            //brDisplay.Background = Brushes.LimeGreen;
            brState.Background = Brushes.LimeGreen;
            stateIndex = 0;
            stateDisplay = radNormal.Content as string;
            ReloadState(Brushes.LimeGreen);
        }

        private void radInfected_Checked(object sender, RoutedEventArgs e)
        {
            //brDisplay.Background = Brushes.Red;
            brState.Background = Brushes.Red;
            stateIndex = 1;
            stateDisplay = radInfected.Content as string;
            ReloadState(Brushes.Red);
        }

        private void radSuspected_Checked(object sender, RoutedEventArgs e)
        {
            //brDisplay.Background = Brushes.Orange;
            brState.Background = Brushes.Orange;
            stateIndex = 2;
            stateDisplay = radSuspected.Content as string;
            ReloadState(Brushes.Orange);
        }

        private void radOthers_Checked(object sender, RoutedEventArgs e)
        {
            //brDisplay.Background = Brushes.Yellow;
            brState.Background = Brushes.Yellow;
            stateIndex = 3;
            stateDisplay = radOthers.Content as string;
            ReloadState(Brushes.Yellow);
        }
        private void SetTxtDefault()
        {
            txtCardId.SelectAll();
            txtCardId.Focus();
        }     
    }
}
