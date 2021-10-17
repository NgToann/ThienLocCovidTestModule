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

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for TestRandomReportResultWindow.xaml
    /// </summary>
    public partial class TestRandomReportResultWindow : Window
    {
        List<TestRandomModel> testRandomByDate;
        List<EmployeeModel> employeeList;
        BackgroundWorker bwLoad;
        BackgroundWorker bwExportExcel;
        public TestRandomReportResultWindow()
        {
            testRandomByDate = new List<TestRandomModel>();
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork; ;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted; ;

            bwExportExcel = new BackgroundWorker();
            bwExportExcel.DoWork += BwExportExcel_DoWork; ;
            bwExportExcel.RunWorkerCompleted += BwExportExcel_RunWorkerCompleted; ;

            employeeList = new List<EmployeeModel>();

            InitializeComponent();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bwLoad.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
            dpFilterDate.SelectedDate = DateTime.Now.Date;
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
        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            btnPreview.IsEnabled = true;
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dateSearch = dpFilterDate.SelectedDate.Value;
                testRandomByDate = TestRandomController.GetByDate(dateSearch);

                FilterData(dateSearch);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void FilterData(DateTime dateSearch)
        {
            var findWhat = txtFindWhat.Text.Trim().ToUpper().ToString();
            List<DisplayDataModel> dataList = new List<DisplayDataModel>();

            foreach (var testItem in testRandomByDate)
            {
                var empById = employeeList.FirstOrDefault(f => f.EmployeeCode.Trim().ToLower().ToString() == testItem.EmployeeCode.Trim().ToLower().ToString());
                if (empById != null)
                {
                    var displayModel = new DisplayDataModel
                    {
                        EmployeeID = empById.EmployeeID,
                        EmployeeCode = empById.EmployeeCode,
                        EmployeeName = empById.EmployeeName,
                        DepartmentName = empById.DepartmentName,
                        TestDate = dateSearch,
                        TimeIn = testItem.TimeIn,
                        Result = testItem.Result,
                        ConfirmTime = testItem.UpdateResultTime,
                        ConfirmedBy = testItem.PersonConfirm,
                        Remark = testItem.Remark
                    };
                    dataList.Add(displayModel);
                }
            }

            dataList = dataList.OrderBy(o => o.TestDate).ThenBy(th => th.DepartmentName).ThenBy(th => th.EmployeeID).ToList();

            if (!string.IsNullOrEmpty(findWhat))
            {
                dataList = dataList.Where(w => w.EmployeeCode == findWhat || w.EmployeeID.Trim().ToUpper() == findWhat).ToList();
            }
            dgReport.ItemsSource = dataList;
            dgReport.Items.Refresh();
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
        private void BwExportExcel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            btnExportExcel.IsEnabled = true;
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
                string reportName = String.Format("CovidTestReport{0:ddMMyyyy}", DateTime.Now.Date);
                worksheet.Name = reportName;

                worksheet.Cells.Rows[1].Font.Size = 11;
                worksheet.Cells.Rows[1].Font.FontStyle = "Bold";

                var headerList = new List<String>();
                headerList.Add("EmployeeCode");
                headerList.Add("EmployeeID");
                headerList.Add("FullName");
                headerList.Add("Department(Line)");
                headerList.Add("TestDate");
                headerList.Add("TimeIn");
                headerList.Add("TestResult");
                headerList.Add("ConfirmedTime");
                headerList.Add("ConfirmedBy");
                headerList.Add("Remark");

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
                    worksheet.Cells[rowIndex, 5] = item.TestDate;
                    worksheet.Cells[rowIndex, 6] = item.TimeIn;
                    worksheet.Cells[rowIndex, 7] = item.Result;
                    worksheet.Cells[rowIndex, 8] = item.ConfirmTime;
                    worksheet.Cells[rowIndex, 9] = item.ConfirmedBy;
                    worksheet.Cells[rowIndex, 10] = item.Remark;

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
                    }
                }));
            }
            catch (System.Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message, "ThienLoc Export Excel File", MessageBoxButton.OK, MessageBoxImage.Error);
                }));
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void txtFindWhat_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnPreview.IsDefault = true;
        }

        private void dgReport_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }


        private class DisplayDataModel
        {
            public string EmployeeCode { get; set; }
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string DepartmentName { get; set; }
            public DateTime TestDate { get; set; }
            public string TimeIn { get; set; }
            public string ConfirmTime { get; set; }
            public string ConfirmedBy { get; set; }
            public string Remark { get; set; }
            public string Result { get; set; }
        }
    }
}
