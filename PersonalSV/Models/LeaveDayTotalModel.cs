using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class LeaveDayTotalModel
    {
        public int RandomNo { get; set; }
        public string EmployeeCode { get; set; }

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public int TotalDay { get; set; }
        public string Remark { get; set; }

        public string Paid { get; set; }
        public bool IsPaid { get; set; }
        public bool IsNotPaid { get; set; }
    }
}
