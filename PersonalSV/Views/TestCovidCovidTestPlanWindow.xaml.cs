using TLCovidTest.Models;
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
    /// Interaction logic for TestCovidCovidTestPlanWindow.xaml
    /// </summary>
    public partial class TestCovidCovidTestPlanWindow : Window
    {
        BackgroundWorker bwExportExcel;
        List<TestRandomModel> testRandomList;
        private DateTime filterDate;
        public TestCovidCovidTestPlanWindow(List<TestRandomModel> testRandomList)
        {
            this.testRandomList = testRandomList;

            bwExportExcel = new BackgroundWorker();
            bwExportExcel.DoWork += BwExportExcel_DoWork;
            bwExportExcel.RunWorkerCompleted += BwExportExcel_RunWorkerCompleted;

            InitializeComponent();
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

            try
            {
                var sources = e.Argument as List<TestRandomModel>;

                worksheet = workbook.ActiveSheet;
                worksheet.Cells.HorizontalAlignment = EXCEL.XlHAlign.xlHAlignCenter;
                worksheet.Cells.Font.Name = "Arial";
                worksheet.Cells.Font.Size = 10;
                string reportName = String.Format("CovidTestPlan{0:ddMMyyyy}", filterDate);
                worksheet.Name = reportName;

                worksheet.Cells.Rows[1].Font.Size = 11;
                worksheet.Cells.Rows[1].Font.FontStyle = "Bold";

                var headerList = new List<String>();
                headerList.Add("EmployeeCode");
                headerList.Add("EmployeeID");
                headerList.Add("FullName");
                headerList.Add("Department(Line)");
                headerList.Add("Test Date");

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

                    rowIndex++;
                    Dispatcher.Invoke(new Action(() => {
                        dgRandomList.SelectedItem = item;
                        dgRandomList.ScrollIntoView(item);
                    }));
                }
                worksheet.Cells.Rows[1].Font.FontStyle = "Bold";

                Dispatcher.Invoke(new Action(() =>
                {
                    if (workbook != null)
                    {
                        var sfd = new System.Windows.Forms.SaveFileDialog();
                        sfd.Title = "SV-HRS Export Excel File";
                        sfd.Filter = "Excel Documents (*.xls)|*.xls|Excel Documents (*.xlsx)|*.xlsx";
                        sfd.FilterIndex = 2;
                        sfd.FileName = reportName;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgRandomList.ItemsSource = testRandomList;
            dgRandomList.Items.Refresh();

            dpTestDate.SelectedDate = DateTime.Now;
            filterDate = dpTestDate.SelectedDate.Value.Date;
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (dgRandomList.ItemsSource == null)
                return;

            if (bwExportExcel.IsBusy == false)
            {
                var sources = dgRandomList.ItemsSource.OfType<TestRandomModel>().ToList();
                btnExportExcel.IsEnabled = false;
                this.Cursor = Cursors.Wait;
                bwExportExcel.RunWorkerAsync(sources);
            }
        }

        private void dgRandomList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
      
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            filterDate = dpTestDate.SelectedDate.Value.Date;
            if (dgRandomList.ItemsSource != null)
            {
                var sources = testRandomList.Where(w => w.TestDate == filterDate).ToList();
                dgRandomList.ItemsSource = sources;
                dgRandomList.Items.Refresh();
            }
        }
    }
}
