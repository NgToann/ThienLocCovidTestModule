using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class TestRandomModel
    {
        public string Id { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime TestDate { get; set; }
        public int Term { get; set; }
        public int Round { get; set; }
        public string Result { get; set; }
        public string PersonConfirm { get; set; }
        public string Remark { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string Status { get; set; }
        public string UpdateResultTime { get; set; }
        public Nullable<DateTime> CreatedTime { get; set; }
        public Nullable<DateTime> UpdatedTime { get; set; }

        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
    }
}
