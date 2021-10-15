using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCovidTest.ViewModels;
using System.ComponentModel;

namespace TLCovidTest.Models
{
    public class OverTimeLimitModel : BaseViewModel
    {
        public string EmployeeCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime OverTimeDate { get; set; }

        private string _TimeOutLimit;
        public string TimeOutLimit
        {
            get { return _TimeOutLimit; }
            set
            {
                _TimeOutLimit = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TimeOutLimit"));
            }
        }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }

        private double _OverTime;
        public double OverTime
        {
            get { return _OverTime; }
            set
            {
                _OverTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("OverTime"));
            }
        }
    }
}
