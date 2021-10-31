using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class WorkerCheckInModel
    {
        public string Id { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public int CheckType { get; set; }
        public string CheckTypeDisplay { get; set; }
        public DateTime CheckInDate { get; set; }
        public string RecordTime { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public int PatientIndex { get; set; }
    }
}
