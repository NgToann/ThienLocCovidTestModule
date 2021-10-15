using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for ArrangeShiftWindow.xaml
    /// </summary>
    public partial class ArrangeWorkingShiftWindow : Window
    {
        BackgroundWorker bwLoad;
        BackgroundWorker bwSave;
        BackgroundWorker bwSearchDepartment;
        List<DepartmentModel> departmentList;
        List<EmployeeModel> employeeList;
        List<WorkingShiftModel> workingShiftList;
        List<AttendanceInforModel> attendanceInforPerDepartmentList;
        int yearClicked = 0, monthClicked = 0;

        public ArrangeWorkingShiftWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwSave = new BackgroundWorker();
            bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);
            bwSave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);

            bwSearchDepartment = new BackgroundWorker();
            bwSearchDepartment.DoWork += BwSearchDepartment_DoWork;
            bwSearchDepartment.RunWorkerCompleted += BwSearchDepartment_RunWorkerCompleted;

            employeeList = new List<EmployeeModel>();
            departmentList = new List<DepartmentModel>();
            workingShiftList = new List<WorkingShiftModel>();
            attendanceInforPerDepartmentList = new List<AttendanceInforModel>();

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
                departmentList = DepartmentController.GetDepartments();
                employeeList = EmployeeController.GetAll();
                workingShiftList = WorkingShiftController.GetAll().Where(w => w.IsActive == 0).OrderBy(o => o.WorkingShiftCode).ToList();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0} !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                }));
            }
        }
        private void bwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Create combobox year,month
            var yearList = new List<int>();
            var monthList = new List<int>();
            for (int y = 2001; y <= DateTime.Now.Year; y++)
            {
                yearList.Add(y);
            }
            for (int m = 1; m <= 12; m++)
            {
                monthList.Add(m);
            }
            cboYear.ItemsSource = yearList.OrderByDescending(o => o);
            cboYear.SelectedItem = yearList.Max();

            cboMonth.ItemsSource = monthList;
            cboMonth.SelectedItem = monthList.FirstOrDefault(w => w == DateTime.Now.Month); // Canot be null

            // Create TreeView
            //lstDepartments.ItemsSource = departmentList;
            var deptParentList = departmentList.Where(w => string.IsNullOrEmpty(w.ParentID) == true).ToList();
            foreach (var departParent in deptParentList)
            {
                TreeViewItem tviParent = new TreeViewItem();
                tviParent.FontWeight = FontWeights.Bold;
                tviParent.Margin = new Thickness(0, 2, 0, 2);
                tviParent.Foreground = Brushes.Black;

                tviParent.Tag = departParent;
                tviParent.MouseDoubleClick += new MouseButtonEventHandler(tvi_MouseDoubleClick);

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
                {
                    tviParent.Header = string.Format("{0} ({1})", departParent.DepartmentName, departmentsChild.Count);
                }

                treeDepartments.Items.Add(tviParent);
            }

            // Create menu item working shift
            var sundayShift = workingShiftList.Where(w => w.IsSunday == true).FirstOrDefault();
            foreach (var workingShift in workingShiftList)
            {
                object[] par = new object[] { workingShift, sundayShift != null ? sundayShift : workingShift };
                MenuItem miWorkingShift = new MenuItem
                {
                    Header = string.Format("{0} - {1}  Time: {2} --> {3}",
                                            workingShift.WorkingShiftCode,
                                            workingShift.WorkingShiftName,
                                            workingShift.TimeIn1,
                                            workingShift.TimeOut1),
                    Tag = par
                };
                miArrange.Items.Add(miWorkingShift);
                miWorkingShift.Click += new RoutedEventHandler(miWorkingShift_Click);
            }
            this.Cursor = null;
        }

        private void tvi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            yearClicked = (int)cboYear.SelectedItem;
            monthClicked = (int)cboMonth.SelectedItem;

            var itemClicked = sender as TreeViewItem;
            var departmentClicked = itemClicked.Tag as DepartmentModel;
            var employeeListPerDepartment = new List<EmployeeModel>();
            if (departmentClicked != null && bwSearchDepartment.IsBusy == false)
            {
                var departmentListClicked = new List<DepartmentModel>();
                var childDeptList = departmentList.Where(w => w.ParentID == departmentClicked.DepartmentID).ToList();
                if (childDeptList.Count() > 0)
                {
                    foreach (var child in childDeptList)
                    {
                        departmentListClicked.Add(child);
                        employeeListPerDepartment.AddRange(
                            employeeList.Where(w => w.DepartmentName.ToUpper().Trim().ToString() == child.DepartmentFullName.Trim().ToUpper().ToString()).OrderBy(o => o.EmployeeID).ToList());
                    }
                }
                else
                {
                    departmentListClicked.Add(departmentClicked);
                    employeeListPerDepartment.AddRange(employeeList.Where(w => w.DepartmentName.ToUpper().Trim().ToString() == departmentClicked.DepartmentFullName.Trim().ToUpper().ToString()).OrderBy(o => o.EmployeeID).ToList());
                }
                dgEmployeePerDepartment.ItemsSource = employeeListPerDepartment;

                treeDepartments.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                //refresh arrangeShift list
                dgAttendanceEmployee.ItemsSource = new List<AttendanceInforModel>();
                dgAttendanceEmployee.Items.Refresh();

                bwSearchDepartment.RunWorkerAsync(departmentListClicked);
            }
        }
        private void BwSearchDepartment_DoWork(object sender, DoWorkEventArgs e)
        {
            var departmentListClicked = e.Argument as List<DepartmentModel>;
            foreach (var department in departmentListClicked)
            {
                attendanceInforPerDepartmentList.AddRange(AttendanceInforController.GetByDepartmentYearMonth(department.DepartmentFullName, yearClicked, monthClicked));
            }
        }
        private void BwSearchDepartment_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            treeDepartments.IsEnabled = true;
        }

        private void miWorkingShift_Click(object sender, RoutedEventArgs e)
        {
            yearClicked = (int)cboYear.SelectedItem;
            monthClicked = (int)cboMonth.SelectedItem;
            var menuItemClicked = sender as MenuItem;
            var par = menuItemClicked.Tag as object[];
            var workingShift = par[0] as WorkingShiftModel;
            var sundayShift = par[1] as WorkingShiftModel;

            var employeeListSelected = dgEmployeePerDepartment.SelectedItems.OfType<EmployeeModel>().ToList();

            if (employeeListSelected.Count() > 0 || workingShift == null || sundayShift != null)
            {
                int daysInMonth = DateTime.DaysInMonth(yearClicked, monthClicked);
                col29.Visibility = Visibility.Collapsed;
                col30.Visibility = Visibility.Collapsed;
                col31.Visibility = Visibility.Collapsed;

                dgEmployeePerDepartment.IsEnabled = false;
                List<AttendanceInforModel> currentAttendanceInforList = new List<AttendanceInforModel>();
                if (dgAttendanceEmployee.ItemsSource != null)
                    currentAttendanceInforList = dgAttendanceEmployee.ItemsSource.OfType<AttendanceInforModel>().ToList();
                string workingShiftCode = workingShift.WorkingShiftCode;
                string sundayShiftCode = sundayShift.WorkingShiftCode;

                List<DateTime> dateFromToList = new List<DateTime>();
                for (DateTime date = dpFrom.SelectedDate.Value; date <= dpTo.SelectedDate.Value; date = date.AddDays(1))
                {
                    dateFromToList.Add(date);
                }
                foreach (var employee in employeeListSelected)
                {
                    // Check AttendanceInfor Before
                    // If already arrange, show data 
                    var attendateSearchedByEmployee = attendanceInforPerDepartmentList.Where(w => w.EmployeeCode == employee.EmployeeCode && w.ShiftYear == yearClicked && w.ShiftMonth == monthClicked).FirstOrDefault();
                    if (attendateSearchedByEmployee != null)
                    {
                        attendateSearchedByEmployee.EmployeeCode = employee != null ? employee.EmployeeCode : "NULL";
                        attendateSearchedByEmployee.EmployeeID = employee != null ? employee.EmployeeID : "NULL";
                        attendateSearchedByEmployee.EmployeeName = employee != null ? employee.EmployeeName : "NULL";

                        CheckColor(ref attendateSearchedByEmployee, yearClicked, monthClicked);
                        currentAttendanceInforList.Add(attendateSearchedByEmployee);
                    }

                    // If new. create data
                    else
                    {
                        AttendanceInforModel attInfor = new AttendanceInforModel();
                        attInfor.EmployeeCode = employee != null ? employee.EmployeeCode : "NULL";
                        attInfor.EmployeeName = employee != null ? employee.EmployeeName : "NULL";
                        attInfor.EmployeeID = employee != null ? employee.EmployeeID : "NULL";
                        attInfor.ShiftYear = yearClicked;
                        attInfor.ShiftMonth = monthClicked;

                        #region bind shiftNo
                        // date 1
                        var dateCheck1 = new DateTime(yearClicked, monthClicked, 1);
                        if (dateCheck1.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_1 = sundayShiftCode;
                            attInfor.Shift_1Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck1))
                        {
                            attInfor.Shift_1 = workingShiftCode;
                        }

                        // date 2
                        var dateCheck2 = new DateTime(yearClicked, monthClicked, 2);
                        if (dateCheck2.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_2 = sundayShiftCode;
                            attInfor.Shift_2Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck2))
                        {
                            attInfor.Shift_2 = workingShiftCode;
                        }

                        // date 3
                        var dateCheck3 = new DateTime(yearClicked, monthClicked, 3);
                        if (dateCheck3.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_3 = sundayShiftCode;
                            attInfor.Shift_3Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck3))
                        {
                            attInfor.Shift_3 = workingShiftCode;
                        }

                        // date 4
                        var dateCheck4 = new DateTime(yearClicked, monthClicked, 4);
                        if (dateCheck4.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_4 = sundayShiftCode;
                            attInfor.Shift_4Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck4))
                        {
                            attInfor.Shift_4 = workingShiftCode;
                        }

                        // date 5
                        var dateCheck5 = new DateTime(yearClicked, monthClicked, 5);
                        if (dateCheck5.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_5 = sundayShiftCode;
                            attInfor.Shift_5Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck5))
                        {
                            attInfor.Shift_5 = workingShiftCode;
                        }

                        // date 6
                        var dateCheck6 = new DateTime(yearClicked, monthClicked, 6);
                        if (dateCheck6.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_6 = sundayShiftCode;
                            attInfor.Shift_6Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck6))
                        {
                            attInfor.Shift_6 = workingShiftCode;
                        }

                        // date 7
                        var dateCheck7 = new DateTime(yearClicked, monthClicked, 7);
                        if (dateCheck7.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_7 = sundayShiftCode;
                            attInfor.Shift_7Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck7))
                        {
                            attInfor.Shift_7 = workingShiftCode;
                        }

                        // date 8
                        var dateCheck8 = new DateTime(yearClicked, monthClicked, 8);
                        if (dateCheck8.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_8 = sundayShiftCode;
                            attInfor.Shift_8Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck8))
                        {
                            attInfor.Shift_8 = workingShiftCode;
                        }

                        // date 9
                        var dateCheck9 = new DateTime(yearClicked, monthClicked, 9);
                        if (dateCheck9.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_9 = sundayShiftCode;
                            attInfor.Shift_9Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck9))
                        {
                            attInfor.Shift_9 = workingShiftCode;
                        }

                        // date 10
                        var dateCheck10 = new DateTime(yearClicked, monthClicked, 10);
                        if (dateCheck10.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_10 = sundayShiftCode;
                            attInfor.Shift_10Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck10))
                        {
                            attInfor.Shift_10 = workingShiftCode;
                        }

                        // date 11
                        var dateCheck11 = new DateTime(yearClicked, monthClicked, 11);
                        if (dateCheck11.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_11 = sundayShiftCode;
                            attInfor.Shift_11Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck11))
                        {
                            attInfor.Shift_11 = workingShiftCode;
                        }

                        // date 12
                        var dateCheck12 = new DateTime(yearClicked, monthClicked, 12);
                        if (dateCheck12.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_12 = sundayShiftCode;
                            attInfor.Shift_12Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck12))
                        {
                            attInfor.Shift_12 = workingShiftCode;
                        }

                        // date 13
                        var dateCheck13 = new DateTime(yearClicked, monthClicked, 13);
                        if (dateCheck13.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_13 = sundayShiftCode;
                            attInfor.Shift_13Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck13))
                        {
                            attInfor.Shift_13 = workingShiftCode;
                        }

                        // date 14
                        var dateCheck14 = new DateTime(yearClicked, monthClicked, 14);
                        if (dateCheck14.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_14 = sundayShiftCode;
                            attInfor.Shift_14Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck14))
                        {
                            attInfor.Shift_14 = workingShiftCode;
                        }

                        // date 15
                        var dateCheck15 = new DateTime(yearClicked, monthClicked, 15);
                        if (dateCheck15.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_15 = sundayShiftCode;
                            attInfor.Shift_15Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck15))
                        {
                            attInfor.Shift_15 = workingShiftCode;
                        }

                        // date 16
                        var dateCheck16 = new DateTime(yearClicked, monthClicked, 16);
                        if (dateCheck16.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_16 = sundayShiftCode;
                            attInfor.Shift_16Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck16))
                        {
                            attInfor.Shift_16 = workingShiftCode;
                        }

                        // date 17
                        var dateCheck17 = new DateTime(yearClicked, monthClicked, 17);
                        if (dateCheck17.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_17 = sundayShiftCode;
                            attInfor.Shift_17Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck17))
                        {
                            attInfor.Shift_17 = workingShiftCode;
                        }

                        // date 18
                        var dateCheck18 = new DateTime(yearClicked, monthClicked, 18);
                        if (dateCheck18.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_18 = sundayShiftCode;
                            attInfor.Shift_18Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck18))
                        {
                            attInfor.Shift_18 = workingShiftCode;
                        }

                        // date 19
                        var dateCheck19 = new DateTime(yearClicked, monthClicked, 19);
                        if (dateCheck19.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_19 = sundayShiftCode;
                            attInfor.Shift_19Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck19))
                        {
                            attInfor.Shift_19 = workingShiftCode;
                        }

                        // date 20
                        var dateCheck20 = new DateTime(yearClicked, monthClicked, 20);
                        if (dateCheck20.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_20 = sundayShiftCode;
                            attInfor.Shift_20Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck20))
                        {
                            attInfor.Shift_20 = workingShiftCode;
                        }

                        // date 21
                        var dateCheck21 = new DateTime(yearClicked, monthClicked, 21);
                        if (dateCheck21.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_21 = sundayShiftCode;
                            attInfor.Shift_21Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck21))
                        {
                            attInfor.Shift_21 = workingShiftCode;
                        }

                        // date 22
                        var dateCheck22 = new DateTime(yearClicked, monthClicked, 22);
                        if (dateCheck22.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_22 = sundayShiftCode;
                            attInfor.Shift_22Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck22))
                        {
                            attInfor.Shift_22 = workingShiftCode;
                        }

                        // date 23
                        var dateCheck23 = new DateTime(yearClicked, monthClicked, 23);
                        if (dateCheck23.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_23 = sundayShiftCode;
                            attInfor.Shift_23Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck23))
                        {
                            attInfor.Shift_23 = workingShiftCode;
                        }

                        // date 24
                        var dateCheck24 = new DateTime(yearClicked, monthClicked, 24);
                        if (dateCheck24.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_24 = sundayShiftCode;
                            attInfor.Shift_24Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck24))
                        {
                            attInfor.Shift_24 = workingShiftCode;
                        }

                        // date 25
                        var dateCheck25 = new DateTime(yearClicked, monthClicked, 25);
                        if (dateCheck25.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_25 = sundayShiftCode;
                            attInfor.Shift_25Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck25))
                        {
                            attInfor.Shift_25 = workingShiftCode;
                        }

                        // date 26
                        var dateCheck26 = new DateTime(yearClicked, monthClicked, 26);
                        if (dateCheck26.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_26 = sundayShiftCode;
                            attInfor.Shift_26Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck26))
                        {
                            attInfor.Shift_26 = workingShiftCode;
                        }

                        // date 27
                        var dateCheck27 = new DateTime(yearClicked, monthClicked, 27);
                        if (dateCheck27.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_27 = sundayShiftCode;
                            attInfor.Shift_27Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck27))
                        {
                            attInfor.Shift_27 = workingShiftCode;
                        }

                        // date 28
                        var dateCheck28 = new DateTime(yearClicked, monthClicked, 28);
                        if (dateCheck28.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attInfor.Shift_28 = sundayShiftCode;
                            attInfor.Shift_28Background = Brushes.Red;
                        }
                        else if (dateFromToList.Contains(dateCheck28))
                        {
                            attInfor.Shift_28 = workingShiftCode;
                        }

                        // date 29
                        if (daysInMonth >= 29)
                        {
                            var dateCheck29 = new DateTime(yearClicked, monthClicked, 29);
                            if (dateCheck29.DayOfWeek == DayOfWeek.Sunday)
                            {
                                attInfor.Shift_29 = sundayShiftCode;
                                attInfor.Shift_29Background = Brushes.Red;
                            }
                            else if (dateFromToList.Contains(dateCheck29))
                            {
                                attInfor.Shift_29 = workingShiftCode;
                            }
                            col29.Visibility = Visibility.Visible;
                        }

                        // date 30
                        if (daysInMonth >= 30)
                        {
                            var dateCheck30 = new DateTime(yearClicked, monthClicked, 30);
                            if (dateCheck30.DayOfWeek == DayOfWeek.Sunday)
                            {
                                attInfor.Shift_30 = sundayShiftCode;
                                attInfor.Shift_30Background = Brushes.Red;
                            }
                            else if (dateFromToList.Contains(dateCheck30))
                            {
                                attInfor.Shift_30 = workingShiftCode;
                            }
                            col30.Visibility = Visibility.Visible;
                        }

                        // date 31
                        if (daysInMonth >= 31)
                        {
                            var dateCheck31 = new DateTime(yearClicked, monthClicked, 31);
                            if (dateCheck31.DayOfWeek == DayOfWeek.Sunday)
                            {
                                attInfor.Shift_31 = sundayShiftCode;
                                attInfor.Shift_31Background = Brushes.Red;
                            }
                            else if (dateFromToList.Contains(dateCheck31))
                            {
                                attInfor.Shift_31 = workingShiftCode;
                            }
                            col31.Visibility = Visibility.Visible;
                        }
                        #endregion
                        currentAttendanceInforList.Add(attInfor);
                    }
                }
                dgAttendanceEmployee.ItemsSource = currentAttendanceInforList;
                dgAttendanceEmployee.Items.Refresh();

                // remove employeeListSelected from
                var currentEmployeeList = dgEmployeePerDepartment.ItemsSource.OfType<EmployeeModel>().ToList();
                if (currentEmployeeList.Count > 0)
                {
                    List<string> employeeCodeListSelected = employeeListSelected.Select(s => s.EmployeeCode).ToList();
                    currentEmployeeList.RemoveAll(r => employeeCodeListSelected.Contains(r.EmployeeCode));
                    dgEmployeePerDepartment.ItemsSource = currentEmployeeList;
                }
                dgEmployeePerDepartment.IsEnabled = true;
            }
        }

        private void MiViewDetail_Click(object sender, RoutedEventArgs e)
        {
            var employeeListSelected = dgEmployeePerDepartment.SelectedItems.OfType<EmployeeModel>().ToList();
            //var itemClicked = dgEmployeePerDepartment.CurrentItem as EmployeeModel;
            if (employeeListSelected.Count() > 0)
            {
                int year = (int)cboYear.SelectedItem;
                int month = (int)cboMonth.SelectedItem;

                WorkingShiftDetailWindow window = new WorkingShiftDetailWindow(employeeListSelected, year, month);
                window.Show();
            }
        }

        private void DgAttendanceEmployee_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            TextBox txtElement = (TextBox)e.EditingElement as TextBox;
            if (String.IsNullOrEmpty(txtElement.Text) == false && workingShiftList.Where(w => w.WorkingShiftCode == txtElement.Text).Count() == 0)
            {
                MessageBox.Show(string.Format("{0}\n{1}", txtElement.Text,
                                                       LanguageHelper.GetStringFromResource("messageDataIncorrect")),
                                this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                txtElement.Foreground = Brushes.Red;
                txtElement.SelectAll();
            }
            else
            {
                txtElement.Foreground = Brushes.Blue;
            }
        }

        private void miRemoveWorkingShift_Click(object sender, RoutedEventArgs e)
        {
            var employeeCodeRemoveList = dgAttendanceEmployee.SelectedItems.OfType<AttendanceInforModel>().ToList();
            if (employeeCodeRemoveList.Count() > 0)
            {
                if (MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageConfirmRemove")),
                                   this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    // Recovery EmployeeList
                    var employeeCurrentPerDepartmentList = dgEmployeePerDepartment.ItemsSource.OfType<EmployeeModel>().ToList();
                    var employeeNeedToAddList = employeeList.Where(w => employeeCodeRemoveList.Select(s => s.EmployeeCode).ToList().Contains(w.EmployeeCode)).ToList();
                    employeeCurrentPerDepartmentList.AddRange(employeeNeedToAddList);
                    dgEmployeePerDepartment.ItemsSource = employeeCurrentPerDepartmentList.OrderBy(o => o.EmployeeID).ToList();
                    dgEmployeePerDepartment.Items.Refresh();

                    // Remove ArrangeWorkingShift List
                    //attendanceInforPerDepartmentList.RemoveAll(r => employeeCodeRemoveList.Select(s => s.EmployeeCode).ToList().Contains(r.EmployeeCode));
                    var currentAttendanceInforList = dgAttendanceEmployee.ItemsSource.OfType<AttendanceInforModel>().ToList();
                    currentAttendanceInforList.RemoveAll(r => employeeCodeRemoveList.Select(s => s.EmployeeCode).ToList().Contains(r.EmployeeCode));
                    dgAttendanceEmployee.ItemsSource = currentAttendanceInforList;
                    dgAttendanceEmployee.Items.Refresh();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var arrangeWorkingShiftSaveList = dgAttendanceEmployee.ItemsSource.OfType<AttendanceInforModel>().ToList();
            if (arrangeWorkingShiftSaveList.Count() > 0 && bwSave.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                btnSave.IsEnabled = false;
                bwSave.RunWorkerAsync(arrangeWorkingShiftSaveList);
            }
        }
        private void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            var arrangeWorkingShiftList = e.Argument as List<AttendanceInforModel>;
            bool result = true;
            foreach (var arrangeWorkingShift in arrangeWorkingShiftList)
            {
                try
                {
                    if (AttendanceInforController.AddOrUpdate(arrangeWorkingShift) == true)
                        attendanceInforPerDepartmentList.Add(arrangeWorkingShift);
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBox.Show(string.Format("{0}\nPlease Try Again !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                        result = false;
                    }));
                }
                e.Result = result;
            }
        }
        private void bwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            if ( result == true)
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageAddDataSucessful")),
                                                this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.Cursor = null;
            btnSave.IsEnabled = true;
        }

        private void CheckColor(ref AttendanceInforModel model, int year, int month)
        {
            int totalDayOfMonth = DateTime.DaysInMonth(year, month);
            #region bind color
            // date 1
            var dateCheck1 = new DateTime(year, month, 1);
            if (dateCheck1.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_1Background = Brushes.Red;
            }


            // date 2
            var dateCheck2 = new DateTime(year, month, 2);
            if (dateCheck2.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_2Background = Brushes.Red;
            }


            // date 3
            var dateCheck3 = new DateTime(year, month, 3);
            if (dateCheck3.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_3Background = Brushes.Red;
            }

            // date 4
            var dateCheck4 = new DateTime(year, month, 4);
            if (dateCheck4.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_4Background = Brushes.Red;
            }

            // date 5
            var dateCheck5 = new DateTime(year, month, 5);
            if (dateCheck5.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_5Background = Brushes.Red;
            }

            // date 6
            var dateCheck6 = new DateTime(year, month, 6);
            if (dateCheck6.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_6Background = Brushes.Red;
            }

            // date 7
            var dateCheck7 = new DateTime(year, month, 7);
            if (dateCheck7.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_7Background = Brushes.Red;
            }

            // date 8
            var dateCheck8 = new DateTime(year, month, 8);
            if (dateCheck8.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_8Background = Brushes.Red;
            }

            // date 9
            var dateCheck9 = new DateTime(year, month, 9);
            if (dateCheck9.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_9Background = Brushes.Red;
            }

            // date 10
            var dateCheck10 = new DateTime(year, month, 10);
            if (dateCheck10.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_10Background = Brushes.Red;
            }

            // date 11
            var dateCheck11 = new DateTime(year, month, 11);
            if (dateCheck11.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_11Background = Brushes.Red;
            }

            // date 12
            var dateCheck12 = new DateTime(year, month, 12);
            if (dateCheck12.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_12Background = Brushes.Red;
            }

            // date 13
            var dateCheck13 = new DateTime(year, month, 13);
            if (dateCheck13.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_13Background = Brushes.Red;
            }

            // date 14
            var dateCheck14 = new DateTime(year, month, 14);
            if (dateCheck14.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_14Background = Brushes.Red;
            }

            // date 15
            var dateCheck15 = new DateTime(year, month, 15);
            if (dateCheck15.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_15Background = Brushes.Red;
            }

            // date 16
            var dateCheck16 = new DateTime(year, month, 16);
            if (dateCheck16.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_16Background = Brushes.Red;
            }

            // date 17
            var dateCheck17 = new DateTime(year, month, 17);
            if (dateCheck17.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_17Background = Brushes.Red;
            }

            // date 18
            var dateCheck18 = new DateTime(year, month, 18);
            if (dateCheck18.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_18Background = Brushes.Red;
            }

            // date 19
            var dateCheck19 = new DateTime(year, month, 19);
            if (dateCheck19.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_19Background = Brushes.Red;
            }

            // date 20
            var dateCheck20 = new DateTime(year, month, 20);
            if (dateCheck20.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_20Background = Brushes.Red;
            }

            // date 21
            var dateCheck21 = new DateTime(year, month, 21);
            if (dateCheck21.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_21Background = Brushes.Red;
            }

            // date 22
            var dateCheck22 = new DateTime(year, month, 22);
            if (dateCheck22.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_22Background = Brushes.Red;
            }

            // date 23
            var dateCheck23 = new DateTime(year, month, 23);
            if (dateCheck23.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_23Background = Brushes.Red;
            }

            // date 24
            var dateCheck24 = new DateTime(year, month, 24);
            if (dateCheck24.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_24Background = Brushes.Red;
            }

            // date 25
            var dateCheck25 = new DateTime(year, month, 25);
            if (dateCheck25.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_25Background = Brushes.Red;
            }

            // date 26
            var dateCheck26 = new DateTime(year, month, 26);
            if (dateCheck26.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_26Background = Brushes.Red;
            }

            // date 27
            var dateCheck27 = new DateTime(year, month, 27);
            if (dateCheck27.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_27Background = Brushes.Red;
            }

            // date 28
            var dateCheck28 = new DateTime(year, month, 28);
            if (dateCheck28.DayOfWeek == DayOfWeek.Sunday)
            {
                model.Shift_28Background = Brushes.Red;
            }

            // date 29
            if (totalDayOfMonth >= 29)
            {
                var dateCheck29 = new DateTime(year, month, 29);
                if (dateCheck29.DayOfWeek == DayOfWeek.Sunday)
                {
                    model.Shift_29Background = Brushes.Red;
                }
                col29.Visibility = Visibility.Visible;
            }

            // date 30
            if (totalDayOfMonth >= 30)
            {
                var dateCheck30 = new DateTime(year, month, 30);
                if (dateCheck30.DayOfWeek == DayOfWeek.Sunday)
                {
                    model.Shift_30Background = Brushes.Red;
                }
                col30.Visibility = Visibility.Visible;
            }

            // date 31
            if (totalDayOfMonth >= 31)
            {
                var dateCheck31 = new DateTime(year, month, 31);
                if (dateCheck31.DayOfWeek == DayOfWeek.Sunday)
                {
                    model.Shift_31Background = Brushes.Red;
                }
                col31.Visibility = Visibility.Visible;
            }
            #endregion
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string employeeSearch = txtEmployeeSearch.Text.Trim().ToUpper().ToString();
            var employeeSearched = employeeList.Where(w => w.EmployeeCode == employeeSearch || w.EmployeeID.Trim().ToUpper().ToString() == employeeSearch).FirstOrDefault();
            if (employeeSearched != null)
            {
                var employeeListCurrent = new List<EmployeeModel>();
                if (dgEmployeePerDepartment.ItemsSource != null)
                    employeeListCurrent = dgEmployeePerDepartment.ItemsSource.OfType<EmployeeModel>().ToList();
                if (employeeListCurrent.Where(w => w.EmployeeCode == employeeSearched.EmployeeCode).Count() == 0)
                {
                    employeeListCurrent.Add(employeeSearched);
                    dgEmployeePerDepartment.ItemsSource = employeeListCurrent;
                    dgEmployeePerDepartment.Items.Refresh();
                    dgEmployeePerDepartment.SelectedItem = employeeSearched;
                    dgEmployeePerDepartment.ScrollIntoView(employeeSearched);
                    //if (employeeListCurrent.Count() > 0)
                    //    txtTotal.Text = string.Format("{0} : {1}", LanguageHelper.GetStringFromResource("commonTextBlockFrom"), employeeListCurrent.Count());
                }
            }
            btnAdd.IsDefault = false;
            txtEmployeeSearch.Focus();
        }

        private void txtEmployeeSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnAdd.IsDefault = true;
        }

        private void txtEmployeeSearch_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            btnAdd.IsDefault = true;
        }

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            var itemsRemoveClicked = dgEmployeePerDepartment.SelectedItems.OfType<EmployeeModel>().ToList();
            if (itemsRemoveClicked.Count() > 0 && dgEmployeePerDepartment.ItemsSource != null)
            {
                //txtTotal.Text = "";
                var employeeListCurrent = dgEmployeePerDepartment.ItemsSource.OfType<EmployeeModel>().ToList();
                employeeListCurrent.RemoveAll(r => itemsRemoveClicked.Select(s => s.EmployeeCode).Contains(r.EmployeeCode));
                dgEmployeePerDepartment.ItemsSource = employeeListCurrent;
                dgEmployeePerDepartment.Items.Refresh();
                //if (employeeListCurrent.Count() > 0)
                //    txtTotal.Text = string.Format("{0} : {1}", LanguageHelper.GetStringFromResource("commonTextBlockFrom"), employeeListCurrent.Count());
            }
        }

        private void dgAttendanceEmployee_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void dgEmployeePerDepartment_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void cboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            yearClicked = (int)cboYear.SelectedItem;
            monthClicked = (int)cboMonth.SelectedItem;

            var firstDateOfTheMonth = new DateTime(yearClicked, monthClicked, 1);
            var lastDateOfTheMonth = firstDateOfTheMonth.AddMonths(1).AddDays(-1);
            dpFrom.SelectedDate = firstDateOfTheMonth;
            dpTo.SelectedDate = lastDateOfTheMonth;

            // Remove 2 datagrid
            if (dgEmployeePerDepartment.ItemsSource != null)
            {
                dgEmployeePerDepartment.ItemsSource = new List<EmployeeModel>();
                dgEmployeePerDepartment.Items.Refresh();
            }
            if (dgAttendanceEmployee.ItemsSource != null)
            {
                dgAttendanceEmployee.ItemsSource = new List<AttendanceInforModel>();
                dgAttendanceEmployee.Items.Refresh();
            }
        }
    }
}
