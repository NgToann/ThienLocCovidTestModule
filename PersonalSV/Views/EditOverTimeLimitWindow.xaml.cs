using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for EditOverTimeLimitWindow.xaml
    /// </summary>
    public partial class EditOverTimeLimitWindow : Window
    {
        List<EmployeeModel> employeeList;
        AttendanceRecordViewModel recordViewModel;
        List<AttendanceRecordViewModel> itemsSelectedList;
        BackgroundWorker bwLoad;
        BackgroundWorker bwSave;
        BackgroundWorker bwDelete;
        bool needToProcess = false;
        public List<AttendanceRecordModel> attdendanRecordProcessedList;

        public EditOverTimeLimitWindow(AttendanceRecordViewModel _recordViewModel, List<EmployeeModel> employeeList, List<AttendanceRecordViewModel> itemsSelectedList)
        {
            this.recordViewModel    = _recordViewModel;
            this.employeeList       = employeeList;
            this.itemsSelectedList  = itemsSelectedList;

            attdendanRecordProcessedList = new List<AttendanceRecordModel>();
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork +=new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwSave = new BackgroundWorker();
            bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);
            bwSave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);

            bwDelete = new BackgroundWorker();
            bwDelete.DoWork += new DoWorkEventHandler(bwDelete_DoWork);
            bwDelete.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwDelete_RunWorkerCompleted);

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (recordViewModel != null)
            {
                dpDateFrom.SelectedDate = recordViewModel.AttendanceDate;
                dpDateTo.SelectedDate = recordViewModel.AttendanceDate;
            }
            else
            {
                dpDateFrom.IsEnabled = false;
                dpDateTo.IsEnabled = false;
                dpDateFrom.SelectedDate = itemsSelectedList.Min(m => m.AttendanceDate);
                dpDateTo.SelectedDate = itemsSelectedList.Max(m => m.AttendanceDate);
            }

            if (bwLoad.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
        }
        private void bwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            List<OverTimeLimitModel> overTimeLoadList = new List<OverTimeLimitModel>();
            try
            {
                if (recordViewModel != null)
                {
                    overTimeLoadList = OverTimeLimitController.GetByEmployeeByDate(recordViewModel.EmployeeCode, recordViewModel.AttendanceDate);
                    overTimeLoadList.ForEach(model => model.EmployeeID = recordViewModel.EmployeeID);
                }
                else
                {
                    foreach (var item in itemsSelectedList)
                    {
                        overTimeLoadList.AddRange(OverTimeLimitController.GetByEmployeeByDate(item.EmployeeCode, item.AttendanceDate));
                        overTimeLoadList.ForEach(model => model.EmployeeID = item.EmployeeID);
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
            e.Result = overTimeLoadList;
        }
        private void bwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var overTimeLoadList = e.Result as List<OverTimeLimitModel>;
            dgOverTimeTemp.ItemsSource = overTimeLoadList;

            if (recordViewModel != null)
            {
                var employee = employeeList.FirstOrDefault(f => f.EmployeeID == recordViewModel.EmployeeID);
                this.Title = String.Format("{0} - {1} : {2}", this.Title, employee != null ? employee.EmployeeID : "", employee != null ? employee.EmployeeName : "");
            }
            else
            {
                this.Title = String.Format("{0} - Editing {1} rows", this.Title, itemsSelectedList.Count());
            }

            this.Cursor = null;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Check UI
            string timeOutLimit = txtTimeOutLimit.Text.Trim().ToString();
            string messageEmptyError = LanguageHelper.GetStringFromResource("messageDataEmpty");
            string messageDataIncorrect = LanguageHelper.GetStringFromResource("messageDataIncorrect");
            string messageDataExist = LanguageHelper.GetStringFromResource("messageDataExist");
            string controlOutLimit = LanguageHelper.GetStringFromResource("commonDatePickerTimeOutLimit");
            string coltrolOverTime = LanguageHelper.GetStringFromResource("commonDatePickerOverTime");

            if (string.IsNullOrEmpty(timeOutLimit))
            {
                MessageBox.Show(string.Format("{0}\n{1}", controlOutLimit, messageEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                goto TheEnd;
            }

            if (timeOutLimit.Length != 4)
            {
                MessageBox.Show(string.Format("{0}\n{1}", controlOutLimit, messageDataIncorrect), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                goto TheEnd;
            }

            double overTime = 0;
            Double.TryParse(txtOverTime.Text.Trim().ToString(), out overTime);
            if (overTime <= 0)
            {
                MessageBox.Show(string.Format("{0}\n{1}", coltrolOverTime, messageDataIncorrect), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                goto TheEnd;
            }

            var overTimeLimitAddList = new List<OverTimeLimitModel>();

            // 1 row
            if (recordViewModel != null)
            {
                DateTime dateFrom = dpDateFrom.SelectedDate.Value;
                DateTime dateTo = dpDateTo.SelectedDate.Value;

                for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
                {
                    var modelAdd = new OverTimeLimitModel();
                    modelAdd.EmployeeCode = recordViewModel.EmployeeCode;
                    modelAdd.EmployeeID = recordViewModel.EmployeeID;
                    modelAdd.OverTimeDate = date;
                    modelAdd.DateIn = date;
                    modelAdd.DateOut = date;
                    modelAdd.OverTime = overTime;
                    modelAdd.TimeOutLimit = timeOutLimit;
                    overTimeLimitAddList.Add(modelAdd);
                }
            }
            // n rows
            else
            {
                foreach (var item in itemsSelectedList)
                {
                    var modelAdd = new OverTimeLimitModel();
                    modelAdd.EmployeeCode = item.EmployeeCode;
                    modelAdd.EmployeeID = item.EmployeeID;
                    modelAdd.OverTimeDate = item.AttendanceDate;
                    modelAdd.DateIn = item.AttendanceDate;
                    modelAdd.DateOut = item.AttendanceDate;
                    modelAdd.OverTime = overTime;
                    modelAdd.TimeOutLimit = timeOutLimit;
                    overTimeLimitAddList.Add(modelAdd);
                }
            }
            
            dgOverTimeTemp.ItemsSource = overTimeLimitAddList;

        TheEnd:
            {
                btnAdd.IsDefault = true;
                return;
            }
        }

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            var itemsSelected = dgOverTimeTemp.SelectedItems.OfType<OverTimeLimitModel>().ToList();
            if (itemsSelected.Count > 0)
            {
                var currentOverTimeLimitList = dgOverTimeTemp.ItemsSource.OfType<OverTimeLimitModel>().ToList();
                foreach (var item in itemsSelected)
                {
                    currentOverTimeLimitList.Remove(item);
                }
                dgOverTimeTemp.ItemsSource = currentOverTimeLimitList;
                dgOverTimeTemp.Items.Refresh();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dgOverTimeTemp.ItemsSource == null)
                return;
            var overTimeAddList = dgOverTimeTemp.ItemsSource.OfType<OverTimeLimitModel>().ToList();
            if (bwSave.IsBusy == false && overTimeAddList.Count > 0)
            {
                this.Cursor = Cursors.Wait;
                btnSave.IsEnabled = false;
                bwSave.RunWorkerAsync(overTimeAddList);
            }
        }
        private void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            var overTimeAddList = e.Argument as List<OverTimeLimitModel>;
            bool result = true;
            foreach (var otLimitAdd in overTimeAddList)
            {
                try
                {
                    OverTimeLimitController.InsertOrUpdate(otLimitAdd);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        dgOverTimeTemp.SelectedItem = otLimitAdd;
                        dgOverTimeTemp.ScrollIntoView(otLimitAdd);
                    }));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBox.Show(string.Format("{0}\nPlease Try Again !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                    result = false;
                }
            }
            e.Result = result;
        }
        private void bwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            if (result == true)
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageAddDataSucessful")),
                                                this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.Cursor = null;
            btnSave.IsEnabled = true;
            needToProcess = true;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgOverTimeTemp.ItemsSource == null)
                return;
            var overTimeDeleteList = dgOverTimeTemp.ItemsSource.OfType<OverTimeLimitModel>().ToList();
            if (bwSave.IsBusy == false && overTimeDeleteList.Count > 0)
            {
                this.Cursor = Cursors.Wait;
                btnDelete.IsEnabled = false;
                bwDelete.RunWorkerAsync(overTimeDeleteList);
            }
        }
        private void bwDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            var overTimeAddList = e.Argument as List<OverTimeLimitModel>;
            bool result = true;
            foreach (var otLimitAdd in overTimeAddList)
            {
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        dgOverTimeTemp.SelectedItem = otLimitAdd;
                        dgOverTimeTemp.ScrollIntoView(otLimitAdd);
                    }));
                    OverTimeLimitController.DeleteByEmployeeByDate(otLimitAdd);
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBox.Show(string.Format("{0}\nPlease Try Again !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                    result = false;
                }
            }
            e.Result = result;
        }
        private void bwDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            if (result == true)
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageDeleteDataSucessful")),
                                                this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                dgOverTimeTemp.ItemsSource = new List<OverTimeLimitModel>();
                needToProcess = true;
            }
            this.Cursor = null;
            btnDelete.IsEnabled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (needToProcess == true)
            {
                if (recordViewModel != null)
                {
                    DateTime dateFrom = dpDateFrom.SelectedDate.Value;
                    DateTime dateTo = dpDateTo.SelectedDate.Value;

                    EmployeeController.ExecuteSalaryData(recordViewModel.EmployeeCode, dateFrom, dateTo);
                    attdendanRecordProcessedList = AttendanceRecordController.GetAttendanceRecordByEmployeeCodeFromTo(recordViewModel.EmployeeCode, dateFrom, dateTo);
                }
                else
                {
                    foreach (var item in itemsSelectedList)
                    {
                        EmployeeController.ExecuteSalaryData(item.EmployeeCode, item.AttendanceDate, item.AttendanceDate);
                        attdendanRecordProcessedList.AddRange(AttendanceRecordController.GetAttendanceRecordByEmployeeCodeFromTo(item.EmployeeCode, item.AttendanceDate, item.AttendanceDate));
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

        private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnAdd.IsDefault = true;
        }
        private void TextBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.SelectAll();
            btnAdd.IsDefault = true;
        }

        private void dgOverTimeTemp_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
