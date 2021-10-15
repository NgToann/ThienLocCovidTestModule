using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for WorkingShiftListWindow.xaml
    /// </summary>
    public partial class WorkingShiftListWindow : Window
    {
        List<WorkingShiftModel> workingShiftList;
        List<WorkingShiftViewModel> workingShiftViewModelList;
        BackgroundWorker bwLoad;
        public ObservableCollection<WorkingShiftViewModel> viewModelList;
        EnumEditMode editMode = EnumEditMode.None;
        public WorkingShiftListWindow()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            workingShiftList = new List<WorkingShiftModel>();
            workingShiftViewModelList = new List<WorkingShiftViewModel>();
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
                Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show(String.Format("{0}", ex.Message), this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }));
            }

        }
        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;

            // Convert To viewModelList
            foreach (var workingShift in workingShiftList)
            {
                var viewModel = new WorkingShiftViewModel();
                ConvertToViewModel(workingShift, viewModel);
                workingShiftViewModelList.Add(viewModel);
            }
            viewModelList = new ObservableCollection<WorkingShiftViewModel>(workingShiftViewModelList);
            dgWorkingShift.ItemsSource = viewModelList;
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            editMode = EnumEditMode.Add;
            var addNew = new WorkingShiftViewModel { Enable = true ,DisableInOutManyTime = true };
            var workingShiftCodeList = workingShiftViewModelList.Select(s => s.WorkingShiftCode).Distinct().ToList();
            AddUpdateWorkingShiftWindow window = new AddUpdateWorkingShiftWindow(addNew, workingShiftCodeList, editMode);
            window.ShowDialog();

            // Add
            if (window.tranferModel != null && window.enumRespone == EnumEditMode.Add)
            {
                var viewModel = new WorkingShiftViewModel();
                ConvertToViewModel(window.tranferModel, viewModel);
                workingShiftViewModelList.Add(viewModel);

                var viewModelList = new ObservableCollection<WorkingShiftViewModel>(workingShiftViewModelList);
                dgWorkingShift.ItemsSource = viewModelList;

                Dispatcher.Invoke(new Action(() =>
                {
                    dgWorkingShift.SelectedItem = viewModel;
                    dgWorkingShift.ScrollIntoView(viewModel);
                }));
            }
        }

        private void DgWorkingShift_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            editMode = EnumEditMode.Update;
            var itemClicked = (WorkingShiftViewModel)dgWorkingShift.SelectedItem;
            if (itemClicked != null)
            {
                var workingShiftCodeList = workingShiftList.Select(s => s.WorkingShiftCode).Distinct().ToList();
                AddUpdateWorkingShiftWindow window = new AddUpdateWorkingShiftWindow(itemClicked, workingShiftCodeList, editMode);
                window.ShowDialog();

                // Delete
                if (window.tranferModel != null && window.enumRespone == EnumEditMode.Delete)
                {
                    workingShiftViewModelList.RemoveAll(r => r.WorkingShiftCode == window.tranferModel.WorkingShiftCode);
                    var viewModelList = new ObservableCollection<WorkingShiftViewModel>(workingShiftViewModelList);

                    dgWorkingShift.ItemsSource = viewModelList;
                    dgWorkingShift.Items.Refresh();
                }

                // Update
                else if (window.tranferModel != null && window.enumRespone == EnumEditMode.Update)
                {
                    ConvertToViewModel(window.tranferModel, itemClicked);
                }
            }
        }

        private void ConvertToViewModel(WorkingShiftModel model, WorkingShiftViewModel viewModel)
        {
            viewModel.WorkingShiftCode = model.WorkingShiftCode;
            viewModel.WorkingShiftName = model.WorkingShiftName;
            viewModel.WorkingShiftFullName = model.WorkingShiftFullName;

            viewModel.TimeIn1 = model.TimeIn1;
            viewModel.TimeOut1 = model.TimeOut1;
            viewModel.MinutesInOut1 = model.MinutesInOut1;

            viewModel.TimeIn2 = model.TimeIn2;
            viewModel.TimeOut2 = model.TimeOut2;
            viewModel.MinutesInOut2 = model.MinutesInOut2;

            viewModel.TimeIn3 = model.TimeIn3;
            viewModel.TimeOut3 = model.TimeOut3;
            viewModel.MinutesInOut3 = model.MinutesInOut3;

            viewModel.WorkingDay = model.WorkingDay;
            viewModel.WorkingHour = model.WorkingHour;
            viewModel.TotalMinutes = model.TotalMinutes;

            //viewModel.IsActive = model.IsActive;
            viewModel.Enable = model.Enable;
            viewModel.Disable = model.Disable;
            viewModel.IsActive = viewModel.Enable == true ? 0 : 1;

            viewModel.IsInOutManyTime = model.IsInOutManyTime;
            viewModel.EnableInOutManyTime = model.EnableInOutManyTime;
            viewModel.DisableInOutManyTime = model.DisableInOutManyTime;

            viewModel.WRK_JB30BZ = model.WRK_JB30BZ;
            viewModel.WRK_XXFZ1 = model.WRK_XXFZ1;
            viewModel.WRK_JBFZ = model.WRK_JBFZ;
        }

        private void dgWorkingShift_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string contentSearch = txtSearch.Text;
            var source = dgWorkingShift.ItemsSource.OfType<WorkingShiftViewModel>().ToList();
            var firstModel = source.FirstOrDefault(f => f.WorkingShiftCode == contentSearch);
            if (firstModel != null)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    dgWorkingShift.SelectedItem = firstModel;
                    dgWorkingShift.ScrollIntoView(firstModel);
                }));
            }
        }
    }
}
