using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for EmployeeListWindow.xaml
    /// </summary>
    public partial class EmployeeListWindow : Window
    {
        BackgroundWorker bwLoad;
        List<DepartmentModel> departmentList;
        List<PositionModel> positionList;
        List<EmployeeModel> employeeList;
        List<EmployeeModel> employeeAllList;
        List<EmployeeViewModel> employeeViewModelList;
        List<EmployeeViewModel> employeeViewModelAllList;
        List<EmployeeViewModel> employeeViewModeFilterlList;
        
        EnumEditMode editMode = EnumEditMode.None;
        private string _SAOVIET = "SaoViet Corporation";
        private string _NGHIVIEC = "NGHI VIEC";
        private string _RESIGNATION = "RESIGNATION";

        List<AddressModel> addressList;
        
        private DateTime dtNothing = new DateTime(1899, 12, 30);

        public EmployeeListWindow()
        {
            departmentList = new List<DepartmentModel>();
            positionList = new List<PositionModel>();

            employeeList = new List<EmployeeModel>();
            employeeAllList = new List<EmployeeModel>();

            employeeViewModelList = new List<EmployeeViewModel>();
            employeeViewModelAllList = new List<EmployeeViewModel>();

            employeeViewModeFilterlList = new List<EmployeeViewModel>();

            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            addressList = new List<AddressModel>();            
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
                departmentList = DepartmentController.GetDepartments();
                employeeList = EmployeeController.GetAvailable();
                employeeAllList = EmployeeController.GetAll();
                positionList = PositionController.GetPositionFromSource();
                addressList = AddressController.GetAddresses();


                // Binding Combobox
                var sectionListFull = new List<DepartmentModel>();
                sectionListFull.Add(new DepartmentModel { DepartmentName = _SAOVIET });
                var sectionList = departmentList.Where(w => String.IsNullOrEmpty(w.ParentID) == true).ToList();
                sectionListFull.AddRange(sectionList);
                if (sectionList.Count() > 0)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        cboSection.ItemsSource = sectionListFull;
                        cboSection.SelectedItem = sectionListFull.FirstOrDefault();
                    }));
                }

                if (employeeList.Count() > 0)
                    employeeList = employeeList.OrderBy(o => o.DepartmentName).ThenBy(th => th.EmployeeID).ToList();

                // Convert to viewModelList;
                foreach (var emp in employeeAllList)
                {
                    var viewModel = new EmployeeViewModel();
                    ConvertToViewModel(emp, viewModel);
                    employeeViewModelAllList.Add(viewModel);
                }

                // Convert to viewModelList;
                foreach (var emp in employeeList)
                {
                    var viewModel = new EmployeeViewModel();
                    ConvertToViewModel(emp, viewModel);
                    employeeViewModelList.Add(viewModel);
                }
                
                Dispatcher.Invoke(new Action(() =>
                {
                    var viewModelList = new ObservableCollection<EmployeeViewModel>(employeeViewModelList);
                    dgEmployee.ItemsSource = viewModelList;
                }));
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

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            editMode = EnumEditMode.Add;
            var employeeAdd = new EmployeeViewModel();
            AddUpdateEmployeeWindow_1 window = new AddUpdateEmployeeWindow_1(employeeAdd, editMode, departmentList, positionList, addressList);
            window.ShowDialog();
            if (window.responeMode == EnumEditMode.Add && window.addEmployeeTranferList.Count() > 0)
            {
                foreach (var emp in window.addEmployeeTranferList)
                {
                    var viewModel = new EmployeeViewModel();
                    ConvertToViewModel(emp, viewModel);
                    employeeViewModelList.Add(viewModel);
                    employeeViewModelAllList.Add(viewModel);
                }
                var viewModelList = new ObservableCollection<EmployeeViewModel>(employeeViewModelList);
                dgEmployee.ItemsSource = viewModelList;
                dgEmployee.Items.Refresh();
            }
        }

        private void DgEmployee_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var itemClicked = dgEmployee.SelectedItem as EmployeeViewModel;
            if (itemClicked != null)
            {
                editMode = EnumEditMode.Update;
                //AddUpdateEmployeeWindow window = new AddUpdateEmployeeWindow(itemClicked, editMode, departmentList, positionList);
                AddUpdateEmployeeWindow_1 window = new AddUpdateEmployeeWindow_1(itemClicked, editMode, departmentList, positionList, addressList);
                window.ShowDialog();
                if (window.updateModelTranfer != null && window.responeMode == EnumEditMode.Update)
                {
                    ConvertToViewModel(window.updateModelTranfer, itemClicked);
                    // Update ViewModelList
                    employeeViewModelAllList.RemoveAll(r => r.EmployeeCode == window.updateModelTranfer.EmployeeCode);
                    employeeViewModelList.RemoveAll(r => r.EmployeeCode == window.updateModelTranfer.EmployeeCode);

                    employeeViewModelAllList.Add(itemClicked);
                    employeeViewModelList.Add(itemClicked);
                }
            }
        }

        private void ConvertToViewModel(EmployeeModel model, EmployeeViewModel viewModel)
        {
            var addressById = addressList.FirstOrDefault(f => f.AddressId == model.AddressId);
            var addressCurentById = addressList.FirstOrDefault(f => f.AddressId == model.AddressCurrentId);

            viewModel.EmployeeCode  = model.EmployeeCode;
            viewModel.EmployeeID    = model.EmployeeID;
            viewModel.EmployeeName  = model.EmployeeName;

            viewModel.Gender = model.Gender;
            if (model.GenderMan != false || model.GenderWoman != false)
            {
                viewModel.Gender = model.GenderWoman != false ? "WOMAN" : "MAN";
            }

            viewModel.DepartmentName = model.DepartmentName;
            if (model.DepartmentSelected != null)
            {
                viewModel.DepartmentName = model.DepartmentSelected.DepartmentFullName;
            }

            viewModel.PositionName = model.PositionName;
            if (model.PositionSelected != null)
            {
                viewModel.PositionName = model.PositionSelected.PositionName;
            }

            viewModel.JoinDate      = model.JoinDate;
            viewModel.DayOfBirth    = model.DayOfBirth;
            viewModel.DayOfBirthDisplay = viewModel.DayOfBirth != dtNothing ? String.Format("{0:MM/dd/yyyy}", viewModel.DayOfBirth) : "";
            viewModel.NationID      = model.NationID;

            viewModel.AddressId     = model.AddressId;
            viewModel.AddressDisplay = model.Address;
            viewModel.AddressDetail = model.Address;
            if (addressById != null)
            {
                viewModel.AddressDetail = model.AddressDetail;
                viewModel.AddressDisplay = String.Format("{0}{1} / {2} / {3}", String.IsNullOrEmpty(model.AddressDetail) == false ? model.AddressDetail + " / " : "", addressById.Commune, addressById.District, addressById.Province);
            }

            viewModel.AddressCurrentId      = model.AddressCurrentId;
            viewModel.AddressCurrentDetail  = model.AddressCurrentDetail;
            viewModel.AddressCurrentDisplay = addressCurentById != null ?
                                                String.Format("{0}{1} / {2} / {3}", String.IsNullOrEmpty(model.AddressCurrentDetail) == false ? model.AddressCurrentDetail + " / " : "", addressCurentById.Commune, addressCurentById.District, addressCurentById.Province)
                                                : "";

            viewModel.PhoneNumber   = model.PhoneNumber;
            viewModel.Remark        = model.Remark;
            //viewModel.ATM           = model.ATM;
        }

        private void CboSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            employeeViewModeFilterlList = new List<EmployeeViewModel>();
            var sectionClicked = cboSection.SelectedItem as DepartmentModel;
            cboDepartment.ItemsSource = new List<DepartmentModel>();
            if (sectionClicked != null)
            {
                cboDepartment.ItemsSource = new List<DepartmentModel>();
                if (sectionClicked.DepartmentName == _SAOVIET)
                {
                    employeeViewModeFilterlList = employeeViewModelList.ToList();
                }
                else if (sectionClicked.DepartmentName.ToUpper() == _NGHIVIEC || sectionClicked.DepartmentName.ToUpper() == _RESIGNATION)
                {
                    employeeViewModeFilterlList = employeeViewModelAllList.Where(w => w.DepartmentName.Trim().ToUpper().Contains(_NGHIVIEC) == true || w.DepartmentName.Trim().ToUpper().Contains(_RESIGNATION) == true).ToList();
                }
                else
                {
                    var childDept = departmentList.Where(w => w.ParentID == sectionClicked.DepartmentID).ToList();
                    if (childDept.Count() > 1)
                    {
                        //groupHeader = departmentList.Where(w => w.DepartmentID == childDept.FirstOrDefault().DepartmentID).FirstOrDefault().DepartmentName;
                        var employeeByChildDept = new List<EmployeeViewModel>();
                        foreach (var child in childDept)
                        {
                            employeeByChildDept = employeeViewModelList.Where(w => w.DepartmentName.ToUpper().Trim().ToString() == child.DepartmentFullName.ToUpper().Trim().ToString()).ToList();
                            employeeViewModeFilterlList.AddRange(employeeByChildDept);
                        }

                        cboDepartment.ItemsSource = childDept;
                        cboDepartment.Items.Refresh();
                    }
                    else
                    {
                        employeeViewModeFilterlList = employeeViewModelList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == sectionClicked.DepartmentName).ToList();
                    }
                }
                FilterDataGrid(employeeViewModeFilterlList);
            }
        }

        private void CboDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            employeeViewModeFilterlList = new List<EmployeeViewModel>();
            var departmentClicked = cboDepartment.SelectedItem as DepartmentModel;
            if (departmentClicked != null)
            {
                employeeViewModeFilterlList = employeeViewModelList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == departmentClicked.DepartmentFullName.Trim().ToUpper().ToString()).ToList();
                FilterDataGrid(employeeViewModeFilterlList);
            }
        }

        private void FilterDataGrid(List<EmployeeViewModel> filterList)
        {
            if (filterList.Count() > 0)
                filterList = filterList.OrderBy(o => o.DepartmentName).ThenBy(th => th.EmployeeID).ToList();
            var viewModelList = new ObservableCollection<EmployeeViewModel>(filterList);
            dgEmployee.ItemsSource = viewModelList;
            dgEmployee.Items.Refresh();
        }

        private void DgEmployee_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void txtEmployeeSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string employeeSearch = txtEmployeeSearch.Text.Trim().ToUpper().ToString();
            if (!String.IsNullOrEmpty(employeeSearch))
            {
                var employeeSearchList = employeeViewModelAllList.Where(w => w.EmployeeID.ToUpper().ToString().Contains(employeeSearch)
                                                                                || w.EmployeeCode.ToUpper().ToString().Contains(employeeSearch)
                                                                                || w.EmployeeName.ToUpper().ToString().Replace(" ", "").Contains(employeeSearch.Replace(" ", ""))
                                                                                || w.PhoneNumber.ToUpper().ToString().Replace(" ","").Contains(employeeSearch.Replace(" ",""))
                                                                                || w.NationID.ToUpper().ToString().Contains(employeeSearch))
                                                                                .ToList();
                FilterDataGrid(employeeSearchList);
            }
            else
            {
                FilterDataGrid(employeeViewModelList);
            }
        }

        private void btnChart_Click(object sender, RoutedEventArgs e)
        {
            EmployeeChartWindow window = new EmployeeChartWindow(employeeList, departmentList);
            window.Show();
        }
    }
}
