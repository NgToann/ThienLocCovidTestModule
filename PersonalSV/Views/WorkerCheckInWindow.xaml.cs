using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TLCovidTest.Controllers;
using TLCovidTest.Helpers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for WorkerCheckInWindow.xaml
    /// </summary>
    public partial class WorkerCheckInWindow : Window
    {
        DispatcherTimer clock;
        BackgroundWorker bwLoad;
        List<EmployeeModel> employeeList;
        private List<WorkerCheckInModel> workerCheckInList;
        private List<PatientModel> patientByEmpIdList;
        private List<string> patientTotalList;

        private string lblNotFound = "", lblWelcome = "";
        string lblMainHeader = "";


        private DateTime toDay = DateTime.Now.Date;
        public WorkerCheckInWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            employeeList = new List<EmployeeModel>();
            workerCheckInList = new List<WorkerCheckInModel>();
            patientTotalList = new List<string>();

            lblNotFound = LanguageHelper.GetStringFromResource("msgPatientNotFound");
            lblMainHeader = LanguageHelper.GetStringFromResource("scanPatientMainHeader");
            lblWelcome = LanguageHelper.GetStringFromResource("msgPatientWelcome");

            clock = new DispatcherTimer();
            clock.Tick += Clock_Tick;
            clock.Start();

            InitializeComponent();
        }
        private void Clock_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                lblClock.Text = string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now);
            }));
        }
        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            txtCardId.IsEnabled = true;
            DoStatistics(patientTotalList, workerCheckInList);
            SetTxtDefault();
        }

        private void BwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                employeeList = EmployeeController.GetAvailable();
                workerCheckInList = WorkerCheckInController.GetByDate(toDay);
                patientTotalList = PatientController.GetTotal();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message.ToString());
                }));
            }
        }

        private void txtCardId_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            var currentTime = string.Format("{0:HH:mm}", DateTime.Now);
            grDisplay.DataContext = null;
            this.Background = Brushes.WhiteSmoke; 
            brDisplay.Background = Brushes.WhiteSmoke;
            toDay = DateTime.Now.Date; 
            tblTitle.Text = string.Format("{0}: {1:dd/MM/yyyy}", lblMainHeader, DateTime.Now);

            if (e.Key == Key.Enter)
            {
                // get worker by cardid
                string scanWhat = txtCardId.Text.Trim().ToUpper().ToString();
                var empById = employeeList.FirstOrDefault(f => f.EmployeeCode.Trim().ToUpper() == scanWhat);
                if (empById != null)
                {
                    try
                    {
                        patientByEmpIdList = PatientController.GetByEmpId(empById.EmployeeID);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                        SetTxtDefault();
                        return;
                    }
                    // Check in patientList
                    if (patientByEmpIdList.Count() > 0)
                    {
                        var patientCurrentState = patientByEmpIdList.FirstOrDefault();
                        AddRecord(empById, patientCurrentState);
                    }
                    else
                    {
                        AlertScan(lblWelcome, Brushes.Green, empById);
                    }
                    DoStatistics(patientTotalList, workerCheckInList);
                }
                else
                {
                    var notFound = new CheckInInfoDisplay
                    {
                        EmployeeName = scanWhat,
                        Message2 = lblNotFound
                    };
                    grDisplay.DataContext = notFound;
                    SetTxtDefault();
                }
            }
        }

        private void DoStatistics(List<string> patientList, List<WorkerCheckInModel> workerCheckInList)
        {
            var totalPatient = LanguageHelper.GetStringFromResource("countTotalPatient");
            var scanDate = LanguageHelper.GetStringFromResource("countScanDate");
            var scanned = LanguageHelper.GetStringFromResource("countScanned");
            var totalCheckedIn = workerCheckInList.Select(s => s.EmployeeCode).Distinct().ToList();

            var displayModel = new CheckInStatistics
            {
                ScanDate = string.Format("{0}: {1:dd/MM/yyyy}", scanDate, toDay),
                TotalPatient = string.Format("{0}: {1}", totalPatient, patientList.Count()),
                Scanned = string.Format("{0}: {1} / {2}", scanned, totalCheckedIn.Count(), patientList.Count())
            };

            grStatistics.DataContext = null;
            grStatistics.DataContext = displayModel;
        }

        private void AlertScan(string msg, SolidColorBrush color ,EmployeeModel empById)
        {
            brDisplay.Background = color;
            var alert = new CheckInInfoDisplay
            {
                EmployeeName = empById.EmployeeName,
                EmployeeID = empById.EmployeeID,
                DepartmentName = empById.DepartmentName,
                Message1 = msg
            };
            grDisplay.DataContext = alert;
            SetTxtDefault();
        }
        
        private void AddRecord( EmployeeModel empById, PatientModel patient)
        {

            var notAllowed = LanguageHelper.GetStringFromResource("msgPatientNotAllowed");
            var contactClinic = LanguageHelper.GetStringFromResource("msgContactClinic");
            string msg1 = notAllowed, msg2 = contactClinic;

            // Check Status
            if (patient.StateIndex == 0)
            {
                msg1 = lblWelcome;
                msg2 = "";
                brDisplay.Background = Brushes.Green;
            }
            else if (patient.StateIndex == 1)
            {
                brDisplay.Background = Brushes.Red;
            }
            else if (patient.StateIndex == 2)
            { 
                brDisplay.Background = Brushes.Orange;
            }    
            else if (patient.StateIndex == 3)
            {
                brDisplay.Background = Brushes.Yellow;
            }

            if (patient.ConfirmDate.Date == toDay.Date)
            {
                msg1 = msg2 = "";
            }

            var record = new WorkerCheckInModel()
            {
                Id = Guid.NewGuid().ToString(),
                EmployeeCode = empById.EmployeeCode,
                EmployeeID = empById.EmployeeID,
                EmployeeName = empById.EmployeeName,
                DepartmentName = empById.DepartmentName,
                CheckType = 0,
                CheckInDate = DateTime.Now,
                RecordTime = String.Format("{0:HH:mm}", DateTime.Now),
                PatientIndex = patient.StateIndex
            };

            try
            {

                var addInfoDisplay = new CheckInInfoDisplay
                {
                    EmployeeName = empById.EmployeeName,
                    EmployeeID = empById.EmployeeID,
                    DepartmentName = empById.DepartmentName,
                    RecordTime = String.Format("Time: {0}", record.RecordTime),
                    Message1 = msg1,
                    Message2 = msg2
                };

                grDisplay.DataContext = addInfoDisplay;
                WorkerCheckInController.Insert(record);
                patientTotalList = PatientController.GetTotal();
                workerCheckInList.Add(record);

                SetTxtDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                SetTxtDefault();
            }
        }

        private void SetTxtDefault()
        {
            txtCardId.SelectAll();
            txtCardId.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tblTitle.Text = string.Format("{0}: {1:dd/MM/yyyy}", lblMainHeader, DateTime.Now);
            if (bwLoad.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
        }

        private void txtCardId_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtCardId.SelectAll();
        }
        
        private class CheckInStatistics
        {
            public string ScanDate { get; set; }
            public string TotalPatient { get; set; }
            public string Scanned { get; set; }
        }
        
        private class CheckInInfoDisplay
        {
            public string EmployeeName { get; set; }
            public string EmployeeID { get; set; }
            public string DepartmentName { get; set; }
            public string RecordTime { get; set; }
            public string Message1 { get; set; }
            public string Message2 { get; set; }
        }
    }
}
