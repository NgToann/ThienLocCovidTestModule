using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class AttendanceRecordModel
    {
        public string EmployeeCode { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string ShiftNo { get; set; }

        public string AttendanceIn1 { get; set; }
        public DateTime? AttendanceDateIn1 { get; set; }

        public string AttendanceIn2 { get; set; }
        public DateTime? AttendanceDateIn2 { get; set; }

        public string AttendanceIn3 { get; set; }
        public DateTime? AttendanceDateIn3 { get; set; }

        public string AttendanceOut1 { get; set; }
        public DateTime? AttendanceDateOut1 { get; set; }

        public string AttendanceOut2 { get; set; }
        public DateTime? AttendanceDateOut2 { get; set; }

        public string AttendanceOut3 { get; set; }
        public DateTime? AttendanceDateOut3 { get; set; }

        public double WorkingDay { get; set; }
        public double? WorkingTime { get; set; }
        public double? WorkingOverTime { get; set; }

        public string Remarks { get; set; }
        public string DayOfWeek { get; set; }

        public string OverTimeIn { get; set; }
        public string OverTimeOut { get; set; }

        public double? TimeLate { get; set; }

        public double? Absent { get; set; }
        public double? Ask { get; set; }
        public double? OverTime2 { get; set; }
    }
}
