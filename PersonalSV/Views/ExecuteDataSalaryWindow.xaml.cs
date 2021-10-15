using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;
using System.Threading;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for ExcuteDataSalaryWindow.xaml
    /// </summary>
    public partial class ExecuteDataSalaryWindow : Window
    {
        BackgroundWorker bwLoad;
        BackgroundWorker bwExecute;
        List<EmployeeModel> employeeList;
        List<EmployeeModel> employeeAllList;
        List<DepartmentModel> departmentList;
        ModeExecute modeExecute;
        public ExecuteDataSalaryWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwExecute = new BackgroundWorker();
            bwExecute.DoWork += new DoWorkEventHandler(bwExecute_DoWork);
            bwExecute.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwExecute_RunWorkerCompleted);

            employeeList = new List<EmployeeModel>();
            employeeAllList = new List<EmployeeModel>();
            departmentList = new List<DepartmentModel>();

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
                employeeList = EmployeeController.GetEmployeeToExecuteSalaryData();
                employeeAllList = EmployeeController.GetAll();
                departmentList = DepartmentController.GetDepartments();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0}", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }));
            }
        }
        private void bwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;

            dpFrom.SelectedDate = DateTime.Now.Date;
            dpTo.SelectedDate = DateTime.Now.Date;

            // Binding cbo Section
            var sectionList = departmentList.Where(w => String.IsNullOrEmpty(w.ParentID) == true).ToList();
            if (sectionList.Count() > 0)
            {
                cboSection.ItemsSource = sectionList;
                cboSection.SelectedItem = sectionList.FirstOrDefault();
            }
            // Binding cbo Department
            var departmentFromPersonalList = employeeList.OrderBy(o => o.DepartmentName).Select(s => s.DepartmentName.Trim().ToUpper()).Distinct().ToList();
            if (departmentFromPersonalList.Count() > 0)
            {
                cboDepartment.ItemsSource = departmentFromPersonalList;
                cboDepartment.SelectedItem = departmentFromPersonalList.FirstOrDefault();
            }
            btnExecute.IsEnabled = true;
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            var sectionClicked = cboSection.SelectedItem as DepartmentModel;
            var departmentClicked = cboDepartment.SelectedItem as string;
            var employeeIDInput = txtEmployeeID.Text.Trim().ToUpper().ToString();
            //var employeeExecute = employeeList.Where(w => w.EmployeeID == employeeIDInput).FirstOrDefault();
            // dambighet?whatsthebook?
            var employeeExecute = employeeAllList.Where(w => w.EmployeeID == employeeIDInput || w.EmployeeCode == employeeIDInput).FirstOrDefault();

            DateTime dateFrom = dpFrom.SelectedDate.Value;
            DateTime dateTo = dpTo.SelectedDate.Value;
            string dateFromTitle = LanguageHelper.GetStringFromResource("commonDatePickerFrom");
            string dateToTitle = LanguageHelper.GetStringFromResource("commonDatePickerTo");
            string confirmExecuteFull = "";

            if (modeExecute == ModeExecute.PerSection)
            {
                confirmExecuteFull = String.Format("{0}\n{1}\n{2}: {3} --> {4}: {5}",
                                                    confirmExcuteTitleTemporary,
                                                    sectionClicked.DepartmentFullName,
                                                    dateFromTitle,
                                                    dateFrom.ToShortDateString(),
                                                    dateToTitle,
                                                    dateTo.ToShortDateString());
            }

            if (modeExecute == ModeExecute.PerDepartment)
            {
                confirmExecuteFull = String.Format("{0}\n{1}\n{2}: {3} --> {4}: {5}",
                                                    confirmExcuteTitleTemporary,
                                                    departmentClicked,
                                                    dateFromTitle,
                                                    dateFrom.ToShortDateString(),
                                                    dateToTitle,
                                                    dateTo.ToShortDateString());
            }
            if (modeExecute == ModeExecute.PerEmployee)
            {
                confirmExecuteFull = String.Format("{0}\n{1}\n{2}: {3} --> {4}: {5}",
                                                    confirmExcuteTitleTemporary,
                                                    employeeIDInput,
                                                    dateFromTitle,
                                                    dateFrom.ToShortDateString(),
                                                    dateToTitle,
                                                    dateTo.ToShortDateString());
            }
            if (modeExecute == ModeExecute.All)
            {
                confirmExecuteFull = String.Format("{0}\n{1}: {2} --> {3}: {4}",
                                                    confirmExcuteTitleTemporary,
                                                    dateFromTitle,
                                                    dateFrom.ToShortDateString(),
                                                    dateToTitle,
                                                    dateTo.ToShortDateString());
            }

            if (MessageBox.Show(confirmExecuteFull, this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                if (modeExecute == ModeExecute.PerEmployee && employeeExecute == null)
                {
                    MessageBox.Show(string.Format("{0}\n{1}", employeeIDInput, LanguageHelper.GetStringFromResource("messageNotFound")),
                        this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (bwExecute.IsBusy == false)
                {
                    this.Cursor = Cursors.Wait;
                    btnExecute.IsEnabled = false;
                    txtProcessing.Text = "";
                    txtStatus.Text = "";
                    prgProcessing.Value = 0;
                    object[] par = new object[] { sectionClicked, departmentClicked, employeeExecute, dateFrom, dateTo };
                    bwExecute.RunWorkerAsync(par);
                }
            }
        }
        private void bwExecute_DoWork(object sender, DoWorkEventArgs e)
        {
            var par = e.Argument as object[];
            var sectionClicked = par[0] as DepartmentModel;
            string deparmentClicked = par[1].ToString();
            var employeeExecuteModePerEmployee = par[2] as EmployeeModel;
            var dateFrom = (DateTime)par[3];
            var dateTo = (DateTime)par[4];

            if (modeExecute == ModeExecute.PerSection)
            {
                var child = departmentList.Where(w => w.ParentID == sectionClicked.DepartmentID).ToList();
                var departmentPerSectionList = new List<DepartmentModel>();
                if (child.Count() > 0)
                    departmentPerSectionList = departmentList.Where(w => w.ParentID == sectionClicked.DepartmentID).ToList();
                if (child.Count == 0)
                    departmentPerSectionList = departmentList.Where(w => w.DepartmentID == sectionClicked.DepartmentID).ToList();
                //= departmentList.Where(w => w.ParentID == sectionClicked.DepartmentID).ToList();
                var employeeListPerSection = new List<EmployeeModel>();
                int deparmentIndex = 1;
                foreach (var department in departmentPerSectionList)
                {
                    var employeeListPerDepartment = employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == department.DepartmentFullName).ToList();
                    if (employeeListPerDepartment.Count() > 0)
                        employeeListPerDepartment = employeeListPerDepartment.OrderBy(o => o.EmployeeID).ToList();
                    int indexProcessing = 1;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        prgProcessing.Maximum = employeeListPerDepartment.Count();
                    }));
                    foreach (var employee in employeeListPerDepartment)
                    {
                        // execute strore
                        EmployeeController.ExecuteSalaryData(employee.EmployeeCode, dateFrom, dateTo);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            txtStatus.Text = String.Format("{0}:  {1},   {2}:  {3}",
                                                        LanguageHelper.GetStringFromResource("commonEmployeeDepartment"),
                                                        department.DepartmentFullName,
                                                        LanguageHelper.GetStringFromResource("commonEmployeeID"),
                                                        employee.EmployeeID);
                            prgProcessing.Value = indexProcessing;
                            txtProcessing.Text = string.Format("{0} / {1}   -   {2} / {3}", deparmentIndex, departmentPerSectionList.Count(), indexProcessing, employeeListPerDepartment.Count());
                        }));
                        indexProcessing++;
                    }
                    deparmentIndex++;
                }
            }

            if (modeExecute == ModeExecute.PerDepartment)
            {
                var employeeListPerDepartment = employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == deparmentClicked).ToList();
                if (employeeListPerDepartment.Count() > 0)
                    employeeListPerDepartment = employeeListPerDepartment.OrderBy(o => o.EmployeeID).ToList();
                int indexProcessing = 1;
                Dispatcher.Invoke(new Action(() =>
                {
                    prgProcessing.Maximum = employeeListPerDepartment.Count();
                }));
                foreach (var employee in employeeListPerDepartment)
                {
                    // execute strore
                    EmployeeController.ExecuteSalaryData(employee.EmployeeCode, dateFrom, dateTo);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        txtStatus.Text = String.Format("{0}:  {1},   {2}:  {3}",
                                                            LanguageHelper.GetStringFromResource("commonEmployeeDepartment"),
                                                            deparmentClicked,
                                                            LanguageHelper.GetStringFromResource("commonEmployeeID"),
                                                            employee.EmployeeID);
                        prgProcessing.Value = indexProcessing;
                        txtProcessing.Text = string.Format("{0} / {1}", indexProcessing, employeeListPerDepartment.Count());
                    }));
                    indexProcessing++;
                }
            }
            if (modeExecute == ModeExecute.PerEmployee)
            {
                EmployeeController.ExecuteSalaryData(employeeExecuteModePerEmployee.EmployeeCode, dateFrom, dateTo);
                Dispatcher.Invoke(new Action(() =>
                {
                    prgProcessing.Maximum = 20;
                    for (int i = 1; i <= 20; i++)
                    {
                        Thread.Sleep(50);
                        prgProcessing.Value = i;
                    }
                    txtProcessing.Text = String.Format("{0}", LanguageHelper.GetStringFromResource("executeSalaryDataProcessCompleted"));
                }));
            }
            if (modeExecute == ModeExecute.All)
            {
                var departmentList = employeeList.OrderBy(o => o.DepartmentName).Select(s => s.DepartmentName.Trim().ToUpper()).Distinct().ToList();

                int deparmentIndex = 1;
                foreach (var deparment in departmentList)
                {
                    var employeeListPerDepartment = employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == deparment).ToList();
                    if (employeeListPerDepartment.Count() > 0)
                        employeeListPerDepartment = employeeListPerDepartment.OrderBy(o => o.EmployeeID).ToList();
                    int indexProcessing = 1;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        prgProcessing.Maximum = employeeListPerDepartment.Count();
                    }));
                    foreach (var employee in employeeListPerDepartment)
                    {
                        // execute strore
                        EmployeeController.ExecuteSalaryData(employee.EmployeeCode, dateFrom, dateTo);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            txtStatus.Text = String.Format("{0}:  {1},   {2}:  {3}",
                                                        LanguageHelper.GetStringFromResource("commonEmployeeDepartment"),
                                                        deparment,
                                                        LanguageHelper.GetStringFromResource("commonEmployeeID"),
                                                        employee.EmployeeID);
                            prgProcessing.Value = indexProcessing;
                            txtProcessing.Text = string.Format("{0} / {1}   -   {2} / {3}", deparmentIndex, departmentList.Count(), indexProcessing, employeeListPerDepartment.Count());
                        }));
                        indexProcessing++;
                    }
                    deparmentIndex++;
                }
            }
        }
        private void bwExecute_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            txtProcessing.Text = String.Format("{0}", LanguageHelper.GetStringFromResource("executeSalaryDataProcessCompleted"));
            txtStatus.Text = "";
            btnExecute.IsEnabled = true;
        }

        string confirmExcuteTitleTemporary = "";
        private void radExcuteByDeparment_Checked(object sender, RoutedEventArgs e)
        {
            modeExecute = ModeExecute.PerDepartment;
            confirmExcuteTitleTemporary = string.Format("{0} {1} ?", LanguageHelper.GetStringFromResource("messageConfirmExecute"),
                                                          LanguageHelper.GetStringFromResource("executeSalaryDataPerDepartment"));
        }
        private void radExcuteByEmployee_Checked(object sender, RoutedEventArgs e)
        {
            modeExecute = ModeExecute.PerEmployee;
            confirmExcuteTitleTemporary = string.Format("{0} {1} ?", LanguageHelper.GetStringFromResource("messageConfirmExecute"),
                                                                     LanguageHelper.GetStringFromResource("executeSalaryDataPerEmployee"));
        }
        private void radExcuteAll_Checked(object sender, RoutedEventArgs e)
        {
            modeExecute = ModeExecute.All;
            confirmExcuteTitleTemporary = string.Format("{0} {1} ?", LanguageHelper.GetStringFromResource("messageConfirmExecute"),
                                                                     LanguageHelper.GetStringFromResource("executeSalaryAll"));
        }

        enum ModeExecute
        {
            PerSection,
            PerDepartment,
            PerEmployee,
            All
        }

        private void radExecutePerSection_Checked(object sender, RoutedEventArgs e)
        {
            modeExecute = ModeExecute.PerSection;
            confirmExcuteTitleTemporary = string.Format("{0} {1} ?", LanguageHelper.GetStringFromResource("messageConfirmExecute"),
                                                                     LanguageHelper.GetStringFromResource("executeSalaryDataPerSection"));
        }

        private void cboSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var sectionClicked = cboSection.SelectedItem as DepartmentModel;
            //if (sectionClicked == null)
            //    return;
            //var child = departmentList.Where(w => w.ParentID == sectionClicked.DepartmentID).ToList();
            //if (child.Count > 0)
            //{
            //    cboDepartment.ItemsSource = child;
            //    cboDepartment.SelectedItem = child.FirstOrDefault();
            //}
        }
    }
}
