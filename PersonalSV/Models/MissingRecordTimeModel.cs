using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class MissingRecordTimeModel
    {
        public string CardId { get; set; }
        public string WorkerId { get; set; }
        public string  Name { get; set; }
        public string Department { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime Date { get; set; }
        public int NoOfScan { get; set; }
        public string RecordTime { get; set; }
    }
}
