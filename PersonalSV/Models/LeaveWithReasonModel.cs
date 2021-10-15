using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class LeaveWithReasonModel
    {
        public string EmployeeCode { get; set; }
        public DateTime AttendanceDate { get; set; }
        public double SalaryRate { get; set; }
        public string Remarks { get; set; }
        public string Reason { get; set; }
        public string FromToDisplay { get; set; }
    }
}
