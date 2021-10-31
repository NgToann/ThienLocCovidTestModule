using System;
using System.Windows;
using System.Windows.Threading;

using TLCovidTest.Helpers;
using TLCovidTest.Views;
using TLCovidTest.Models;

namespace TLCovidTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer clock;
        AccountModel account;
        public MainWindow(AccountModel account)
        {
            this.account = account;
            clock = new DispatcherTimer();
            clock.Tick += Clock_Tick;
            clock.Start();

            InitializeComponent();
            lblUserName.Text = string.Format("User: {0}", account.FullName);
        }

        private void Clock_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                lblTimer.Text = string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now);
            }));
        }

        private void miAddUpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            //AddUpdateEmployeeWindow window = new AddUpdateEmployeeWindow();
            //window.Show();
        }

        private void miSettingsLanguage_Click(object sender, RoutedEventArgs e)
        {
            SettingLanguageWindow window = new SettingLanguageWindow();
            window.ShowDialog();
        }

        private void miChangeCard_Click(object sender, RoutedEventArgs e)
        {
            ChangeEmployeeCodeWindow_1 window = new ChangeEmployeeCodeWindow_1();
            window.ShowDialog();
        }

        private void miCheckingRecordTime_Click(object sender, RoutedEventArgs e)
        {
            AttendanceRecordDetailWindow window = new AttendanceRecordDetailWindow();
            window.Show();
        }

        private void miAddRecordTime_Click(object sender, RoutedEventArgs e)
        {
            AddRecordTimeWindow window = new AddRecordTimeWindow();
            window.Show();
        }

        private void miArrangeWorkingShift_Click(object sender, RoutedEventArgs e)
        {
            ArrangeWorkingShiftWindow window = new ArrangeWorkingShiftWindow();
            window.Show();
        }

        private void miExcuteDataSalary_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDataSalaryWindow window = new ExecuteDataSalaryWindow();
            window.ShowDialog();
        }

        private void miOverTimeLimit_Click(object sender, RoutedEventArgs e)
        {
            LeavWithSalaryWindow window = new LeavWithSalaryWindow();
            window.Show();
        }

        private void MiAttendanceDetail_Click(object sender, RoutedEventArgs e)
        {
            AttendanceRecordDetailWindow window = new AttendanceRecordDetailWindow();
            window.Show();
        }

        private void miWorkingTime_Click(object sender, RoutedEventArgs e)
        {
            AddRecordTimeWindow window = new AddRecordTimeWindow();
            window.Show();
        }

        private void miWorkingShiftList_Click(object sender, RoutedEventArgs e)
        {
            WorkingShiftListWindow window = new WorkingShiftListWindow();
            window.Show();
        }

        private void miEmployeeList_Click(object sender, RoutedEventArgs e)
        {
            EmployeeListWindow window = new EmployeeListWindow();
            window.Show();
        }

        private void miDailyReport_Click(object sender, RoutedEventArgs e)
        {
            DailyReportWindow window = new DailyReportWindow();
            window.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageConfirmClosing")), this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void miSalarySummaryReport_Click(object sender, RoutedEventArgs e)
        {
            WorkingDaySummaryReportWindow window = new WorkingDaySummaryReportWindow();
            window.Show();
        }

        private void miAboutMe_Click(object sender, RoutedEventArgs e)
        {
            AboutMeWindow window = new AboutMeWindow();
            window.Show();
        }

        private void miLine_Click(object sender, RoutedEventArgs e)
        {
            LineListWindow window = new LineListWindow();
            window.Show();
        }

        private void miReport2020_Click(object sender, RoutedEventArgs e)
        {
            Report2020Window window = new Report2020Window();
            window.Show();
        }

        private void miLeaveWithSalary_Click(object sender, RoutedEventArgs e)
        {
            LeaveWithSalaryWindow window = new LeaveWithSalaryWindow();
            window.Show();
        }

        private void miReportMissingRecordTime_Click(object sender, RoutedEventArgs e)
        {
            MissingRecordTimeWindow window = new MissingRecordTimeWindow();
            window.Show();
        }

        private void miCheckOut_Click(object sender, RoutedEventArgs e)
        {
            CheckOutWindow window = new CheckOutWindow();
            window.ShowDialog();
        }

        private void miImportRemarksWorker_Click(object sender, RoutedEventArgs e)
        {
            ImportRemarksWorkerWindow window = new ImportRemarksWorkerWindow();
            window.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (account.IsCovidTest)
            {
                miCovidTest.IsEnabled = true;
                miManager.IsEnabled = false;
                miSalary.IsEnabled = false;

                miDailyReport.IsEnabled = false;
                miSalarySummaryReport.IsEnabled = false;
                miReport2020.IsEnabled = false;
                miReportMissingRecordTime.IsEnabled = false;
            }
            if (account.IsPersonel)
            {
                miCovidTest.IsEnabled = true;
                miManager.IsEnabled = true;
                miSalary.IsEnabled = true;
                miReports.IsEnabled = true;
            }

            if (account.Branch.Equals("THIENLOC"))
            {
                miAboutMe.Visibility = Visibility.Collapsed;
                miSalary.Visibility = Visibility.Collapsed;
                miManager.Visibility = Visibility.Collapsed;

                miDailyReport.Visibility = Visibility.Collapsed;
                miSalarySummaryReport.Visibility = Visibility.Collapsed;
                miReport2020.Visibility = Visibility.Collapsed;
                miReportMissingRecordTime.Visibility = Visibility.Collapsed;

                miWorkerCheckIn.Visibility = Visibility.Collapsed;
                miWorkerCheckOut.Visibility = Visibility.Collapsed;

                //reports
                miReportScanTimeInOut.Visibility = Visibility.Collapsed;
                miTestCovidRandom.Visibility = Visibility.Collapsed;
                miReports.Visibility = Visibility.Collapsed;
                miCovidTest.Visibility = Visibility.Collapsed;

                if (account.IsScan && !account.IsClinic && !account.IsCovidTest)
                {
                    miGift.Visibility = Visibility.Collapsed;
                    miClinic.Visibility = Visibility.Collapsed;
                    WorkerCheckInWindow window = new WorkerCheckInWindow();
                    window.ShowDialog();
                }
            }
            else if(account.Branch.Equals("SAOVIET") || account.Branch.Equals("DAILOC"))
            {
                miCreateTestTerm.Visibility = Visibility.Collapsed;
                miTLTestRandom.Visibility = Visibility.Collapsed;
            }
        }

        private void miWorkerCheckIn_Click(object sender, RoutedEventArgs e)
        {
            WorkerCheckInWindow window = new WorkerCheckInWindow();
            window.ShowDialog();
        }

        private void miWorkerCheckOut_Click(object sender, RoutedEventArgs e)
        {
            WorkerCheckOutWindow window = new WorkerCheckOutWindow();
            window.ShowDialog();
        }

        private void miReportScanTimeInOut_Click(object sender, RoutedEventArgs e)
        {
            WorkerCheckInReportWindow window = new WorkerCheckInReportWindow();
            window.Show();
        }

        private void miTestCovidRandom_Click(object sender, RoutedEventArgs e)
        {
            TestRandomWindow window = new TestRandomWindow();
            window.Show();
        }

        private void miCreateTestTerm_Click(object sender, RoutedEventArgs e)
        {
            TestCovidCreateTestTermWindow window = new TestCovidCreateTestTermWindow();
            window.ShowDialog();
        }

        private void miTLTestRandom_Click(object sender, RoutedEventArgs e)
        {
            TestRandomThienLocWindow window = new TestRandomThienLocWindow();
            window.ShowDialog();
        }

        private void miConfirmTestResult_Click(object sender, RoutedEventArgs e)
        {
            TestCovidConfirmResultWindow window = new TestCovidConfirmResultWindow();
            window.ShowDialog();
        }

        private void miReportCovidTest_Click(object sender, RoutedEventArgs e)
        {
            TestRandomReportResultWindow window = new TestRandomReportResultWindow();
            window.Show();
        }

        private void miScan_Click(object sender, RoutedEventArgs e)
        {
            WorkerPrizeScanWindow window = new WorkerPrizeScanWindow();
            window.ShowDialog();
        }

        private void miImportList_Click(object sender, RoutedEventArgs e)
        {
            WorkerPrizeImportListWindow window = new WorkerPrizeImportListWindow();
            window.ShowDialog();
        }

        private void miUpdatePatientState_Click(object sender, RoutedEventArgs e)
        {
            TLClinicWindow window = new TLClinicWindow();
            window.ShowDialog();
        }

        private void miPatientScan_Click(object sender, RoutedEventArgs e)
        {
            WorkerCheckInWindow window = new WorkerCheckInWindow();
            window.ShowDialog();
        }

        private void miPatientReport_Click(object sender, RoutedEventArgs e)
        {
            WorkerCheckInReportWindow window = new WorkerCheckInReportWindow();
            window.Show();
        }

        //private void miLeaveWithReason_Click(object sender, RoutedEventArgs e)
        //{
        //    //AbsenteesReasonWindow window = new AbsenteesReasonWindow();
        //    //window.Show();
        //}
    }
}
