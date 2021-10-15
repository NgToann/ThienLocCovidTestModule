using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TLCovidTest.ViewModels;
using System.ComponentModel;

namespace TLCovidTest.Models
{
    public class WorkingShiftViewModel : BaseViewModel
    {
        public string WorkingShiftCode { get; set; }
        private string _WorkingShiftName;
        public string WorkingShiftName
        {
            get { return _WorkingShiftName; }
            set
            {
                _WorkingShiftName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WorkingShiftName"));
            }
        }
        public string WorkingShiftFullName { get; set; }

        private string _TimeIn1;
        public string TimeIn1
        {
            get { return _TimeIn1; }
            set
            {
                _TimeIn1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TimeIn1"));
            }
        }
        private string _TimeOut1;
        public string TimeOut1
        {
            get { return _TimeOut1; }
            set
            {
                _TimeOut1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TimeOut1"));
            }
        }
        private int _MinutesInOut1;
        public int MinutesInOut1
        {
            get { return _MinutesInOut1; }
            set
            {
                _MinutesInOut1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MinutesInOut1"));
            }
        }

        private string _TimeIn2;
        public string TimeIn2
        {
            get { return _TimeIn2; }
            set
            {
                _TimeIn2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TimeIn2"));
            }
        }
        private string _TimeOut2;
        public string TimeOut2
        {
            get { return _TimeOut2; }
            set
            {
                _TimeOut2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TimeOut2"));
            }
        }
        private int _MinutesInOut2;
        public int MinutesInOut2
        {
            get { return _MinutesInOut2; }
            set
            {
                _MinutesInOut2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MinutesInOut2"));
            }
        }

        private string _TimeIn3;
        public string TimeIn3
        {
            get { return _TimeIn3; }
            set
            {
                _TimeIn3 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TimeIn3"));
            }
        }
        private string _TimeOut3;
        public string TimeOut3
        {
            get { return _TimeOut3; }
            set
            {
                _TimeOut3 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TimeOut3"));
            }
        }
        private int _MinutesInOut3;
        public int MinutesInOut3
        {
            get { return _MinutesInOut3; }
            set
            {
                _MinutesInOut3 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MinutesInOut3"));
            }
        }

        private decimal _WorkingDay;
        public decimal WorkingDay
        {
            get { return _WorkingDay; }
            set
            {
                _WorkingDay = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WorkingDay"));
            }
        }

        private int _WorkingHour;
        public int WorkingHour
        {
            get { return _WorkingHour; }
            set
            {
                _WorkingHour = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WorkingHour"));
            }
        }

        private int _TotalMinutes;
        public int TotalMinutes
        {
            get { return _TotalMinutes; }
            set
            {
                _TotalMinutes = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TotalMinutes"));
            }
        }

        public int _IsActive;
        public int IsActive
        {
            get { return _IsActive; }
            set
            {
                _IsActive = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsActive"));
            }
        }
        public bool IsSunday { get; set; }

        private bool _Enable;
        public bool Enable
        {
            get { return _Enable; }
            set
            {
                _Enable = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Enable"));
            }
        }

        private bool _Disable;
        public bool Disable
        {
            get { return _Disable; }
            set
            {
                _Disable = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Disable"));
            }
        }

        private int _IsInOutManyTime;
        public int IsInOutManyTime
        {
            get { return _IsInOutManyTime; }
            set
            {
                _IsInOutManyTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsInOutManyTime"));
            }
        }

        private bool _EnableInOutManyTime;
        public bool EnableInOutManyTime
        {
            get { return _EnableInOutManyTime; }
            set
            {
                _EnableInOutManyTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("EnableInOutManyTime"));
            }
        }

        private bool _DisableInOutManyTime;
        public bool DisableInOutManyTime
        {
            get { return _DisableInOutManyTime; }
            set
            {
                _DisableInOutManyTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DisableInOutManyTime"));
            }
        }

        private int _WRK_JB30BZ;
        public int WRK_JB30BZ {
            get { return _WRK_JB30BZ; }
            set {
                _WRK_JB30BZ = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WRK_JB30BZ"));
            }
        }
        private int _WRK_XXFZ1;
        public int WRK_XXFZ1
        {
            get { return _WRK_XXFZ1; }
            set
            {
                _WRK_XXFZ1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WRK_XXFZ1"));
            }
        }
        private int _WRK_JBFZ;
        public int WRK_JBFZ
        {
            get { return _WRK_JBFZ; }
            set
            {
                _WRK_JBFZ = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WRK_JBFZ"));
            }
        }

    }
}
