using System;
using System.ComponentModel;
using System.Windows.Media;

namespace TLCovidTest.ViewModels
{
    public class AttendanceRecordViewModel : INotifyPropertyChanged
    {
        public string EmployeeCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime AttendanceDate { get; set; }
        //public string ShiftNo { get; set; }
        private string _ShiftNo;
        public string ShiftNo
        {
            get { return _ShiftNo; }
            set
            {
                _ShiftNo = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ShiftNo"));
            }
        }
        //public string AttendanceIn1 { get; set; }
        public string _AttendanceIn1;
        public string AttendanceIn1
        {
            get { return _AttendanceIn1; }
            set
            {
                _AttendanceIn1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AttendanceIn1"));
            }
        }
        public DateTime? AttendanceDateIn1 { get; set; }

        //public string AttendanceIn2 { get; set; }
        public string _AttendanceIn2;
        public string AttendanceIn2
        {
            get { return _AttendanceIn2; }
            set
            {
                _AttendanceIn2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AttendanceIn2"));
            }
        }
        public DateTime? AttendanceDateIn2 { get; set; }

        //public string AttendanceIn3 { get; set; }
        public string _AttendanceIn3;
        public string AttendanceIn3
        {
            get { return _AttendanceIn3; }
            set
            {
                _AttendanceIn3 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AttendanceIn3"));
            }
        }
        public DateTime? AttendanceDateIn3 { get; set; }

        //public string AttendanceOut1 { get; set; }
        public string _AttendanceOut1;
        public string AttendanceOut1
        {
            get { return _AttendanceOut1; }
            set
            {
                _AttendanceOut1 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AttendanceOut1"));
            }
        }
        public DateTime? AttendanceDateOut1 { get; set; }

        //public string AttendanceOut2 { get; set; }
        public string _AttendanceOut2;
        public string AttendanceOut2
        {
            get { return _AttendanceOut2; }
            set
            {
                _AttendanceOut2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AttendanceOut2"));
            }
        }
        public DateTime? AttendanceDateOut2 { get; set; }

        //public string AttendanceOut3 { get; set; }
        public string _AttendanceOut3;
        public string AttendanceOut3
        {
            get { return _AttendanceOut3; }
            set
            {
                _AttendanceOut3 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AttendanceOut3"));
            }
        }
        public DateTime? AttendanceDateOut3 { get; set; }

        //public double WorkingDay { get; set; }
        public double? _WorkingDay;
        public double? WorkingDay
        {
            get { return _WorkingDay; }
            set
            {
                _WorkingDay = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WorkingDay"));
            }
        }

        //public double WorkingTime { get; set; }
        public double? _WorkingTime;
        public double? WorkingTime
        {
            get { return _WorkingTime; }
            set
            {
                _WorkingTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WorkingTime"));
            }
        }

        //public double WorkingOverTime { get; set; }
        public double? _WorkingOverTime;
        public double? WorkingOverTime
        {
            get { return _WorkingOverTime; }
            set
            {
                _WorkingOverTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WorkingOverTime"));
            }
        }

        //public string Remarks { get; set; }
        public string _Remarks;
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                _Remarks = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Remarks"));
            }
        }


        public string DayOfWeek { get; set; }
        public string _DayOfWeekFull;
        public string DayOfWeekFull
        {
            get { return _DayOfWeekFull; }
            set
            {
                _DayOfWeekFull = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DayOfWeekFull"));
            }
        }

        private Brush _RowForeground = Brushes.Black;
        public Brush RowForeground
        {
            get { return _RowForeground; }
            set
            {
                _RowForeground = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RowForeground"));
            }
        }

        public string OverTimeIn { get; set; }
        public string OverTimeOut { get; set; }

        private double? _TimeLate;
        public double? TimeLate
        {
            get { return _TimeLate; }
            set
            {
                _TimeLate = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TimeLate"));
            }
        }

        private double? _Absent;
        public double? Absent
        {
            get { return _Absent; }
            set
            {
                _Absent = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Absent"));
            }
        }

        public double? Ask { get; set; }

        private double? _OverTime2;
        public double? OverTime2
        {
            get { return _OverTime2; }
            set
            {
                _OverTime2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs("OverTime2"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
