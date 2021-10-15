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
    /// Interaction logic for LimitOverTimeWindow.xaml
    /// </summary>
    public partial class LeavWithSalaryWindow : Window
    {
        BackgroundWorker bwLoad;
        BackgroundWorker bwTreeviewClicked;
        BackgroundWorker bwSave;
        BackgroundWorker bwDelete;
        List<EmployeeModel> employeeList;
        List<DepartmentModel> departmentList;

        public LeavWithSalaryWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwTreeviewClicked = new BackgroundWorker();
            bwTreeviewClicked.DoWork +=new DoWorkEventHandler(bwTreeviewClicked_DoWork);
            bwTreeviewClicked.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwTreeviewClicked_RunWorkerCompleted);

            bwSave = new BackgroundWorker();
            bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);
            bwSave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);

            bwDelete = new BackgroundWorker();
            bwDelete.DoWork += new DoWorkEventHandler(bwDelete_DoWork);
            bwDelete.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwDelete_RunWorkerCompleted);

            departmentList = new List<DepartmentModel>();
            employeeList = new List<EmployeeModel>();
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
                departmentList = DepartmentController.GetDepartments();
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
            // cretate treeview
            var deptParentList = departmentList.Where(w => string.IsNullOrEmpty(w.ParentID) == true).ToList();
            foreach (var departParent in deptParentList)
            {
                TreeViewItem tviParent = new TreeViewItem();
                
                var departmentsChild = departmentList.Where(w => w.ParentID == departParent.DepartmentID).ToList();
                foreach (var child in departmentsChild)
                {
                    TreeViewItem tviChild = new TreeViewItem();
                    tviChild.Header = child.DepartmentName;
                    tviChild.Margin = new Thickness(0, 2, 0, 2);
                    tviChild.FontWeight = FontWeights.Normal;
                    tviChild.Foreground = Brushes.Blue;
                    tviChild.Tag = child;
                    tviChild.MouseDoubleClick += new MouseButtonEventHandler(tvi_MouseDoubleClick);

                    tviParent.Items.Add(tviChild);
                }

                tviParent.Header = departParent.DepartmentName;
                if (departmentsChild.Count() > 0)
                {
                    tviParent.Header = string.Format("{0} ({1})", departParent.DepartmentName, departmentsChild.Count);
                }
                tviParent.FontWeight = FontWeights.Bold;
                tviParent.Margin = new Thickness(0, 2, 0, 2);
                tviParent.Foreground = Brushes.Black;

                tviParent.Tag = departParent;
                tviParent.MouseDoubleClick += new MouseButtonEventHandler(tvi_MouseDoubleClick);

                treeDepartments.Items.Add(tviParent);
            }

            gridOTInfor.DataContext = new OverTimeLimitModel();

            dpDateFrom.SelectedDate = DateTime.Now.Date;
            dpDateTo.SelectedDate = DateTime.Now.Date;
            this.Cursor = null;
        }

        private void tvi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var itemClicked = sender as TreeViewItem;
            var departmentClicked = itemClicked.Tag as DepartmentModel;

            if (departmentClicked != null && bwTreeviewClicked.IsBusy == false)
            {
                // Check Is Parent Dept
                var departmentListClicked = new List<DepartmentModel>();
                var employeeSelectedList = new List<EmployeeModel>();
                //searchTitle = "";
                var childDeptList = departmentList.Where(w => w.ParentID == departmentClicked.DepartmentID).ToList();
                if (childDeptList.Count() > 0)
                {
                    //string parentID = "parentID";
                    foreach (var child in childDeptList)
                    {
                        //parentID = child.ParentID;
                        departmentListClicked.Add(child);
                        employeeSelectedList.AddRange(employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == child.DepartmentFullName.Trim().ToUpper().ToString()));
                    }

                    //var parentName = departmentList.Where(w => w.DepartmentID == parentID).FirstOrDefault();
                    //if (parentName != null)
                    //    searchTitle = string.Format("{0} : {1}", LanguageHelper.GetStringFromResource("commonEmployeeSection"), parentName.DepartmentName);
                    //var parentName = departmentList.Where(w => w.DepartmentID == parentID).FirstOrDefault();
                    //var grandName = departmentList.Where(w => string.IsNullOrEmpty(w.ParentID)).ToList();
                }
                else
                {
                    departmentListClicked.Add(departmentClicked);
                    employeeSelectedList.AddRange(employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == departmentClicked.DepartmentFullName.Trim().ToUpper().ToString()));
                    //searchTitle = string.Format("{0} : {1}", LanguageHelper.GetStringFromResource("commonEmployeeDepartment"), departmentClicked.DepartmentFullName);
                }

                treeDepartments.IsEnabled = false;
                bwTreeviewClicked.RunWorkerAsync(employeeSelectedList);
                this.Cursor = Cursors.Wait;
            }
        }

        private void bwTreeviewClicked_DoWork(object sender, DoWorkEventArgs e)
        {
            var employeeListPerDepartment = e.Argument as List<EmployeeModel>;
            e.Result = employeeListPerDepartment;
        }
        private void bwTreeviewClicked_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtTotal.Text = "";
            this.Cursor = null;
            treeDepartments.IsEnabled = true;
            var employeeListPerDepartment = e.Result as List<EmployeeModel>;

            dgEmployeePerDepartment.ItemsSource = employeeListPerDepartment;
            if (employeeListPerDepartment.Count > 0)
                txtTotal.Text = String.Format("{0} {1}", LanguageHelper.GetStringFromResource("commonTotalEmployee"), employeeListPerDepartment.Count);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string employeeSearch = txtEmployeeSearch.Text.Trim().ToUpper().ToString();
            var employeeSearched = employeeList.Where(w => w.EmployeeCode == employeeSearch || w.EmployeeID.Trim().ToUpper().ToString() == employeeSearch).FirstOrDefault();
            if (employeeSearched != null)
            {
                var employeeListCurrent = new List<EmployeeModel>();
                if (dgEmployeePerDepartment.ItemsSource != null)
                    employeeListCurrent = dgEmployeePerDepartment.ItemsSource.OfType<EmployeeModel>().ToList();
                if (employeeListCurrent.Where(w => w.EmployeeCode == employeeSearched.EmployeeCode).Count() == 0)
                {
                    employeeListCurrent.Add(employeeSearched);
                    dgEmployeePerDepartment.ItemsSource = employeeListCurrent;
                    dgEmployeePerDepartment.Items.Refresh();
                    dgEmployeePerDepartment.SelectedItem = employeeSearched;
                    dgEmployeePerDepartment.ScrollIntoView(employeeSearched);
                    if (employeeListCurrent.Count() > 0)
                        txtTotal.Text = string.Format("{0} : {1}", LanguageHelper.GetStringFromResource("commonTextBlockFrom"), employeeListCurrent.Count());
                }
            }
            btnSearch.IsDefault = false;
            txtEmployeeSearch.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var overTimeSaveModel = gridOTInfor.DataContext as OverTimeLimitModel;
            if (dgEmployeePerDepartment.ItemsSource == null)
                return;
            var employeeSaveList = dgEmployeePerDepartment.ItemsSource.OfType<EmployeeModel>().ToList();
            
            // get Message
            string messageDataIncorrect = LanguageHelper.GetStringFromResource("messageDataIncorrect");
            string controlTimeOutLimit = LanguageHelper.GetStringFromResource("commonDatePickerTimeOutLimit");
            string controlOverTime = LanguageHelper.GetStringFromResource("commonDatePickerOverTime");

            if (overTimeSaveModel.OverTime <= 0)
            {
                MessageBox.Show(string.Format("{0}\n{1}", controlOverTime, messageDataIncorrect), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (String.IsNullOrEmpty(overTimeSaveModel.TimeOutLimit) == true || overTimeSaveModel.TimeOutLimit.Length != 4)
            {
                MessageBox.Show(string.Format("{0}\n{1}", controlTimeOutLimit, messageDataIncorrect), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var overTimeLimitSaveList = new List<OverTimeLimitModel>();

            var dateFrom = dpDateFrom.SelectedDate.Value.Date;
            var dateTo = dpDateTo.SelectedDate.Value.Date;
            foreach (var employee in employeeSaveList)
            {
                for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
                {
                    var saveModel = new OverTimeLimitModel
                    {
                        EmployeeCode = employee.EmployeeCode,
                        OverTimeDate = date,
                        DateIn = date,
                        DateOut = date,
                        OverTime = overTimeSaveModel.OverTime,
                        TimeOutLimit = overTimeSaveModel.TimeOutLimit
                    };
                    overTimeLimitSaveList.Add(saveModel);
                }
            }

            if (bwSave.IsBusy == false)
            {
                btnSave.IsEnabled = false;
                this.Cursor = Cursors.Wait;
                bwSave.RunWorkerAsync(overTimeLimitSaveList);
            }
        }
        private void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            var insertList = e.Argument as List<OverTimeLimitModel>;
            bool result = true;
            foreach (var insertModel in insertList)
            {
                try
                {
                    // Update UI
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (dgEmployeePerDepartment.ItemsSource != null)
                        {
                            var currentList = dgEmployeePerDepartment.ItemsSource.OfType<EmployeeModel>().ToList();
                            var currentModel = currentList.Where(w => w.EmployeeCode == insertModel.EmployeeCode).FirstOrDefault();
                            if (currentModel != null)
                            {
                                dgEmployeePerDepartment.SelectedItem = currentModel;
                                dgEmployeePerDepartment.ScrollIntoView(currentModel);
                            }
                        }
                    }));
                    OverTimeLimitController.InsertOrUpdate(insertModel);
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
            }
            this.Cursor = null;
            btnSave.IsEnabled = true;
        }

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            var itemsRemoveClicked = dgEmployeePerDepartment.SelectedItems.OfType<EmployeeModel>().ToList();
            if (itemsRemoveClicked.Count() > 0 && dgEmployeePerDepartment.ItemsSource != null)
            {
                txtTotal.Text = "";
                var employeeListCurrent = dgEmployeePerDepartment.ItemsSource.OfType<EmployeeModel>().ToList();
                employeeListCurrent.RemoveAll(r=>itemsRemoveClicked.Select(s=>s.EmployeeCode).Contains(r.EmployeeCode));
                dgEmployeePerDepartment.ItemsSource = employeeListCurrent;
                dgEmployeePerDepartment.Items.Refresh();
                if (employeeListCurrent.Count() > 0)
                    txtTotal.Text = string.Format("{0} : {1}", LanguageHelper.GetStringFromResource("commonTextBlockFrom"), employeeListCurrent.Count());
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var overTimeDeleteModel = gridOTInfor.DataContext as OverTimeLimitModel;
            if (dgEmployeePerDepartment.ItemsSource == null)
                return;

            var employeeDeleteList = dgEmployeePerDepartment.ItemsSource.OfType<EmployeeModel>().ToList();

            var overTimeLimitDeleteList = new List<OverTimeLimitModel>();
            var dateFrom = dpDateFrom.SelectedDate.Value.Date;
            var dateTo = dpDateTo.SelectedDate.Value.Date;

            foreach (var employee in employeeDeleteList)
            {
                for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
                {
                    var deleteModel = new OverTimeLimitModel
                    {
                        EmployeeCode = employee.EmployeeCode,
                        OverTimeDate = date,
                        DateIn = date,
                        DateOut = date,
                        OverTime = overTimeDeleteModel.OverTime,
                        TimeOutLimit = overTimeDeleteModel.TimeOutLimit
                    };
                    overTimeLimitDeleteList.Add(deleteModel);
                }
            }

            if (MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageConfirmRemove")),
                                   this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                if (bwDelete.IsBusy == false && overTimeLimitDeleteList.Count() > 0)
                {
                    this.Cursor = Cursors.Wait;
                    bwDelete.RunWorkerAsync(overTimeLimitDeleteList);
                }
            }
        }
        private void bwDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            var overTimeLimitDeleteList = e.Argument as List<OverTimeLimitModel>;
            int i = 1;
            bool result = true;
            foreach (var deleteModel in overTimeLimitDeleteList)
            {
                try
                {
                    OverTimeLimitController.DeleteByEmployeeByDate(deleteModel);
                    // updateUI
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (dgEmployeePerDepartment.ItemsSource != null)
                        {
                            txtTotal.Text = "";
                            var currentList = dgEmployeePerDepartment.ItemsSource.OfType<EmployeeModel>().ToList();

                            var currentModel = currentList.Where(w => w.EmployeeCode == deleteModel.EmployeeCode).FirstOrDefault();
                            if (currentModel != null)
                            {
                                dgEmployeePerDepartment.SelectedItem = currentModel;
                                dgEmployeePerDepartment.ScrollIntoView(currentModel);
                            }
                            if (currentList.Count - i > 0)
                                txtTotal.Text = string.Format("{0} : {1}", LanguageHelper.GetStringFromResource("commonTextBlockFrom"), currentList.Count - i);
                        }
                    }));
                    i++;
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
            e.Result = result;
        }
        private void bwDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            if (result == true)
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageDeleteDataSucessful")),
                                                this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                dgEmployeePerDepartment.ItemsSource = null;
            }
            this.Cursor = null;
            btnDelete.IsEnabled = true;
        }

        private void txtEmployeeSearch_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            btnSearch.IsDefault = true;
        }
        private void txtEmployeeSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnSearch.IsDefault = true;
        }
        private void TextBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.SelectAll();
        }

        private void dgEmployeePerDepartment_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
