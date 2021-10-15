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
    /// Interaction logic for TestCovidCreateTestTermWindow.xaml
    /// </summary>
    public partial class TestCovidCreateTestTermWindow : Window
    {
        List<EmployeeModel> employeeList;
        List<EmployeeModel> employeeOriginList;
        List<TestRandomModel> testRandomList;
        private PrivateDefineModel defModel;
        BackgroundWorker bwLoad;
        BackgroundWorker bwInsert;
        public TestCovidCreateTestTermWindow()
        {
            employeeList = new List<EmployeeModel>();
            defModel = new PrivateDefineModel();
            testRandomList = new List<TestRandomModel>();

            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            bwInsert = new BackgroundWorker();
            bwInsert.DoWork += BwInsert_DoWork; 
            bwInsert.RunWorkerCompleted += BwInsert_RunWorkerCompleted;

            InitializeComponent();
        }

        

        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            lblTestRate.Text = string.Format("{0} %", defModel.TestRandomRatio);
            btnCreate.IsEnabled = true;
        }

        private void BwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                defModel = CommonController.GetDefineProps();
                employeeList = EmployeeController.GetAvailableForTestCovid();
                employeeOriginList = employeeList.ToList();
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

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            List<TestRandomModel> testRandomCreateList = new List<TestRandomModel>();
            var createdDate = dpTestDate.SelectedDate.Value.Date;

            int randomPercent = defModel.TestRandomRatio != 0 ? defModel.TestRandomRatio : 20;
            var totalEmp = employeeList.Count();
            var requestNumber = (int)(randomPercent * totalEmp / 100);

            // For the first time
            if (testRandomList.Count() == 0)
            {
                var randomWorkerList = employeeList.OrderBy(o => Guid.NewGuid()).Take(requestNumber).ToList();
                foreach (var emp in randomWorkerList)
                {
                    var randomInsert = new TestRandomModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        EmployeeCode = emp.EmployeeCode,
                        TestDate = createdDate,
                        Term = 1,
                        Round = 1,
                        Result = "",
                        PersonConfirm = "",
                        Remark = "",
                        TimeIn = "",
                        TimeOut = "",
                        Status = "Plan",
                        EmployeeID = emp.EmployeeID,
                        EmployeeName = emp.EmployeeName,
                        DepartmentName = emp.DepartmentName
                    };
                    testRandomCreateList.Add(randomInsert);
                }
            }
            // Next time
            else
            {
                // check in process
                var inProcessList = testRandomList.Where(w => w.TestDate == createdDate && !w.Status.Equals("Plan")).ToList();
                if (inProcessList.Count() > 0)
                {
                    MessageBox.Show(string.Format("Test plan on: {0:dd/MM/yyyy} already started !\nCan not create new plan !", createdDate), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var currentTerm = testRandomList.Count() > 0 ? testRandomList.Max(m => m.Term) : 1;
                var currentRound = testRandomList.Count() > 0 ? testRandomList.Max(m => m.Round) + 1 : 1;

                // check already created
                var testRandomListByDate = testRandomList.Where(w => w.TestDate == createdDate).ToList();
                if (testRandomListByDate.Count() > 0)
                {
                    if (MessageBox.Show(string.Format("Test plan on: {0:dd/MM/yyyy} already created !\nDo you want to delete old data ?", createdDate),
                                    this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                    {
                        return;
                    }
                    currentTerm = testRandomListByDate.Max(m => m.Term);
                    currentRound = testRandomListByDate.Max(m => m.Round);

                    // Remove old data
                    TestRandomController.DeleteByDate(createdDate);
                    testRandomList.RemoveAll(r => r.TestDate == createdDate);
                }

                var workerInCurrentTermList = testRandomList.Where(w => w.Term == currentTerm).Select(s => s.EmployeeCode).Distinct().ToList();
                if (workerInCurrentTermList.Count() >= totalEmp)
                {
                    currentTerm = currentTerm + 1;
                    currentRound = 1;
                }

                var workerOutOfCurrentTermList = employeeList.Where(w => !workerInCurrentTermList.Contains(w.EmployeeCode)).ToList();
                var randomWorkerList = workerOutOfCurrentTermList.OrderBy(o => Guid.NewGuid()).Take(requestNumber).ToList();

                // If not enoudh reuquest number
                var additionList = new List<EmployeeModel>();
                if (randomWorkerList.Count() < requestNumber)
                {
                    var idCurrentList = randomWorkerList.Select(s => s.EmployeeCode).Distinct().ToList();
                    var remain = requestNumber - randomWorkerList.Count();
                    additionList = employeeList.Where(w => !idCurrentList.Contains(w.EmployeeCode)).OrderBy(o => Guid.NewGuid()).Take(remain).ToList();
                }
                randomWorkerList.AddRange(additionList);

                foreach (var emp in randomWorkerList)
                {
                    var randomInsert = new TestRandomModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        EmployeeCode = emp.EmployeeCode,
                        TestDate = createdDate,
                        Term = currentTerm,
                        Round = currentRound,
                        Result = "",
                        PersonConfirm = "",
                        Remark = "",
                        TimeIn = "",
                        TimeOut = "",
                        Status = "Plan",
                        EmployeeID = emp.EmployeeID,
                        EmployeeName = emp.EmployeeName,
                        DepartmentName = emp.DepartmentName
                    };
                    testRandomCreateList.Add(randomInsert);
                }
            }
            dgRandomList.ItemsSource = testRandomCreateList;
            dgRandomList.Items.Refresh();
            
            btnSave.IsEnabled = true;
        }


        private void hlViewWorkerList_Click(object sender, RoutedEventArgs e)
        {
            TestRandomViewEmployeeListWindow window = new TestRandomViewEmployeeListWindow(employeeOriginList);
            window.Show();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dgRandomList.ItemsSource != null)
            {
                if (bwInsert.IsBusy==false)
                {
                    var insertList = dgRandomList.ItemsSource.OfType<TestRandomModel>().ToList();
                    this.Cursor = Cursors.Wait;
                    btnSave.IsEnabled = false;
                    bwInsert.RunWorkerAsync(insertList);
                }
            }
        }
        private void BwInsert_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            if ((bool)e.Result)
            {
                MessageBox.Show("Saved !", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BwInsert_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = true;
                var source = e.Argument as List<TestRandomModel>;
                foreach (var item in source)
                {
                    TestRandomController.Insert(item);
                    testRandomList.Add(item);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        dgRandomList.SelectedItem = item;
                        dgRandomList.ScrollIntoView(item);
                    }));
                }
            }
            catch (Exception ex)
            {
                e.Result = false
                    ;
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message.ToString());
                }));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dpTestDate.SelectedDate = DateTime.Now; 
            if (bwLoad.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
        }
        private void dgRandomList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void hlTestPlanlist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                testRandomList = TestRandomController.GetAll();
                foreach (var item in testRandomList)
                {
                    var empByCode = employeeOriginList.FirstOrDefault(f => f.EmployeeCode == item.EmployeeCode);
                    if (empByCode != null)
                    {
                        item.EmployeeID = empByCode.EmployeeID;
                        item.EmployeeName = empByCode.EmployeeName;
                        item.DepartmentName = empByCode.DepartmentName;
                    }
                }
                testRandomList = testRandomList.OrderByDescending(o => o.TestDate).ThenBy(th => th.DepartmentName).ThenBy(th => th.EmployeeID).ToList();
                TestCovidCovidTestPlanWindow window = new TestCovidCovidTestPlanWindow(testRandomList);
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
