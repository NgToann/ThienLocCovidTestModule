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
    /// Interaction logic for TestCovidConfirmResultWindow.xaml
    /// </summary>
    public partial class TestCovidConfirmResultWindow : Window
    {
        BackgroundWorker bwLoad;
        BackgroundWorker bwSave;
        private string lblResourceHeader = "";
        List<EmployeeModel> employeeList;
        private DateTime toDay = DateTime.Now.Date;

        List<TestRandomModel> testRandomListByEmpCodeToDay;
        List<TestRandomModel> testRandomListByDate;
        private string[] results = { "Negative", "Positive" };
        public TestCovidConfirmResultWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            bwSave = new BackgroundWorker();
            bwSave.DoWork += BwSave_DoWork;
            bwSave.RunWorkerCompleted += BwSave_RunWorkerCompleted;

            lblResourceHeader = LanguageHelper.GetStringFromResource("confirmTestResultHeader");

            employeeList = new List<EmployeeModel>();

            testRandomListByEmpCodeToDay = new List<TestRandomModel>();
            testRandomListByDate = new List<TestRandomModel>();

            InitializeComponent();
        }

        private void BwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
            {
                MessageBox.Show("Successful !", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.Cursor = null;
            SetTxtDefault();
            btnSave.IsEnabled = true;
            DoCounter(testRandomListByDate);
            ClearUI();
        }

        private void BwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            var updateTestResultModel = e.Argument as TestRandomModel;
            try
            {
                TestRandomController.Update(updateTestResultModel, 3);
                testRandomListByDate = TestRandomController.GetByDate(toDay);
                e.Result = true;
            }
            catch (Exception ex)
            {
                e.Result = false;
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.InnerException.Message.ToString());
                }));
            }
        }

        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            txtCardId.IsEnabled = true;

            SetTxtDefault();
            btnSave.IsEnabled = true;
            cboResult.ItemsSource = results;
            cboResult.SelectedItem = results[0];

            DoCounter(testRandomListByDate);
        }

        private void BwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                employeeList = EmployeeController.GetAvailableForTestCovid();
                testRandomListByDate = TestRandomController.GetByDate(toDay);
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
            if (e.Key == Key.Enter)
            {
                ClearUI();
                string findWhat = txtCardId.Text.Trim().ToLower().ToString();
                var empByCode = employeeList.FirstOrDefault(f => f.EmployeeCode == findWhat);
                if (empByCode != null)
                {
                    try
                    {
                        testRandomListByEmpCodeToDay = TestRandomController.GetByEmpCode(empByCode.EmployeeCode).Where(w => w.TestDate == toDay).ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.InnerException.Message.ToString());
                        return;   
                    }
                    if (testRandomListByEmpCodeToDay.Count() == 0)
                    {
                        MessageBox.Show(string.Format("Worker Not Exist In Covid Test List day: {0:dd/MM/yyyy} !", toDay), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var workerTestToday = testRandomListByEmpCodeToDay.FirstOrDefault();
                        if (string.IsNullOrEmpty(workerTestToday.TimeIn))
                        {
                            MessageBox.Show(string.Format("Worker Does Not CheckIn day: {0:dd/MM/yyyy} !", toDay), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            var confirmResultInfo = new ConfirmResultInfo
                            {
                                Id = workerTestToday.Id,
                                EmployeeName = empByCode.EmployeeName,
                                EmployeeCode = empByCode.EmployeeCode,
                                EmployeeID = empByCode.EmployeeID,
                                DepartmentName = empByCode.DepartmentName,
                                TestDate = string.Format("{0:dd/MM/yyyy}", workerTestToday.TestDate),
                                TimeIn = workerTestToday.TimeIn,
                                UpdateResultTime = workerTestToday.UpdateResultTime
                            };
                            brDisplay.DataContext = confirmResultInfo;
                            cboResult.SelectedItem = string.IsNullOrEmpty(workerTestToday.Result) == false ? workerTestToday.Result.Equals("Negative") ? results[0] : results[1] : results[0];
                            txtConfirmBy.Text = workerTestToday.PersonConfirm;
                            txtRemark.Text = workerTestToday.Remark;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Worker Not Found !", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ClearUI()
        {
            brDisplay.DataContext = null;
            cboResult.ItemsSource = results;
            cboResult.SelectedItem = results[0];
            txtConfirmBy.Text = "";
            txtRemark.Text = "";
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var updateTestResultModel = testRandomListByEmpCodeToDay.FirstOrDefault();
            if (bwSave.IsBusy == false && updateTestResultModel != null)
            {
                updateTestResultModel.Result = cboResult.SelectedItem.ToString();
                updateTestResultModel.UpdateResultTime = string.Format("{0:HH:mm}", DateTime.Now);
                updateTestResultModel.Status = "ConfirmedResult";
                updateTestResultModel.Remark = txtRemark.Text.Trim().ToString();
                updateTestResultModel.PersonConfirm = txtConfirmBy.Text.Trim().ToString();

                this.Cursor = Cursors.Wait;
                btnSave.IsEnabled = false;
                bwSave.RunWorkerAsync(updateTestResultModel);
            }
        }

        private void txtCardId_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtCardId.SelectAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bwLoad.IsBusy == false)
            {
                lblHeader.Text = string.Format("{0}: {1:dd/MM/yyyy}", lblResourceHeader, toDay);
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
        }
        private void SetTxtDefault()
        {
            txtCardId.SelectAll();
            txtCardId.Focus();
        }
        
        private void DoCounter(List<TestRandomModel> testRandomListByDate)
        {
            lblConfirmed.Text = "";
            lblTotalCheckIn.Text = "";

            int totalCheckIn = testRandomListByDate.Where(w => !string.IsNullOrEmpty(w.TimeIn)).ToList().Count();
            int confirmed = testRandomListByDate.Where(w => !string.IsNullOrEmpty(w.Result)).ToList().Count();

            lblConfirmed.Text = string.Format("Total CheckIn: {0}", totalCheckIn);
            lblTotalCheckIn.Text = string.Format("Confirmed: {0}", confirmed);

        }

        private class ConfirmResultInfo
        {
            public string Id { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeCode { get; set; }
            public string EmployeeID { get; set; }
            public string DepartmentName { get; set; }
            public string TestDate { get; set; }
            public string TimeIn { get; set; }
            public string UpdateResultTime { get; set; }
        }
    }
}
