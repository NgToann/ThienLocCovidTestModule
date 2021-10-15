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
    /// Interaction logic for EditLeaveDayWindow.xaml
    /// </summary>
    public partial class EditLeaveDayWindow : Window
    {
        List<EmployeeModel> _employeeList;
        AttendanceRecordViewModel _recordViewModel;
        List<AttendanceRecordViewModel> _itemsSelectedList;

        BackgroundWorker bwLoad;
        BackgroundWorker bwSave;
        BackgroundWorker bwDelete;

        List<LeaveDayDetailModel> leaveDetailList;
        List<LeaveDayTotalModel> leaveTotalList;

        public List<AttendanceRecordModel> attdendanRecordProcessedList;
        bool needToProcess = false;

        public EditLeaveDayWindow(AttendanceRecordViewModel _recordViewModel, List<EmployeeModel> _employeeList, List<AttendanceRecordViewModel> _itemsSelectedList)
        {
            this._recordViewModel   = _recordViewModel;
            this._employeeList      = _employeeList;
            this._itemsSelectedList = _itemsSelectedList;

            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwSave = new BackgroundWorker();
            bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);
            bwSave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);

            bwDelete = new BackgroundWorker();
            bwDelete.DoWork += new DoWorkEventHandler(bwDelete_DoWork);
            bwDelete.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwDelete_RunWorkerCompleted);

            leaveDetailList = new List<LeaveDayDetailModel>();
            leaveTotalList = new List<LeaveDayTotalModel>();

            attdendanRecordProcessedList = new List<AttendanceRecordModel>();

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 1 row
            if (_itemsSelectedList.Count() == 1)
            {
                var employeeEditting = _employeeList.FirstOrDefault(f => f.EmployeeCode == _itemsSelectedList.FirstOrDefault().EmployeeCode);
                this.Title = String.Format("{0} - {1} : {2}", this.Title, employeeEditting != null ? employeeEditting.EmployeeID : "", employeeEditting != null ? employeeEditting.EmployeeName : "");
            }
            // n rows
            else
            {
                this.Title = String.Format("{0} - Editting {1} rows", this.Title, _itemsSelectedList.Count());
                dpDateFrom.IsEnabled = false;
                dpDateTo.IsEnabled = false;
            }
            if (bwLoad.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
        }
        private void bwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var employeeCodeEdittingList = _itemsSelectedList.Select(s => s.EmployeeCode).Distinct().ToList();
                foreach (var employeeCode in employeeCodeEdittingList)
                {
                    leaveDetailList.AddRange(LeaveDayDetailController.GetByEmployeeCode(employeeCode));
                    leaveTotalList.AddRange(LeaveDayTotalController.GetByEmployeeCode(employeeCode));
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0} !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }));
            }
        }
        private void bwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Binding UI
            // 1 Row
            if (_itemsSelectedList.Count() == 1)
            {
                var leaveDetailByDate = leaveDetailList.FirstOrDefault(w => w.LeaveDate == _recordViewModel.AttendanceDate);
                if (leaveDetailByDate != null && leaveDetailByDate.RandomNo != 1)
                {
                    var leaveTotalByRandomNo = leaveTotalList.Where(w => w.RandomNo == leaveDetailByDate.RandomNo).FirstOrDefault();
                    // <> null is paid
                    if (leaveTotalByRandomNo != null)
                    {
                        // tranfer paid from 1,0 to radio button
                        leaveTotalByRandomNo.IsPaid = leaveTotalByRandomNo.Paid.Trim() == "1" ? true : false;
                        leaveTotalByRandomNo.IsNotPaid = leaveTotalByRandomNo.Paid.Trim() == "0" ? true : false;
                        gridLeaveDay.DataContext = leaveTotalByRandomNo;
                    }

                    // == null is not paid
                    else
                    {
                        var leaveDetailByRandomNo = leaveDetailList.Where(w => w.RandomNo == leaveDetailByDate.RandomNo).ToList();
                        var leaveTotalNotPaid = new LeaveDayTotalModel
                        {
                            RandomNo = leaveDetailByDate.RandomNo,
                            EmployeeCode = leaveDetailByDate.EmployeeCode,
                            BeginDate = leaveDetailByRandomNo.Min(m => m.LeaveDate),
                            EndDate = leaveDetailByRandomNo.Max(m => m.LeaveDate),
                            TotalDay = leaveDetailByRandomNo.Sum(s => s.TotalDay),
                            Remark = leaveDetailByDate.Remark,
                            Paid = "0",
                            IsPaid = false,
                            IsNotPaid = true
                        };
                        gridLeaveDay.DataContext = leaveTotalNotPaid;
                    }

                    // binding to datagrid
                    var leaveDayDetailByRandomList = leaveDetailList.Where(w => w.RandomNo == leaveDetailByDate.RandomNo).OrderBy(o => o.LeaveDate).ToList();
                    leaveDayDetailByRandomList.ForEach(f => f.EmployeeID = _itemsSelectedList.FirstOrDefault(w => w.EmployeeCode == f.EmployeeCode).EmployeeID);
                    dgLeaveDay.ItemsSource = leaveDayDetailByRandomList;
                    dgLeaveDay.Items.Refresh();
                }
                else if (leaveDetailByDate != null && leaveDetailByDate.RandomNo == 1)
                {
                    var leaveDayTotalByDate = leaveTotalList.Where(w => w.BeginDate == leaveDetailByDate.LeaveDate).FirstOrDefault();
                    // paid
                    if (leaveDayTotalByDate != null)
                    {
                        leaveDayTotalByDate.IsPaid = leaveDayTotalByDate.Paid.Trim() == "1" ? true : false;
                        leaveDayTotalByDate.IsNotPaid = leaveDayTotalByDate.Paid.Trim() == "0" ? true : false;
                        gridLeaveDay.DataContext = leaveDayTotalByDate;
                    }
                    // not paid
                    else
                    {
                        var leaveTotalNotPaid = new LeaveDayTotalModel
                        {
                            RandomNo = leaveDetailByDate.RandomNo,
                            EmployeeCode = leaveDetailByDate.EmployeeCode,
                            BeginDate = leaveDetailByDate.LeaveDate,
                            EndDate = leaveDetailByDate.LeaveDate,
                            TotalDay = leaveDetailByDate.TotalDay,
                            Remark = leaveDetailByDate.Remark,
                            Paid = leaveDetailByDate.Paid,
                            IsPaid = false,
                            IsNotPaid = true
                        };
                        gridLeaveDay.DataContext = leaveTotalNotPaid;
                    }

                    // binding to datagrid
                    var displayList = leaveDetailList.Where(w => w.LeaveDate == _recordViewModel.AttendanceDate).ToList();
                    displayList.ForEach(f => f.EmployeeID = _itemsSelectedList.FirstOrDefault(w => w.EmployeeCode == f.EmployeeCode).EmployeeID);
                    dgLeaveDay.ItemsSource = displayList;
                    dgLeaveDay.Items.Refresh();
                }
                else
                {
                    var leaveDayTotalNew = new LeaveDayTotalModel
                    {
                        RandomNo = leaveTotalList.Count() > 0 ? leaveTotalList.Max(m => m.RandomNo) + 16 : 16,
                        EmployeeCode = _recordViewModel.EmployeeCode,
                        BeginDate = _recordViewModel.AttendanceDate,
                        EndDate = _recordViewModel.AttendanceDate,
                        IsNotPaid = true,
                        TotalDay = 1,
                        Remark = "1day"
                    };
                    gridLeaveDay.DataContext = leaveDayTotalNew;
                }
            }
            // n Rows
            else
            {
                var leaveDayDetailDisplayList = new List<LeaveDayDetailModel>();
                foreach (var item in _itemsSelectedList)
                {
                    var leaveDayByEmpCodeByDate = leaveDetailList.FirstOrDefault(f => f.EmployeeCode == item.EmployeeCode && f.LeaveDate == item.AttendanceDate);
                    if (leaveDayByEmpCodeByDate != null)
                    {
                        leaveDayDetailDisplayList.Add(leaveDayByEmpCodeByDate);
                    }
                }
                leaveDayDetailDisplayList.ForEach(f => f.EmployeeID = _itemsSelectedList.FirstOrDefault(w => w.EmployeeCode == f.EmployeeCode).EmployeeID);
                dgLeaveDay.ItemsSource = leaveDayDetailDisplayList;
                dgLeaveDay.Items.Refresh();

                // set default is not paid
                txtRemark.IsEnabled     = false;
                txtTotalDay.IsEnabled   = false;
                gridLeaveDay.DataContext = new LeaveDayTotalModel { Remark = "Mode = ManyRows", IsNotPaid = true, BeginDate = _itemsSelectedList.Min(m => m.AttendanceDate), EndDate = _itemsSelectedList.Max(m => m.AttendanceDate), RandomNo = 2020 };
            }
            this.Cursor = null;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // 1 Row
            if (_itemsSelectedList.Count() == 1)
            {
                var addModel = gridLeaveDay.DataContext as LeaveDayTotalModel;
                if (addModel == null)
                    return;
                List<LeaveDayDetailModel> leaveDayAddList = new List<LeaveDayDetailModel>();
                for (DateTime date = addModel.BeginDate; date <= addModel.EndDate; date = date.AddDays(1))
                {
                    if (date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        continue;
                    }

                    string paidOrNot = addModel.IsPaid == true ? "1" : "0";

                    var leaveDayAdd = new LeaveDayDetailModel
                    {
                        RandomNo        = addModel.RandomNo,
                        EmployeeCode    = addModel.EmployeeCode,
                        EmployeeID      = _recordViewModel.EmployeeID,
                        LeaveDate       = date,
                        TotalDay        = 1,
                        Remark          = addModel.Remark,
                        Paid            = paidOrNot
                    };
                    leaveDayAddList.Add(leaveDayAdd);
                }

                dgLeaveDay.ItemsSource = leaveDayAddList;
                dgLeaveDay.Items.Refresh();
            }
            // n Rows
            else
            {
                var leaveDayAddList = new List<LeaveDayDetailModel>();
                var empCodeAddList = _itemsSelectedList.Select(s => s.EmployeeCode).Distinct().ToList();
                foreach (var employeeCode in empCodeAddList)
                {
                    var leaveDayDetailByEmployeeCodeCurrentList = leaveDetailList.Where(w => w.EmployeeCode == employeeCode).ToList();
                    var itemEdittingListByCode = _itemsSelectedList.Where(w => w.EmployeeCode == employeeCode).ToList();
                    foreach (var item in itemEdittingListByCode)
                    {
                        if (item.AttendanceDate.DayOfWeek == DayOfWeek.Sunday)
                        {
                            continue;
                        }
                        var leaveDayByItem = leaveDayDetailByEmployeeCodeCurrentList.FirstOrDefault(f => f.LeaveDate == item.AttendanceDate);
                        if (leaveDayByItem != null)
                            continue;

                        string paidOrNot = radPaidLeave.IsChecked == true ? "1" : "0";
                        int randomNoCurrent = leaveDayDetailByEmployeeCodeCurrentList.Max(r => r.RandomNo);
                        var addModel = new LeaveDayDetailModel
                        {
                            RandomNo     = randomNoCurrent + 16,
                            EmployeeCode = employeeCode,
                            EmployeeID   = item.EmployeeID,
                            LeaveDate    = item.AttendanceDate,
                            TotalDay     = 1,
                            Remark       = "1day",
                            Paid         = paidOrNot
                        };
                        leaveDayDetailByEmployeeCodeCurrentList.Add(addModel);
                        leaveDayAddList.Add(addModel);
                    }
                }
                
                dgLeaveDay.ItemsSource = leaveDayAddList;
                dgLeaveDay.Items.Refresh();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dgLeaveDay.ItemsSource == null)
                return;
            var leaveDetailSaveList = dgLeaveDay.ItemsSource.OfType<LeaveDayDetailModel>().ToList();

            if (bwSave.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                btnSave.IsEnabled = false;
                bwSave.RunWorkerAsync(leaveDetailSaveList);
            }
        }
        private void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            var leaveDetailSaveList = e.Argument as List<LeaveDayDetailModel>;
            bool result = true;
            try
            {
                // Insert or Update Detail
                foreach (var leaveDetail in leaveDetailSaveList)
                {
                    LeaveDayDetailController.Add(leaveDetail);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        dgLeaveDay.SelectedItem = leaveDetail;
                        dgLeaveDay.ScrollIntoView(leaveDetail);
                    }));
                }

                // Insert or Update Total
                var leaveTotalSave = new LeaveDayTotalModel();
                var leaveDetailPaid = leaveDetailSaveList.FirstOrDefault();
                if (leaveDetailPaid != null)
                {
                    leaveTotalSave.EmployeeCode = leaveDetailPaid.EmployeeCode;

                    leaveTotalSave.BeginDate = leaveDetailSaveList.Min(m => m.LeaveDate);
                    leaveTotalSave.EndDate = leaveDetailSaveList.Max(m => m.LeaveDate);
                    leaveTotalSave.TotalDay = leaveDetailSaveList.Sum(s => s.TotalDay);

                    leaveTotalSave.Paid = leaveDetailPaid.Paid;
                    leaveTotalSave.Remark = string.Format("{0}day", leaveTotalSave.TotalDay);
                    leaveTotalSave.RandomNo = leaveDetailPaid.RandomNo;

                    LeaveDayTotalController.Add(leaveTotalSave);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0}", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    result = false;
                }));
            }
            e.Result = result;
        }
        private void bwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            if (result == true)
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageAddDataSucessful")), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                needToProcess = true;
            }
            this.Cursor = null;
            btnSave.IsEnabled = true;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageConfirmDelete")),
                this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
            {
                return;
            }
            if (dgLeaveDay.ItemsSource == null)
                return;

            var leaveDetailDeleteList = dgLeaveDay.ItemsSource.OfType<LeaveDayDetailModel>().ToList();
            if (bwDelete.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                btnDelete.IsEnabled = false;
                bwDelete.RunWorkerAsync(leaveDetailDeleteList);
            }
        }
        private void bwDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            bool result = true;
            var leaveDetailDeleteList = e.Argument as List<LeaveDayDetailModel>;
            try
            {
                // Delete LeaveDetail
                foreach (var deleteModel in leaveDetailDeleteList)
                {
                    LeaveDayDetailController.Delete(deleteModel);
                }

                // Delete LeaveTotal
                var leaveTotalDelete = new LeaveDayTotalModel();
                var leaveDetailDelete = leaveDetailDeleteList.FirstOrDefault();
                if (leaveDetailDelete != null)
                {
                    leaveTotalDelete.EmployeeCode = leaveDetailDelete.EmployeeCode;

                    leaveTotalDelete.BeginDate = leaveDetailDeleteList.Min(m => m.LeaveDate);
                    leaveTotalDelete.EndDate = leaveDetailDeleteList.Max(m => m.LeaveDate);
                    leaveTotalDelete.TotalDay = leaveDetailDeleteList.Sum(s => s.TotalDay);

                    leaveTotalDelete.Paid = leaveDetailDelete.Paid;
                    leaveTotalDelete.Remark = leaveDetailDelete.Remark;
                    leaveTotalDelete.RandomNo = leaveDetailDelete.RandomNo;

                    LeaveDayTotalController.Delete(leaveTotalDelete);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0}", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    result = false;
                }));
            }
            e.Result = result;
        }
        private void bwDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            if (result == true)
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageDeleteDataSucessful")), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                needToProcess = true;
                dgLeaveDay.ItemsSource = null;
                dgLeaveDay.Items.Refresh();
            }
            this.Cursor = null;
            btnDelete.IsEnabled = true;
        }

        private void dgLeaveDay_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void dpDateTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var dateFromSelected = dpDateFrom.SelectedDate.Value;
            var dateToSelected = dpDateTo.SelectedDate.Value;
            int totalDay = 0;
            for (DateTime date = dateFromSelected; date <= dateToSelected; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                totalDay++;
            }
            txtTotalDay.Text = totalDay.ToString();
            if (totalDay > 0)
                txtRemark.Text = string.Format("{0}day", totalDay);
        }

        private void txtTotalDay_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtTotalDay.SelectAll();
        }

        private void txtTotalDay_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int totalDay = 0;
            Int32.TryParse(txtTotalDay.Text.Trim(), out totalDay);
            if (totalDay > 0)
                dpDateTo.SelectedDate = dpDateFrom.SelectedDate.Value.AddDays(totalDay - 1);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (needToProcess == true)
            {
                // 1 Row
                if (_itemsSelectedList.Count() == 1)
                {
                    DateTime dateFrom = dpDateFrom.SelectedDate.Value;
                    DateTime dateTo = dpDateTo.SelectedDate.Value;

                    EmployeeController.ExecuteSalaryData(_recordViewModel.EmployeeCode, dateFrom, dateTo);
                    attdendanRecordProcessedList = AttendanceRecordController.GetAttendanceRecordByEmployeeCodeFromTo(_recordViewModel.EmployeeCode, dateFrom, dateTo);
                }
                // n Rows
                else
                {
                    var employeeCodeEdittingList = _itemsSelectedList.Select(s => s.EmployeeCode).Distinct().ToList();
                    foreach (var employeeCode in employeeCodeEdittingList)
                    {
                        var itemListByEmpCode = _itemsSelectedList.Where(w => w.EmployeeCode == employeeCode).ToList();
                        EmployeeController.ExecuteSalaryData(employeeCode, itemListByEmpCode.Min(m=>m.AttendanceDate), itemListByEmpCode.Max(m => m.AttendanceDate));
                        attdendanRecordProcessedList.AddRange(AttendanceRecordController.GetAttendanceRecordByEmployeeCodeFromTo(employeeCode, itemListByEmpCode.Min(m => m.AttendanceDate), itemListByEmpCode.Max(m => m.AttendanceDate)));
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
    }
}
