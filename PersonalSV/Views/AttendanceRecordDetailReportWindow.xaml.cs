using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;

using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using TLCovidTest.DataSets;


using Microsoft.Reporting.WinForms;
using System.Data;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for AttendanceRecordDetailReportWindow.xaml
    /// </summary>
    public partial class AttendanceRecordDetailReportWindow : Window
    {
        List<AttendanceRecordViewModel> reportList;
        List<DepartmentModel> departmentList;
        List<EmployeeModel> employeeList;
        BackgroundWorker bwLoad;

        public AttendanceRecordDetailReportWindow(List<AttendanceRecordViewModel> _reportList, List<DepartmentModel> _departmentList, List<EmployeeModel> _employeeList)
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);
            bwLoad.WorkerSupportsCancellation = true;
            this.reportList = _reportList;
            this.departmentList = _departmentList;
            this.employeeList = _employeeList;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bwLoad.IsBusy == false)
            {
                this.Cursor = null;
                bwLoad.RunWorkerAsync();
            }
        }

        private void bwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                DataTable dt = new AttendanceRecordDataSet().Tables["AttendanceRecordTable"];
                foreach (var reportModel in reportList)
                {
                    DataRow dr = dt.NewRow();
                    dr["EmployeeCode"]          = reportModel.EmployeeCode;
                    dr["AttendanceDate"]        = reportModel.AttendanceDate;
                    dr["DayOfWeek"]             = reportModel.DayOfWeek;
                    dr["AttendanceIn1"]         = DisplayTime(reportModel.AttendanceIn1);
                    dr["AttendanceOut1"]        = DisplayTime(reportModel.AttendanceOut1);
                    dr["AttendanceIn2"]         = DisplayTime(reportModel.AttendanceIn2);
                    dr["AttendanceOut2"]        = DisplayTime(reportModel.AttendanceOut2);

                    dr["OverTimeIn"]            = DisplayTime(reportModel.OverTimeIn);
                    dr["OverTimeOut"]           = DisplayTime(reportModel.OverTimeOut);
                    dr["WorkingDay"]            = reportModel.WorkingDay;
                    dr["WorkingTime"]           = reportModel.WorkingTime;
                    dr["WorkingOverTime"]       = reportModel.WorkingOverTime;
                    dr["TimeLate"]              = reportModel.TimeLate;
                    dr["OverTime2"]             = reportModel.OverTime2;
                    dr["Absent"]                = reportModel.Absent;
                    dr["Ask"]                   = reportModel.Ask;
                    dr["Remarks"]               = reportModel.Remarks;

                    var employeeModel = employeeList.FirstOrDefault(w => w.EmployeeCode.Trim().ToUpper() == reportModel.EmployeeCode.Trim().ToUpper());
                    if (employeeModel != null)
                    {
                        dr["EmployeeID"]        = employeeModel.EmployeeID;
                        dr["EmployeeName"]      = employeeModel.EmployeeName;
                        dr["JoinDate"]          = employeeModel.JoinDate;
                        dr["DepartmentName"]    = employeeModel.DepartmentName;
                    }

                    dt.Rows.Add(dr);
                }
                e.Result = dt;
            }));
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

        private void bwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var dt = e.Result;
            //ReportParameter rp = new ReportParameter("Line", "");

            ReportDataSource rds = new ReportDataSource();
            rds.Name = "AttendanceRecordDetailReport";
            rds.Value = dt;
            //reportViewer.LocalReport.ReportPath = @"C:\Users\IT02\OneDrive\SVProject\1001\SaoVietPersonal\SaoVietPersonal\PersonalSV\Reports\AttendanceRecordDetailReport.rdlc";
            reportViewer.LocalReport.ReportPath = @"Reports\AttendanceRecordDetailReport.rdlc";
            //reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp });
            reportViewer.LocalReport.DataSources.Add(rds);
            reportViewer.RefreshReport();
            this.Cursor = null;
        }
    }
}
