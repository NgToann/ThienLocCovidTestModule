using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media;

using TLCovidTest.Controllers;
using TLCovidTest.Helpers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for DailyReportWindow.xaml
    /// </summary>
    public partial class DailyReportWindow : Window
    {
        BackgroundWorker bwLoad;
        BackgroundWorker bwFilter;
        BackgroundWorker bwCreateChart;
        FilterMode filterMode = new FilterMode();

        List<DepartmentModel> departmentList;
        List<EmployeeModel> employeeList;
        List<EmployeeModel> employeeByDepartmentList;
        List<WorkerRemarkModel> workerRemarkList;

        List<DailyReportModel> dailyReportFilterList;
        //List<EmployeeModel> employeeListFromSourceByDate;

        List<SourceModel> sourceList;
        DateTime dateFilter;
        DateTime dateFilterTo;
        private string _SAOVIET = "SaoViet Corporation";
        string groupHeaderByComboboxSelect = "";
        string groupHeader = "";

        public DailyReportWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwFilter = new BackgroundWorker();
            bwFilter.DoWork += new DoWorkEventHandler(bwFilter_DoWork);
            bwFilter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwFilter_RunWorkerCompleted);

            bwCreateChart = new BackgroundWorker();
            bwCreateChart.DoWork += BwCreateChart_DoWork;
            bwCreateChart.RunWorkerCompleted += BwCreateChart_RunWorkerCompleted;
            filterMode = FilterMode.All;

            employeeList = new List<EmployeeModel>();
            //employeeListFromSourceByDate = new List<EmployeeModel>();
            employeeByDepartmentList = new List<EmployeeModel>();
            departmentList = new List<DepartmentModel>();
            dailyReportFilterList = new List<DailyReportModel>();

            workerRemarkList = new List<WorkerRemarkModel>();
            sourceList = new List<SourceModel>();
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _qtyLabel = LanguageHelper.GetStringFromResource("dailyAttendanceQuantity");
            _attendanceLabel= LanguageHelper.GetStringFromResource("dailyAttendanceAttendance");
            _absentLabel = LanguageHelper.GetStringFromResource("dailyAttendanceAbsent");

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
                departmentList = DepartmentController.GetDepartments();
                employeeList = EmployeeController.GetAvailable();
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
            var sectionListFull = new List<DepartmentModel>();
            sectionListFull.Add(new DepartmentModel { DepartmentName = _SAOVIET});
            var sectionList = departmentList.Where(w => String.IsNullOrEmpty(w.ParentID) == true).ToList();
            sectionListFull.AddRange(sectionList);
            if (sectionList.Count() > 0)
            {
                cboSection.ItemsSource = sectionListFull;
                cboSection.SelectedItem = sectionListFull.FirstOrDefault();
            }
            dpFilterDate.SelectedDate = DateTime.Now.Date;
            dpFilterDateTo.SelectedDate = DateTime.Now.Date;
            this.Cursor = null;
        }

        string employeeSearchWhat = "";
        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            dateFilter = dpFilterDate.SelectedDate.Value;
            dateFilterTo = dpFilterDateTo.SelectedDate.Value;
            if (bwFilter.IsBusy == false)
            {
                employeeSearchWhat = txtEmployeeSearch.Text.Trim().ToUpper().ToString();
                this.Cursor = Cursors.Wait;
                btnPreview.IsEnabled = false;
                btnPreview.IsDefault = false;
                bwFilter.RunWorkerAsync();
            }
        }
        private void bwFilter_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //employeeListFromSourceByDate = EmployeeController.GetFromSourceByDate(dateFilter);
                sourceList = SourceController.SelectSourceByDateFromTo(dateFilter, dateFilterTo);
                //workerRemarkList = WorkerRemarksController.GetFromTo(dateFilter, dateFilterTo);
                workerRemarkList = WorkerRemarksController.GetAll();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0}", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }));
            }

            dailyReportFilterList = new List<DailyReportModel>();
            groupHeader = groupHeaderByComboboxSelect;

            var employeeSearchList = employeeByDepartmentList.ToList();
            if (!string.IsNullOrEmpty(employeeSearchWhat))
            {
                employeeSearchList = employeeList.Where(w => w.EmployeeCode == employeeSearchWhat || w.EmployeeID == employeeSearchWhat).ToList();
                groupHeader = employeeSearchWhat;
            }
            var sourceListByEmployeeList = new List<SourceModel>();
            sourceListByEmployeeList = sourceList.Where(w => employeeSearchList.Select(s => s.EmployeeCode).Contains(w.EmployeeCode)).ToList();

            int dateIndex = 1;
            var totalDay = (dateFilterTo - dateFilter).TotalDays + 1;
            for (var date = dateFilter.Date; date <= dateFilterTo.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday && checkSunday == false)
                    continue;
                int employeeIndex = 1;
                foreach (var employee in employeeSearchList)
                {
                    if (employee.JoinDate.Date <= date.Date)
                    {
                        var remark = workerRemarkList.Where(w => w.EmployeeID == employee.EmployeeID).OrderBy(o => o.Date).LastOrDefault();

                        var reportModel = new DailyReportModel();
                        reportModel.DateSearch = date;
                        reportModel.EmployeeName = employee.EmployeeName;
                        reportModel.EmployeeID = employee.EmployeeID;
                        reportModel.DepartmentName = employee.DepartmentName;
                        reportModel.TimeIn = "";
                        reportModel.TimeInView = "";
                        reportModel.Remarks = remark != null ? remark.Remarks : "";
                        //var reportHasTimeIn = sourceListByEmployee.Where(w => w.EmployeeCode == employee.EmployeeCode).ToList();
                        var reportHasTimeIn = sourceListByEmployeeList.Where(w => w.EmployeeCode == employee.EmployeeCode && w.SourceDate == date).ToList();
                        if (reportHasTimeIn.Count() > 0)
                        {
                            reportModel.Remarks = "";
                            reportModel.TimeInView = reportHasTimeIn.OrderBy(o => o.SourceTime).FirstOrDefault().SourceTimeView;
                            reportModel.TimeIn = reportHasTimeIn.OrderBy(o => o.SourceTime).FirstOrDefault().SourceTime;
                            if (reportHasTimeIn.Count() > 1)
                            {
                                reportModel.TimeOutView = reportHasTimeIn.OrderBy(o => o.SourceTime).LastOrDefault().SourceTimeView;
                                reportModel.TimeOut = reportHasTimeIn.OrderBy(o => o.SourceTime).LastOrDefault().SourceTime;
                            }
                        }

                        dailyReportFilterList.Add(reportModel);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            //tblStatus.Text = String.Format("Creating {0} / {1} date {2} / {3} employee", dateIndex, totalDay, employeeIndex, employeeSearchList.Count());
                        }));
                        employeeIndex++;
                    }
                }
                dateIndex++;
            }
        }
        private void bwFilter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Bind to Group Summary
            txtGroupSummaryHeader.Text = "";
            txtQuantity.Text = "";
            txtAttendance.Text = "";
            txtAbsent.Text = "";

            string dateView = String.Format("{0:dd/MM/yyyy}", dateFilter);
            if (dateFilter < dateFilterTo)
                dateView = String.Format(" {0:dd/MM/yyyy} -> {1:dd/MM/yyyy}", dateFilter, dateFilterTo);
            txtGroupSummaryHeader.Text = groupHeader + ": " + dateView;

            txtQuantity.Text = dailyReportFilterList.Count().ToString();
            txtAttendance.Text = dailyReportFilterList.Where(w => String.IsNullOrEmpty(w.TimeIn) == false).ToList().Count().ToString();
            txtAbsent.Text = dailyReportFilterList.Where(w => String.IsNullOrEmpty(w.TimeIn) == true).ToList().Count().ToString();

            if (filterMode == FilterMode.Attendance)
            {
                dailyReportFilterList = dailyReportFilterList.Where(w => String.IsNullOrEmpty(w.TimeIn) == false).ToList();
            }
            if (filterMode == FilterMode.Absent)
            {
                dailyReportFilterList = dailyReportFilterList.Where(w => String.IsNullOrEmpty(w.TimeIn) == true).ToList();
            }

            string timeInFromString = txtTimeInFrom.Text.ToString();
            string timeInToString = txtTimeInTo.Text.ToString();

            string timeOutFromString = txtTimeOutFrom.Text.ToString();
            string timeOutToString = txtTimeOutTo.Text.ToString();

            if (string.IsNullOrEmpty(timeInFromString) == false)
            {
                //timeInFromString = "";
                dailyReportFilterList = dailyReportFilterList.Where(w => String.Compare(timeInFromString, w.TimeIn, true) <= 0).ToList();
            }
            if (string.IsNullOrEmpty(timeInToString) == false)
            {
                //timeInToString = "2400";
                dailyReportFilterList = dailyReportFilterList.Where(w => String.Compare(w.TimeIn, timeInToString, true) <= 0).ToList();
            }
            if (string.IsNullOrEmpty(timeOutFromString) == false)
            {
                //timeOutFromString = "";
                dailyReportFilterList = dailyReportFilterList.Where(w => String.Compare(timeOutFromString, w.TimeOut, true) <= 0).ToList();
            }
            if (string.IsNullOrEmpty(timeOutToString) == false)
            {
                //timeOutToString = "2400";
                dailyReportFilterList = dailyReportFilterList.Where(w => String.Compare(w.TimeOut, timeOutToString, true) <= 0).ToList();
            }

            //dailyReportList = dailyReportList.Where(w => String.Compare(timeInFromString, w.TimeIn, true) <= 0 &&
            //                                             String.Compare(w.TimeIn, timeInToString, true) <= 0 &&
            //                                             String.Compare(timeOutFromString, w.TimeOut, true) <= 0 &&
            //                                             String.Compare(w.TimeOut, timeOutToString, true) <= 0)
            //                                 .ToList();

            //dailyReportList = dailyReportList.Where(w => (String.Compare(timeInFromString, w.TimeIn, true) <= 0 &&
            //                                             String.Compare(w.TimeIn, timeInToString, true) <= 0 )&&
            //                                             (String.Compare(timeOutFromString, w.TimeOut, true) <= 0 &&
            //                                             String.Compare(w.TimeOut, timeOutToString, true) <= 0))
            //                                 .ToList();

            //if (string.IsNullOrEmpty(timeInFromString) == false && string.IsNullOrEmpty(timeInToString) == false)
            //{
            //    dailyReportList = dailyReportList.Where(w => string.IsNullOrEmpty(w.TimeIn) == false).ToList();
            //    dailyReportList = dailyReportList.Where(w => String.Compare(timeInFromString, w.TimeIn, true) <= 0 && String.Compare(w.TimeIn, timeInToString, true) <= 0).ToList();
            //}
            //if (string.IsNullOrEmpty(timeOutFromString) == false && string.IsNullOrEmpty(timeOutToString) == false)
            //{
            //    dailyReportList = dailyReportList.Where(w => string.IsNullOrEmpty(w.TimeOut) == false).ToList();
            //    dailyReportList = dailyReportList.Where(w => String.Compare(timeOutFromString, w.TimeOut, true) <= 0 && String.Compare(w.TimeOut, timeOutToString, true) <= 0).ToList();
            //}
            if (dailyReportFilterList.Count() > 0)
                dailyReportFilterList = dailyReportFilterList.OrderBy(o => o.DateSearch).ThenBy(t => t.DepartmentName).ThenBy(th => th.EmployeeID).ToList();
            dgResult.ItemsSource = dailyReportFilterList;
            btnPreview.IsEnabled = true;
            this.Cursor = null;

            if (bwCreateChart.IsBusy == false) // && dateFilter == dateFilterTo
            {
                this.Cursor = Cursors.Wait;
                bwCreateChart.RunWorkerAsync();
            }
        }
        
        string _qtyLabel = "", _attendanceLabel = "", _absentLabel = "";
        private void BwCreateChart_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                //btnShowChart.Content = String.Format("Chart : {0:dd/MM/yyyy}", dateFilter);
                chartTitle.Text = String.Format("Chart : {0:dd/MM/yyyy}", dateFilter);
                List<DailyReportModel> dailyReportList = new List<DailyReportModel>();
                var sourceListCreateChart = sourceList.Where(w => w.SourceDate.Date == dateFilter.Date).ToList();
                foreach (var employee in employeeList)
                {
                    var reportModel             = new DailyReportModel();
                    reportModel.EmployeeName    = employee.EmployeeName;
                    reportModel.EmployeeID      = employee.EmployeeID;
                    reportModel.DepartmentName  = employee.DepartmentName;
                    reportModel.TimeIn          = "";
                    reportModel.TimeInView      = "";
                    var reportHasTimeIn = sourceListCreateChart.Where(w => w.EmployeeCode == employee.EmployeeCode).ToList();
                    if (reportHasTimeIn.Count() > 0)
                    {
                        reportModel.TimeInView      = reportHasTimeIn.OrderBy(o => o.SourceTime).FirstOrDefault().SourceTimeView;
                        reportModel.TimeIn          = reportHasTimeIn.OrderBy(o => o.SourceTime).FirstOrDefault().SourceTime;
                        if (reportHasTimeIn.Count() > 1)
                        {
                            reportModel.TimeOutView = reportHasTimeIn.OrderBy(o => o.SourceTime).LastOrDefault().SourceTimeView;
                            reportModel.TimeOut     = reportHasTimeIn.OrderBy(o => o.SourceTime).LastOrDefault().SourceTime;
                        }
                    }
                    dailyReportList.Add(reportModel);
                }

                stkChart.Children.Clear();
                var sectionSelected = cboSection.SelectedItem as DepartmentModel;
                var departmentListBySection = new List<DepartmentModel>();
                if (sectionSelected.DepartmentName == _SAOVIET)
                {
                    // Section list
                    var sectionList = cboSection.ItemsSource.OfType<DepartmentModel>().Where(w => w.DepartmentName != _SAOVIET).ToList();
                    int indexColor = 0;
                    foreach (var section in sectionList)
                    {
                        var departmentsBySection = departmentList.Where(w => w.ParentID == section.DepartmentID).Select(s => s.DepartmentFullName.Trim().ToUpper()).ToList();
                        var stackE = new StackPanel();
                        stackE.Orientation = Orientation.Horizontal;
                        var tblLineName = new TextBlock
                        {
                            Text = section.DepartmentName,
                            TextWrapping = TextWrapping.Wrap,
                            Width = 138
                        };
                        if (indexColor % 2 == 0)
                            tblLineName.Foreground = Brushes.Blue;

                        stackE.Children.Add(tblLineName);
                        int attended = 0, absent = 0;
                        if (departmentsBySection.Count() == 0)
                        {
                            attended    = dailyReportList.Where(w => w.DepartmentName.Trim().ToUpper() == section.DepartmentFullName.Trim().ToUpper() && String.IsNullOrEmpty(w.TimeIn) == false).ToList().Count;
                            absent      = dailyReportList.Where(w => w.DepartmentName.Trim().ToUpper() == section.DepartmentFullName.Trim().ToUpper() && String.IsNullOrEmpty(w.TimeIn) == true).ToList().Count;
                        }
                        else
                        {
                            attended    = dailyReportList.Where(w => departmentsBySection.Contains(w.DepartmentName.Trim().ToUpper()) && String.IsNullOrEmpty(w.TimeIn) == false).ToList().Count;
                            absent      = dailyReportList.Where(w => departmentsBySection.Contains(w.DepartmentName.Trim().ToUpper()) && String.IsNullOrEmpty(w.TimeIn) == true).ToList().Count;
                        }
                        var tblAttended = new TextBlock
                        {
                            Text = attended.ToString(),
                            Width = attended / 1,
                            Background = Brushes.Green,
                            Height = 20,
                            TextAlignment = TextAlignment.Center
                        };
                        stackE.Children.Add(tblAttended);

                        var tblAbsent = new TextBlock
                        {
                            Text = absent.ToString(),
                            Width = absent / 1,
                            Background = Brushes.Red,
                            Height = 20,
                            FontSize = 10,
                            TextAlignment = TextAlignment.Center,
                            Padding = new Thickness (0,2,0,0)
                        };

                        if (attended != 0 || absent != 0)
                        {
                            stackE.Children.Add(tblAbsent);
                            stackE.ToolTip = String.Format("{0}\n{1}: {2}\n{3}: {4}\n{5}: {6}", section.DepartmentName , _qtyLabel, attended + absent, _attendanceLabel, attended, _absentLabel, absent);
                            stackE.Margin = new Thickness(0, 5, 0, 0);
                            stkChart.Children.Add(stackE);
                            indexColor++;
                        }
                    }
                }
                else
                {
                    var lineList = new List<DepartmentModel>();
                    lineList = departmentList.Where(w => w.ParentID == sectionSelected.DepartmentID).ToList();
                    if (lineList.Count() == 0)
                        lineList.Add(sectionSelected);
                    int indexColor = 0;

                    foreach (var line in lineList)
                    {
                        var stackE = new StackPanel();
                        stackE.Orientation = Orientation.Horizontal;
                        var tblLineName = new TextBlock
                        {
                            Text = line.DepartmentFullName,
                            TextWrapping = TextWrapping.Wrap,
                            Width = 138
                        };

                        if (indexColor % 2 == 0)
                            tblLineName.Foreground = Brushes.Blue;
                        stackE.Children.Add(tblLineName);
                        int attended = 0, absent = 0;

                        attended = dailyReportList.Where(w => w.DepartmentName.Trim().ToUpper() == line.DepartmentFullName.Trim().ToUpper() && String.IsNullOrEmpty(w.TimeIn) == false).ToList().Count;
                        absent = dailyReportList.Where(w => w.DepartmentName.Trim().ToUpper() == line.DepartmentFullName.Trim().ToUpper() && String.IsNullOrEmpty(w.TimeIn) == true).ToList().Count;

                        var tblAttended = new TextBlock
                        {
                            Text = attended.ToString(),
                            Width = attended * 5,
                            Background = Brushes.Green,
                            Height = 20,
                            TextAlignment = TextAlignment.Center
                        };
                        stackE.Children.Add(tblAttended);

                        var tblAbsent = new TextBlock
                        {
                            Text = absent.ToString(),
                            Width = absent * 6,
                            Background = Brushes.Red,
                            Height = 20,
                            FontSize = 10,
                            TextAlignment = TextAlignment.Center,
                            Padding = new Thickness(0, 2, 0, 0)
                        };

                        if (attended != 0 || absent != 0)
                        {
                            stackE.Children.Add(tblAbsent);
                            stackE.ToolTip = String.Format("{0}\n{1}: {2}\n{3}: {4}\n{5}: {6}", line.DepartmentFullName, _qtyLabel, attended + absent, _attendanceLabel, attended, _absentLabel, absent);
                            stackE.Margin = new Thickness(0, 5, 0, 0);
                            stkChart.Children.Add(stackE);
                            indexColor++;
                        }
                    }
                }
            }));
        }
        private void BwCreateChart_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
        }
        private void radAttendance_Checked(object sender, RoutedEventArgs e)
        {
            filterMode = FilterMode.Attendance;
        }
        private void radAbsent_Checked(object sender, RoutedEventArgs e)
        {
            filterMode = FilterMode.Absent;
        }
        private void radAll_Checked(object sender, RoutedEventArgs e)
        {
            filterMode = FilterMode.All;
        }
        private enum FilterMode
        {
            All,
            Attendance,
            Absent
        }
        private void cboSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sectionClicked = cboSection.SelectedItem as DepartmentModel;
            cboDepartment.ItemsSource = new List<DepartmentModel>();
            employeeByDepartmentList = new List<EmployeeModel>();
            if (sectionClicked != null)
            {
                groupHeaderByComboboxSelect = sectionClicked.DepartmentName;
                cboDepartment.ItemsSource = new List<DepartmentModel>();
                if (sectionClicked.DepartmentName == _SAOVIET)
                {
                    employeeByDepartmentList = employeeList.ToList();
                    groupHeaderByComboboxSelect = sectionClicked.DepartmentName;
                }
                else
                {
                    var childDept = departmentList.Where(w => w.ParentID == sectionClicked.DepartmentID).ToList();
                    if (childDept.Count() > 1)
                    {
                        var employeeByChildDept = new List<EmployeeModel>();
                        foreach (var child in childDept)
                        {
                            employeeByChildDept = employeeList.Where(w => w.DepartmentName.ToUpper().Trim().ToString() == child.DepartmentFullName.ToUpper().Trim().ToString()).ToList();
                            employeeByDepartmentList.AddRange(employeeByChildDept);
                        }
                        //groupHeader = departmentList.Where(w => w.DepartmentID == childDept.FirstOrDefault().DepartmentID).FirstOrDefault().DepartmentName;
                        cboDepartment.ItemsSource = childDept;
                        cboDepartment.Items.Refresh();
                    }
                    else
                    {
                        employeeByDepartmentList = employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == sectionClicked.DepartmentName).ToList();
                    }
                }
            }
        }
        private void cboDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            employeeByDepartmentList = new List<EmployeeModel>();
            var departmentClicked = cboDepartment.SelectedItem as DepartmentModel;
            if (departmentClicked != null)
            {
                employeeByDepartmentList = employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == departmentClicked.DepartmentFullName.Trim().ToUpper().ToString()).ToList();
                groupHeaderByComboboxSelect = departmentClicked.DepartmentFullName;
            }
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            if (dgResult.ItemsSource != null)
            {
                var dailyReportList = dgResult.ItemsSource.OfType<DailyReportModel>().ToList();
                DailyReportReportWindow window = new DailyReportReportWindow(dailyReportList, dateFilter, dateFilterTo);
                window.Show();
            }
        }

        private void DgResult_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private bool checkSunday = false;
        private void chkSunday_Checked(object sender, RoutedEventArgs e)
        {
            checkSunday = true;
        }

        private void chkSunday_Unchecked(object sender, RoutedEventArgs e)
        {
            checkSunday = false;
        }

        private void btnShowChart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                groupChart.Visibility = Visibility.Visible;
                groupChart.Width = 450.0;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                groupChart.Visibility = Visibility.Collapsed;
            }
        }

        private void txtEmployeeSearch_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            btnPreview.IsDefault = true;
        }

        private void txtEmployeeSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnPreview.IsDefault = true;
        }
    }
}
