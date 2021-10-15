using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for CreateUppdateEmployeeWindow.xaml
    /// </summary>
    public partial class AddUpdateEmployeeWindow : Window
    {
        BackgroundWorker bwLoad;
        BackgroundWorker bwSave;
        List<DepartmentModel> departmentList;
        List<PositionModel> positionList;
        EmployeeViewModel employeeInjectModel;
        EmployeeModel showData;

        List<EmployeeModel> employeeList;

        public EmployeeModel updateModelTranfer;
        public List<EmployeeModel> addEmployeeTranferList;
        EnumEditMode editMode;
        public EnumEditMode responeMode = EnumEditMode.None;

        public AddUpdateEmployeeWindow(EmployeeViewModel _injectModel, EnumEditMode _editMode, List<DepartmentModel> _departmentList, List<PositionModel> _positionList)
        {
            this.employeeInjectModel = _injectModel;
            this.editMode = _editMode;
            this.departmentList = _departmentList;
            this.positionList = _positionList;

            bwLoad = new BackgroundWorker();
            bwLoad.DoWork +=new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwSave = new BackgroundWorker();
            bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);
            bwSave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);

            addEmployeeTranferList = new List<EmployeeModel>();

            showData = new EmployeeModel();
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
            // Binding to UI
            ConvertToModel(employeeInjectModel, showData);

            gridMain.DataContext = showData;
            if (editMode == EnumEditMode.Add)
            {
                txtEmployeeCode.IsEnabled = true;
            }
            // Binding Combobox
            cbDepartment.ItemsSource = departmentList;
            cbPosition.ItemsSource = positionList;

            this.Cursor = null;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var addModel = gridMain.DataContext as EmployeeModel;
            if (addModel != null)
            {
                string messageEmptyError = LanguageHelper.GetStringFromResource("messageDataEmpty");
                string messageExistError = LanguageHelper.GetStringFromResource("messageDataExist");
                string controlEmployeeCode = LanguageHelper.GetStringFromResource("commonEmployeeCode");
                string controlEmployeeID = LanguageHelper.GetStringFromResource("commonEmployeeID");
                string controlNationID = LanguageHelper.GetStringFromResource("commonEmployeeNationID");

                // Check Empty
                if (String.IsNullOrEmpty(addModel.EmployeeID))
                {
                    MessageBox.Show(string.Format("{0}\n{1}", controlEmployeeID, messageEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (String.IsNullOrEmpty(addModel.NationID))
                {
                    MessageBox.Show(string.Format("{0}\n{1}", controlNationID, messageEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Check Exist
                // Add New
                if (editMode == EnumEditMode.Add)
                {
                    if (employeeList.Where(w => w.EmployeeCode.ToUpper().Trim().ToString() == addModel.EmployeeCode.ToUpper().Trim().ToString()).Count() > 0)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlEmployeeCode, messageExistError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (employeeList.Where(w => w.EmployeeID.ToUpper().Trim().ToString() == addModel.EmployeeID.ToUpper().Trim().ToString()).Count() > 0)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlEmployeeID, messageExistError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (employeeList.Where(w => w.NationID.ToUpper().Trim().ToString() == addModel.NationID.ToUpper().Trim().ToString()).Count() > 0)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlNationID, messageExistError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                // Update
                if (editMode == EnumEditMode.Update)
                {
                    var checkList = employeeList.Where(w => w.EmployeeCode != addModel.EmployeeCode).ToList();
                    if (checkList.Where(w => w.EmployeeID.ToUpper().Trim().ToString() == addModel.EmployeeID.ToUpper().Trim().ToString()).Count() > 0)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlEmployeeID, messageExistError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (checkList.Where(w => w.NationID.ToUpper().Trim().ToString() == addModel.NationID.ToUpper().Trim().ToString()).Count() > 0)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlNationID, messageExistError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                if (bwSave.IsBusy == false)
                {
                    this.Cursor = Cursors.Wait;
                    btnSave.IsEnabled = false;
                    bwSave.RunWorkerAsync(addModel);
                }
            }
        }
        private void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            bool result = false;
            var addModel = e.Argument as EmployeeModel;
            try
            {
                EmployeeController.AddOrUpdate(addModel, editMode);
                updateModelTranfer = addModel as EmployeeModel;
                responeMode = EnumEditMode.Update;
                if (editMode == EnumEditMode.Add)
                {
                    employeeList.Add(addModel);
                    addEmployeeTranferList.Add(addModel);
                    responeMode = EnumEditMode.Add;
                }
                result = true;
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
        private void bwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            if (result == true)
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("messageAddDataSucessful")),
                                    this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                if (editMode == EnumEditMode.Add)
                {
                    var refreshModel = new EmployeeModel();
                    gridMain.DataContext = refreshModel;
                }
            }
            btnSave.IsEnabled = true;
            this.Cursor = null;
        }

        private void ConvertToModel(EmployeeViewModel viewModel, EmployeeModel model)
        {
            model.EmployeeCode = viewModel.EmployeeCode;
            model.EmployeeID = viewModel.EmployeeID;
            model.EmployeeName = viewModel.EmployeeName;

            if (!string.IsNullOrEmpty(viewModel.Gender))
            {
                model.GenderWoman = viewModel.Gender.ToUpper() == "WOMAN" ? true : false;
                model.GenderMan = viewModel.Gender.ToUpper() == "MAN" ? true : false;
            }
            if (!string.IsNullOrEmpty(viewModel.DepartmentName))
            {
                model.DepartmentSelected = departmentList.Where(w => w.DepartmentFullName.ToUpper().Trim().ToString() == viewModel.DepartmentName.ToUpper().Trim().ToString()).FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(viewModel.PositionName))
            {
                model.PositionSelected = positionList.Where(w => w.PositionName.ToUpper().Trim().ToString() == viewModel.PositionName.ToUpper().Trim().ToString()).FirstOrDefault();
            }

            model.JoinDate = viewModel.JoinDate;
            model.NationID = viewModel.NationID;
            model.Address = viewModel.Address;
            model.PhoneNumber = viewModel.PhoneNumber;
            model.Remark = viewModel.Remark;
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
