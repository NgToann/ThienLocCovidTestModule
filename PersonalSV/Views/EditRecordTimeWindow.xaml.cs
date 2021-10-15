using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for RecordTimeDetailWindow.xaml
    /// </summary>
    public partial class EditRecordTimeWindow : Window
    {
        AttendanceRecordViewModel recordViewModel;
        List<AttendanceRecordViewModel> itemsSelectedList;
        BackgroundWorker bwLoad;
        BackgroundWorker bwSave;
        List<EmployeeModel> employeeList;
        List<SourceModel> sourceList;
        EmployeeModel employeeView;
        public List<AttendanceRecordModel> attendanceRecordProcessedList;
        bool needToProcess = false;
        public EditRecordTimeWindow(AttendanceRecordViewModel _recordViewModel, List<EmployeeModel> _employeeList, List<AttendanceRecordViewModel> _itemsSelectedList)
        {
            this.recordViewModel    = _recordViewModel;
            this.employeeList       = _employeeList;
            this.itemsSelectedList  = _itemsSelectedList;
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwSave = new BackgroundWorker();
            bwSave.DoWork +=new DoWorkEventHandler(bwSave_DoWork);
            bwSave.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);

            sourceList = new List<SourceModel>();
            attendanceRecordProcessedList = new List<AttendanceRecordModel>();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bwLoad.IsBusy == false)
            {
                gridEmployeeInfor.DataContext = new EmployeeViewModel();
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
        }
        private void bwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // 1 row
                if (recordViewModel != null)
                {
                    employeeView = employeeList.First(w => w.EmployeeCode == recordViewModel.EmployeeCode);
                    sourceList = SourceController.SelectSourceByEmployeeCodeAndDate(recordViewModel.EmployeeCode, recordViewModel.AttendanceDate);
                    sourceList.ForEach(f => f.EmployeeID = employeeView.EmployeeID);
                }
                // n rows
                else
                {
                    foreach (var item in itemsSelectedList)
                    {
                        sourceList.AddRange(SourceController.SelectSourceByEmployeeCodeAndDate(item.EmployeeCode, item.AttendanceDate));
                        sourceList.ForEach(f => f.EmployeeID = itemsSelectedList.FirstOrDefault(w => w.EmployeeCode == f.EmployeeCode).EmployeeID);
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0} !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                }));
            }
        }
        private void bwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            dgRecordTime.ItemsSource = sourceList;

            // 1 row
            if (recordViewModel != null)
            {
                gridEmployeeInfor.DataContext = employeeView;
                this.Title = String.Format("{0} - {1} : {2}", this.Title, employeeView != null ? employeeView.EmployeeID : "", employeeView != null ? employeeView.EmployeeName : "");
            }
            // n rows
            else {
                int noOfWorkers = itemsSelectedList.Select(s => s.EmployeeID).Distinct().ToList().Count();
                // Display Tempo Data
                var temp = new EmployeeModel();
                txtEmployeeCode.Text = String.Format("{0} Worker{1}", noOfWorkers, noOfWorkers > 1 ? "s" : "");
                this.Title = String.Format("{0} - Editting {1} rows", this.Title, itemsSelectedList.Count());
            }
            btnSave.IsEnabled = true;
            btnAddTime.IsEnabled = true;
        }

        private void btnAddTime_Click(object sender, RoutedEventArgs e)
        {
            string timeAdd = txtRecordTimeAdd.Text.Trim().ToString();

            string messageEmptyError    = LanguageHelper.GetStringFromResource("messageDataEmpty");
            string messageDataIncorrect = LanguageHelper.GetStringFromResource("messageDataIncorrect");
            string messageDataExist     = LanguageHelper.GetStringFromResource("messageDataExist");
            string controlTime          = LanguageHelper.GetStringFromResource("commonTime");

            if (string.IsNullOrEmpty(timeAdd))
            {
                MessageBox.Show(string.Format("{0}\n{1}", controlTime, messageEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                goto TheEnd;
            }

            if (timeAdd.Length != 4)
            {
                MessageBox.Show(string.Format("{0}\n{1}", controlTime, messageDataIncorrect), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                goto TheEnd;
            }

            List<SourceModel> recordTimeAddList = new List<SourceModel>();

            // 1 row
            if (recordViewModel != null)
            {
                var recordAddModel = new SourceModel
                {
                    EmployeeCode = employeeView.EmployeeCode,
                    EmployeeID = employeeView.EmployeeID,
                    SourceDate = recordViewModel.AttendanceDate,
                    SourceTime = timeAdd,
                    SourceTimeView = timeAdd[0].ToString() + timeAdd[1].ToString() + ":" + timeAdd[2].ToString() + timeAdd[3].ToString(),
                    CardNo = "",
                };
                recordTimeAddList.Add(recordAddModel);
            }
            // n rows
            else
            {
                foreach (var item in itemsSelectedList)
                {
                    var recordAddModel = new SourceModel
                    {
                        EmployeeCode = item.EmployeeCode,
                        EmployeeID  = item.EmployeeID,
                        SourceDate = item.AttendanceDate,
                        SourceTime = timeAdd,
                        SourceTimeView = timeAdd[0].ToString() + timeAdd[1].ToString() + ":" + timeAdd[2].ToString() + timeAdd[3].ToString(),
                        CardNo = "",
                    };
                    recordTimeAddList.Add(recordAddModel);
                }
            }


            // Add data to grid
            var recordTimeListOnGrid = new List<SourceModel>();
            if (dgRecordTime.ItemsSource != null)
            {
                recordTimeListOnGrid = dgRecordTime.ItemsSource.OfType<SourceModel>().ToList();
            }

            // Check Exist
            foreach (var recordAddModel in recordTimeAddList)
            {
                var checkRecord = recordTimeListOnGrid.Where(w => w.EmployeeCode == recordAddModel.EmployeeCode && w.SourceDate == recordAddModel.SourceDate && w.SourceTime == recordAddModel.SourceTime).FirstOrDefault();
                if (checkRecord != null)
                {
                    MessageBox.Show(string.Format("EmployeeCode: {0}\nDate:         {1}\nTime:         {2}\n{3}", checkRecord.EmployeeCode,
                                                                                            checkRecord.SourceDate.ToShortDateString(),
                                                                                            checkRecord.SourceTimeView,
                                                                                            messageDataExist),
                                this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }
                recordTimeListOnGrid.Add(recordAddModel);
            }
            if (recordTimeListOnGrid.Count() > 0)
                recordTimeListOnGrid = recordTimeListOnGrid.OrderBy(o => o.EmployeeID).ThenBy(th => th.SourceDate).ThenBy(th => th.SourceTime).ToList();
            dgRecordTime.ItemsSource = recordTimeListOnGrid;
            dgRecordTime.Items.Refresh();

        TheEnd:
            {
                btnAddTime.IsDefault = true;
                txtRecordTimeAdd.Focus();
                return;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dgRecordTime.ItemsSource == null)
                return;
            var recordAddList = dgRecordTime.ItemsSource.OfType<SourceModel>().ToList();
            if (bwSave.IsBusy == false && recordAddList.Count > 0)
            {
                this.Cursor = Cursors.Wait;
                btnSave.IsEnabled = false;
                bwSave.RunWorkerAsync(recordAddList);
            }
        }
        private void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            var recordAddList = e.Argument as List<SourceModel>;
            foreach (var recordAdd in recordAddList)
            {
                bool result = true;
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        dgRecordTime.SelectedItem = recordAdd;
                        dgRecordTime.ScrollIntoView(recordAdd);
                    }));
                    SourceController.Add(recordAdd);
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBox.Show(string.Format("{0}\nPlease Try Again !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                        result = false;
                    }));
                }
                e.Result = result;
            }
        }
        private void bwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            if (result == true)
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageAddDataSucessful")),
                                                this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                needToProcess = true;
            }
            this.Cursor = null;
            btnSave.IsEnabled = true;
        }

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            var itemsSelected = dgRecordTime.SelectedItems.OfType<SourceModel>().ToList();
            if (itemsSelected.Count > 0)
            {
                bool result = true;
                var currentRecordList = dgRecordTime.ItemsSource.OfType<SourceModel>().ToList();
                foreach (var item in itemsSelected)
                {
                    try
                    {
                        currentRecordList.Remove(item);
                        SourceController.Delete(item);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            MessageBox.Show(string.Format("{0}\nPlease Try Again !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                            result = false;
                        }));
                    }
                }
                if (result == true)
                {
                    MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageDeleteDataSucessful")),
                                                    this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    dgRecordTime.ItemsSource = currentRecordList;
                    dgRecordTime.Items.Refresh();
                    needToProcess = true;
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Process Data For Employee
            
            if (needToProcess == true)
            {
                if (recordViewModel != null)
                {
                    EmployeeController.ExecuteSalaryData(recordViewModel.EmployeeCode, recordViewModel.AttendanceDate, recordViewModel.AttendanceDate);
                    attendanceRecordProcessedList.Add(AttendanceRecordController.GetAttendanceRecordByEmployeeCodeFromTo(recordViewModel.EmployeeCode, recordViewModel.AttendanceDate, recordViewModel.AttendanceDate).FirstOrDefault());
                }
                else
                {
                    foreach (var item in itemsSelectedList)
                    {
                        EmployeeController.ExecuteSalaryData(item.EmployeeCode, item.AttendanceDate, item.AttendanceDate);
                        attendanceRecordProcessedList.Add(AttendanceRecordController.GetAttendanceRecordByEmployeeCodeFromTo(item.EmployeeCode, item.AttendanceDate, item.AttendanceDate).FirstOrDefault());
                    }
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void txtRecordTimeAdd_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            btnAddTime.IsDefault = true;
        }
        private void txtRecordTimeAdd_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnAddTime.IsDefault = true;
        }

        private void dgRecordTime_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
