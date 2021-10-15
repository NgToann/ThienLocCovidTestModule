using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;
using EXCEL = Microsoft.Office.Interop.Excel;
namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for CheckingScanRecordWindow.xaml
    /// </summary>
    public partial class AttendanceRecordDetailWindow : Window
    {
        BackgroundWorker bwLoad;
        BackgroundWorker bwLoadAttendance;
        BackgroundWorker bwExportExcel;

        List<DepartmentModel> departmentList;
        List<AttendanceRecordViewModel> attendanceRecordViewModelList;
        List<AttendanceRecordModel> attendanceDetailByEmployeeSearchList;
        public ObservableCollection<AttendanceRecordViewModel> viewModelList;
        List<EmployeeModel> employeeList;
        List<EmployeeModel> employeeSelectedList;
        List<EmployeeModel> employeeAvailableList;
        DateTime dateSearchFrom;
        DateTime dateSearchTo;
        string searchTitle = "";
        List<string> remarkList;
        private string _SaovietCorporation = "Saoviet Corporation";
        List<WorkingShiftModel> workingShiftList;
        private string miHeaderSetOverTimeLimit = "", miHeaderSetTimeInTimeOut = "", miHeaderSetLeaveDay = "";
        private bool addPressed = true;
        public AttendanceRecordDetailWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwLoadAttendance = new BackgroundWorker();
            bwLoadAttendance.DoWork +=new DoWorkEventHandler(bwLoadAttendance_DoWork);
            bwLoadAttendance.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bwLoadAttendance_RunWorkerCompleted);

            bwExportExcel = new BackgroundWorker();
            bwExportExcel.DoWork += new DoWorkEventHandler(bwExportExcel_DoWork);
            bwExportExcel.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwExportExcel_RunWorkerCompleted);
            bwExportExcel.WorkerSupportsCancellation = true;

            departmentList = new List<DepartmentModel>();

            attendanceDetailByEmployeeSearchList = new List<AttendanceRecordModel>();
            attendanceRecordViewModelList = new List<AttendanceRecordViewModel>();
            viewModelList = new ObservableCollection<AttendanceRecordViewModel>();

            employeeList = new List<EmployeeModel>();
            employeeSelectedList = new List<EmployeeModel>();
            employeeAvailableList = new List<EmployeeModel>();

            workingShiftList = new List<WorkingShiftModel>();

            dateSearchFrom = new DateTime(2000, 1, 1);
            dateSearchTo = new DateTime(2000, 1, 1);

            remarkList = new List<string>();
            remarkList.Add("");

            miHeaderSetOverTimeLimit    = LanguageHelper.GetStringFromResource("checkRecordingTimeMiSetOverTimeLimit");
            miHeaderSetTimeInTimeOut    = LanguageHelper.GetStringFromResource("checkRecordingTimeMiSetTimeInTimeOut");
            miHeaderSetLeaveDay         = LanguageHelper.GetStringFromResource("checkRecordingTimeMiSetLeaveDay");
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
                employeeList            = EmployeeController.GetAll();
                employeeAvailableList   = EmployeeController.GetAvailable();
                departmentList          = DepartmentController.GetDepartments();
                workingShiftList        = WorkingShiftController.GetAll();
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
            // set datetimedefault
            dpTo.SelectedDate = DateTime.Now.Date;
            dpFrom.SelectedDate = DateTime.Now.Date;
            var deptParentList = new List<DepartmentModel>();
            deptParentList.Add(new DepartmentModel { DepartmentName = _SaovietCorporation, DepartmentFullName = _SaovietCorporation });
            deptParentList.AddRange(departmentList.Where(w => string.IsNullOrEmpty(w.ParentID) == true).ToList());
            //var deptParentList = departmentList.Where(w => string.IsNullOrEmpty(w.ParentID) == true).ToList();
            foreach (var departParent in deptParentList)
            {
                TreeViewItem tviParent = new TreeViewItem();
                var departmentsChild = departmentList.Where(w => w.ParentID == departParent.DepartmentID).ToList();
                foreach (var child in departmentsChild)
                {
                    TreeViewItem tviChild = new TreeViewItem();
                    tviChild.Header = child.DepartmentName;
                    tviChild.Margin = new Thickness(0, 2, 0, 2);
                    tviChild.FontWeight = FontWeights.Normal;
                    tviChild.Foreground = Brushes.Blue;
                    tviChild.Tag = child;

                    tviChild.MouseDoubleClick += new MouseButtonEventHandler(tvi_MouseDoubleClick);
                    tviParent.Items.Add(tviChild);
                }

                tviParent.Header = departParent.DepartmentName;
                if (departmentsChild.Count() > 0)
                    tviParent.Header = string.Format("{0} ({1})", departParent.DepartmentName, departmentsChild.Count);

                tviParent.FontWeight = FontWeights.Bold;
                tviParent.Margin = new Thickness(0, 2, 0, 2);
                tviParent.Foreground = Brushes.Black;

                tviParent.Tag = departParent;
                tviParent.MouseDoubleClick += new MouseButtonEventHandler(tvi_MouseDoubleClick);
                treeDepartments.Items.Add(tviParent);
            }
            this.Cursor = null;
        }

        // Get List Employee From TreeView.
        private void tvi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dateSearchFrom = dpFrom.SelectedDate.Value;
            dateSearchTo = dpTo.SelectedDate.Value;
            
            var itemClicked = sender as TreeViewItem;
            var departmentClicked = itemClicked.Tag as DepartmentModel;

            if (departmentClicked != null && bwLoadAttendance.IsBusy == false)
            {
                // Check Is Parent Dept
                var departmentListClicked = new List<DepartmentModel>();
                employeeSelectedList = new List<EmployeeModel>();
                searchTitle = "";
                var childDeptList = departmentList.Where(w => w.ParentID == departmentClicked.DepartmentID).ToList();
                if (childDeptList.Count() > 0)
                {
                    string parentID = "parentID";
                    foreach (var child in childDeptList)
                    {
                        parentID = child.ParentID;
                        departmentListClicked.Add(child);
                        employeeSelectedList.AddRange(employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == child.DepartmentFullName.Trim().ToUpper().ToString()));
                    }
                    var parentName = departmentList.Where(w => w.DepartmentID == parentID).FirstOrDefault();
                    if (parentName != null)
                        searchTitle = string.Format("{0} : {1}", LanguageHelper.GetStringFromResource("commonEmployeeSection"), parentName.DepartmentName);
                }
                else
                {
                    if (departmentClicked.DepartmentName == _SaovietCorporation)
                    {
                        employeeSelectedList = employeeAvailableList.ToList();
                        searchTitle = string.Format("{0}", departmentClicked.DepartmentFullName);
                    }
                    else
                    {
                        departmentListClicked.Add(departmentClicked);
                        employeeSelectedList.AddRange(employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == departmentClicked.DepartmentFullName.Trim().ToUpper().ToString()));
                        searchTitle = string.Format("{0} : {1}", LanguageHelper.GetStringFromResource("commonEmployeeDepartment"), departmentClicked.DepartmentFullName);
                    }
                }

                treeDepartments.IsEnabled = false;
                bwLoadAttendance.RunWorkerAsync(employeeSelectedList);
                this.Cursor = Cursors.Wait;
            }
        }
        // Get Employee From TextBox.

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            addPressed = false;
            dateSearchFrom  = dpFrom.SelectedDate.Value;
            dateSearchTo    = dpTo.SelectedDate.Value;
            searchTitle = "";
            txtGriNumber.Text = "";
            string employeeIDCode = txtEmployeeIDSearch.Text.Trim().ToUpper().ToString();
            var employeeSearched = employeeList.Where(w => w.EmployeeID.ToUpper().Trim().ToString() == employeeIDCode || w.EmployeeCode.ToUpper().Trim().ToString() == employeeIDCode).FirstOrDefault();
            employeeSelectedList = new List<EmployeeModel>();
            if (employeeSearched != null && bwLoadAttendance.IsBusy == false)
            {
                employeeSelectedList.Add(employeeSearched);
                searchTitle = string.Format("{0} : {1}", LanguageHelper.GetStringFromResource("commonEmployee"), employeeSearched.EmployeeName);
                btnSearch.IsEnabled = false;
                this.Cursor = null;
                bwLoadAttendance.RunWorkerAsync(employeeSelectedList);
            }
            else
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageNotFound")), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                txtTitle.Text = "";
                dgAttendanceRecordResult.ItemsSource = new List<AttendanceRecordViewModel>();
                txtEmployeeIDSearch.Focus();
                txtEmployeeIDSearch.SelectAll();
            }
        }
        private void btnAddSearch_Click(object sender, RoutedEventArgs e)
        {
            addPressed = true;
            dateSearchFrom = dpFrom.SelectedDate.Value;
            dateSearchTo = dpTo.SelectedDate.Value;
            searchTitle = "";
            txtGriNumber.Text = "";
            string employeeIDCode = txtEmployeeIDSearch.Text.Trim().ToUpper().ToString();
            var employeeSearched = employeeList.Where(w => w.EmployeeID.ToUpper().Trim().ToString() == employeeIDCode || w.EmployeeCode.ToUpper().Trim().ToString() == employeeIDCode).FirstOrDefault();
            var checkExist = employeeSelectedList.FirstOrDefault(f => f.EmployeeID.ToUpper().Trim().ToString() == employeeIDCode || f.EmployeeCode.ToUpper().Trim().ToString() == employeeIDCode);
            
            if (checkExist != null)
            {
                txtTitle.Text = "";
                txtEmployeeIDSearch.Focus();
                txtEmployeeIDSearch.SelectAll();
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageDataExist")), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (employeeSearched != null && bwLoadAttendance.IsBusy == false)
            {
                employeeSelectedList.Add(employeeSearched);
                searchTitle = string.Format("{0} Worker{1}", employeeSelectedList.Count(), employeeSelectedList.Count() > 1 ? "s" : "");
                btnAddSearch.IsEnabled = false;
                this.Cursor = null;
                bwLoadAttendance.RunWorkerAsync(employeeSelectedList);
            }
            else
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageNotFound")), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                txtTitle.Text = "";
                txtEmployeeIDSearch.Focus();
                txtEmployeeIDSearch.SelectAll();
            }
        }

        private void bwLoadAttendance_DoWork(object sender, DoWorkEventArgs e)
        {
            var employeeSelectedList = e.Argument as List<EmployeeModel>;
            if (employeeSelectedList.Count() > 0)
                employeeSelectedList = employeeSelectedList.OrderBy(o => o.EmployeeID).ToList();
            attendanceDetailByEmployeeSearchList = new List<AttendanceRecordModel>();
            foreach (var employee in employeeSelectedList)
            {
                attendanceDetailByEmployeeSearchList.AddRange(AttendanceRecordController.GetAttendanceRecordByEmployeeCodeFromTo(employee.EmployeeCode, dateSearchFrom, dateSearchTo));
            }
        }

        private void bwLoadAttendance_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Convert to Viewmodel List;
            var attendanceViewModelList = new List<AttendanceRecordViewModel>();
            foreach ( var attendanceRecord in attendanceDetailByEmployeeSearchList)
            {
                var attendanceViewModel = new AttendanceRecordViewModel();
                ConvertToViewModel(attendanceRecord, attendanceViewModel);
                attendanceViewModelList.Add(attendanceViewModel);
            }
            // Show Data to Gridview
            viewModelList = new ObservableCollection<AttendanceRecordViewModel>(attendanceViewModelList);
            dgAttendanceRecordResult.ItemsSource = viewModelList;

            // Create MenuContext
            if (viewModelList.Count() > 0)
            {
                var contextMenu = new ContextMenu();
                
                var miSetOverTimeLimit = new MenuItem();
                miSetOverTimeLimit.Header = miHeaderSetOverTimeLimit;
                miSetOverTimeLimit.Click += MiSetOverTimeLimit_Click;

                var miSetTimeInTimeOut = new MenuItem();
                miSetTimeInTimeOut.Header = miHeaderSetTimeInTimeOut;
                miSetTimeInTimeOut.Click += MiSetTimeInTimeOut_Click;

                var miSetLeaveDay = new MenuItem();
                miSetLeaveDay.Header = miHeaderSetLeaveDay;
                miSetLeaveDay.Click += MiSetLeaveDay_Click;

                contextMenu.Items.Add(miSetOverTimeLimit);
                contextMenu.Items.Add(miSetTimeInTimeOut);
                contextMenu.Items.Add(miSetLeaveDay);

                dgAttendanceRecordResult.ContextMenu = contextMenu;
            }

            

            // Binding to cboRemark
            var remarkSearchedList = viewModelList.Select(s => s.Remarks).Distinct().ToList();
            remarkList.AddRange(remarkSearchedList);
            cboRemark.ItemsSource = remarkList.Distinct();
            cboRemark.SelectedItem = remarkList.FirstOrDefault();
            
            txtTitle.Text = searchTitle;
            if (viewModelList.Count() == 0)
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageNotFound")), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            
            if (viewModelList.Select(s => s.EmployeeCode).Distinct().Count() > 0)
                txtGriNumber.Text = string.Format("{0}: {1}", LanguageHelper.GetStringFromResource("commonTotalEmployee"), viewModelList.Select(s => s.EmployeeCode).Distinct().Count());
            

            this.Cursor = null;
            btnSearch.IsEnabled = true;
            btnAddSearch.IsEnabled = true;
            txtEmployeeIDSearch.SelectAll();
            txtEmployeeIDSearch.Focus();
            treeDepartments.IsEnabled = true;
        }

        private void MiSetLeaveDay_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttendanceRecordResult.ItemsSource == null)
                return;
            var itemsSelectedList = dgAttendanceRecordResult.SelectedItems.OfType<AttendanceRecordViewModel>().ToList();
            if (itemsSelectedList.Count() == 0)
                return;

            EditLeaveDayWindow window = new EditLeaveDayWindow(itemsSelectedList.FirstOrDefault(), employeeList, itemsSelectedList);
            window.ShowDialog();

            // Update UI
            UpdateUIManyRows(window.attdendanRecordProcessedList);
        }

        private void MiSetTimeInTimeOut_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttendanceRecordResult.ItemsSource == null)
                return;
            var itemsSelectedList = dgAttendanceRecordResult.SelectedItems.OfType<AttendanceRecordViewModel>().ToList();
            if (itemsSelectedList.Count() == 0)
                return;

            var attRecordProcessedList = new List<AttendanceRecordModel>();
            // 1 Row
            if (itemsSelectedList.Count() == 1)
            {
                EditRecordTimeWindow window = new EditRecordTimeWindow(itemsSelectedList.FirstOrDefault(), employeeList, null);
                window.ShowDialog();
                attRecordProcessedList = window.attendanceRecordProcessedList;
            }
            // n Rows
            else
            {
                EditRecordTimeWindow window = new EditRecordTimeWindow(null, employeeList, itemsSelectedList);
                window.ShowDialog();
                attRecordProcessedList = window.attendanceRecordProcessedList;
            }

            //Update UI
            UpdateUIManyRows(attRecordProcessedList);
        }

        private void MiSetOverTimeLimit_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttendanceRecordResult.ItemsSource == null)
                return;
            var itemsSelectedList = dgAttendanceRecordResult.SelectedItems.OfType<AttendanceRecordViewModel>().ToList();
            if (itemsSelectedList.Count() == 0)
                return;

            var attRecordProcessedList = new List<AttendanceRecordModel>();
            // 1 Row
            if (itemsSelectedList.Count == 1)
            {
                EditOverTimeLimitWindow window = new EditOverTimeLimitWindow(itemsSelectedList.FirstOrDefault(), employeeList, null);
                window.ShowDialog();
                attRecordProcessedList = window.attdendanRecordProcessedList;
            }
            // n Rows
            else
            {
                EditOverTimeLimitWindow window = new EditOverTimeLimitWindow(null, employeeList, itemsSelectedList);
                window.ShowDialog();
                attRecordProcessedList = window.attdendanRecordProcessedList;
            }

            //Update UI
            UpdateUIManyRows(attRecordProcessedList);
        }

        private void UpdateUIManyRows(List<AttendanceRecordModel> attRecordProcessedList)
        {
            foreach (var attRecordProcessed in attRecordProcessedList)
            {
                var viewModelNeedUpdate = viewModelList.FirstOrDefault(f => f.EmployeeCode == attRecordProcessed.EmployeeCode && f.AttendanceDate == attRecordProcessed.AttendanceDate);
                if (viewModelNeedUpdate != null)
                    ConvertToViewModel(attRecordProcessed, viewModelNeedUpdate);
            }
        }

        private void dgAttendanceResult_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DataGridCellInfo cellCurrent = dgAttendanceRecordResult.CurrentCell;
                // Edit Record Time
                if (cellCurrent != null && cellCurrent.Column != null && ( cellCurrent.Column == colTime1 ||
                                                                           cellCurrent.Column == colTime2 ))
                {
                    var cellClicked = (AttendanceRecordViewModel)cellCurrent.Item;
                    if (cellClicked != null)
                    {
                        EditRecordTimeWindow window = new EditRecordTimeWindow(cellClicked, employeeList, null);
                        window.ShowDialog();

                        // Update UI
                        if (window.attendanceRecordProcessedList.Count() > 0)
                            ConvertToViewModel(window.attendanceRecordProcessedList.FirstOrDefault(), cellClicked);
                    }
                }

                // Edit ShiftNo
                if (cellCurrent != null && cellCurrent.Column != null && (cellCurrent.Column == colShiftNo))
                {
                    var cellClicked = (AttendanceRecordViewModel)cellCurrent.Item;
                    if (cellClicked != null)
                    {
                        EditWorkingShiftWindow window = new EditWorkingShiftWindow(cellClicked, employeeList);
                        window.ShowDialog();

                        // Update UI
                        if (window.attdendanRecordProcessedList.Count() > 0)
                            ReloadAfterProcess(window.attdendanRecordProcessedList, viewModelList);
                    }
                }

                // Edit OverTime Limit
                if (cellCurrent != null && cellCurrent.Column != null && (cellCurrent.Column == colOverTime))
                {
                    var cellClicked = (AttendanceRecordViewModel)cellCurrent.Item;
                    if (cellClicked != null)
                    {
                        EditOverTimeLimitWindow window = new EditOverTimeLimitWindow(cellClicked, employeeList, null);
                        window.ShowDialog();

                        //Update UI
                        if (window.attdendanRecordProcessedList.Count() > 0)
                            ReloadAfterProcess(window.attdendanRecordProcessedList, viewModelList);
                    }
                }

                // Edit Leave Day
                if (cellCurrent != null && cellCurrent.Column != null && (cellCurrent.Column == col16))
                {
                    var cellClicked = (AttendanceRecordViewModel)cellCurrent.Item;
                    if (cellClicked != null)
                    {
                        var itemSelectedList = new List<AttendanceRecordViewModel>();
                        itemSelectedList.Add(cellClicked);
                        EditLeaveDayWindow window = new EditLeaveDayWindow(cellClicked, employeeList, itemSelectedList);
                        window.ShowDialog();

                        // Update UI
                        if (window.attdendanRecordProcessedList.Count() > 0)
                            ReloadAfterProcess(window.attdendanRecordProcessedList, viewModelList);
                        
                    }
                }
            }
            catch { return; }
        }

        private void ReloadAfterProcess(List<AttendanceRecordModel> recordAferProcessList, ObservableCollection<AttendanceRecordViewModel> viewModelList)
        {
            var employeeUpdate = recordAferProcessList.FirstOrDefault().EmployeeCode;
            var dateUpdateList = recordAferProcessList.Select(s => s.AttendanceDate);
            var attendanceNeedUpdateList = viewModelList.Where(w => w.EmployeeCode == employeeUpdate && dateUpdateList.Contains(w.AttendanceDate)).ToList();
            //ObservableCollection<AttendanceRecordViewModel> viewModelNeedUpdate = new ObservableCollection<AttendanceRecordViewModel>(attendanceNeedUpdateList);
            foreach (var attendanceUpdate in attendanceNeedUpdateList)
            {
                var source = recordAferProcessList.Where(w => w.EmployeeCode == attendanceUpdate.EmployeeCode && w.AttendanceDate == attendanceUpdate.AttendanceDate).FirstOrDefault();
                // Update Rows Processed
                if (source != null)
                    ConvertToViewModel(source, attendanceUpdate);
            }
        }

        private void ConvertToViewModel(AttendanceRecordModel model, AttendanceRecordViewModel modelConverted)
        {
            //var employeeX = employeeList.FirstOrDefault(f => f.EmployeeCode == model.EmployeeCode);
            var employee = employeeList.FirstOrDefault(f => f.EmployeeCode.Trim().ToUpper() == model.EmployeeCode.Trim().ToUpper());

            modelConverted.EmployeeCode             = model.EmployeeCode;
            modelConverted.EmployeeID               = employee != null ? employee.EmployeeID.ToUpper() : "";
            modelConverted.AttendanceDate           = model.AttendanceDate;
            modelConverted.ShiftNo                  = model.ShiftNo;

            modelConverted.AttendanceIn1            = model.AttendanceIn1;
            modelConverted.AttendanceDateIn1        = model.AttendanceDateIn1;

            modelConverted.AttendanceOut1           = model.AttendanceOut1;
            modelConverted.AttendanceDateOut1       = model.AttendanceDateOut1;

            modelConverted.AttendanceIn2            = model.AttendanceIn2;
            modelConverted.AttendanceDateIn2        = model.AttendanceDateIn2;

            modelConverted.AttendanceOut2           = model.AttendanceOut2;
            modelConverted.AttendanceDateOut2       = model.AttendanceDateOut2;

            modelConverted.AttendanceIn3            = model.AttendanceIn3;
            modelConverted.AttendanceDateIn3        = model.AttendanceDateIn3;

            modelConverted.AttendanceOut3           = model.AttendanceOut3;
            modelConverted.AttendanceDateOut3       = model.AttendanceDateOut3;

            modelConverted.WorkingDay               = model.WorkingDay;
            modelConverted.WorkingTime              = model.WorkingTime;
            modelConverted.WorkingOverTime          = model.WorkingOverTime;

            modelConverted.Remarks                  = model.Remarks;
            modelConverted.OverTimeIn               = model.OverTimeIn;
            modelConverted.OverTimeOut              = model.OverTimeOut;
            modelConverted.TimeLate                 = model.TimeLate;
            modelConverted.Absent                   = model.Absent;
            modelConverted.Ask                      = model.Ask;
            modelConverted.OverTime2                = model.OverTime2;
            modelConverted.DayOfWeek                = model.DayOfWeek;

            modelConverted.RowForeground = Brushes.Black;
            // timein, timeout from shift
            var workingShift = workingShiftList.Where(w => w.WorkingShiftCode == modelConverted.ShiftNo).FirstOrDefault();
            if (workingShift != null)
            {
                int _minutesLate = 5;
                var LATE_CORLOR = Brushes.Fuchsia;
                if (!String.IsNullOrEmpty(modelConverted.AttendanceIn1.Trim()))
                {
                    if (SubTime(workingShift.TimeIn1, modelConverted.AttendanceIn1) > _minutesLate)
                        modelConverted.RowForeground = LATE_CORLOR;
                }
                if (!String.IsNullOrEmpty(modelConverted.AttendanceIn2.Trim()))
                {
                    if (SubTime(workingShift.TimeIn2, modelConverted.AttendanceIn2) > _minutesLate)
                        modelConverted.RowForeground = LATE_CORLOR;
                }
                if (!String.IsNullOrEmpty(modelConverted.AttendanceIn3.Trim()))
                {
                    if (SubTime(workingShift.TimeIn3, modelConverted.AttendanceIn3) > _minutesLate)
                        modelConverted.RowForeground = LATE_CORLOR;
                }

                int _minutesEarly = -5;
                var EARLY_CORLOR = Brushes.OrangeRed;
                if (!String.IsNullOrEmpty(modelConverted.AttendanceOut1.Trim()))
                {
                    if (SubTime(workingShift.TimeOut1, modelConverted.AttendanceOut1) < _minutesEarly)
                        modelConverted.RowForeground = EARLY_CORLOR;
                }
                if (!String.IsNullOrEmpty(modelConverted.AttendanceOut2.Trim()))
                {
                    if (SubTime(workingShift.TimeOut2, modelConverted.AttendanceOut2) < _minutesEarly)
                        modelConverted.RowForeground = EARLY_CORLOR;
                }
                if (!String.IsNullOrEmpty(modelConverted.AttendanceOut3.Trim()))
                {
                    if (SubTime(workingShift.TimeOut3, modelConverted.AttendanceOut3) < _minutesEarly)
                        modelConverted.RowForeground = EARLY_CORLOR;
                }

                // Check NightShift
                if (!String.IsNullOrEmpty(modelConverted.AttendanceIn1.Trim()) && !String.IsNullOrEmpty(modelConverted.AttendanceOut1.Trim()))
                {
                    if (SubTime(modelConverted.AttendanceIn1, modelConverted.AttendanceOut1) < 0)
                        if (SubTimeOverTime(modelConverted.AttendanceIn1, modelConverted.AttendanceOut1) < _minutesEarly)
                            modelConverted.RowForeground = EARLY_CORLOR;
                        else modelConverted.RowForeground = Brushes.Black;
                }
            }

            // abnormal and workingday == 0 || lawday = red
            if (model.Remarks.ToLower().Contains("abnormal"))
            {
                if (model.WorkingDay == 0)
                    modelConverted.RowForeground = Brushes.Red;
            }
            if (modelConverted.Remarks.ToLower().Contains("law day"))
                modelConverted.RowForeground = Brushes.Red;
            

            // leaveday = blue
            if (model.Remarks.ToLower().Contains("leave"))
                modelConverted.RowForeground = Brushes.Blue;
            

            int dateOfWeekIndex = 0;
            Int32.TryParse(model.DayOfWeek, out dateOfWeekIndex);
            modelConverted.DayOfWeekFull = DayOfWeekList.DayOfWeekListInit()[dateOfWeekIndex].ToString();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttendanceRecordResult.ItemsSource != null)
            {
                var datagridCurrent = dgAttendanceRecordResult.ItemsSource.OfType<AttendanceRecordViewModel>().ToList();
                foreach (var attendanceReport in datagridCurrent)
                {
                    string empID = "";
                    var employeeModel = employeeList.Where(w => w.EmployeeCode == attendanceReport.EmployeeCode).FirstOrDefault();
                    if (employeeModel != null)
                        empID = employeeModel.EmployeeID;
                    attendanceReport.EmployeeID = empID;
                }
                if (datagridCurrent.Count > 0)
                    datagridCurrent = datagridCurrent.OrderBy(o => o.EmployeeID).ToList();
                AttendanceRecordDetailReportWindow window = new AttendanceRecordDetailReportWindow(datagridCurrent, departmentList, employeeList);
                window.Show();
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            double workingDay = 0, workingDayTo = 0, overTime = 0, overTimeTo = 0, absent = 0, late = 0;
            string remark = "";
            Double.TryParse(txtFilterWorkingDay.Text.Trim().ToString(), out workingDay);
            Double.TryParse(txtFilterWorkingDayTo.Text.Trim().ToString(), out workingDayTo);
            Double.TryParse(txtFilterOverTime.Text.Trim().ToString(), out overTime); ;
            Double.TryParse(txtFilterOverTimeTo.Text.Trim().ToString(), out overTimeTo);
            Double.TryParse(txtFilterAbsent.Text.Trim().ToString(), out absent);
            Double.TryParse(txtFilterLate.Text.Trim().ToString(), out late);

            remark = cboRemark.SelectedItem as string;
            var attendanceRecordViewModelListFilter = new List<AttendanceRecordModel>();

            //bool filterWorkingDay = false, filterOverTime = false, filterAbsent = false, filterLate = false, filterRemark = false;
            if (!string.IsNullOrEmpty(remark))
                attendanceRecordViewModelListFilter = attendanceDetailByEmployeeSearchList.Where(w => w.Remarks == remark).ToList();

            if (workingDay > 0)
            {
                attendanceRecordViewModelListFilter = attendanceDetailByEmployeeSearchList.Where(w => w.WorkingDay >= workingDay).ToList();
                if (workingDayTo > 0)
                    attendanceRecordViewModelListFilter = attendanceDetailByEmployeeSearchList.Where(w => w.WorkingDay >= workingDay && w.WorkingDay <= workingDayTo).ToList();
            }

            if (overTime > 0)
            {
                attendanceRecordViewModelListFilter = attendanceDetailByEmployeeSearchList.Where(w => w.WorkingOverTime >= overTime).ToList();
                if (overTimeTo > 0)
                    attendanceRecordViewModelListFilter = attendanceDetailByEmployeeSearchList.Where(w => w.WorkingOverTime >= overTime && w.WorkingOverTime <= overTimeTo).ToList();
            }
            
            if (absent > 0)
                attendanceRecordViewModelListFilter = attendanceDetailByEmployeeSearchList.Where(w => w.Absent >= absent).ToList();
            
            if (late > 0)
                attendanceRecordViewModelListFilter = attendanceDetailByEmployeeSearchList.Where(w => w.TimeLate >= late).ToList();
            

            // convert to viewmodel list;
            var attendanceViewModelFilterList = new List<AttendanceRecordViewModel>();
            if (attendanceRecordViewModelListFilter.Count() > 0)
            {
                foreach (var attendanceRecord in attendanceRecordViewModelListFilter)
                {
                    var attendanceViewModel = new AttendanceRecordViewModel();
                    ConvertToViewModel(attendanceRecord, attendanceViewModel);
                    attendanceViewModelFilterList.Add(attendanceViewModel);
                }
                //var viewModelFilterList = new ObservableCollection<AttendanceRecordViewModel>(attendanceViewModelFilterList);
                viewModelList = new ObservableCollection<AttendanceRecordViewModel>(attendanceViewModelFilterList);
                //dgAttendanceRecordResult.ItemsSource = viewModelFilterList;viewModelList
                dgAttendanceRecordResult.ItemsSource = viewModelList;
                dgAttendanceRecordResult.Items.Refresh();
                
                txtGriNumber.Text = "";
                //if (viewModelFilterList.Select(s => s.EmployeeCode).Distinct().Count() > 0)
                if (viewModelList.Select(s => s.EmployeeCode).Distinct().Count() > 0)
                {
                    //txtGriNumber.Text = string.Format("{0}: {1}", LanguageHelper.GetStringFromResource("commonTotalEmployee"), viewModelFilterList.Select(s => s.EmployeeCode).Distinct().Count());
                    txtGriNumber.Text = string.Format("{0}: {1}", LanguageHelper.GetStringFromResource("commonTotalEmployee"), viewModelList.Select(s => s.EmployeeCode).Distinct().Count());
                }
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            txtFilterWorkingDay.Text = "";
            txtFilterWorkingDayTo.Text = "";
            txtFilterOverTime.Text = "";
            txtFilterOverTimeTo.Text = "";
            txtFilterAbsent.Text = "";
            txtFilterLate.Text = "";
            cboRemark.SelectedItem = remarkList.FirstOrDefault();

            if (bwLoadAttendance.IsBusy == false)
            {
                this.Cursor = null;
                bwLoadAttendance.RunWorkerAsync(employeeSelectedList);
            }
            //var attendanceViewModelList = new List<AttendanceRecordViewModel>();
            //foreach (var attendanceRecord in attendanceDetailByEmployeeSearchList)
            //{
            //    var attendanceViewModel = new AttendanceRecordViewModel();
            //    ConvertToViewModel(attendanceRecord, attendanceViewModel);
            //    attendanceViewModelList.Add(attendanceViewModel);
            //}
            //// Show Data to Gridview
            //viewModelList = new ObservableCollection<AttendanceRecordViewModel>(attendanceViewModelList);
            //dgAttendanceRecordResult.ItemsSource = viewModelList;
            //dgAttendanceRecordResult.Items.Refresh();

            //txtGriNumber.Text = "";
            //if (viewModelList.Select(s => s.EmployeeCode).Distinct().Count() > 0)
            //{
            //    txtGriNumber.Text = string.Format("{0}: {1}", LanguageHelper.GetStringFromResource("commonTotalEmployee"), viewModelList.Select(s => s.EmployeeCode).Distinct().Count());
            //}
        }
        
        private void btnReportSummary_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttendanceRecordResult.ItemsSource != null)
            {
                var datagridCurrent = dgAttendanceRecordResult.ItemsSource.OfType<AttendanceRecordViewModel>().ToList();
                foreach (var attendanceReport in datagridCurrent)
                {
                    var employeeModel = employeeList.Where(w => w.EmployeeCode == attendanceReport.EmployeeCode).FirstOrDefault();
                    if (employeeModel != null)
                    {
                        attendanceReport.EmployeeID = employeeModel.EmployeeID;
                        attendanceReport.EmployeeName = employeeModel.EmployeeName;
                    }
                }
                var dateFrom = dpFrom.SelectedDate.Value;
                var dateTo = dpTo.SelectedDate.Value;
                //AttendanceRecordDetailReportSummaryWindow window = new AttendanceRecordDetailReportSummaryWindow(datagridCurrent, dateFrom, dateTo, employeeList);
                //window.Show();
                if (bwExportExcel.IsBusy == false)
                {
                    this.Cursor = Cursors.Wait;
                    txtGriNumber.Text = "";
                    btnReportSummary.IsEnabled = false;
                    object[] par = new object[] { datagridCurrent, dateFrom, dateTo, employeeList };
                    bwExportExcel.RunWorkerAsync(par);
                }
            }
        }
        private void bwExportExcel_DoWork(object sender, DoWorkEventArgs e)
        {
            var par                 = e.Argument as object[];
            var attendanRecordList  = par[0] as List<AttendanceRecordViewModel>;
            var dateFrom            = (DateTime)par[1];
            var dateTo              = (DateTime)par[2];
            var employeeList        = par[3] as List<EmployeeModel>;

            EXCEL._Application excel    = new Microsoft.Office.Interop.Excel.Application();
            EXCEL._Workbook workbook    = excel.Workbooks.Add(Type.Missing);
            EXCEL._Worksheet worksheet  = null;

            try
            {
                worksheet = workbook.ActiveSheet;
                worksheet.Cells.HorizontalAlignment = EXCEL.XlHAlign.xlHAlignCenter;
                worksheet.Cells.Font.Name = "Arial";
                worksheet.Cells.Font.Size = 10;
                worksheet.Name = String.Format("{0:yyyy-MM-dd} -> {1:yyyy-MM-dd}", dateFrom, dateTo);

                // Header
                int rowHeader = 1;
                worksheet.Cells[rowHeader, 1]   = "Employee Code";
                worksheet.Cells[rowHeader, 2]   = "Employee ID";
                worksheet.Cells[rowHeader, 3]   = "Full Name";
                worksheet.Cells[rowHeader, 4]   = "Department";
                worksheet.Cells[rowHeader, 5]   = "Day Of Week";
                worksheet.Cells[rowHeader, 6]   = "Date";
                worksheet.Cells[rowHeader, 7]   = "In 1";
                worksheet.Cells[rowHeader, 8]   = "Out 1";
                worksheet.Cells[rowHeader, 9]   = "In 2";
                worksheet.Cells[rowHeader, 10]  = "Out 2";
                worksheet.Cells[rowHeader, 11]  = "OT In";
                worksheet.Cells[rowHeader, 12]  = "OT Out";
                worksheet.Cells[rowHeader, 13]  = "Day";
                worksheet.Cells[rowHeader, 14]  = "Hour";
                worksheet.Cells[rowHeader, 15]  = "Over Time";
                worksheet.Cells[rowHeader, 16]  = "Late";
                worksheet.Cells[rowHeader, 17]  = "OT 2";
                worksheet.Cells[rowHeader, 18]  = "Absent";
                worksheet.Cells[rowHeader, 19]  = "Ask";
                worksheet.Cells[rowHeader, 20]  = "Remark";

                worksheet.Cells.Rows[1].Font.Size = 11;
                worksheet.Cells.Rows[1].Font.FontStyle = "Bold";

                int rowContent = 2;
                foreach (var attendance in attendanRecordList)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        txtGriNumber.Text = string.Format("Writing {0} rows / {1}", rowContent - 1, attendanRecordList.Count());
                    }));

                    var employee = employeeList.Where(w => w.EmployeeCode == attendance.EmployeeCode).FirstOrDefault();
                    worksheet.Cells[rowContent, 1]  = String.Format("'{0}", attendance.EmployeeCode);
                    worksheet.Cells[rowContent, 2]  = attendance.EmployeeID;
                    worksheet.Cells[rowContent, 3]  = attendance.EmployeeName;
                    worksheet.Cells[rowContent, 4]  = employee != null ? employee.DepartmentName : "";

                    worksheet.Cells[rowContent, 5]  = attendance.DayOfWeek;
                    worksheet.Cells[rowContent, 6]  = attendance.AttendanceDate;

                    worksheet.Cells[rowContent, 7]  = String.Format("'{0}", DisplayTime(attendance.AttendanceIn1));
                    worksheet.Cells[rowContent, 8]  = String.Format("'{0}", DisplayTime(attendance.AttendanceOut1));
                    worksheet.Cells[rowContent, 9]  = String.Format("'{0}", DisplayTime(attendance.AttendanceIn2));
                    worksheet.Cells[rowContent, 10] = String.Format("'{0}", DisplayTime(attendance.AttendanceOut2));
                    worksheet.Cells[rowContent, 11] = String.Format("'{0}", DisplayTime(attendance.AttendanceIn3));
                    worksheet.Cells[rowContent, 12] = String.Format("'{0}", DisplayTime(attendance.AttendanceOut3));

                    worksheet.Cells[rowContent, 13] = attendance.WorkingDay;
                    worksheet.Cells[rowContent, 14] = attendance.WorkingTime;
                    worksheet.Cells[rowContent, 15] = attendance.WorkingOverTime;
                    worksheet.Cells[rowContent, 16] = attendance.TimeLate;
                    worksheet.Cells[rowContent, 17] = attendance.OverTime2;
                    worksheet.Cells[rowContent, 18] = attendance.Absent;
                    worksheet.Cells[rowContent, 19] = attendance.Ask;
                    worksheet.Cells[rowContent, 20] = attendance.Remarks;

                    rowContent++;
                }
                Dispatcher.Invoke(new Action(() =>
                    {
                        if (workbook != null)
                        {
                            var sfd = new System.Windows.Forms.SaveFileDialog();
                            //sfd.Filter = "Excel Documents (*.xls)|Excel Documents (*.xlsx)"; //"txt files (*.txt)|*.txt|All files (*.*)|*.*";
                            sfd.Title = "SV-HRS Export Excel File";
                            //sfd.CheckFileExists = true;
                            //sfd.CheckPathExists = true;
                            sfd.Filter = "Excel Documents (*.xls)|*.xls|Excel Documents (*.xlsx)|*.xlsx";
                            sfd.FilterIndex = 2;
                            sfd.FileName = String.Format("Attendance Detail Report");
                            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                workbook.SaveAs(sfd.FileName);
                                MessageBox.Show("Export Successful !", "SV-HRS Export Excel File", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }));
            }
            catch (System.Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBox.Show(ex.Message, "SV-HRS Export Excel File", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
        private void bwExportExcel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            btnReportSummary.IsEnabled = true;
            txtGriNumber.Text = "";
        }

        private int SubTime(string timeDF, string timeAct)
        {
            timeDF = timeDF.Trim();
            timeAct = timeAct.Trim();
            if (timeDF.Contains(":"))
                timeDF = timeDF.Replace(":", "");
            if (timeAct.Contains(":"))
                timeAct = timeAct.Replace(":", "");

            return TranferMinutes(timeAct) - TranferMinutes(timeDF);
        }
        private int SubTimeOverTime(string timeOutDF, string timeOutAct)
        {
            timeOutDF = timeOutDF.Trim();
            timeOutAct = timeOutAct.Trim();
            if (timeOutDF.Contains(":"))
                timeOutDF = timeOutDF.Replace(":", "");
            if (timeOutAct.Contains(":"))
                timeOutAct = timeOutAct.Replace(":", "");

            return (TranferMinutes(timeOutAct) + 24 * 60) - TranferMinutes(timeOutDF);
        }
        private int TranferMinutes(string time)
        {
            int result = 0;
            if (time.Length >= 4)
            {
                string hour = time[0].ToString() + time[1].ToString();
                string minute = time[2].ToString() + time[3].ToString();
                int hourInt = 0, minuteInt = 0;
                Int32.TryParse(hour, out hourInt);
                Int32.TryParse(minute, out minuteInt);
                result = hourInt * 60 + minuteInt;
            }
            return result;
        }

        private void TxtEmployeeIDSearch_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            btnSearch.IsDefault = !addPressed;
            btnAddSearch.IsDefault = addPressed;
            btnFilter.IsDefault = false;
        }
        private void TxtEmployeeIDSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnSearch.IsDefault = !addPressed;
            btnAddSearch.IsDefault = addPressed;
            btnFilter.IsDefault = false;
        }
        private void txtFilterWorkingDay_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.SelectAll();
            btnFilter.IsDefault = true;

            btnSearch.IsDefault = false;
            btnAddSearch.IsDefault = false;
        }

        private string DisplayTime(string time)
        {
            string result = "";
            if (!String.IsNullOrEmpty(time.Trim().ToString()) && !time.Contains(":"))
            {
                result = time.Insert(2, ":");
                return result;
            }
            else return time;
        }
    }
}
