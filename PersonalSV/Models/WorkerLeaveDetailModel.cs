using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class WorkerLeaveDetailModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime LeaveDate { get; set; }
        public string Reason { get; set; }
        public string Remark { get; set; }
        public string DateDisplay { get; set; }
    }
}
