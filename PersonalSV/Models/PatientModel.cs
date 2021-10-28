using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace TLCovidTest.Models
{
    public class PatientModel
    {
        public string EmployeeID { get; set; }
        public DateTime ConfirmDate { get; set; }
        public int StateIndex { get; set; }
        public string StateDisplay { get; set; }
        public string Remarks { get; set; }
        public string ConfirmBy { get; set; }
        public Nullable<DateTime> CreatedTime { get; set; }
        public Nullable<DateTime> UpdatedTime { get; set; }
        public Brush Background { get; set; } = Brushes.Black;
    }
}
