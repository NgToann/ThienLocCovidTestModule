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
    /// Interaction logic for WorkerCheckInWindow.xaml
    /// </summary>
    public partial class WorkerCheckInWindow : Window
    {
        DispatcherTimer clock;
        BackgroundWorker bwLoad;
        List<EmployeeModel> employeeList;

        private string lblResourceNotFound = "", lblNotExitsInWorkList = "", lblNotAllowed = "", lblNotExistInTestList = "";
        private string lblInfoTestDate = "", lblInfoTotalWorkList = "", lblInfoScanned = "", lblInfoRatio = "";

        private string lblNextTestDate = "", lblTestTime = "", lblWorkTime = "", lblGetInQueue = "";

        private PrivateDefineModel defModel;
        private List<WorkListModel> workList;
        private List<WorkListModel> workListAll;
        private List<WorkerCheckInModel> workerCheckInList;

        private DateTime toDay = DateTime.Now.Date;
        private string afternoonStone = "";
        public WorkerCheckInWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            defModel = new PrivateDefineModel();
            employeeList = new List<EmployeeModel>();
            workList = new List<WorkListModel>();
            workListAll = new List<WorkListModel>();
            workerCheckInList = new List<WorkerCheckInModel>();

            lblResourceNotFound = LanguageHelper.GetStringFromResource("messageNotFound");
            lblNotExitsInWorkList = LanguageHelper.GetStringFromResource("workerCheckInMessageNotPriority");
            lblNotAllowed = LanguageHelper.GetStringFromResource("workerCheckInMessageNotAllowed");
            lblNotExistInTestList = LanguageHelper.GetStringFromResource("workerCheckInMessageNotExistInTestList");

            lblInfoTestDate = LanguageHelper.GetStringFromResource("workerCheckInStatisticsTestDate");
            lblInfoTotalWorkList = LanguageHelper.GetStringFromResource("workerCheckInStatisticsTotalWorkList");
            lblInfoScanned = LanguageHelper.GetStringFromResource("workerCheckInStatisticsCurrentScan");
            lblInfoRatio = LanguageHelper.GetStringFromResource("workerCheckInStatisticsRatio");

            lblNextTestDate = LanguageHelper.GetStringFromResource("workerCheckInLblNextTestDate");
            lblTestTime = LanguageHelper.GetStringFromResource("workerCheckInLblTestTime");
            lblWorkTime = LanguageHelper.GetStringFromResource("workerCheckInLblWorkTime");
            lblGetInQueue = LanguageHelper.GetStringFromResource("workerCheckInLblGetInQueue");

            clock = new DispatcherTimer();
            clock.Tick += Clock_Tick;
            clock.Start();

            InitializeComponent();
        }
        private void Clock_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                lblClock.Text = string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now);
            }));
        }
        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            DoStatistics(workListAll, workerCheckInList);

            txtCardId.IsEnabled = true;
            SetTxtDefault();
        }

        private void BwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                employeeList = EmployeeController.GetAvailable();
                workListAll = WorkListController.Get();
                workerCheckInList = WorkerCheckInController.GetByDate(toDay);
                defModel = CommonController.GetDefineProps();

                afternoonStone = defModel.AfternoonStone;
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
            var currentTime = string.Format("{0:HH:mm}", DateTime.Now);
            grDisplay.DataContext = null;
            this.Background = Brushes.WhiteSmoke; 
            brDisplay.Background = Brushes.WhiteSmoke;

            if (e.Key == Key.Enter)
            {
                // get worker by cardid
                string scanWhat = txtCardId.Text.Trim().ToUpper().ToString();
                var empById = employeeList.FirstOrDefault(f => f.EmployeeCode == scanWhat);
                if (empById != null)
                {
                    try
                    {
                        workList = WorkListController.GetByEmpId(empById.EmployeeID);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                        SetTxtDefault();
                        return;
                    }
                    // Check in worklist
                    if (workList.Count() > 0)
                    {
                        var testToday = workList.Where(w => w.TestDate == toDay).ToList();
                        var testNextDay = workList.Where(w => w.TestDate > toDay).ToList();
                        var testBefore = workList.Where(w => w.TestDate < toDay).ToList();

                        // Morning currenttime <= afternoon
                        if (String.Compare(currentTime, afternoonStone) < 1)
                        {
                            if (testToday.Count() > 0)
                            {
                                var workerTestToday = testToday.FirstOrDefault();
                                if (workerTestToday.TestStatus == 0)
                                {
                                    brDisplay.Background = Brushes.Yellow;
                                    AddRecord(empById, workerTestToday, false, true);                                    
                                }
                                else if (workerTestToday.TestStatus == 1)
                                {
                                    brDisplay.Background = Brushes.Green;
                                    AddRecord(empById, null, false, false);
                                }
                                else if (workerTestToday.TestStatus == 2)
                                {
                                    AlertScan(lblNotAllowed, Brushes.Red, empById);
                                }
                            }
                            else if (testBefore.Count() > 0)
                            {
                                var workerTestLatest = testBefore.OrderBy(o => o.TestDate).LastOrDefault();
                                if (workerTestLatest.TestStatus == 0)
                                {
                                    brDisplay.Background = Brushes.Yellow;
                                    //AddRecord(empById, workerTestLatest, false, true);
                                    string alertNotExistInTestListToDay = string.Format("{0}: {1:dd/MM/yyyy}", lblNotExistInTestList, toDay);
                                    AlertScan(alertNotExistInTestListToDay, Brushes.Yellow, empById);
                                }
                                else if (workerTestLatest.TestStatus == 1)
                                {
                                    brDisplay.Background = Brushes.Green;
                                    AddRecord(empById, null , false, false);
                                }
                                else if (workerTestLatest.TestStatus == 2)
                                {
                                    AlertScan(lblNotAllowed, Brushes.Red, empById);
                                }
                            }
                        }
                        // Afternoon
                        else
                        {
                            var testBeforeOrToday = workList.Where(w => w.TestDate <= toDay).ToList();
                            if (testNextDay.Count() > 0)
                            {
                                var workerTestNextDay = testNextDay.FirstOrDefault();
                                brDisplay.Background = Brushes.Yellow;
                                AddRecord(empById, workerTestNextDay, true, false);
                            }
                            else if (testBeforeOrToday.Count() > 0)
                            {
                                var workerTestBeforeOrToday = testBeforeOrToday.OrderBy(o => o.TestDate).LastOrDefault();
                                if (workerTestBeforeOrToday.TestStatus == 1)
                                {
                                    brDisplay.Background = Brushes.Green;
                                    AddRecord(empById, null, false, false);
                                }
                                else if (workerTestBeforeOrToday.TestStatus == 0)
                                {
                                    brDisplay.Background = Brushes.Yellow;
                                    //AddRecord(empById, null, false, true);
                                    string alertNotExistInTestListToDay = string.Format("{0}: {1:dd/MM/yyyy}", lblNotExistInTestList, toDay);
                                    AlertScan(alertNotExistInTestListToDay, Brushes.Yellow, empById);
                                }
                                else if (workerTestBeforeOrToday.TestStatus == 2)
                                {
                                    AlertScan(lblNotAllowed, Brushes.Red, empById);
                                }
                            }
                        }
                        DoStatistics(workListAll, workerCheckInList);
                    }
                    else
                    {
                        AlertScan(lblNotExitsInWorkList, Brushes.Red, empById);
                    }                   
                }
                else
                {
                    var notFound = new CheckInInfoDisplay
                    {
                        EmployeeDisplay = scanWhat,
                        NextTestDate = lblResourceNotFound
                    };
                    grDisplay.DataContext = notFound;
                    SetTxtDefault();
                }
                
                // Refresh Counter
            }
        }

        private void checkTestToday()
        {

        }

        private void DoStatistics(List<WorkListModel> workListAll, List<WorkerCheckInModel> workerCheckInList)
        {
            var totalWorker = workListAll.Select(s => s.EmployeeID).Distinct().ToList().Count();
            var totalCheckedIn = workerCheckInList.Where(w => !string.IsNullOrEmpty(w.RecordTime) && w.CheckType == 0).Select(s => s.EmployeeCode).Distinct().ToList().Count();
            double percent = 0;
            if (totalWorker != 0)
            {
                percent = Math.Round(((double)totalCheckedIn / (double)totalWorker * 100), 1);
            }

            var displayModel = new CheckInStatistics
            {
                TestDate = string.Format("{0}: {1:dd/MM/yyyy}", lblInfoTestDate, toDay),
                TotalWorkList = string.Format("{0}: {1}", lblInfoTotalWorkList, totalWorker),
                Scanned = string.Format("{0}: {1} / {2}", lblInfoScanned, totalCheckedIn, totalWorker),
                Ratio = string.Format("{0}: {1} %", lblInfoRatio, percent)
            };

            grStatistics.DataContext = null;
            grStatistics.DataContext = displayModel;
        }

        private void AlertScan(string msg, SolidColorBrush color ,EmployeeModel empById)
        {
            brDisplay.Background = color;
            var alert = new CheckInInfoDisplay
            {
                //EmployeeDisplay = String.Format("{0} - {1}", empById.EmployeeName, empById.EmployeeID),
                EmployeeDisplay = empById.EmployeeName,
                DepartmentName = empById.EmployeeID,
                //RecordTime = msg
                TestTime = msg
            };
            grDisplay.DataContext = alert;
            SetTxtDefault();
        }
        
        private void AddRecord( EmployeeModel empById, WorkListModel WorkListNextTestById, bool isNextDay, bool getInQueue)
        {
            var record = new WorkerCheckInModel()
            {
                Id = Guid.NewGuid().ToString(),
                EmployeeCode = empById.EmployeeCode,
                EmployeeID = empById.EmployeeID,
                EmployeeName = empById.EmployeeName,
                DepartmentName = empById.DepartmentName,
                CheckType = 0,
                CheckInDate = DateTime.Now,
                RecordTime = String.Format("{0:HH:mm}", DateTime.Now)
            };

            try
            {
                string workTime = WorkListNextTestById != null ? String.Format("{0}: {1}", lblWorkTime ,WorkListNextTestById.WorkTime) : "";
                string testTime = WorkListNextTestById != null ? String.Format("{0}: {1}", lblTestTime ,WorkListNextTestById.TestTime) : "";
                string nextTestDate = WorkListNextTestById != null ? String.Format("{0}: {1:dd/MM/yyyy}", lblNextTestDate, WorkListNextTestById.TestDate) : "";

                if (WorkListNextTestById != null && string.IsNullOrEmpty(WorkListNextTestById.WorkTime))
                {
                    workTime = "";
                }
                if (WorkListNextTestById != null && string.IsNullOrEmpty(WorkListNextTestById.TestTime))
                {
                    testTime = "";
                }
                if (!isNextDay)
                {
                    nextTestDate = "";
                }

                if (getInQueue && workTime == "" && testTime == "" && nextTestDate == "")
                {
                    testTime = lblGetInQueue;
                }

                var addInfoDisplay = new CheckInInfoDisplay
                {
                    EmployeeDisplay = String.Format("{0} - {1}", empById.EmployeeName, empById.EmployeeID),
                    DepartmentName = empById.DepartmentName,
                    RecordTime = String.Format("Time: {0}", record.RecordTime),
                    NextTestDate = nextTestDate,
                    WorkTime = workTime,
                    TestTime = testTime,
                };

                grDisplay.DataContext = addInfoDisplay;
                WorkerCheckInController.Insert(record);
                workerCheckInList.Add(record);

                SetTxtDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                SetTxtDefault();
            }
        }

        private void SetTxtDefault()
        {
            txtCardId.SelectAll();
            txtCardId.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //
            string lblResourceTitle = LanguageHelper.GetStringFromResource("workerCheckInTitle");
            tblTitle.Text = string.Format("{0}: {1:dd/MM/yyyy}", lblResourceTitle, DateTime.Now);
            if (bwLoad.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
        }

        private void dgWorkerCheckIn_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void txtCardId_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtCardId.SelectAll();
        }
        
        private class CheckInStatistics
        {
            public string TestDate { get; set; }
            public string TotalWorkList { get; set; }
            public string Scanned { get; set; }
            public string Ratio { get; set; }
        }
        
        private class CheckInInfoDisplay
        {
            public string EmployeeDisplay { get; set; }
            public string DepartmentName { get; set; }
            public string RecordTime { get; set; }
            public string NextTestDate { get; set; }
            public string WorkTime { get; set; }
            public string TestTime { get; set; }
            public string NextDayInfo { get; set; }
        }
    }
}
