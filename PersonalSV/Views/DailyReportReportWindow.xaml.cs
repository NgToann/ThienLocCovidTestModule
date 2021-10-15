using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

using TLCovidTest.Models;
using TLCovidTest.DataSets;
using System.Data;
using Microsoft.Reporting.WinForms;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for DailyReportReportWindow.xaml
    /// </summary>
    public partial class DailyReportReportWindow : Window
    {
        BackgroundWorker bwLoad;
        List<DailyReportModel> dailyReport;
        DateTime date;
        DateTime _dateTo;
        public DailyReportReportWindow(List<DailyReportModel> _dailyReport, DateTime _date, DateTime _dateTo)
        {
            this.dailyReport = _dailyReport;
            this.date = _date;
            this._dateTo = _dateTo;

            bwLoad = new BackgroundWorker();
            bwLoad.DoWork +=new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);
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
            Dispatcher.Invoke(new Action(() =>
            {
                DataTable dt = new DailyReportDataSet().Tables["DailyReportTable"];
                if (dailyReport.Count() > 0)
                    dailyReport = dailyReport.OrderBy(o => o.DateSearch).ThenBy(t => t.DepartmentName).ThenBy(th => th.EmployeeID).ToList();
                int No = 1;
                foreach (var reportModel in dailyReport)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"] = No;
                    dr["DateSearch"] = reportModel.DateSearch;
                    dr["EmployeeName"] = reportModel.EmployeeName;
                    dr["EmployeeCode"] = reportModel.EmployeeID;
                    dr["DepartmentName"] = reportModel.DepartmentName;
                    dr["TimeIn"] = reportModel.TimeInView;
                    dr["TimeOut"] = reportModel.TimeOutView;
                    dr["Remarks"] = reportModel.Remarks;
                    dt.Rows.Add(dr);
                    No++;
                }
                e.Result = dt;
            }));
        }
        private void bwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var dt = e.Result;
            string dateView = String.Format("{0:dd-MM-yyyy}", date);
            if (date < _dateTo)
                dateView = String.Format(" {0:dd-MM-yyyy} -> {1:dd-MM-yyyy}", date, _dateTo);
            ReportParameter rp = new ReportParameter("Date", dateView);

            ReportDataSource rds = new ReportDataSource();
            rds.Name = "DailyReportDetail";
            rds.Value = dt;
            //reportViewer.LocalReport.ReportPath = @"C:\Users\IT02\OneDrive\SVProject\1001\SaoVietPersonal\SaoVietPersonal\PersonalSV\Reports\DailyReport.rdlc";
            reportViewer.LocalReport.ReportPath = @"Reports\DailyReport.rdlc";
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp });
            reportViewer.LocalReport.DataSources.Add(rds);
            reportViewer.RefreshReport();
            this.Cursor = null;
        }
    }
}
