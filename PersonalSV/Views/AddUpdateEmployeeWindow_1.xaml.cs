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
using System.Windows.Media.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for AddUpdateEmployeeWindow_1.xaml
    /// </summary>
    public partial class AddUpdateEmployeeWindow_1 : Window
    {
        BackgroundWorker bwLoad;
        BackgroundWorker bwSave;
        List<DepartmentModel> departmentList;
        List<PositionModel> positionList;
        EmployeeViewModel employeeInjectModel;
        EmployeeModel showData;

        List<EmployeeModel> employeeList;
        List<AddressModel> addressList;
        EmployeeFaceModel employeeFace;
        public EmployeeModel updateModelTranfer;
        public List<EmployeeModel> addEmployeeTranferList;
        EnumEditMode editMode;
        public EnumEditMode responeMode = EnumEditMode.None;

        public AddUpdateEmployeeWindow_1(EmployeeViewModel _injectModel, EnumEditMode _editMode, List<DepartmentModel> _departmentList, List<PositionModel> _positionList, List<AddressModel> _addressList)
        {
            this.employeeInjectModel    = _injectModel;
            this.editMode               = _editMode;
            this.departmentList         = _departmentList;
            this.positionList           = _positionList;
            this.addressList            = _addressList;

            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
            bwLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoad_RunWorkerCompleted);

            bwSave = new BackgroundWorker();
            bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);
            bwSave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);

            addEmployeeTranferList = new List<EmployeeModel>();
            employeeFace = new EmployeeFaceModel();
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
                if (employeeInjectModel != null && employeeInjectModel.EmployeeID != null)
                    employeeFace = EmployeeFaceController.GetImageByID(employeeInjectModel.EmployeeID);
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
        
        List<byte[]> imageFaceList = new List<byte[]>();
        private void bwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Binding to UI
            ConvertToModel(employeeInjectModel, showData);
            gridMain.DataContext = showData;

            // Binding Address
            var provinceList = new List<AddressModel>();
            var prvIds = addressList.Select(s => s.ProvinceId).Distinct().ToList();
            foreach (var prvId in prvIds)
            {
                provinceList.Add(addressList.FirstOrDefault(f => f.ProvinceId == prvId));
            }

            var addressById = addressList.FirstOrDefault(f => f.AddressId == showData.AddressId);
            var addressCurrentById = addressList.FirstOrDefault(f => f.AddressId == showData.AddressCurrentId);

            if (editMode == EnumEditMode.Add)
            {
                txtEmployeeCode.IsEnabled = true;
                // Display Today
                dpJoinDate.SelectedDate = DateTime.Now.Date;
                dpDayOfBirth.SelectedDate = new DateTime(1992, 11, 16);

                
                cboProvince.ItemsSource = provinceList;
                cboProvinceTemp.ItemsSource = provinceList;
            }
            else if (addressById != null && addressCurrentById != null)
            {
                provinceList = new List<AddressModel>();
                var provinceCurrentList = new List<AddressModel>();
                foreach (var prvId in prvIds)
                {
                    if (prvId == addressById.ProvinceId)
                        provinceList.Add(addressById);
                    else
                        provinceList.Add(addressList.FirstOrDefault(f => f.ProvinceId == prvId));

                    if (prvId == addressCurrentById.ProvinceId)
                        provinceCurrentList.Add(addressCurrentById);
                    else
                        provinceCurrentList.Add(addressList.FirstOrDefault(f => f.ProvinceId == prvId));
                }

                cboProvince.ItemsSource = provinceList;
                cboProvinceTemp.ItemsSource = provinceCurrentList;

                cboProvince.SelectedItem = addressById;
                cboProvinceTemp.SelectedItem = addressCurrentById;
            }
            else
            {
                cboProvince.ItemsSource = provinceList;
                cboProvinceTemp.ItemsSource = provinceList;
            }

            // Binding Combobox
            cbDepartment.ItemsSource = departmentList;
            cbPosition.ItemsSource = positionList;

            // Show Image Worker
            //imgWorker.Source = null;
            imageFaceList = new List<byte[]>();
            if (employeeFace != null)
            {
                //if (employeeFace.Face1 != null)
                //{
                //    imageFaceList.Add(employeeFace.Face1.ToArray());
                //}
                //if (employeeFace.Face2 != null)
                //{
                //    imageFaceList.Add(employeeFace.Face2.ToArray());
                //}
                //if (employeeFace.Face3 != null)
                //{
                //    imageFaceList.Add(employeeFace.Face3.ToArray());
                //}
                //if (employeeFace.Face4 != null)
                //{
                //    imageFaceList.Add(employeeFace.Face4.ToArray());
                //}
                //if (employeeFace.Face5 != null)
                //{
                //    imageFaceList.Add(employeeFace.Face5.ToArray());
                //}

                if (employeeFace.FaceImage11 != null)
                {
                    imageFaceList.Add(employeeFace.FaceImage11.ToArray());
                }
                if (employeeFace.FaceImage21 != null)
                {
                    imageFaceList.Add(employeeFace.FaceImage21.ToArray());
                }
                if (employeeFace.FaceImage31 != null)
                {
                    imageFaceList.Add(employeeFace.FaceImage31.ToArray());
                }
                if (employeeFace.FaceImage41 != null)
                {
                    imageFaceList.Add(employeeFace.FaceImage41.ToArray());
                }
                if (employeeFace.FaceImage51 != null)
                {
                    imageFaceList.Add(employeeFace.FaceImage51.ToArray());
                }
                if (imageFaceList.Count > 0)
                {
                    imgWorker.Source = GetBitmapImageFromJPG(imageFaceList[0]);

                    btnNextImage.IsEnabled = true;
                    btnBackImage.IsEnabled = true;
                }
            }
      
            this.Cursor = null;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var addModel = gridMain.DataContext as EmployeeModel;
            if (addModel != null)
            {
                string msgEmptyError        = LanguageHelper.GetStringFromResource("messageDataEmpty");
                string msgExistError        = LanguageHelper.GetStringFromResource("messageDataExist");
                string msgIrregularError    = LanguageHelper.GetStringFromResource("messageDataIrRegular");

                string controlEmployeeCode  = LanguageHelper.GetStringFromResource("commonEmployeeCode");
                string controlEmployeeID    = LanguageHelper.GetStringFromResource("commonEmployeeID");
                string controlNationID      = LanguageHelper.GetStringFromResource("commonEmployeeNationID");
                string controlDayOfBirth    = LanguageHelper.GetStringFromResource("commonEmployeeDayOfBirth");
                string controlPhoneNumber   = LanguageHelper.GetStringFromResource("commonEmployeePhoneNumber");
                string controlJoinDate      = LanguageHelper.GetStringFromResource("commonEmployeeJoinDate");
                string controlPosition      = LanguageHelper.GetStringFromResource("commonEmployeePosition");
                string controlDepartment    = LanguageHelper.GetStringFromResource("commonEmployeeDepartment");
                string controlAddress       = LanguageHelper.GetStringFromResource("commonEmployeeAddress");
                string controlAddressCurrent    = LanguageHelper.GetStringFromResource("commonEmployeeAddressCurrent");

                // Check Empty
                if (String.IsNullOrEmpty(addModel.EmployeeCode))
                {
                    MessageBox.Show(string.Format("{0}\n{1}", controlEmployeeCode, msgEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (String.IsNullOrEmpty(addModel.EmployeeID))
                {
                    MessageBox.Show(string.Format("{0}\n{1}", controlEmployeeID, msgEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (String.IsNullOrEmpty(addModel.NationID))
                {
                    MessageBox.Show(string.Format("{0}\n{1}", controlNationID, msgEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (addModel.DepartmentSelected == null)
                {
                    MessageBox.Show(string.Format("{0}\n{1}", controlDepartment, msgEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (addModel.PositionSelected == null)
                {
                    MessageBox.Show(string.Format("{0}\n{1}", controlPosition, msgEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Check Exist
                // Add New
                if (editMode == EnumEditMode.Add)
                {
                    if (employeeList.Where(w => w.EmployeeCode.ToUpper().Trim().ToString() == addModel.EmployeeCode.ToUpper().Trim().ToString()).Count() > 0)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlEmployeeCode, msgExistError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (employeeList.Where(w => w.EmployeeID.ToUpper().Trim().ToString() == addModel.EmployeeID.ToUpper().Trim().ToString()).Count() > 0)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlEmployeeID, msgExistError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (employeeList.Where(w => w.NationID.ToUpper().Trim().ToString() == addModel.NationID.ToUpper().Trim().ToString()).Count() > 0)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlNationID, msgExistError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (_addressSelected == null)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlAddress, msgEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (_addressCurrentSelected == null)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlAddressCurrent, msgEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                // Update
                if (editMode == EnumEditMode.Update)
                {
                    var checkList = employeeList.Where(w => w.EmployeeCode != addModel.EmployeeCode).ToList();
                    if (checkList.Where(w => w.EmployeeID.ToUpper().Trim().ToString() == addModel.EmployeeID.ToUpper().Trim().ToString()).Count() > 0)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlEmployeeID, msgExistError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (checkList.Where(w => w.NationID.ToUpper().Trim().ToString() == addModel.NationID.ToUpper().Trim().ToString()).Count() > 0)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlNationID, msgExistError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (_addressSelected == null && addressCboChanged)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlAddress, msgEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (_addressCurrentSelected == null && currentAddressCboChanged)
                    {
                        MessageBox.Show(string.Format("{0}\n{1}", controlAddressCurrent, msgEmptyError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Check regular
                var regNumber = new Regex(@"[\d]");
                var regWord = new Regex("[a-z]|[A-Z]");
                if (regWord.IsMatch(addModel.EmployeeCode) == true)
                {
                    MessageBox.Show(string.Format("{0}: {1}\n{2}", controlEmployeeCode, addModel.EmployeeCode, msgIrregularError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (regWord.IsMatch(addModel.EmployeeID) == false)
                {
                    MessageBox.Show(string.Format("{0}: {1}\n{2}", controlEmployeeID, addModel.EmployeeID, msgIrregularError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                if (addModel.DayOfBirth == new DateTime(01, 01, 01))
                {
                    MessageBox.Show(string.Format("{0}: {1:MM/dd/yyyy}\n{2}", controlDayOfBirth, addModel.DayOfBirth, msgIrregularError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                if (addModel.JoinDate == new DateTime(01, 01, 01))
                {
                    MessageBox.Show(string.Format("{0}: {1:MM/dd/yyyy}\n{2}", controlJoinDate, addModel.JoinDate, msgIrregularError), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                addModel.AddressId          = _addressSelected != null ? _addressSelected.AddressId : 0;
                addModel.AddressCurrentId   = _addressCurrentSelected != null ? _addressCurrentSelected.AddressId : 0;
                addModel.Address            = _addressSelected != null ?
                                                String.Format("{0}{1} / {2} / {3}", String.IsNullOrEmpty(addModel.AddressDetail) == false ? addModel.AddressDetail + " / " : "", _addressSelected.Commune, _addressSelected.District, _addressSelected.Province)
                                                : addModel.AddressDetail;

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
                    txtEmployeeName.Focus();
                }
            }
            btnSave.IsEnabled = true;
            this.Cursor = null;
        }

        private void ConvertToModel(EmployeeViewModel viewModel, EmployeeModel model)
        {
            model.EmployeeCode  = viewModel.EmployeeCode;
            model.EmployeeID    = viewModel.EmployeeID;
            model.EmployeeName  = viewModel.EmployeeName;

            if (!string.IsNullOrEmpty(viewModel.Gender))
            {
                model.GenderWoman   = viewModel.Gender.ToUpper() == "WOMAN" ? true : false;
                model.GenderMan     = viewModel.Gender.ToUpper() == "MAN" ? true : false;
            }
            if (!string.IsNullOrEmpty(viewModel.DepartmentName))
            {
                model.DepartmentSelected    = departmentList.Where(w => w.DepartmentFullName.ToUpper().Trim().ToString() == viewModel.DepartmentName.ToUpper().Trim().ToString()).FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(viewModel.PositionName))
            {
                model.PositionSelected      = positionList.Where(w => w.PositionName.ToUpper().Trim().ToString() == viewModel.PositionName.ToUpper().Trim().ToString()).FirstOrDefault();
            }

            model.JoinDate      = viewModel.JoinDate;
            model.DayOfBirth    = viewModel.DayOfBirth;
            model.NationID      = viewModel.NationID;
            model.Address       = viewModel.Address;
            model.PhoneNumber   = viewModel.PhoneNumber;
            model.Remark        = viewModel.Remark;
            model.AddressId     = viewModel.AddressId;
            model.AddressDetail = viewModel.AddressDetail;
            model.AddressCurrentId      = viewModel.AddressCurrentId;
            model.AddressCurrentDetail  = viewModel.AddressCurrentDetail;
            //model.ATM           = viewModel.ATM;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        int imageIndex = 0;
        private void btnNextImage_Click(object sender, RoutedEventArgs e)
        {
            if (imageFaceList.Count == 0)
            {
                return;
            }
            imageIndex++;
            if (imageIndex > imageFaceList.Count - 1)
            {
                imageIndex = 0;
            }
            imgWorker.Source = imgWorker.Source = GetBitmapImageFromJPG(imageFaceList[imageIndex]);
        }

        private void btnBackImage_Click(object sender, RoutedEventArgs e)
        {
            if (imageFaceList.Count == 0)
            {
                return;
            }
            imageIndex--;
            if (imageIndex < 0)
            {
                imageIndex = imageFaceList.Count - 1;
            }
            imgWorker.Source = imgWorker.Source = GetBitmapImageFromJPG(imageFaceList[imageIndex]);
        }

        public static byte[] GetJPGFromBitmapImage(BitmapImage bitmapImage)
        {
            MemoryStream memoryStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(memoryStream);
            return memoryStream.ToArray();
        }

        public static BitmapImage GetBitmapImageFromJPG(byte[] jpg)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(jpg);
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

        List<AddressModel> districtList = new List<AddressModel>();
        List<AddressModel> communeList = new List<AddressModel>();
        AddressModel _addressSelected;
        private bool addressCboChanged = false;
        private void cboProvince_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;
            var cboSelected = sender as System.Windows.Controls.ComboBox;
            var prvSelected = cboSelected.SelectedItem as AddressModel;
            if (prvSelected == null)
                return;
            addressCboChanged = true;
            districtList.Clear();
            communeList.Clear();
            _addressSelected = null;
            var distIds = addressList.Where(w => w.ProvinceId == prvSelected.ProvinceId).Select(s => s.DistrictId).Distinct().ToList();
            var addressById = addressList.FirstOrDefault(f => f.AddressId == showData.AddressId);
            
            foreach (var distId in distIds)
            {
                if (addressById != null && addressById.DistrictId == distId)
                    districtList.Add(addressById);
                else
                    districtList.Add(addressList.FirstOrDefault(f => f.DistrictId == distId));
            }
            if (districtList.Count() > 0)
            {
                cboDistrict.ItemsSource = districtList.OrderBy(o => o.District).ToList();
                cboCommune.ItemsSource = communeList.OrderBy(o => o.Commune).ToList();
                if (addressById != null)
                    cboDistrict.SelectedItem = addressById;
            }
        }

        private void cboDistrict_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;
            var cboSelected = sender as System.Windows.Controls.ComboBox;
            var distSelected = cboSelected.SelectedItem as AddressModel;
            if (distSelected == null)
                return;

            communeList.Clear();
            _addressSelected = null;
            var comnIds = addressList.Where(w => w.DistrictId == distSelected.DistrictId).Select(s => s.CommuneId).Distinct().ToList();
            var addressById = addressList.FirstOrDefault(f => f.AddressId == showData.AddressId);
            foreach (var comnId in comnIds)
            {
                if (addressById != null && addressById.CommuneId == comnId)
                    communeList.Add(addressById);
                else
                    communeList.Add(addressList.FirstOrDefault(f => f.CommuneId == comnId));
            }
            if (communeList.Count() > 0)
            {
                cboCommune.ItemsSource = communeList.OrderBy(o => o.Commune).ToList();
                if (addressById != null)
                    cboCommune.SelectedItem = addressById;
            }
        }

        private void cboCommune_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var cboSelected = sender as System.Windows.Controls.ComboBox;
            var communeSelected = cboSelected.SelectedItem as AddressModel;
            if (communeSelected == null)
                return;
            _addressSelected = communeSelected;
        }

        List<AddressModel> districtTempList = new List<AddressModel>();
        List<AddressModel> communeTempList = new List<AddressModel>();
        AddressModel _addressCurrentSelected;
        private bool currentAddressCboChanged = false;
        private void cboProvinceTemp_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            var cboSelected = sender as System.Windows.Controls.ComboBox;
            var prvSelected = cboSelected.SelectedItem as AddressModel;
            if (prvSelected == null)
                return;

            currentAddressCboChanged = true;
            districtTempList.Clear();
            communeTempList.Clear();
            _addressCurrentSelected = null;
            var distIds = addressList.Where(w => w.ProvinceId == prvSelected.ProvinceId).Select(s => s.DistrictId).Distinct().ToList();
            var addressCurrentById = addressList.FirstOrDefault(f => f.AddressId == showData.AddressCurrentId);
            foreach (var distId in distIds)
            {
                if (addressCurrentById != null && addressCurrentById.DistrictId == distId)
                    districtTempList.Add(addressCurrentById);
                else
                    districtTempList.Add(addressList.FirstOrDefault(f => f.DistrictId == distId));
                
            }
            if (districtTempList.Count() > 0)
            {
                cboDistrictTemp.ItemsSource = districtTempList.OrderBy(o => o.District).ToList();
                cboCommuneTemp.ItemsSource = communeTempList.OrderBy(o => o.Commune).ToList();
                if (addressCurrentById != null)
                    cboDistrictTemp.SelectedItem = addressCurrentById;
            }
        }

        private void cboDistrictTemp_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            var cboSelected = sender as System.Windows.Controls.ComboBox;
            var distSelected = cboSelected.SelectedItem as AddressModel;
            if (distSelected == null)
                return;

            communeTempList.Clear();
            _addressCurrentSelected = null;

            var comnIds = addressList.Where(w => w.DistrictId == distSelected.DistrictId).Select(s => s.CommuneId).Distinct().ToList();
            var addressCurrentById = addressList.FirstOrDefault(f => f.AddressId == showData.AddressCurrentId);
            foreach (var comnId in comnIds)
            {
                if (addressCurrentById != null && addressCurrentById.CommuneId == comnId)
                    communeTempList.Add(addressCurrentById);
                else
                    communeTempList.Add(addressList.FirstOrDefault(f => f.CommuneId == comnId));
            }
            if (communeTempList.Count() > 0)
            {
                cboCommuneTemp.ItemsSource = communeTempList.OrderBy(o => o.Commune).ToList();
                if (addressCurrentById != null)
                    cboCommuneTemp.SelectedItem = addressCurrentById;
            }
        }

        private void cboCommuneTemp_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var cboSelected = sender as System.Windows.Controls.ComboBox;
            var communeSelected = cboSelected.SelectedItem as AddressModel;
            if (communeSelected == null)
                return;
            _addressCurrentSelected = communeSelected;
        }
    }
}
