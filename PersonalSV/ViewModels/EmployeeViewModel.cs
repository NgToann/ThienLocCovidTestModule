using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCovidTest.ViewModels;
using System.ComponentModel;

namespace TLCovidTest.Models
{
    public class EmployeeViewModel : BaseViewModel
    {
        public string EmployeeCode { get; set; }

        private string _EmployeeID;
        public string EmployeeID
        {
            get { return _EmployeeID; }
            set
            {
                _EmployeeID = value;
                OnPropertyChanged(new PropertyChangedEventArgs("EmployeeID"));
            }
        }

        private string _EmployeeName;
        public string EmployeeName
        {
            get { return _EmployeeName; }
            set
            {
                _EmployeeName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("EmployeeName"));
            }
        }

        private string _Gender;
        public string Gender
        {
            get { return _Gender; }
            set
            {
                _Gender = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Gender"));
            }
        }

        public bool GenderMan { get; set; }
        public bool GenderWoman { get; set; }

        private string _DepartmentName;
        public string DepartmentName
        {
            get { return _DepartmentName; }
            set
            {
                _DepartmentName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DepartmentName"));
            }
        }
        private DepartmentModel _DepartmentSelected;
        public DepartmentModel DepartmentSelected
        {
            get { return _DepartmentSelected; }
            set
            {
                _DepartmentSelected = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DepartmentSelected"));
            }
        }

        private string _PositionName;
        public string PositionName
        {
            get { return _PositionName; }
            set
            {
                _PositionName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PositionName"));
            }
        }
        private PositionModel _PositionSelected;
        public PositionModel PositionSelected
        {
            get { return _PositionSelected; }
            set
            {
                _PositionSelected = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PositionSelected"));
            }
        }

        public DateTime JoinDate { get; set; }

        private DateTime _DayOfBirth;
        public  DateTime DayOfBirth 
        {
            get { return _DayOfBirth; }
            set
            {
                _DayOfBirth = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DayOfBirth"));
            }
        }

        public string DayOfBirthDisplay { get; set; }

        private string _NationID;
        public string NationID
        {
            get { return _NationID; }
            set
            {
                _NationID = value;
                OnPropertyChanged(new PropertyChangedEventArgs("NationID"));
            }
        }

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Address"));
            }
        }

        private string _PhoneNumber;
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set
            {
                _PhoneNumber = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PhoneNumber"));
            }
        }

        private string _Remark;
        public string Remark
        {
            get { return _Remark; }
            set
            {
                _Remark = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Remark"));
            }
        }

        private string _ATM;
        public string ATM
        {
            get { return _ATM; }
            set
            {
                _ATM = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ATM"));
            }
        }

        public int AddressId { get; set; }

        private string _AddressDetail;
        public string AddressDetail
        {
            get { return _AddressDetail; }
            set
            {
                _AddressDetail = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AddressDetail"));
            }
        }

        private string _AddressDisplay;
        public string AddressDisplay 
        {
            get { return _AddressDisplay; }
            set
            {
                _AddressDisplay = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AddressDisplay"));
            }
        }

        public int AddressCurrentId { get; set; }

        private string _AddressCurrentDetail;
        public string AddressCurrentDetail 
        {
            get { return _AddressCurrentDetail; }
            set
            {
                _AddressCurrentDetail = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AddressCurrentDetail"));
            }
        }
        private string _AddressCurrentDisplay;
        public string AddressCurrentDisplay
        {
            get { return _AddressCurrentDisplay; }
            set
            {
                _AddressCurrentDisplay = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AddressCurrentDisplay"));
            }
        }

        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
