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
using System.Data;
using TLCovidTest.DataSets;
using Microsoft.Reporting.WinForms;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for SalarySummaryReportWindow.xaml
    /// </summary>
    public partial class WorkingDaySummaryReportWindow : Window
    {
        BackgroundWorker bwLoad;
        BackgroundWorker bwPreview;

        List<DepartmentModel> departmentList;
        List<EmployeeModel> employeeList;
        List<EmployeeModel> employeeSelectedList;
        List<AttendanceRecordModel> attendanceRecordList;
        DateTime dateFrom, dateTo;
        private string _SAOVIET = "SaoViet Corporation";

        public WorkingDaySummaryReportWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwPreview = new BackgroundWorker();
            bwPreview.DoWork += new DoWorkEventHandler(bwPreview_DoWork);
            bwPreview.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwPreview_RunWorkerCompleted);

            departmentList = new List<DepartmentModel>();
            employeeList = new List<EmployeeModel>();
            employeeSelectedList = new List<EmployeeModel>();

            attendanceRecordList = new List<AttendanceRecordModel>();

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
            // Binding Combobox
            var sectionListFull = new List<DepartmentModel>();
            sectionListFull.Add(new DepartmentModel { DepartmentName = _SAOVIET });
            var sectionList = departmentList.Where(w => String.IsNullOrEmpty(w.ParentID) == true).ToList();
            sectionListFull.AddRange(sectionList);
            if (sectionList.Count() > 0)
            {
                cboSection.ItemsSource = sectionListFull;
                cboSection.SelectedItem = sectionListFull.FirstOrDefault();
            }
            dpDateFrom.SelectedDate = DateTime.Now.Date;
            dpDateTo.SelectedDate = DateTime.Now.Date;
            this.Cursor = null;
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            // By Section
            var sectionClicked = cboSection.SelectedItem as DepartmentModel;
            if (sectionClicked != null)
            {
                employeeSelectedList = new List<EmployeeModel>();
                if (sectionClicked.DepartmentName == _SAOVIET)
                {
                    employeeSelectedList = employeeList.ToList();
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
                            employeeSelectedList.AddRange(employeeByChildDept);
                        }
                    }
                    else
                    {
                        employeeSelectedList = employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == sectionClicked.DepartmentName).ToList();
                    }
                }
            }

            // By Department
            var departmentClicked = cboDepartment.SelectedItem as DepartmentModel;
            if (departmentClicked != null)
            {
                employeeSelectedList = new List<EmployeeModel>();
                employeeSelectedList = employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == departmentClicked.DepartmentFullName).ToList();
            }

            // By Employee
            string employeeSearch = txtEmployeeSearch.Text.Trim().ToUpper().ToString();
            if (!String.IsNullOrEmpty(employeeSearch))
            {
                employeeSelectedList = new List<EmployeeModel>();
                employeeSelectedList = employeeList.Where(w => w.EmployeeCode == employeeSearch || w.EmployeeID.Trim().ToUpper().ToString() == employeeSearch).ToList();
            }


            if (bwPreview.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                btnPreview.IsEnabled = false;
                btnPreview.IsDefault = false;
                attendanceRecordList = new List<AttendanceRecordModel>();
                
                dateFrom = dpDateFrom.SelectedDate.Value;
                dateTo = dpDateTo.SelectedDate.Value;
                bwPreview.RunWorkerAsync();
            }
        }
        private void bwPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (employeeSelectedList.Count() > 0)
                    employeeSelectedList = employeeSelectedList.OrderBy(o => o.DepartmentName.Trim().ToUpper().ToString()).ThenBy(t => t.EmployeeID.Trim().ToUpper().ToString()).ToList();
                foreach (var employee in employeeSelectedList)
                {
                    attendanceRecordList.AddRange(AttendanceRecordController.GetAttendanceRecordByEmployeeCodeFromTo(employee.EmployeeCode, dateFrom, dateTo).ToList());
                }

                DataTable dt = new SalarySummaryDataSet().Tables["SalarySummaryTable"];
                int No = 1;
                var employeeCodeList = attendanceRecordList.Select(s => s.EmployeeCode).Distinct().ToList();

                foreach (var employeeCode in employeeCodeList)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"] = No;
                    var employeeByCode = employeeList.FirstOrDefault(f => f.EmployeeCode == employeeCode);
                    if (employeeByCode != null)
                    {
                        dr["EmployeeCode"]   = employeeByCode.EmployeeCode;
                        dr["EmployeeID"]     = employeeByCode.EmployeeID;
                        dr["EmployeeName"]   = employeeByCode.EmployeeName;
                        dr["DepartmentName"] = employeeByCode.DepartmentName;
                    }

                    var attendanceSumByEmployee = attendanceRecordList.Where(w => w.EmployeeCode == employeeCode).ToList();
                    dr["TotalWorkingDay"]   = attendanceSumByEmployee.Sum(s => s.WorkingDay);
                    dr["TotalWorkingTime"]  = attendanceSumByEmployee.Sum(s => s.WorkingTime);
                    dr["TotalOverTime"]     = attendanceSumByEmployee.Sum(s => s.WorkingOverTime);
                    dr["TotalLate"]         = attendanceSumByEmployee.Sum(s => s.TimeLate);
                    dr["TotalOverTime2"]    = attendanceSumByEmployee.Sum(s => s.OverTime2);
                    dr["TotalAbsent"]       = attendanceSumByEmployee.Sum(s => s.Absent);

                    dt.Rows.Add(dr);
                    No++;
                }
                e.Result = dt;

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
        private void bwPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var dt = e.Result;

            ReportParameter rp = new ReportParameter("DateFrom", String.Format("{0:dd/MM/yyyy}", dateFrom));
            ReportParameter rp1 = new ReportParameter("DateTo", String.Format("{0:dd/MM/yyyy}", dateTo));
            ReportDataSource rds = new ReportDataSource();
            rds.Name = "SalarySummaryDetail";
            rds.Value = dt;
            //reportViewer.LocalReport.ReportPath = @"C:\Users\IT02\OneDrive\SVProject\1001\SaoVietPersonal\SaoVietPersonal\PersonalSV\Reports\SalarySummaryReport.rdlc";
            reportViewer.LocalReport.ReportPath = @"Reports\SalarySummaryReport.rdlc";
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(rds);
            reportViewer.RefreshReport();
            btnPreview.IsEnabled = true;
            this.Cursor = null;
        }

        private void cboSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sectionClicked = cboSection.SelectedItem as DepartmentModel;
            cboDepartment.ItemsSource = new List<DepartmentModel>();
            if (sectionClicked != null)
            {
                cboDepartment.ItemsSource = new List<DepartmentModel>();
                var childDept = departmentList.Where(w => w.ParentID == sectionClicked.DepartmentID).ToList();
                if (childDept.Count() > 1)
                {
                    cboDepartment.ItemsSource = childDept;
                    cboDepartment.Items.Refresh();
                }
            }
        }

        private void txtEmployeeSearch_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            btnPreview.IsDefault = true;
        }
        private void txtEmployeeSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnPreview.IsDefault = true;
        }
    }
}
