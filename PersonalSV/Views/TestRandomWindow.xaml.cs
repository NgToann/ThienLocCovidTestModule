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
    /// Interaction logic for TestRandomWindow.xaml
    /// </summary>
    public partial class TestRandomWindow : Window
    {
        BackgroundWorker bwLoad;
        DispatcherTimer clock;
        List<EmployeeModel> employeeList;
        List<EmployeeModel> employeeOriginList;
        private PrivateDefineModel defModel;
        List<TestRandomModel> testRandomList;
        List<TestRandomModel> testRequestListToDay;
        private DateTime toDay = DateTime.Now.Date;
        private string lblInfoRatio = "", lblInfoTestDate = "", lblInfoTerm = "", lblInfoRound = "", lblInfoTotalWorker = "", lblInfoCurrent = "", lblResourceNotFound = "", lblResourceTime = "", lblResouceVaccine = "", lblResouceVaccineNull = "", lblRequireTest = "", lblResouceWelcome = "", lblResourceHeader = "";
        public TestRandomWindow()
        {
            employeeList = new List<EmployeeModel>();
            defModel = new PrivateDefineModel();
            testRandomList = new List<TestRandomModel>();
            testRequestListToDay = new List<TestRandomModel>();

            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            clock = new DispatcherTimer();
            clock.Tick += Clock_Tick;
            clock.Start();

            lblResourceNotFound = LanguageHelper.GetStringFromResource("messageNotFound");
            lblResourceTime = LanguageHelper.GetStringFromResource("testRandomLblTime");
            lblResouceVaccine = LanguageHelper.GetStringFromResource("testRandomLblVaccine");
            lblResouceVaccineNull = LanguageHelper.GetStringFromResource("testRandomLblVaccineNull");
            lblRequireTest = LanguageHelper.GetStringFromResource("testRandomLblRequireTest");
            lblResouceWelcome = LanguageHelper.GetStringFromResource("testRandomLblWelcome");
            lblResourceHeader = LanguageHelper.GetStringFromResource("testRandomHeader");
            lblInfoTestDate = LanguageHelper.GetStringFromResource("testRandomInfoTestDate");
            lblInfoTerm = LanguageHelper.GetStringFromResource("testRandomInfoTerm");
            lblInfoRound = LanguageHelper.GetStringFromResource("testRandomInfoRound");
            lblInfoTotalWorker = LanguageHelper.GetStringFromResource("testRandomInfoTotalWorker");
            lblInfoCurrent = LanguageHelper.GetStringFromResource("testRandomInfoCurrent");
            lblInfoRatio = LanguageHelper.GetStringFromResource("testRandomInfoRandomRatio");

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
            btnRefresh.IsEnabled = true;
            DisplayData();

            txtCardId.IsEnabled = true;
            txtCardId.Focus();
        }

        private void BwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                defModel = CommonController.GetDefineProps();
                employeeList = EmployeeController.GetAvailableForTestCovid();
                testRandomList = TestRandomController.GetAll();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message.ToString());
                }));
            }
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

        private void dgWorkerNeedToTestList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void DisplayData()
        {
            // get 20% of list
            int randomPercent = defModel.TestRandomRatio != 0 ? defModel.TestRandomRatio : 20;
            var totalEmp = employeeList.Count();
            var requestNumber = (int)(randomPercent * totalEmp / 100);

            // update 20 worker has firstdose
            foreach (var updateFirstDose in employeeList.OrderBy(o => Guid.NewGuid()).Take(20).ToList())
            {
                updateFirstDose.VaccineFirstType = "Pfizer";
                updateFirstDose.FirstInjectDate = new DateTime(2021, 10, 02);
            }

            // update 10 worker has fullvaccince
            foreach (var updateFullVaccine in employeeList.OrderBy(o => Guid.NewGuid()).Take(10).ToList())
            {
                updateFullVaccine.VaccineFirstType = "Astrazeneca";
                updateFullVaccine.FirstInjectDate = new DateTime(2021, 09, 15);

                updateFullVaccine.VaccineSecondType = "Astrazeneca";
                updateFullVaccine.SecondInjectDate = new DateTime(2021, 09, 30);

            }
            employeeOriginList = employeeList.ToList();

            // get worker has 1st vacine.
            var workersHasFirstDose = employeeList.Where(w => !string.IsNullOrEmpty(w.VaccineFirstType) && string.IsNullOrEmpty(w.VaccineSecondType)).ToList();

            // get requestedtoday
            testRequestListToDay = testRandomList.Where(w => w.TestDate == toDay).ToList();
            var workerRequestedToday = testRequestListToDay.Select(s => s.EmployeeCode).Distinct().ToList();

            var currentTerm = testRandomList.Count() > 0 ? testRandomList.Max(m => m.Term) : 0;
            var currentRound = testRandomList.Count() > 0 ? testRandomList.Max(m => m.Round) : 0;

            var workerTestedCurrentTerm = testRandomList.Where(w => w.Term == currentTerm).Select(s => s.EmployeeCode).Distinct().ToList();
            if (currentTerm == 0)
            {
                currentTerm = 1;
            }
            else
            {
                // is new term
                if (workerTestedCurrentTerm.Count() >= totalEmp)
                {
                    currentTerm = currentTerm + 1;
                    currentRound = 0;
                }
                else
                {
                    // Remove worker at the current term
                    workersHasFirstDose.RemoveAll(r => workerTestedCurrentTerm.Contains(r.EmployeeCode));
                    employeeList.RemoveAll(r => workerTestedCurrentTerm.Contains(r.EmployeeCode));
                }
            }

            // Remove workerRequestToday
            employeeList.RemoveAll(r => workerRequestedToday.Contains(r.EmployeeCode));
            workersHasFirstDose.RemoveAll(r => workerRequestedToday.Contains(r.EmployeeCode));

            // priority vaccine 1st
            var workerHasVaccinceRequestList = workersHasFirstDose.OrderBy(o => Guid.NewGuid()).Take(requestNumber).ToList();
            int requestNumberRemain = 0;
            if (workerHasVaccinceRequestList.Count() < requestNumber)
            {
                requestNumberRemain = requestNumber - workerHasVaccinceRequestList.Count();
            }
            else
            {
                requestNumberRemain = requestNumber;
            }
            var workerIdsHasVaccince = workerHasVaccinceRequestList.Select(s => s.EmployeeCode).Distinct().ToList();
            var workerRequestList = employeeList.Where(w => !workerIdsHasVaccince.Contains(w.EmployeeCode)).OrderBy(o => Guid.NewGuid()).Take(requestNumberRemain).ToList();

            var totalRequestList = new List<EmployeeModel>();
            totalRequestList.AddRange(workerHasVaccinceRequestList);
            totalRequestList.AddRange(workerRequestList);

            if (testRequestListToDay.Count() == 0)
            {
                try
                {
                    foreach (var req in totalRequestList)
                    {
                        var randomInsert = new TestRandomModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            EmployeeCode = req.EmployeeCode,
                            TestDate = toDay,
                            Term = currentTerm,
                            Round = currentRound + 1,
                            Result = "",
                            PersonConfirm = "",
                            Remark = "",
                            TimeIn = "",
                            TimeOut = "",
                            Status = "Queue"
                        };
                        TestRandomController.Insert(randomInsert);
                        testRequestListToDay.Add(randomInsert);
                    }
                    dgWorkerNeedToTestList.ItemsSource = totalRequestList;
                    dgWorkerNeedToTestList.Items.Refresh();
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                var workerIdsRequestTestToday = testRequestListToDay.Select(s => s.EmployeeCode).Distinct().ToList();
                var displayList = employeeOriginList.Where(w => workerIdsRequestTestToday.Contains(w.EmployeeCode)).ToList();
                dgWorkerNeedToTestList.ItemsSource = displayList;
                dgWorkerNeedToTestList.Items.Refresh();
            }

            // Display TestInfo
            var firstTestRequest = testRequestListToDay.FirstOrDefault();
            var testInfo = new TestDisplayInfo
            {
                TestDate = string.Format("{0}: {1:dd/MM/yyyy}", lblInfoTestDate, firstTestRequest.TestDate),
                Term = String.Format("{0}: {1}", lblInfoTerm, firstTestRequest.Term),
                Round = String.Format("{0}: {1}", lblInfoRound, firstTestRequest.Round),
                TotalRequestWorker = String.Format("{0}: {1}", lblInfoTotalWorker, testRequestListToDay.Count()),
                CurrentScan = "",
                RandomRatio = String.Format("{0}: {1} %", lblInfoRatio, defModel.TestRandomRatio)
            };
            grTestInfo.DataContext = testInfo;
            UpdateCurrentScanned(testRequestListToDay);
        }
        
        private void UpdateCurrentScanned(List<TestRandomModel> testRequestToday)
        {
            var total = testRequestToday.Count();
            var scanned = testRequestToday.Where(w => !String.IsNullOrEmpty(w.TimeIn)).ToList();

            var currentInfoDisplay = grTestInfo.DataContext as TestDisplayInfo;
            if (currentInfoDisplay != null)
            {
                currentInfoDisplay.CurrentScan = String.Format("{0}: {1} / {2}", lblInfoCurrent, scanned.Count(), total);
            }
            grTestInfo.DataContext = null;
            grTestInfo.DataContext = currentInfoDisplay;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (bwLoad.IsBusy == false)
            {
                btnRefresh.IsEnabled = false;
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
        }

        private void txtCardId_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            brDisplay.DataContext = null;
            this.Background = Brushes.WhiteSmoke;

            if (e.Key == Key.Enter)
            {
                string findWhat = txtCardId.Text.Trim().ToLower().ToString();
                // check worker exist in test request today
                var testRequestByEmpId = testRequestListToDay.FirstOrDefault(f => f.EmployeeCode == findWhat);
                var empById = employeeOriginList.FirstOrDefault(f => f.EmployeeCode == findWhat);
                var vaccinceStatus = lblResouceVaccineNull;

                if (empById != null && !string.IsNullOrEmpty(empById.VaccineFirstType))
                {
                    vaccinceStatus = string.Format("( {0} )", empById.VaccineFirstType);
                    if (!string.IsNullOrEmpty(empById.VaccineSecondType))
                    {
                        vaccinceStatus = string.Format("( {0} + {1} )", empById.VaccineFirstType, empById.VaccineSecondType);
                    }
                }

                if (empById == null)
                {
                    var empNotFound = new DisplayInfo
                    {
                        EmployeeName = findWhat,
                        TimeDisplay = lblResourceNotFound
                    };
                    brDisplay.DataContext = empNotFound;
                    SetTxtDefault();
                }
                else if (testRequestByEmpId != null)
                {
                    brDisplay.Background = Brushes.Yellow;
                    var displayRequireTestInfo = new DisplayInfo
                    {
                        EmployeeName = String.Format("{0} - {1}", empById.EmployeeName, empById.EmployeeID),
                        DepartmentName = empById.DepartmentName,
                        VaccineStatus = string.Format("{0}: {1}", lblResouceVaccine, vaccinceStatus),
                        RequireTest = lblRequireTest,
                        TimeDisplay = string.Format("{0} {1:HH:mm}", lblResourceTime, DateTime.Now),
                        TimeIn = string.Format("{0:HH:mm}", DateTime.Now),
                        IdDisplay = string.Format("testId: {0}", testRequestByEmpId.Id)
                    };

                    var updateTestModelById = testRequestListToDay.FirstOrDefault(f => f.EmployeeCode == testRequestByEmpId.EmployeeCode);
                    updateTestModelById.TimeIn = displayRequireTestInfo.TimeIn;
                    updateTestModelById.Status = "Scanned";

                    try
                    {
                        TestRandomController.Update(updateTestModelById, 1);
                        brDisplay.DataContext = displayRequireTestInfo;
                        UpdateCurrentScanned(testRequestListToDay);

                        SetTxtDefault();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                        SetTxtDefault();                       
                    }
                }
                else
                {
                    brDisplay.Background = Brushes.Green;
                    var displayWelcomeInfo = new DisplayInfo
                    {
                        EmployeeName = String.Format("{0} - {1}", empById.EmployeeName, empById.EmployeeID),
                        DepartmentName = empById.DepartmentName,
                        VaccineStatus = string.Format("{0}: {1}", lblResouceVaccine, vaccinceStatus),
                        RequireTest = lblResouceWelcome,
                        TimeDisplay = string.Format("{0} {1:HH:mm}", lblResourceTime, DateTime.Now),
                        TimeIn = string.Format("{0:HH:mm}", DateTime.Now)
                    };

                    brDisplay.DataContext = displayWelcomeInfo;
                    SetTxtDefault();
                }
            }
        }

        private void txtCardId_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtCardId.SelectAll();
        }

        private void hlViewWorkerList_Click(object sender, RoutedEventArgs e)
        {
            var sources = dgWorkerNeedToTestList.ItemsSource.OfType<EmployeeModel>().ToList();
            TestRequestListWindow window = new TestRequestListWindow(sources, testRequestListToDay);
            window.ShowDialog();
            SetTxtDefault();
        }
        
        private void SetTxtDefault()
        {
            txtCardId.SelectAll();
            txtCardId.Focus();
        }
        private class DisplayInfo
        {
            public string IdDisplay { get; set; }
            public string EmployeeName { get; set; }
            public string DepartmentName { get; set; }
            public string VaccineStatus { get; set; }
            public string RequireTest { get; set; }
            public string TimeIn { get; set; }
            public string TimeDisplay { get; set; }
        }
        
        private class TestDisplayInfo
        {
            public string TestDate { get; set; }
            public string Term { get; set; }
            public string Round { get; set; }
            public string RandomRatio { get; set; }
            public string TotalRequestWorker { get; set; }
            public string CurrentScan { get; set; }
        }
    }
}
