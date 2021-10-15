using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class WorkingShiftModel
    {
        public string WorkingShiftCode { get; set; }
        public string WorkingShiftName { get; set; }
        public string WorkingShiftFullName { get; set; }

        public string TimeIn1 { get; set; }
        public string TimeOut1 { get; set; }
        public int MinutesInOut1 { get; set; }

        public string TimeIn2 { get; set; }
        public string TimeOut2 { get; set; }
        public int MinutesInOut2 { get; set; }

        public string TimeIn3 { get; set; }
        public string TimeOut3 { get; set; }
        public int MinutesInOut3 { get; set; }

        public decimal WorkingDay { get; set; }
        public int WorkingHour { get; set; }
        public int TotalMinutes { get; set; }
        public int IsActive { get; set; }

        public bool Enable { get; set; }
        public bool Disable { get; set; }

        public int IsInOutManyTime { get; set; }
        public bool EnableInOutManyTime { get; set; }
        public bool DisableInOutManyTime { get; set; }

        public int WRK_JB30BZ { get; set; }
        public int WRK_XXFZ1 { get; set; }
        public int WRK_JBFZ { get; set; }

        public bool IsSunday { get; set; }
    }
}
