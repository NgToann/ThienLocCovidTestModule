using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;


namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for WorkingShiftDetailWindow.xaml
    /// </summary>
    public partial class WorkingShiftDetailWindow : Window
    {
        BackgroundWorker bwLoad;
        List<EmployeeModel> employeeList;
        List<AttendanceInforModel> resultsList;
        int year, month;
        public WorkingShiftDetailWindow(List<EmployeeModel> _employeeList, int _year, int _month)
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            this.employeeList = _employeeList;
            this.year = _year;
            this.month = _month;
            resultsList = new List<AttendanceInforModel>();

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
            int daysInMonth = DateTime.DaysInMonth(year, month);
            foreach (var employee in employeeList)
            {
                try
                {
                    var attendanceRecordSearch = AttendanceInforController.GetByEmployeeCodeYearMonth(employee.EmployeeCode, year, month);
                    if (attendanceRecordSearch != null)
                    {
                        attendanceRecordSearch.EmployeeName = employee != null ? employee.EmployeeName : "NULL";
                        attendanceRecordSearch.EmployeeID = employee != null ? employee.EmployeeID : "NULL";

                        #region bind data per date
                        // date 1
                        var dateCheck1 = new DateTime(year, month, 1);
                        if (dateCheck1.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_1Background = Brushes.Red;
                        }

                        // date 2
                        var dateCheck2 = new DateTime(year, month, 2);
                        if (dateCheck2.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_2Background = Brushes.Red;
                        }


                        // date 3
                        var dateCheck3 = new DateTime(year, month, 3);
                        if (dateCheck3.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_3Background = Brushes.Red;
                        }


                        // date 4
                        var dateCheck4 = new DateTime(year, month, 4);
                        if (dateCheck4.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_4Background = Brushes.Red;
                        }

                        // date 5
                        var dateCheck5 = new DateTime(year, month, 5);
                        if (dateCheck5.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_5Background = Brushes.Red;
                        }

                        // date 6
                        var dateCheck6 = new DateTime(year, month, 6);
                        if (dateCheck6.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_6Background = Brushes.Red;
                        }

                        // date 7
                        var dateCheck7 = new DateTime(year, month, 7);
                        if (dateCheck7.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_7Background = Brushes.Red;
                        }

                        // date 8
                        var dateCheck8 = new DateTime(year, month, 8);
                        if (dateCheck8.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_8Background = Brushes.Red;
                        }

                        // date 9
                        var dateCheck9 = new DateTime(year, month, 9);
                        if (dateCheck9.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_9Background = Brushes.Red;
                        }

                        // date 10
                        var dateCheck10 = new DateTime(year, month, 10);
                        if (dateCheck10.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_10Background = Brushes.Red;
                        }


                        // date 11
                        var dateCheck11 = new DateTime(year, month, 11);
                        if (dateCheck11.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_11Background = Brushes.Red;
                        }


                        // date 12
                        var dateCheck12 = new DateTime(year, month, 12);
                        if (dateCheck12.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_12Background = Brushes.Red;
                        }


                        // date 13
                        var dateCheck13 = new DateTime(year, month, 13);
                        if (dateCheck13.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_13Background = Brushes.Red;
                        }


                        // date 14
                        var dateCheck14 = new DateTime(year, month, 14);
                        if (dateCheck14.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_14Background = Brushes.Red;
                        }


                        // date 15
                        var dateCheck15 = new DateTime(year, month, 15);
                        if (dateCheck15.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_15Background = Brushes.Red;
                        }


                        // date 16
                        var dateCheck16 = new DateTime(year, month, 16);
                        if (dateCheck16.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_16Background = Brushes.Red;
                        }


                        // date 17
                        var dateCheck17 = new DateTime(year, month, 17);
                        if (dateCheck17.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_17Background = Brushes.Red;
                        }


                        // date 18
                        var dateCheck18 = new DateTime(year, month, 18);
                        if (dateCheck18.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_18Background = Brushes.Red;
                        }


                        // date 19
                        var dateCheck19 = new DateTime(year, month, 19);
                        if (dateCheck19.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_19Background = Brushes.Red;
                        }


                        // date 20
                        var dateCheck20 = new DateTime(year, month, 20);
                        if (dateCheck20.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_20Background = Brushes.Red;
                        }


                        // date 21
                        var dateCheck21 = new DateTime(year, month, 21);
                        if (dateCheck21.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_21Background = Brushes.Red;
                        }


                        // date 22
                        var dateCheck22 = new DateTime(year, month, 22);
                        if (dateCheck22.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_22Background = Brushes.Red;
                        }


                        // date 23
                        var dateCheck23 = new DateTime(year, month, 23);
                        if (dateCheck23.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_23Background = Brushes.Red;
                        }


                        // date 24
                        var dateCheck24 = new DateTime(year, month, 24);
                        if (dateCheck24.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_24Background = Brushes.Red;
                        }


                        // date 25
                        var dateCheck25 = new DateTime(year, month, 25);
                        if (dateCheck25.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_25Background = Brushes.Red;
                        }


                        // date 26
                        var dateCheck26 = new DateTime(year, month, 26);
                        if (dateCheck26.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_26Background = Brushes.Red;
                        }


                        // date 27
                        var dateCheck27 = new DateTime(year, month, 27);
                        if (dateCheck27.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_27Background = Brushes.Red;
                        }


                        // date 28
                        var dateCheck28 = new DateTime(year, month, 28);
                        if (dateCheck28.DayOfWeek == DayOfWeek.Sunday)
                        {
                            attendanceRecordSearch.Shift_28Background = Brushes.Red;
                        }


                        // date 29
                        if (daysInMonth >= 29)
                        {
                            var dateCheck29 = new DateTime(year, month, 29);
                            if (dateCheck29.DayOfWeek == DayOfWeek.Sunday)
                            {
                                attendanceRecordSearch.Shift_29Background = Brushes.Red;
                            }
                            Dispatcher.Invoke(new Action(() =>
                            {
                                col29.Visibility = Visibility.Visible;
                            }));
                        }

                        // date 30
                        if (daysInMonth >= 30)
                        {
                            var dateCheck30 = new DateTime(year, month, 30);
                            if (dateCheck30.DayOfWeek == DayOfWeek.Sunday)
                            {
                                attendanceRecordSearch.Shift_30Background = Brushes.Red;
                            }
                            Dispatcher.Invoke(new Action(() =>
                            {
                                col30.Visibility = Visibility.Visible;
                            }));
                        }

                        // date 31
                        if (daysInMonth >= 31)
                        {
                            var dateCheck31 = new DateTime(year, month, 31);
                            if (dateCheck31.DayOfWeek == DayOfWeek.Sunday)
                            {
                                attendanceRecordSearch.Shift_31Background = Brushes.Red;
                            }
                            Dispatcher.Invoke(new Action(() =>
                            {
                                col31.Visibility = Visibility.Visible;
                            }));
                        }

                        #endregion
                        resultsList.Add(attendanceRecordSearch);
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
        }

        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dgWorkingShiftDetail.ItemsSource = resultsList;
            this.Cursor = null;
        }

        private void dgWorkingShiftDetail_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
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
