using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TLCovidTest.ViewModels
{
    public class AddRecordTimeViewModel : BaseViewModel
    {
        public string EmployeeCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }

        private DateTime _DateAdd;
        public DateTime DateAdd
        {
            get { return _DateAdd; }
            set
            {
                _DateAdd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DateAdd"));
            }
        }

        private string _TimeAdd;
        public string TimeAdd
        {
            get { return _TimeAdd; }
            set
            {
                _TimeAdd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TimeAdd"));
            }
        }
    }
}
