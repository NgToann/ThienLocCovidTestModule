using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class DepartmentModel
    {
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentFullName { get; set; }
        public string pid { get; set; }
        public int DepartmentLevel { get; set; }
        public string FullPath { get; set; }
        public string ParentID { get; set; }
    }
}
