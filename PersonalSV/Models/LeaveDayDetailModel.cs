using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class LeaveDayDetailModel
    {
        public string EmployeeCode { get; set; }
        public string EmployeeID { get; set; }
        public DateTime LeaveDate { get; set; }
        public string Paid { get; set; }
        public int TotalDay { get; set; }
        public string Remark { get; set; }
        public int RandomNo { get; set; }
    }
}
