using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;

using TLCovidTest.ViewModels;
using TLCovidTest.DataSets;
using System.Data;
using Microsoft.Reporting.WinForms;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for AttendanceRecordDetailReportSummaryWindow.xaml
    /// </summary>
    public partial class AttendanceRecordDetailReportSummaryWindow : Window
    {
        List<AttendanceRecordViewModel> reportList;
        List<EmployeeModel> employeeList;
        DateTime dateFrom, dateTo;
        BackgroundWorker bwLoad;
        public AttendanceRecordDetailReportSummaryWindow(List<AttendanceRecordViewModel> _reportList, DateTime _dateFrom, DateTime _dateTo, List<EmployeeModel> _employeeList)
        {
            this.reportList = _reportList;
            this.dateFrom = _dateFrom;
            this.dateTo = _dateTo;
            this.employeeList = _employeeList;
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

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
            Dispatcher.Invoke(new Action(() =>
            {
                DataTable dt = new AttendanceRecordDataSet().Tables["AttendanceRecordTable"];
                int No = 1;
                foreach (var reportModel in reportList)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"]                 = No;
                    dr["EmployeeCode"]       = reportModel.EmployeeCode;
                    dr["EmployeeID"]         = reportModel.EmployeeID;
                    dr["EmployeeName"]       = reportModel.EmployeeName;
                    dr["DepartmentName"]     = employeeList.FirstOrDefault(f => f.EmployeeCode == reportModel.EmployeeCode) != null ? 
                                               employeeList.FirstOrDefault(f => f.EmployeeCode == reportModel.EmployeeCode).DepartmentName : 
                                               "";
                    
                    dr["AttendanceDate"]     = reportModel.AttendanceDate;
                    dr["DayOfWeek"]          = reportModel.DayOfWeek;
                    dr["AttendanceIn1"]      = reportModel.AttendanceIn1;
                    dr["AttendanceOut1"]     = reportModel.AttendanceOut1;
                    dr["AttendanceIn2"]      = reportModel.AttendanceIn2;
                    dr["AttendanceOut2"]     = reportModel.AttendanceOut2;

                    dr["OverTimeIn"]         = reportModel.OverTimeIn;
                    dr["OverTimeOut"]        = reportModel.OverTimeOut;
                    dr["WorkingDay"]         = reportModel.WorkingDay;
                    dr["WorkingTime"]        = reportModel.WorkingTime;
                    dr["WorkingOverTime"]    = reportModel.WorkingOverTime;
                    dr["TimeLate"]           = reportModel.TimeLate;
                    dr["OverTime2"]          = reportModel.OverTime2;
                    dr["Absent"]             = reportModel.Absent;
                    dr["Ask"]                = reportModel.Ask;
                    dr["Remarks"]            = reportModel.Remarks;

                    var employeeModel = employeeList.Where(w => w.EmployeeCode == reportModel.EmployeeCode).FirstOrDefault();
                    if (employeeModel != null)
                    {
                        dr["EmployeeID"]     = employeeModel.EmployeeID;
                        dr["EmployeeName"]   = employeeModel.EmployeeName;
                        dr["JoinDate"]       = employeeModel.JoinDate;
                        dr["DepartmentName"] = employeeModel.DepartmentName;
                    }

                    dt.Rows.Add(dr);
                    No++;
                }
                e.Result = dt;
            }));
        }

        private void bwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var dt = e.Result;

            ReportParameter  rp = new ReportParameter("DateFrom", String.Format("{0:dd/MM/yyyy}", dateFrom));
            ReportParameter rp1 = new ReportParameter("DateTo", String.Format("{0:dd/MM/yyyy}", dateTo));
            ReportDataSource rds = new ReportDataSource();
            rds.Name = "AttendanceSummaryDetail";
            rds.Value = dt;
            //reportViewer.LocalReport.ReportPath = @"C:\Users\IT02\OneDrive\SVProject\1001\SaoVietPersonal\SaoVietPersonal\PersonalSV\Reports\AttendanceRecordSummaryReport.rdlc";
            reportViewer.LocalReport.ReportPath = @"Reports\AttendanceRecordSummaryReport.rdlc";
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            reportViewer.LocalReport.DataSources.Add(rds);
            reportViewer.RefreshReport();
            this.Cursor = null;
        }
    }
}
