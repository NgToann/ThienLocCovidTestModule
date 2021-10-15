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
    /// Interaction logic for TestRandomThienLocWindow.xaml
    /// </summary>
    public partial class TestRandomThienLocWindow : Window
    {
        BackgroundWorker bwLoad;
        DispatcherTimer clock;
        private PrivateDefineModel defModel;
        List<TestRandomModel> testRandomListByEmpCode;
        List<TestRandomModel> testRandomListByDate;
        List<EmployeeModel> employeeList;
        private string lblResourceHeader = "", lblResourceNotFound = "", lblGetInQueue = "", lblDoNotConfirmTestResult = "", lblNotAllowed = "", lblWelcome="", lblNextTestDate = "", lblDoNotCheckIn = "";
        private string lblTestDate = "", lblTotalWorker = "", lblCurrentTimeIn = "", lblTestRate = "";
        private DateTime toDay = DateTime.Now.Date;
        public TestRandomThienLocWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            clock = new DispatcherTimer();
            clock.Tick += Clock_Tick;
            clock.Start();

            lblResourceHeader = LanguageHelper.GetStringFromResource("testRandomHeader");
            lblResourceNotFound = LanguageHelper.GetStringFromResource("messageNotFound");
            lblGetInQueue = LanguageHelper.GetStringFromResource("tlTestRandomMsgGetInQueue");            
            lblDoNotConfirmTestResult = LanguageHelper.GetStringFromResource("tlTestRandomMsgDoNotConfirmTestResult");
            lblNotAllowed = LanguageHelper.GetStringFromResource("tlTestRandomMsgNotAllowed");
            lblWelcome = LanguageHelper.GetStringFromResource("tlTestRandomMsgWelcome");
            lblNextTestDate = LanguageHelper.GetStringFromResource("tlTestRandomNextTestDate");
            lblDoNotCheckIn = LanguageHelper.GetStringFromResource("confirmTestResultMsgDoNotCheckIn");

            lblTestDate = LanguageHelper.GetStringFromResource("tlTestRandomTestDate");
            lblTotalWorker = LanguageHelper.GetStringFromResource("tlTestRandomTotalWorkerTest");
            lblCurrentTimeIn = LanguageHelper.GetStringFromResource("tlTestRandomNextCurrentTimeIn");
            lblTestRate = LanguageHelper.GetStringFromResource("tlTestRandomTestRate");

            defModel = new PrivateDefineModel();
            testRandomListByDate = new List<TestRandomModel>();
            testRandomListByEmpCode = new List<TestRandomModel>();
            employeeList = new List<EmployeeModel>();

            InitializeComponent();
        }

        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            DoCounter(testRandomListByDate);
            txtCardId.IsEnabled = true;
            SetTxtDefault();
        }

        private void BwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                defModel = CommonController.GetDefineProps();
                employeeList = EmployeeController.GetAvailableForTestCovid();
                testRandomListByDate = TestRandomController.GetByDate(toDay);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message.ToString());
                }));
            }
        }

        private void Clock_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                lblClock.Text = string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now);
            }));
        }

        private void txtCardId_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            brDisplay.DataContext = null;
            brDisplay.Background = Brushes.WhiteSmoke;

            if(e.Key == Key.Enter)
            {
                string findWhat = txtCardId.Text.Trim().ToLower().ToString();
                var empByCode = employeeList.FirstOrDefault(f => f.EmployeeCode == findWhat);
                if (empByCode != null)
                {

                    try
                    {
                        testRandomListByEmpCode = TestRandomController.GetByEmpCode(empByCode.EmployeeCode);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                        return;
                    }

                    var testRandomListToday = testRandomListByEmpCode.Where(w => w.TestDate == toDay).ToList();
                    var testRandomNextListToday = testRandomListByEmpCode.Where(w => w.TestDate > toDay).ToList();
                    var testRandomBeforeListToday = testRandomListByEmpCode.Where(w => w.TestDate < toDay).ToList();

                    if (testRandomListToday.Count() > 0)
                    {
                        var testRdToday = testRandomListToday.FirstOrDefault();
                        // if not yet check in => welcome
                        string timeScanCompare = string.Format("{0:HH:mm}", DateTime.Now.AddMinutes(-defModel.WaitingMinutes));
                        if (string.IsNullOrEmpty(testRdToday.TimeIn) || string.Compare(testRdToday.TimeIn, timeScanCompare) >= 1)
                        {
                            UpdateTimeIn(testRdToday, empByCode, Brushes.Yellow, lblGetInQueue);
                            SetTxtDefault();
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(testRdToday.Result))
                            {
                                Alert(empByCode, Brushes.Red, lblDoNotConfirmTestResult, testRdToday.Id);
                            }
                            // Not Allowed Positive
                            else if (testRdToday.Result.Equals("Positive"))
                            {
                                Alert(empByCode, Brushes.Red, lblNotAllowed, testRdToday.Id);
                            }
                            else if (testRdToday.Result.Equals("Negative"))
                            {
                                // Check Worker has next schedule test
                                if (testRandomNextListToday.Count() > 0)
                                {
                                    CheckNextTestDate(testRandomNextListToday, empByCode);
                                }
                                else
                                {
                                    Alert(empByCode, Brushes.Green, lblWelcome, testRdToday.Id);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Check Result Latest
                        if (testRandomBeforeListToday.Count() > 0)
                        {
                            var workerTestLatest = testRandomBeforeListToday.OrderBy(o => o.TestDate).LastOrDefault();
                            if (workerTestLatest.Result.Equals("Positive"))
                            {
                                Alert(empByCode, Brushes.Red, lblNotAllowed, workerTestLatest.Id);
                            }
                            else if (workerTestLatest.Result.Equals("Negative"))
                            {
                                // Check Next Schedule
                                if (testRandomNextListToday.Count() > 0)
                                {
                                    CheckNextTestDate(testRandomNextListToday, empByCode);
                                }
                                else
                                {
                                    Alert(empByCode, Brushes.Green, lblWelcome, workerTestLatest.Id);
                                }
                            }
                            else
                            {
                                Alert(empByCode, Brushes.Yellow, string.Format("{0}: {1:dd/MM/yyyy}", lblDoNotCheckIn, workerTestLatest.TestDate), workerTestLatest.Id);
                            }
                        }
                        else if (testRandomNextListToday.Count() > 0)
                        {
                            CheckNextTestDate(testRandomNextListToday, empByCode);
                        }
                    }
                }

                else
                {
                    var empNotFound = new TestInfo
                    {
                        EmployeeName = findWhat,
                        Message = lblResourceNotFound
                    };
                    brDisplay.DataContext = empNotFound;
                    SetTxtDefault();
                }

            }
        }

        private void CheckNextTestDate(List<TestRandomModel> testRandomNextListToday, EmployeeModel empByCode)
        {
            var nextTestDate = testRandomNextListToday.Min(m => m.TestDate);
            if ((nextTestDate - toDay).TotalDays <= defModel.RemindTestDate)
            {
                var workerTestNextTestDate = testRandomNextListToday.FirstOrDefault(f => f.TestDate == nextTestDate);
                Alert(empByCode, Brushes.Yellow, string.Format("{0}: {1:dd/MM/yyyy}", lblNextTestDate, nextTestDate), workerTestNextTestDate.Id);
            }
            else
            {
                Alert(empByCode, Brushes.Green, lblWelcome, null);
            }
        }

        private void UpdateTimeIn(TestRandomModel testRandomUpdateTimeIn, EmployeeModel empByCode, SolidColorBrush color ,string msg)
        {
            testRandomUpdateTimeIn.TimeIn = String.Format("{0:HH:mm}", DateTime.Now);
            testRandomUpdateTimeIn.Status = "In";
            try
            {
                TestRandomController.Update(testRandomUpdateTimeIn, 1);
                testRandomListByDate = TestRandomController.GetByDate(toDay);
                brDisplay.Background = color;
                var displayInfo = new TestInfo
                {
                    EmployeeName = string.Format("{0} - {1}", empByCode.EmployeeName, empByCode.EmployeeID),
                    DepartmentName = empByCode.DepartmentName,
                    TimeIn = testRandomUpdateTimeIn.TimeIn,
                    Message = lblGetInQueue,
                    IdDisplay = string.Format("testid: {0}", testRandomUpdateTimeIn.Id)
                };
                brDisplay.DataContext = displayInfo;
                DoCounter(testRandomListByDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message.ToString());
            }
        }

        private void Alert(EmployeeModel empByCode, SolidColorBrush alertColor, string msg, string testId)
        {
            brDisplay.Background = alertColor;
            var alertMsg = new TestInfo
            {
                EmployeeName = string.Format("{0}", empByCode.EmployeeName),
                DepartmentName = empByCode.EmployeeID,
                Message = msg,
                IdDisplay = testId != null ? string.Format("tesid: {0}", testId) : ""
            };
            brDisplay.DataContext = alertMsg;
            SetTxtDefault();
        }

        private void DoCounter(List<TestRandomModel> testRandomToday)
        {
            int totalWorkerToday = testRandomToday.Select(s => s.EmployeeCode).Distinct().Count();
            int totalWorkerHasTimeIn = testRandomToday.Where(w => !string.IsNullOrEmpty(w.TimeIn)).Select(s => s.EmployeeCode).Distinct().Count();

            var counterDisplay = new CounterInfo
            {
                TestDate = string.Format("{0}: {1:dd/MM/yyyy}", lblTestDate, toDay),
                RandomRatio = string.Format("{0}: {1} %", lblTestRate, defModel.TestRandomRatio),
                TotalRequestWorker = string.Format("{0}: {1}", lblTotalWorker, totalWorkerToday),
                CurrentTimeIn = string.Format("{0}: {1} / {2}", lblCurrentTimeIn, totalWorkerHasTimeIn, totalWorkerToday)
            };

            grTestInfo.DataContext = null;
            grTestInfo.DataContext = counterDisplay;
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
        private void txtCardId_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtCardId.SelectAll();
        }

        private class TestInfo
        {
            public string IdDisplay { get; set; }
            public string EmployeeName { get; set; }
            public string DepartmentName { get; set; }
            public string TimeIn { get; set; }
            public string Message { get; set; }
        }
        
        private class CounterInfo
        {
            public string TestDate { get; set; }
            public string Term { get; set; }
            public string Round { get; set; }
            public string RandomRatio { get; set; }
            public string TotalRequestWorker { get; set; }
            public string CurrentTimeIn { get; set; }
        }
    }
}
