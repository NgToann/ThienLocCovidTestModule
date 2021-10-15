using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for ChangeEmployeeCodeWindow_1.xaml
    /// </summary>
    public partial class ChangeEmployeeCodeWindow_1 : Window
    {
        BackgroundWorker bwLoad;
        BackgroundWorker bwSave;
        List<EmployeeModel> employeeAllList;
        private bool _isResign = false;

        public ChangeEmployeeCodeWindow_1()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwSave = new BackgroundWorker();
            bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);
            bwSave.WorkerSupportsCancellation = true;
            bwSave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);

            employeeAllList = new List<EmployeeModel>();

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

        private void bwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                employeeAllList = EmployeeController.GetAll();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0} !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }));
            }
        }

        private void bwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string employeeIDSearch = txtEmployeeIDSearch.Text;
            txtNewEmployeeCode.Clear();
            var employeeSearched = employeeAllList.Where(w => w.EmployeeID.ToUpper().ToString() == employeeIDSearch.Trim().ToUpper().ToString()
                                                          || w.EmployeeCode == employeeIDSearch.Trim().ToUpper().ToString()).FirstOrDefault();
            if (employeeSearched != null)
            {
                grbWorkerInformation.DataContext = employeeSearched;
                btnSave.IsEnabled = true;
            }
            else
            {
                MessageBox.Show(string.Format("{0} {1}", employeeIDSearch, LanguageHelper.GetStringFromResource("messageNotFound")),
                                this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                txtEmployeeIDSearch.Focus();
                txtEmployeeIDSearch.SelectAll();
                grbWorkerInformation.DataContext = new EmployeeModel();
                btnSave.IsEnabled = false;
            }
            btnSearch.IsDefault = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string employeeCodeOld = txtEmployeeCode.Text;
            string employeeCodeNew = txtNewEmployeeCode.Text.Trim();
            string employeeID = txtEmployeeID.Text;

            // Check Empty
            if (string.IsNullOrEmpty(employeeCodeNew))
            {
                MessageBox.Show(string.Format("{0}\n{1}", LanguageHelper.GetStringFromResource("changeEmployeeCodeWindowTxtNewCardNumber"),
                                                         LanguageHelper.GetStringFromResource("messageDataEmpty")),
                                this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Check Equal
            if (employeeCodeOld == employeeCodeNew)
            {
                MessageBox.Show(string.Format("{0}\n{1}\n{2}", employeeCodeOld, employeeCodeNew,
                                                         LanguageHelper.GetStringFromResource("messageDataDuplicate")),
                                this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Check Exist
            if (employeeCodeOld != employeeCodeNew && employeeAllList.Where(w => w.EmployeeCode == employeeCodeNew).Count() > 0)
            {
                MessageBox.Show(string.Format("{0}\n{1}", employeeCodeNew,
                                                         LanguageHelper.GetStringFromResource("messageDataExist")),
                                this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Confirm
            if (MessageBox.Show(string.Format("{0}\n{1} --> {2}", LanguageHelper.GetStringFromResource("changeEmployeeCodeWindowMsgConfirm"), employeeCodeOld, employeeCodeNew), this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
            {
                return;
            }

            if (bwSave.IsBusy == false)
            {
                btnSave.IsEnabled = false;
                this.Cursor = Cursors.Wait;
                object[] updateWhat = new object[] { employeeID, employeeCodeNew };
                bwSave.RunWorkerAsync(updateWhat);
            }
        }

        private void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            var updateWhat = e.Argument as object[];
            var empIDOld = updateWhat[0].ToString();
            var newCode = updateWhat[1].ToString();
            List<object> results = new List<object>();
            bool result = false;
            // Excute Database
            try
            {
                EmployeeController.UpdateEmployeeCode(empIDOld, newCode, _isResign);
                // Remove employee by old code
                employeeAllList.RemoveAll(r => r.EmployeeID == empIDOld);
                // Get employee by new code
                var employeeByNewCode = EmployeeController.SelectEmployeeByEmployeeCode(newCode);
                employeeAllList.Add(employeeByNewCode);

                result = true;
                results.Add(result);
                results.Add(employeeByNewCode);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0}\nPlease Try Again !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }));
            }
            e.Result = results;
        }

        private void bwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var results = e.Result as List<object>;
            var result = results.Count() > 0 ? (bool)results[0] : false;
            if (result == true)
            {
                var employeeByNewCode = results[1] as EmployeeModel;
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageAddDataSucessful")),
                                    this.Title, MessageBoxButton.OK, MessageBoxImage.Information);

                grbWorkerInformation.DataContext = new EmployeeModel();
                grbWorkerInformation.DataContext = employeeByNewCode;
                txtNewEmployeeCode.Clear();
                txtEmployeeIDSearch.SelectAll();
            }
            this.Cursor = null;
            btnSave.IsEnabled = true;
        }

        private void TxtEmployeeIDSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnSearch.IsDefault = true;
        }

        private void txtNewEmployeeCode_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnSearch.IsDefault = false;
        }

        private void radChangeCard_Checked(object sender, RoutedEventArgs e)
        {
            _isResign = false;
        }

        private void radResign_Checked(object sender, RoutedEventArgs e)
        {
            _isResign = true;
        }
    }
}
