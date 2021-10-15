using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.Helpers;
using System;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for AddUpdateWorkingShiftWindow.xaml
    /// </summary>
    public partial class AddUpdateWorkingShiftWindow : Window
    {
        BackgroundWorker bwSave;
        BackgroundWorker bwDelete;
        WorkingShiftViewModel viewModel;
        WorkingShiftModel showData;

        List<String> workingShiftCodeList;
        public WorkingShiftModel tranferModel;
        EnumEditMode editMode;
        public EnumEditMode enumRespone = EnumEditMode.None;

        public AddUpdateWorkingShiftWindow(WorkingShiftViewModel _viewModel, List<String> _workingShiftCodeList, EnumEditMode _editMode)
        {
            this.viewModel = _viewModel;
            this.workingShiftCodeList = _workingShiftCodeList;
            this.editMode = _editMode;

            showData = new WorkingShiftModel();

            bwSave = new BackgroundWorker();
            bwSave.DoWork +=new DoWorkEventHandler(bwSave_DoWork);
            bwSave.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);

            bwDelete = new BackgroundWorker();
            bwDelete.DoWork += BwDelete_DoWork;
            bwDelete.RunWorkerCompleted += BwDelete_RunWorkerCompleted;

            InitializeComponent();
        }
     
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Convert From ViewModel to Model;
            ConvertToModel(viewModel, showData);
            // rad jb30bz
            radjb30bz1.IsChecked = viewModel.WRK_JB30BZ == 1 ? true : false;
            radjb30bz0.IsChecked = viewModel.WRK_JB30BZ == 0 ? true : false;
            // rad xxfz1

            radxxfz1.IsChecked = viewModel.WRK_XXFZ1 == 1 ? true : false;
            radxxfz0.IsChecked = viewModel.WRK_XXFZ1 == 0 ? true : false;

            gridMain.DataContext = showData;
            if (editMode == EnumEditMode.Update)
            {
                btnDelete.IsEnabled = true;
                txtWorkingShiftCode.IsEnabled = false;
            }
            btnSave.IsEnabled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var addModel = gridMain.DataContext as WorkingShiftModel;
            addModel.WRK_JB30BZ = radjb30bz1.IsChecked == true ? 1 : 0;
            addModel.WRK_XXFZ1 = radxxfz1.IsChecked == true ? 1 : 0;
            if (String.IsNullOrEmpty(addModel.WorkingShiftCode))
            {
                MessageBox.Show(string.Format("{0}\n{1}", LanguageHelper.GetStringFromResource("addUpdateWorkingShiftTxtWorkingShiftID"), 
                    LanguageHelper.GetStringFromResource("messageDataEmpty")), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var checkExist = workingShiftCodeList.Where(w => w == addModel.WorkingShiftCode).FirstOrDefault();
            if (checkExist != null && editMode == EnumEditMode.Add)
            {
                MessageBox.Show(string.Format("{0}\n{1}", addModel.WorkingShiftCode, LanguageHelper.GetStringFromResource("messageDataExist")),
                    this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (bwSave.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                btnSave.IsEnabled = false;
                bwSave.RunWorkerAsync(addModel);
            }
        }
        private void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            bool result = true;
            var AddModel = e.Argument as WorkingShiftModel;
            try
            {
                WorkingShiftController.AddOrUpdate(AddModel);
                tranferModel = AddModel as WorkingShiftModel;
                enumRespone = EnumEditMode.Update;
                if (editMode == EnumEditMode.Add)
                {
                    enumRespone = EnumEditMode.Add;
                    workingShiftCodeList.Add(AddModel.WorkingShiftCode);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show(string.Format("{0}", ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error));
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
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageAddDataSucessful")), 
                    this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.Cursor = null;
            btnSave.IsEnabled = true;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var deleteModel = gridMain.DataContext as WorkingShiftModel;
            if (MessageBox.Show(string.Format("{0}\n{1}", LanguageHelper.GetStringFromResource("messageConfirmDelete"), deleteModel.WorkingShiftCode),
                this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                if (bwDelete.IsBusy == false && !String.IsNullOrEmpty(deleteModel.WorkingShiftCode))
                {
                    this.Cursor = Cursors.Wait;
                    bwDelete.RunWorkerAsync(deleteModel);
                }
            }
        }
        private void BwDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            bool result = true;
            var deleteModel = e.Argument as WorkingShiftModel;
            try
            {
                WorkingShiftController.Delete(deleteModel);
                workingShiftCodeList.RemoveAll(r => r == deleteModel.WorkingShiftCode);
                tranferModel = deleteModel as WorkingShiftModel;
                enumRespone = EnumEditMode.Delete;
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show(string.Format("{0}", ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error));
                    result = false;
                }));
            }
            e.Result = result;
        }
        private void BwDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            if (result == true)
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageDeleteDataSucessful")), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                gridMain.DataContext = new WorkingShiftModel();
            }
            this.Cursor = null;
            btnDelete.IsEnabled = true;
        }

        private void TextBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.SelectAll();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void ConvertToModel(WorkingShiftViewModel viewModel, WorkingShiftModel model)
        {
            model.WorkingShiftCode = viewModel.WorkingShiftCode;
            model.WorkingShiftName = viewModel.WorkingShiftName;
            model.WorkingShiftFullName = viewModel.WorkingShiftFullName;

            model.TimeIn1 = viewModel.TimeIn1;
            model.TimeOut1 = viewModel.TimeOut1;
            model.MinutesInOut1 = viewModel.MinutesInOut1;

            model.TimeIn2 = viewModel.TimeIn2;
            model.TimeOut2 = viewModel.TimeOut2;
            model.MinutesInOut2 = viewModel.MinutesInOut2;

            model.TimeIn3 = viewModel.TimeIn3;
            model.TimeOut3 = viewModel.TimeOut3;
            model.MinutesInOut3 = viewModel.MinutesInOut3;

            model.WorkingDay = viewModel.WorkingDay;
            model.WorkingHour = viewModel.WorkingHour;
            model.TotalMinutes = viewModel.TotalMinutes;
            model.IsActive = viewModel.IsActive;

            model.Enable = viewModel.Enable;
            model.Disable = viewModel.Disable;

            model.IsInOutManyTime = viewModel.IsInOutManyTime;
            model.EnableInOutManyTime = viewModel.EnableInOutManyTime;
            model.DisableInOutManyTime = viewModel.DisableInOutManyTime;

            model.WRK_JB30BZ = viewModel.WRK_JB30BZ;
            model.WRK_XXFZ1 = viewModel.WRK_XXFZ1;
            model.WRK_JBFZ = viewModel.WRK_JBFZ;
        }

        private void radjb30bz0_Checked(object sender, RoutedEventArgs e)
        {
            //viewModel.WRK_JB30BZ = 0;
        }

        private void radjb30bz1_Checked(object sender, RoutedEventArgs e)
        {
            //viewModel.WRK_JB30BZ = 1;
        }

        private void radxxfz1_Checked(object sender, RoutedEventArgs e)
        {
            //viewModel.WRK_XXFZ1 = 1;
        }

        private void radxxfz0_Checked(object sender, RoutedEventArgs e)
        {
            //viewModel.WRK_XXFZ1 = 0;
        }
    }
}
