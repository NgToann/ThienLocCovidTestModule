using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class SourceModel
    {
        public string EmployeeCode { get; set; }
        public string EmployeeID { get; set; }
        public DateTime SourceDate { get; set; }
        public string SourceTime { get; set; }
        public string SourceTimeView { get; set; }
        public string CardNo { get; set; }
    }
}
