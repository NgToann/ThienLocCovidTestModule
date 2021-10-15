using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace TLCovidTest.Models
{
    public class ShiftTempModel
    {
        public string EmployeeCode { get; set; }
        public DateTime ShiftDate { get; set; }
        public string ShiftCode { get; set; }
        public SolidColorBrush ShiftBackground { get; set; }
    }
}
