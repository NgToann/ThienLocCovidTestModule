using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;


namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for ChangeEmployeeCodeWindow.xaml 
    /// </summary>
    public partial class ChangeEmployeeCodeWindow : Window
    {
        BackgroundWorker bwLoad;
        BackgroundWorker bwSave;
        List<EmployeeModel> employeeList;
        ObservableCollection<EmployeeModel> employeeViewModelList;

        public ChangeEmployeeCodeWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwSave = new BackgroundWorker();
            bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);
            bwSave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);

            employeeList = new List<EmployeeModel>();
            employeeViewModelList = new ObservableCollection<EmployeeModel>();

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

        private void bwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                employeeList = EmployeeController.GetAll();
                // convert to viewmodels
                foreach (var employee in employeeList)
                {
                    EmployeeModel empViewModel = new EmployeeModel();
                    empViewModel.EmployeeCode = employee.EmployeeCode != null ? employee.EmployeeCode.ToUpper().ToString() : "";
                    empViewModel.EmployeeID = employee.EmployeeID != null ? employee.EmployeeID.ToUpper().ToString() : "";
                    empViewModel.EmployeeName = employee.EmployeeName != null ? employee.EmployeeName : "";
                    empViewModel.DepartmentName = employee.DepartmentName != null ? employee.DepartmentName : "";
                    empViewModel.PositionName = employee.PositionName != null ? employee.PositionName : "";

                    employeeViewModelList.Add(empViewModel);
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
            this.Cursor = null;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string employeeIDSearch = txtEmployeeIDSearch.Text;
            txtNewEmployeeCode.Clear();
            var employeeSearched = employeeViewModelList.Where(w => w.EmployeeID.ToUpper().ToString() == employeeIDSearch.Trim().ToUpper().ToString()
                                                          || w.EmployeeCode == employeeIDSearch.Trim().ToUpper().ToString()).FirstOrDefault();
            if (employeeSearched != null)
            {
                grbWorkerInformation.DataContext = employeeSearched;
                btnSave.IsEnabled = true;
            }
            else
            {
                MessageBox.Show(string.Format("{0} {1}", employeeIDSearch, LanguageHelper.GetStringFromResource("messageNotFound")),
                                this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                grbWorkerInformation.DataContext = new EmployeeModel();
                btnSave.IsEnabled = false;
            }
            btnSearch.IsDefault = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string employeeCodeOld = txtEmployeeCode.Text;
            string employeeCodeNew = txtNewEmployeeCode.Text.Trim();
            string employeeID = txtEmployeeID.Text;
            // Check Empty
            if (string.IsNullOrEmpty(employeeCodeNew))
            {
                MessageBox.Show(string.Format("{0}\n{1}", LanguageHelper.GetStringFromResource("changeEmployeeCodeWindowTxtNewCardNumber"), 
                                                         LanguageHelper.GetStringFromResource("messageDataEmpty")),
                                this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check Equal
            if (employeeCodeOld == employeeCodeNew)
            {
                MessageBox.Show(string.Format("{0}\n{1}\n{2}", employeeCodeOld ,employeeCodeNew ,
                                                         LanguageHelper.GetStringFromResource("messageDataDuplicate")),
                                this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check Exist
            if (employeeCodeOld != employeeCodeNew && employeeViewModelList.Where(w => w.EmployeeCode == employeeCodeNew).Count() > 0)
            {
                MessageBox.Show(string.Format("{0}\n{1}", employeeCodeNew,
                                                         LanguageHelper.GetStringFromResource("messageDataExist")),
                                this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (bwSave.IsBusy == false)
            {
                btnSave.IsEnabled = false;
                this.Cursor = Cursors.Wait;
                object[] updateWhat = new object[] { employeeID, employeeCodeNew };
                bwSave.RunWorkerAsync(updateWhat);
            }
        }

        private void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            var updateWhat = e.Argument as object[];
            bool result = false;
            // Excute Database
            try
            {
                EmployeeController.UpdateEmployeeCode(updateWhat[0].ToString(), updateWhat[1].ToString(), false);
                result = true;
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0}\nPlease Try Again !", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }));
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

                // Update View Model.
                string employeeCodeOld = txtEmployeeCode.Text;
                string employeeCodeNew = txtNewEmployeeCode.Text.Trim();

                var employeeViewModelCurrent = employeeViewModelList.Where(w => w.EmployeeCode == employeeCodeOld).FirstOrDefault();
                employeeViewModelCurrent.EmployeeCode = employeeCodeNew;

                // Update UI
                grbWorkerInformation.DataContext = new EmployeeModel();
                grbWorkerInformation.DataContext = employeeViewModelCurrent;
            }
            this.Cursor = null;
            btnSave.IsEnabled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            employeeViewModelList = new ObservableCollection<EmployeeModel>();
        }

        private void TxtEmployeeIDSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnSearch.IsDefault = true;
        }

        private void txtNewEmployeeCode_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnSearch.IsDefault = false;
        }
    }
}
