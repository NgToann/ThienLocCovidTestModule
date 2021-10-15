using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TLCovidTest.Models;

namespace TLCovidTest.ViewModels
{
    public class EmployeeModel : INotifyPropertyChanged
    {
        //private string _EmployeeCode;
        public string EmployeeCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }

        public string Gender { get; set; }
        private bool _GenderWoman;
        public bool GenderWoman
        {
            get { return _GenderWoman; }
            set
            {
                _GenderWoman = value;
                OnPropertyChanged(new PropertyChangedEventArgs("GenderWoman"));
            }
        }

        private bool _GenderMan;
        public bool GenderMan
        {
            get { return _GenderMan; }
            set
            {
                _GenderMan = value;
                OnPropertyChanged(new PropertyChangedEventArgs("GenderMan"));
            }
        }

        public string DepartmentName { get; set; }
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

        public string PositionName { get; set; }
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
        public DateTime DayOfBirth { get; set; }
        public string NationID { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Remark { get; set; }
        public string ATM { get; set; }
        public int AddressId { get; set; }
        public string AddressDetail { get; set; }
        public int AddressCurrentId { get; set; }
        public string AddressCurrentDetail { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        // Covid Info
        public string Reason { get; set; }
        public string LeaveRemark { get; set; }
        public string FromToDisplay { get; set; }

        public string VaccineFirstType { get; set; }
        public DateTime FirstInjectDate { get; set; }
        public string VaccineSecondType { get; set; }
        public DateTime SecondInjectDate { get; set; }

        public string TestRandomTimeIn { get; set; }
    }
}
