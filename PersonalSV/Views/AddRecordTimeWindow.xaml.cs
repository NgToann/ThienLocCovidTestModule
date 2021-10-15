using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for AddWorkingTimeWindow.xaml
    /// </summary>
    public partial class AddRecordTimeWindow : Window
    {
        List<EmployeeModel> employeeList;
        List<SourceModel> sourceModelList;
        BackgroundWorker bwLoad;
        BackgroundWorker bwAdd;
        DateTime dtDefault = new DateTime(2000, 01, 01);
        public AddRecordTimeWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork +=new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwAdd = new BackgroundWorker();
            bwAdd.DoWork +=new DoWorkEventHandler(bwAdd_DoWork);
            bwAdd.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bwAdd_RunWorkerCompleted);

            employeeList = new List<EmployeeModel>();
            sourceModelList = new List<SourceModel>();

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
                employeeList = EmployeeController.GetAvailable();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(string.Format("{0}", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
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
            string employeeIDSearch = txtEmployeeIDSearch.Text.Trim().ToUpper();
            var employeeModelSearch = employeeList.Where(w => w.EmployeeID.ToUpper() == employeeIDSearch).FirstOrDefault();
            dpDateAdd.SelectedDate = dtDefault;

            btnAdd.IsEnabled = false;

            AddRecordTimeViewModel recordTimeSearch = new AddRecordTimeViewModel();
            if (employeeModelSearch != null)
            {
                recordTimeSearch.EmployeeCode = employeeModelSearch.EmployeeCode;
                recordTimeSearch.EmployeeID = employeeModelSearch.EmployeeID;
                recordTimeSearch.EmployeeName = employeeModelSearch.EmployeeName;
                recordTimeSearch.DateAdd = DateTime.Now.Date;

                btnAdd.IsEnabled = true;
            }
            gridEmployeeInfor.DataContext = recordTimeSearch;
            dpDateAdd.SelectedDate = DateTime.Now.Date;
            btnSearch.IsDefault = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var currentAddRecord = gridEmployeeInfor.DataContext as AddRecordTimeViewModel;
            string messageEmptyError = LanguageHelper.GetStringFromResource("messageDataEmpty");
            string messageDataIncorrect = LanguageHelper.GetStringFromResource("messageDataIncorrect");
            string messageDataExist = LanguageHelper.GetStringFromResource("messageDataExist");
            string controlTime = LanguageHelper.GetStringFromResource("commonDatePickerTime");

            if (string.IsNullOrEmpty(currentAddRecord.TimeAdd))
            {
                MessageBox.Show(string.Format("{0}\n{1}", controlTime, messageEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (currentAddRecord.TimeAdd.Length != 4)
            {
                MessageBox.Show(string.Format("{0}\n{1}", controlTime, messageDataIncorrect), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Created Addition SourceModel
            SourceModel addSource = new SourceModel
            {
                EmployeeCode = currentAddRecord.EmployeeCode,
                SourceDate = currentAddRecord.DateAdd,
                SourceTime = currentAddRecord.TimeAdd,
                SourceTimeView = currentAddRecord.TimeAdd[0].ToString() + currentAddRecord.TimeAdd[1].ToString() + ":" + currentAddRecord.TimeAdd[2].ToString() + currentAddRecord.TimeAdd[3].ToString()
            };

            if (sourceModelList.Where(w => w.EmployeeCode == addSource.EmployeeCode && w.SourceDate == addSource.SourceDate && w.SourceTime == addSource.SourceTime).Count() > 0)
            {
                MessageBox.Show(string.Format("CardNo: {0}\nDate: {1}\nTime: {2}\n{3}", addSource.EmployeeCode,
                                                                                        addSource.SourceDate.ToShortDateString(),
                                                                                        addSource.SourceTimeView,
                                messageDataExist), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (bwAdd.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                bwAdd.RunWorkerAsync(addSource);
            }
            btnAdd.IsDefault = false;
        }
        private void bwAdd_DoWork(object sender, DoWorkEventArgs e)
        {
            var addSourceModel = e.Argument as SourceModel;
            if (addSourceModel != null)
            {
                if (SourceController.Add(addSourceModel) == true)
                {
                    sourceModelList.Add(addSourceModel);
                }
            }
        }
        private void bwAdd_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            dgRecordTime.ItemsSource = null;
            dgRecordTime.ItemsSource = sourceModelList;
        }

        private void dgRecordTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var itemClicked = dgRecordTime.CurrentItem as SourceModel;
            if (itemClicked != null)
            {
                if (MessageBox.Show(string.Format("{0}\n\nCardNo:  {1}\nDate:       {2}\nTime:      {3}", 
                                                    LanguageHelper.GetStringFromResource("messageConfirmDelete"),
                                                    itemClicked.EmployeeCode,
                                                    itemClicked.SourceDate.ToShortDateString(),
                                                    itemClicked.SourceTimeView),
                               this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    if (SourceController.Delete(itemClicked) == true)
                    {
                        sourceModelList.RemoveAll(r => r.EmployeeCode == itemClicked.EmployeeCode && r.SourceDate == itemClicked.SourceDate && r.SourceTime == itemClicked.SourceTime);
                        dgRecordTime.ItemsSource = null;
                        dgRecordTime.ItemsSource = sourceModelList;
                    }
                    else
                    {
                        MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageAddDataError")),
                            this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void dpDateAdd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentView = gridEmployeeInfor.DataContext as AddRecordTimeViewModel;
            if (currentView != null && currentView.EmployeeCode != null)
            {
                sourceModelList = new List<SourceModel>();
                sourceModelList = SourceController.SelectSourceByEmployeeCodeAndDate(currentView.EmployeeCode, currentView.DateAdd);
                dgRecordTime.ItemsSource = null; ;
                dgRecordTime.ItemsSource = sourceModelList;
            }
        }

        private void TxtEmployeeIDSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnSearch.IsDefault = true;
        }

        private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnAdd.IsDefault = true;
        }
    }
}
