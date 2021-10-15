using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;

using TLCovidTest.Models;
using TLCovidTest.Controllers;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for ImportRemarksWorkerWindow.xaml
    /// </summary>
    public partial class ImportRemarksWorkerWindow : Window
    {
        string[] filePathArray;
        BackgroundWorker bwReadExcel;
        BackgroundWorker bwImport;
        List<WorkerRemarkModel> workerRemarksList;
        public ImportRemarksWorkerWindow()
        {
            bwReadExcel = new BackgroundWorker();
            bwReadExcel.WorkerSupportsCancellation = true;
            bwReadExcel.DoWork += BwReadExcel_DoWork;
            bwReadExcel.RunWorkerCompleted += BwReadExcel_RunWorkerCompleted;

            bwImport = new BackgroundWorker();
            bwImport.WorkerSupportsCancellation = true;
            bwImport.DoWork += BwImport_DoWork;
            bwImport.RunWorkerCompleted += BwImport_RunWorkerCompleted;

            workerRemarksList = new List<WorkerRemarkModel>();

            InitializeComponent();
        }

        private void BwImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            if (result == true)
            {
                MessageBox.Show("Saved !", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                txtStatus.Text = "Completed !";
            }
            this.Cursor = null;
            btnImport.IsEnabled = true;
        }

        private void BwImport_DoWork(object sender, DoWorkEventArgs e)
        {
            var importList = e.Argument as List<WorkerRemarkModel>;
            bool result = true;
            try
            { 
                
                Dispatcher.Invoke(new Action(() =>
                {
                    prgStatus.Maximum = importList.Count();
                    txtStatus.Text = string.Format("Importing ...");
                }));

                int index = 1;
                foreach (var import in importList)
                {
                    WorkerRemarksController.AddRecord(import);
                    Dispatcher.Invoke(new Action(() => {
                        dgWorkerRemarks.SelectedItem = import;
                        dgWorkerRemarks.ScrollIntoView(import);
                        prgStatus.Value = index;
                    }));
                    index++;
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0} !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    result = false;
                }));
            }
            e.Result = result;
        }

        private void BwReadExcel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            txtStatus.Text = "Done...";
            dgWorkerRemarks.ItemsSource = workerRemarksList;
            btnImport.IsEnabled = true;

        }

        private void BwReadExcel_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                txtStatus.Text = "Reading ...";
                prgStatus.Maximum = filePathArray.Count();
            }));
            int filePathIndex = 1;

            foreach (string filePath in filePathArray)
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

                    Dispatcher.Invoke(new Action(() =>
                    {
                        txtStatus.Text = String.Format("Reading Excel File");
                        prgStatus.Value = filePathIndex;
                    }));

                    for (int r = 2; r <= excelRange.Rows.Count; r++)
                    {
                        var empIdSource = (excelRange.Cells[r, 2] as Excel.Range).Value2;
                        if (empIdSource != null)
                        {
                            var date = (excelRange.Cells[r, 1] as Excel.Range).Value2;
                            double dDate = 0;
                            Double.TryParse(date.ToString(), out dDate);
                            DateTime dateDate = DateTime.FromOADate(dDate);

                            var empId = empIdSource != null ? empIdSource.ToString() : "";

                            string remarksSource = (excelRange.Cells[r, 3] as Excel.Range).Value2;
                            var remarks = remarksSource != null ? remarksSource.ToString() : "";

                            workerRemarksList.Add(new WorkerRemarkModel
                            {
                                Date = dateDate,
                                EmployeeID = empId,
                                Remarks = remarks
                            });
                        }
                    }
                }
                catch
                { }
                finally
                {
                    excelWorkbook.Close(false, Missing.Value, Missing.Value);
                    excelApplication.Quit();
                }
                filePathIndex++;
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(string.Format("Confirm Import ?"), this.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Information, MessageBoxResult.No);
            if (result == MessageBoxResult.Cancel || result == MessageBoxResult.No)
            {
                return;
            }
            if (bwImport.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                btnImport.IsEnabled = false;
                List<WorkerRemarkModel> importList = new List<WorkerRemarkModel>();
                if (dgWorkerRemarks.ItemsSource != null)
                    importList = dgWorkerRemarks.ItemsSource.OfType<WorkerRemarkModel>().ToList();
                txtStatus.Text = "";
                prgStatus.Value = 0;
                bwImport.RunWorkerAsync(importList);
            }
        }

        private void btnOpenExcelFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select excel file";
            openFileDialog.Filter = "EXCEL Files (*.xls, *.xlsx)|*.xls;*.xlsx";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                filePathArray = openFileDialog.FileNames;
                if (bwReadExcel.IsBusy == false)
                {
                    this.Cursor = Cursors.Wait;
                    workerRemarksList.Clear();
                    dgWorkerRemarks.ItemsSource = null;
                    bwReadExcel.RunWorkerAsync();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void dgWorkerRemarks_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
