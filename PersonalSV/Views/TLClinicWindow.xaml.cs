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
        private string stateDisplay = "", lblMainHeader = "";
        BackgroundWorker bwLoad;
        List<EmployeeModel> employeeList;
        private DateTime confirmDate;
        public TLClinicWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            employeeList = new List<EmployeeModel>();
            confirmDate = DateTime.Now.Date;

            lblMainHeader = LanguageHelper.GetStringFromResource("clinicMainHeader");
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
            dpConfirmDate.SelectedDate = confirmDate;
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
                                ConfirmDate = confirmDate,
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
                        SetTxtDefault();
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
                    ResetUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message.ToString());
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var patientDelete = grPatientInfo.DataContext as PatientModel;
            if (patientDelete == null)
                return;
            if (MessageBox.Show(string.Format("Confirm Delete Patient Info ?\nXác Nhận Xóa Thông Tin {0} Ngày {1:dd/MM/yyyy} ?", patientDelete.EmployeeID, patientDelete.ConfirmDate),
                                       this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
            {
                return;
            }
            try
            {
                PatientController.Delete(patientDelete);
                MessageBox.Show("Deleted !\nĐã Xoá Dữ Liệu!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                ResetUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message.ToString());
                SetTxtDefault();
            }
        }

        private void ResetUI()
        {
            radNormal.IsChecked = true;
            brDisplay.Background = Brushes.WhiteSmoke;
            brState.Background = Brushes.WhiteSmoke;
            grWorkerInfo.DataContext = null;
            grPatientInfo.DataContext = null;
            SetTxtDefault();
        }

        private void ReloadState(SolidColorBrush bgColor)
        {
            var currentState = grPatientInfo.DataContext as PatientModel;
            if (currentState != null)
            {
                currentState.StateIndex = stateIndex;
                currentState.ConfirmDate = confirmDate;
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
            brState.Background = Brushes.Green;
            stateIndex = 0;
            stateDisplay = radNormal.Content as string;
            ReloadState(Brushes.Green);
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

        private void cbChangeConfirmDate_Checked(object sender, RoutedEventArgs e)
        {
            dpConfirmDate.Visibility = Visibility.Visible;
        }

        private void cbChangeConfirmDate_Unchecked(object sender, RoutedEventArgs e)
        {
            dpConfirmDate.Visibility = Visibility.Collapsed;
        }

        private void dpConfirmDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var dateChange = dpConfirmDate.SelectedDate.Value.Date;
            if (dateChange > DateTime.Now.Date)
            {
                MessageBox.Show("Can not change date grather than today\nKhông chọn ngày lớn hơn ngày hôm nay!", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                dpConfirmDate.SelectedDate = DateTime.Now.Date;
                return;
            }
            confirmDate = dateChange;
            lblHeader.Text = string.Format("{0}: {1:dd/MM/yyyy}", lblMainHeader, confirmDate);
            cbChangeConfirmDate.IsChecked = false;
        }
    }
}
