using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using EXCEL = Microsoft.Office.Interop.Excel;
using TLCovidTest.Helpers;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for WorkerCheckInReportWindow.xaml
    /// </summary>
    public partial class WorkerCheckInReportWindow : Window
    {
        List<WorkerCheckInModel> checkInListFromTo;
        List<PatientModel> patientListFromTo;
        List<EmployeeModel> employeeList;
        List<SourceModel> sourceFromTo;

        BackgroundWorker bwLoad;
        BackgroundWorker bwExportExcel;
        BackgroundWorker bwReport;

        private string normal = "", infected = "", suspected = "", others = "";

        public WorkerCheckInReportWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            bwExportExcel = new BackgroundWorker();
            bwExportExcel.DoWork += BwExportExcel_DoWork;
            bwExportExcel.RunWorkerCompleted += BwExportExcel_RunWorkerCompleted;

            bwReport = new BackgroundWorker();
            bwReport.DoWork += BwReport_DoWork; 
            bwReport.RunWorkerCompleted += BwReport_RunWorkerCompleted;

            checkInListFromTo = new List<WorkerCheckInModel>();
            employeeList = new List<EmployeeModel>();
            patientListFromTo = new List<PatientModel>();
            sourceFromTo = new List<SourceModel>();

            normal = LanguageHelper.GetStringFromResource("clinicRadNormal");
            infected = LanguageHelper.GetStringFromResource("clinicRadInfectedCovid");
            suspected = LanguageHelper.GetStringFromResource("clinicRadSuspectedCovid");
            others = LanguageHelper.GetStringFromResource("clinicRadOthers");

            InitializeComponent();
        }

        

        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            btnPreview.IsEnabled = true;
        }

        private void BwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                employeeList = EmployeeController.GetAvailable();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message.ToString());
                }));
            }
        }

        private void dgReport_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (!bwReport.IsBusy)
            {
                btnPreview.IsEnabled = false;
                dgReport.ItemsSource = null;
                var dateFrom = dpFrom.SelectedDate.Value;
                var dateTo = dpTo.SelectedDate.Value;
                var findWhat = txtFindWhat.Text.Trim().ToUpper().ToString();
                object[] par = new object[] { dateFrom, dateTo, findWhat };
                bwReport.RunWorkerAsync(par);
            }
        }

        private void BwReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            btnPreview.IsEnabled = true;
        }

        private void BwReport_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var args = e.Argument as object[];
                var dateFrom = (DateTime)args[0];
                var dateTo = (DateTime)args[1];
                var findWhat = args[2] as string;

                patientListFromTo = PatientController.GetFromTo(dateFrom, dateTo);
                checkInListFromTo = WorkerCheckInController.GetFromTo(dateFrom, dateTo);
                sourceFromTo = SourceController.SelectSourceByDateFromTo(dateFrom, dateTo);


                Dispatcher.Invoke(new Action(() =>
                {
                    List<DisplayDataModel> dataList = new List<DisplayDataModel>();

                    if (string.IsNullOrEmpty(findWhat))
                    {
                        if (patientListFromTo.Count() > 0)
                        {
                            dateFrom = patientListFromTo.Min(m => m.ConfirmDate);
                            dateTo = patientListFromTo.Max(m => m.ConfirmDate);
                        }

                        for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
                        {
                            var empIds = patientListFromTo.Where(w => w.ConfirmDate == date).Select(s => s.EmployeeID).Distinct().ToList();
                            foreach (var id in empIds)
                            {
                                var empById = employeeList.FirstOrDefault(f => f.EmployeeID == id);
                                if (empById == null)
                                    continue;

                                var patientByIdByDate = patientListFromTo.FirstOrDefault(f => f.ConfirmDate == date && f.EmployeeID == id);
                                var checkInByDateByEmpCode = checkInListFromTo.Where(w => w.CheckInDate == date && w.EmployeeCode == empById.EmployeeCode).ToList();
                                var sourceByDateByEmpCode = sourceFromTo.Where(w => w.SourceDate == date && w.EmployeeCode == empById.EmployeeCode).ToList();

                                var timeInScan = checkInByDateByEmpCode.Count() > 0 ? checkInByDateByEmpCode.Min(m => m.RecordTime) : "";
                                var timeInOrigin = sourceByDateByEmpCode.Count() > 0 ? sourceByDateByEmpCode.Min(m => m.SourceTime) : "";

                                var displayDataModel = new DisplayDataModel
                                {
                                    EmployeeCode = empById.EmployeeCode,
                                    EmployeeID = empById.EmployeeID,
                                    EmployeeName = empById.EmployeeName,
                                    DepartmentName = empById.DepartmentName,
                                    ConfirmDate = date,
                                    Remarks = patientByIdByDate.Remarks,
                                    StateIndexDisplay = getStateByIndex(patientByIdByDate.StateIndex),
                                    TimeInScan = timeInScan,
                                    TimeInOrigin = timeInOrigin
                                };
                                dataList.Add(displayDataModel);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in checkInListFromTo)
                        {
                            var empByCode = employeeList.FirstOrDefault(f => f.EmployeeCode.Trim().ToUpper() == item.EmployeeCode.Trim().ToUpper());
                            if (empByCode == null)
                                continue;
                            var sourceByDateByEmpCode = sourceFromTo.Where(w => w.SourceDate == item.CheckInDate && w.EmployeeCode == empByCode.EmployeeCode).ToList();
                            var timeInOrigin = sourceByDateByEmpCode.Count() > 0 ? sourceByDateByEmpCode.Min(m => m.SourceTime) : "";
                            var displayDataModel = new DisplayDataModel
                            {
                                EmployeeCode = empByCode.EmployeeCode,
                                EmployeeID = empByCode.EmployeeID,
                                EmployeeName = empByCode.EmployeeName,
                                DepartmentName = empByCode.DepartmentName,
                                ConfirmDate = item.CheckInDate,
                                StateIndexDisplay = getStateByIndex(item.PatientIndex),
                                TimeInScan = item.RecordTime,
                                TimeInOrigin = timeInOrigin
                            };
                            dataList.Add(displayDataModel);
                        }
                    }

                    if (dataList.Count() > 0)
                        dataList = dataList.OrderBy(o => o.ConfirmDate).ThenBy(th => th.DepartmentName).ThenBy(th => th.EmployeeID).ThenBy(th => th.TimeInScan).ToList();

                    dgReport.ItemsSource = dataList;
                    dgReport.Items.Refresh();
                }));
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.InnerException.Message.ToString());
                }));
            }
        }

        private string getStateByIndex(int index)
        {
            if (index == 0)
                return normal;
            else if (index == 1)
                return infected;
            else if (index == 2)
                return suspected;
            else if (index == 3)
                return others;
            else
                return "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bwLoad.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
            dpFrom.SelectedDate = DateTime.Now.Date;
            dpTo.SelectedDate = DateTime.Now.Date;
        }
        
        private void txtFindWhat_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnPreview.IsDefault = true;
        }
        
        private class DisplayDataModel
        {
            public string EmployeeCode { get; set; }
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public DateTime ConfirmDate { get; set; }
            public string DepartmentName { get; set; }
            public string TimeInScan { get; set; }
            public string TimeInOrigin { get; set; }
            public string StateIndexDisplay { get; set; }
            public string Remarks { get; set; }
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (dgReport.ItemsSource == null)
                return;

            if (bwExportExcel.IsBusy == false)
            {
                var sources = dgReport.ItemsSource.OfType<DisplayDataModel>().ToList();
                btnExportExcel.IsEnabled = false;
                this.Cursor = Cursors.Wait;
                bwExportExcel.RunWorkerAsync(sources);
            }
        }


        private void BwExportExcel_DoWork(object sender, DoWorkEventArgs e)
        {
            EXCEL._Application excel = new Microsoft.Office.Interop.Excel.Application();
            EXCEL._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            EXCEL._Worksheet worksheet = null;

            var sources = e.Argument as List<DisplayDataModel>;

            try
            {
                worksheet = workbook.ActiveSheet;
                worksheet.Cells.HorizontalAlignment = EXCEL.XlHAlign.xlHAlignCenter;
                worksheet.Cells.Font.Name = "Arial";
                worksheet.Cells.Font.Size = 10;
                string reportName = String.Format("PatientReport{0:ddMMyyyy}", DateTime.Now.Date);
                worksheet.Name = reportName;

                worksheet.Cells.Rows[1].Font.Size = 11;
                worksheet.Cells.Rows[1].Font.FontStyle = "Bold";

                var headerList = new List<String>();
                headerList.Add("EmployeeCode");
                headerList.Add("EmployeeID");
                headerList.Add("FullName");
                headerList.Add("Department(Line)");
                headerList.Add("Date");
                headerList.Add("TimeIn(Scan)");
                headerList.Add("TimeOut(Origin)");
                headerList.Add("Status");
                headerList.Add("Remarks");

                for (int i = 0; i < headerList.Count(); i++)
                {
                    worksheet.Cells[1, i + 1] = headerList[i];
                }
                int rowIndex = 2;
                foreach (var item in sources)
                {
                    worksheet.Cells[rowIndex, 1] = String.Format("'{0}", item.EmployeeCode);
                    worksheet.Cells[rowIndex, 2] = item.EmployeeID;
                    worksheet.Cells[rowIndex, 3] = item.EmployeeName;
                    worksheet.Cells[rowIndex, 4] = item.DepartmentName;
                    worksheet.Cells[rowIndex, 5] = item.ConfirmDate;
                    worksheet.Cells[rowIndex, 6] = item.TimeInScan;
                    worksheet.Cells[rowIndex, 7] = item.TimeInOrigin;
                    worksheet.Cells[rowIndex, 8] = item.StateIndexDisplay;
                    worksheet.Cells[rowIndex, 9] = item.Remarks;

                    rowIndex++;
                    Dispatcher.Invoke(new Action(() => {
                        dgReport.SelectedItem = item;
                        dgReport.ScrollIntoView(item);
                    }));
                }
                worksheet.Cells.Rows[1].Font.FontStyle = "Bold";

                Dispatcher.Invoke(new Action(() =>
                {
                    if (workbook != null)
                    {
                        var sfd = new System.Windows.Forms.SaveFileDialog();
                        sfd.Title = "ThienLoc Export Excel File";
                        sfd.Filter = "Excel Documents (*.xls)|*.xls|Excel Documents (*.xlsx)|*.xlsx";
                        sfd.FilterIndex = 2;
                        sfd.FileName = reportName;
                        if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            workbook.SaveAs(sfd.FileName);
                            MessageBox.Show("Export Successful !", "ThienLoc Export Excel File", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        excel.Quit();
                        workbook = null;
                        excel = null;
                    }
                }));
            }
            catch (System.Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message, "ThienLoc Export Excel File", MessageBoxButton.OK, MessageBoxImage.Error);
                    excel.Quit();
                    workbook = null;
                    excel = null;
                }));

            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
        private void BwExportExcel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            btnExportExcel.IsEnabled = true;
        }
    }
}
