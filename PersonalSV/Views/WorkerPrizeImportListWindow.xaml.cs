using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TLCovidTest.Controllers;
using TLCovidTest.Helpers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for WorkerPrizeImportListWindow.xaml
    /// </summary>
    public partial class WorkerPrizeImportListWindow : Window
    {
        BackgroundWorker bwReadExcel;
        BackgroundWorker bwInsert;
        private string filePath = "";
        private List<WorkerPrizeModel> giftList;
        public WorkerPrizeImportListWindow()
        {
            giftList = new List<WorkerPrizeModel>();

            bwReadExcel = new BackgroundWorker();
            bwReadExcel.DoWork += BwReadExcel_DoWork;
            bwReadExcel.RunWorkerCompleted += BwReadExcel_RunWorkerCompleted;


            bwInsert = new BackgroundWorker();
            bwInsert.DoWork += BwInsert_DoWork;
            bwInsert.RunWorkerCompleted += BwInsert_RunWorkerCompleted;

            InitializeComponent();
        }

        private void btnImportExcel_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Import Worker Gift List";
            openFileDialog.Filter = "EXCEL Files (*.xls, *.xlsx)|*.xls;*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                if (bwReadExcel.IsBusy == false)
                {
                    this.Cursor = Cursors.Wait;
                    bwReadExcel.RunWorkerAsync();
                }
            }
            else
            {
                this.Close();
            }
        }
        private void BwReadExcel_DoWork(object sender, DoWorkEventArgs e)
        {
            Excel.Application excelApplication = new Excel.Application();
            Excel.Workbook excelWorkbook = excelApplication.Workbooks.Open(filePath);
            //excelApplication.Visible = true;
            Excel.Worksheet excelWorksheet;
            Excel.Range excelRange;
            try
            {
                excelWorksheet = (Excel.Worksheet)excelWorkbook.Worksheets[1];
                excelRange = excelWorksheet.UsedRange;
                for (int i = 2; i <= excelRange.Rows.Count; i++)
                {
                    var workerId    = (excelRange.Cells[i, 1] as Excel.Range).Value2;
                    var cardId      = (excelRange.Cells[i, 2] as Excel.Range).Value2;
                    var fullName    = (excelRange.Cells[i, 3] as Excel.Range).Value2;
                    var dept        = (excelRange.Cells[i, 4] as Excel.Range).Value2;
                    if (workerId != null)
                    {
                        var giftModel = new WorkerPrizeModel
                        {
                            WorkerId = workerId,
                            CardId = cardId,
                            FullName = fullName,
                            DepartmentName = dept
                        };

                        giftList.Add(giftModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.InnerException.Message).ToString();
                    giftList.Clear();
                }));
            }
            finally
            {
                excelWorkbook.Close(false, Missing.Value, Missing.Value);
                excelApplication.Quit();
            }

        }
        private void BwReadExcel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            if (giftList.Count() > 0)
            {
                dgWorkerPrize.ItemsSource = giftList;
                dgWorkerPrize.Items.Refresh();
                MessageBox.Show(string.Format("Read Completed. {0} Records", giftList.Count()), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void dgWorkerPrize_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (bwInsert.IsBusy == false && dgWorkerPrize.ItemsSource != null)
            {
                var insertList = dgWorkerPrize.ItemsSource.OfType<WorkerPrizeModel>().ToList();
                this.Cursor = Cursors.Wait;
                bwInsert.RunWorkerAsync(insertList);
            }
        }
        private void BwInsert_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            if ((bool)e.Result)
            {
                MessageBox.Show("Saved !", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BwInsert_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = true;
                var source = e.Argument as List<WorkerPrizeModel>;
                WorkerPrizeController.Delete();
                foreach (var item in source)
                {
                    WorkerPrizeController.Insert(item);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        dgWorkerPrize.SelectedItem = item;
                        dgWorkerPrize.ScrollIntoView(item);
                    }));
                }
            }
            catch (Exception ex)
            {
                e.Result = false
                    ;
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message.ToString());
                }));
            }
        }
    }
}
