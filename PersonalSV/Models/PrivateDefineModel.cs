using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class PrivateDefineModel
    {
        public int PrivateDefineId { get; set; }
        public int NoOfValueDate { get; set; }
        public DateTime StartDateScanWorkerCheckIn { get; set; }
        public int TestRandomRatio { get; set; }
        public string AfternoonStone { get; set; }
        public double RemindTestDate { get; set; }
        public double WaitingMinutes { get; set; }
    }
}
