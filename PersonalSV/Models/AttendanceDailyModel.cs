using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class AttendanceDailyModel
    {
        public string SectionName { get; set; }
        public string DepartmentName { get; set; }
        public int Total { get; set; }
        public int Attendance { get; set; }
        public int Absent { get; set; }
    }
}
