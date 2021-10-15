using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for WorkingShiftWindow.xaml
    /// </summary>
    public partial class EditWorkingShiftWindow : Window
    {
        AttendanceRecordViewModel recordViewModel;
        List<WorkingShiftModel> workingShiftList;
        AttendanceInforModel attendanceWorkingShift;
        List<EmployeeModel> employeeList;
        BackgroundWorker bwLoad;
        BackgroundWorker bwSave;
        public List<AttendanceRecordModel> attdendanRecordProcessedList;
        string sundayShiftCode = "999";
        DateTime dateFrom, dateTo;

        public EditWorkingShiftWindow(AttendanceRecordViewModel _recordViewModel, List<EmployeeModel> _employeeList)
        {
            this.recordViewModel = _recordViewModel;
            this.employeeList = _employeeList;
            workingShiftList = new List<WorkingShiftModel>();
            attendanceWorkingShift = new AttendanceInforModel();

            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            bwSave = new BackgroundWorker();
            bwSave.DoWork += BwSave_DoWork;
            bwSave.RunWorkerCompleted += BwSave_RunWorkerCompleted;

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
        private void BwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                workingShiftList = WorkingShiftController.GetAll();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0} !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                }));
            }
        }
        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dpFrom.SelectedDate = recordViewModel.AttendanceDate;
            dpTo.SelectedDate = recordViewModel.AttendanceDate;

            // binding to combobox.
            if (workingShiftList.Count() > 0)
            {
                cboWorkingShiftNo.ItemsSource = workingShiftList.OrderBy(o => o.WorkingShiftCode);
                var currentShift = workingShiftList.Where(w => w.WorkingShiftCode == recordViewModel.ShiftNo).FirstOrDefault();
                if (currentShift != null)
                    cboWorkingShiftNo.SelectedItem = currentShift;
            }

            // binding to datagrid
            int year = recordViewModel.AttendanceDate.Year;
            int month = recordViewModel.AttendanceDate.Month;
            int day = recordViewModel.AttendanceDate.Day;
            string shiftNo = recordViewModel.ShiftNo;
            
            var sundayShift = workingShiftList.Where(w => w.IsSunday == true).FirstOrDefault();
            if (sundayShift != null)
                sundayShiftCode = sundayShift.WorkingShiftCode;

            dgShiftNoTemp.ItemsSource = CreateShiftTempList(recordViewModel.EmployeeCode,
                recordViewModel.AttendanceDate, recordViewModel.AttendanceDate, recordViewModel.ShiftNo, sundayShiftCode);

            var employee = employeeList.FirstOrDefault(w => w.EmployeeID == recordViewModel.EmployeeID);
            //this.Title = string.Format("{0}  -  {1}", this.Title, recordViewModel.EmployeeID);
            this.Title = String.Format("{0} - {1} : {2}", this.Title, employee != null ? employee.EmployeeID : "", employee != null ? employee.EmployeeName : "");

            this.Cursor = null;
            btnSave.IsEnabled = true;
        }

        private void cboWorkingShiftNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var shiftCodeUpdate = cboWorkingShiftNo.SelectedItem as WorkingShiftModel;
            if (shiftCodeUpdate != null)
            {
                var dateFrom = dpFrom.SelectedDate.Value;
                var dateTo = dpTo.SelectedDate.Value;
                dgShiftNoTemp.ItemsSource = CreateShiftTempList(recordViewModel.EmployeeCode, dateFrom, dateTo, shiftCodeUpdate.WorkingShiftCode, sundayShiftCode);
                dgShiftNoTemp.Items.Refresh();
            }
        }

        private void MiRemove_Click(object sender, RoutedEventArgs e)
        {
            var itemsRemoveClicked = dgShiftNoTemp.SelectedItems.OfType<ShiftTempModel>().ToList();
            if (itemsRemoveClicked.Count() > 0 && dgShiftNoTemp.ItemsSource != null)
            {
                var shitftTempList = dgShiftNoTemp.ItemsSource.OfType<ShiftTempModel>().ToList();
                shitftTempList.RemoveAll(r => itemsRemoveClicked.Select(s => s.EmployeeCode).Contains(r.EmployeeCode) && itemsRemoveClicked.Select(s => s.ShiftDate).Contains(r.ShiftDate));

                dgShiftNoTemp.ItemsSource = shitftTempList;
                dgShiftNoTemp.Items.Refresh();
            }
        }
        
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            dateFrom = dpFrom.SelectedDate.Value;
            dateTo = dpTo.SelectedDate.Value;

            var attendanceInforModel = new AttendanceInforModel();
            var shiftAddList = dgShiftNoTemp.ItemsSource.OfType<ShiftTempModel>().ToList();
            //for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))

            if (bwSave.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                btnSave.IsEnabled = false;
                bwSave.RunWorkerAsync(shiftAddList);
            }
        }
        private void BwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            var shiftAddList = e.Argument as List<ShiftTempModel>;
            bool result = true;
            foreach (var shift in shiftAddList)
            {
                try
                {
                    var attendanceInforAdd = Convert(shift.ShiftDate.Year, shift.ShiftDate.Month, shift.ShiftDate.Day, shift.ShiftCode, sundayShiftCode);
                    AttendanceInforController.AddOrUpdate(attendanceInforAdd);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        dgShiftNoTemp.SelectedItem = shift;
                        dgShiftNoTemp.ScrollIntoView(shift);
                    }));

                    // Process Data
                    EmployeeController.ExecuteSalaryData(recordViewModel.EmployeeCode, dateFrom, dateTo);
                    attdendanRecordProcessedList = AttendanceRecordController.GetAttendanceRecordByEmployeeCodeFromTo(recordViewModel.EmployeeCode, dateFrom, dateTo);
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
        private void BwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            if (result == true)
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageAddDataSucessful")),
                                                this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.Cursor = null;
            btnSave.IsEnabled = true;
        }

        private List<ShiftTempModel> CreateShiftTempList(string employeeCode, DateTime dateFrom, DateTime dateTo, string shiftCode, string sundayShift)
        {
            var resultList = new List<ShiftTempModel>();
            for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
            {
                var shiftTemp = new ShiftTempModel()
                {
                    EmployeeCode = employeeCode,
                    ShiftDate = date,
                    ShiftCode = shiftCode,
                };
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    shiftTemp.ShiftBackground = Brushes.Red;
                    shiftTemp.ShiftCode = sundayShift;
                }
                resultList.Add(shiftTemp);
            }
            return resultList;
        }

        private AttendanceInforModel Convert(int year, int month, int dayInMonth, string shiftNo, string sundayShift)
        {
            int maxDayInMonth = DateTime.DaysInMonth(year, month);
            var result = new AttendanceInforModel();
            var employee = employeeList.Where(w => w.EmployeeCode == recordViewModel.EmployeeCode).FirstOrDefault();
            if (employee != null)
            {
                result.EmployeeName = employee.EmployeeName;
                result.EmployeeID = employee.EmployeeID;
                result.EmployeeCode = employee.EmployeeCode;
            }
            result.ShiftYear = year;
            result.ShiftMonth = month;

            var dateCheck1 = new DateTime(year, month, 1);
            if (dateCheck1.Day == dayInMonth)
            {
                result.Shift_1 = shiftNo; 
            }
            var dateCheck2 = new DateTime(year, month, 2);
            if (dateCheck2.Day == dayInMonth)
            {
                result.Shift_2 = shiftNo;
            }
            var dateCheck3 = new DateTime(year, month, 3);
            if (dateCheck3.Day == dayInMonth)
            {
                result.Shift_3 = shiftNo;
            }
            var dateCheck4 = new DateTime(year, month, 4);
            if (dateCheck4.Day == dayInMonth)
            {
                result.Shift_4 = shiftNo;
            }
            var dateCheck5 = new DateTime(year, month, 5);
            if (dateCheck5.Day == dayInMonth)
            {
                result.Shift_5 = shiftNo;
            }
            var dateCheck6 = new DateTime(year, month, 6);
            if (dateCheck6.Day == dayInMonth)
            {
                result.Shift_6 = shiftNo;
            }
            var dateCheck7 = new DateTime(year, month, 7);
            if (dateCheck7.Day == dayInMonth)
            {
                result.Shift_7 = shiftNo;
            }
            var dateCheck8 = new DateTime(year, month, 8);
            if (dateCheck8.Day == dayInMonth)
            {
                result.Shift_8 = shiftNo;
            }
            var dateCheck9 = new DateTime(year, month, 9);
            if (dateCheck9.Day == dayInMonth)
            {
                result.Shift_9 = shiftNo;
            }
            var dateCheck10 = new DateTime(year, month, 10);
            if (dateCheck10.Day == dayInMonth)
            {
                result.Shift_10 = shiftNo;
            }

            var dateCheck11 = new DateTime(year, month, 11);
            if (dateCheck11.Day == dayInMonth)
            {
                result.Shift_11 = shiftNo;
            }
            var dateCheck12 = new DateTime(year, month, 12);
            if (dateCheck12.Day == dayInMonth)
            {
                result.Shift_12 = shiftNo;
            }
            var dateCheck13 = new DateTime(year, month, 13);
            if (dateCheck13.Day == dayInMonth)
            {
                result.Shift_13 = shiftNo;
            }
            var dateCheck14 = new DateTime(year, month, 14);
            if (dateCheck14.Day == dayInMonth)
            {
                result.Shift_14 = shiftNo;
            }
            var dateCheck15 = new DateTime(year, month, 15);
            if (dateCheck15.Day == dayInMonth)
            {
                result.Shift_15 = shiftNo;
            }
            var dateCheck16 = new DateTime(year, month, 16);
            if (dateCheck16.Day == dayInMonth)
            {
                result.Shift_16 = shiftNo;
            }
            var dateCheck17 = new DateTime(year, month, 17);
            if (dateCheck17.Day == dayInMonth)
            {
                result.Shift_17 = shiftNo;
            }
            var dateCheck18 = new DateTime(year, month, 18);
            if (dateCheck18.Day == dayInMonth)
            {
                result.Shift_18 = shiftNo;
            }
            var dateCheck19 = new DateTime(year, month, 19);
            if (dateCheck19.Day == dayInMonth)
            {
                result.Shift_19 = shiftNo;
            }
            var dateCheck20 = new DateTime(year, month, 20);
            if (dateCheck20.Day == dayInMonth)
            {
                result.Shift_20 = shiftNo;
            }
            var dateCheck21 = new DateTime(year, month, 21);
            if (dateCheck21.Day == dayInMonth)
            {
                result.Shift_21 = shiftNo;
            }
            var dateCheck22 = new DateTime(year, month, 22);
            if (dateCheck22.Day == dayInMonth)
            {
                result.Shift_22 = shiftNo;
            }
            var dateCheck23 = new DateTime(year, month, 23);
            if (dateCheck23.Day == dayInMonth)
            {
                result.Shift_23 = shiftNo;
            }
            var dateCheck24 = new DateTime(year, month, 24);
            if (dateCheck24.Day == dayInMonth)
            {
                result.Shift_24 = shiftNo;
            }
            var dateCheck25 = new DateTime(year, month, 25);
            if (dateCheck25.Day == dayInMonth)
            {
                result.Shift_25 = shiftNo;
            }
            var dateCheck26 = new DateTime(year, month, 26);
            if (dateCheck26.Day == dayInMonth)
            {
                result.Shift_26 = shiftNo;
            }
            var dateCheck27 = new DateTime(year, month, 27);
            if (dateCheck27.Day == dayInMonth)
            {
                result.Shift_27 = shiftNo;
            }
            var dateCheck28 = new DateTime(year, month, 28);
            if (dateCheck28.Day == dayInMonth)
            {
                result.Shift_28 = shiftNo;
            }
            if (maxDayInMonth > 28)
            {
                var dateCheck29 = new DateTime(year, month, 29);
                if (dateCheck29.Day == dayInMonth)
                {
                    result.Shift_29 = shiftNo;
                }
                if (maxDayInMonth > 29)
                {
                    var dateCheck30 = new DateTime(year, month, 30);
                    if (dateCheck30.Day == dayInMonth)
                    {
                        result.Shift_30 = shiftNo;
                    }
                }
                if (maxDayInMonth > 30)
                {
                    var dateCheck31 = new DateTime(year, month, 31);
                    if (dateCheck31.Day == dayInMonth)
                    {
                        result.Shift_31 = shiftNo;
                    }
                }
            }
            return result;
        }

        private void dgShiftNoTemp_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
