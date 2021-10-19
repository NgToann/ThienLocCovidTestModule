using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class WorkerPrizeModel
    {
        public string WorkerId { get; set; }
        public string CardId { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string Message { get; set; }
        public string TimeScan { get; set; }
    }
}
