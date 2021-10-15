using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class EmployeeFaceModel
    {
        public string EmployeeCode { get; set; }
        public string EmployeeId { get; set; }
        public byte[] Face1 { get; set; }
        public byte[] Face2 { get; set; }
        public byte[] Face3 { get; set; }
        public byte[] Face4 { get; set; }
        public byte[] Face5 { get; set; }
        public byte[] FaceImage11 { get; set; }
        public byte[] FaceImage21 { get; set; }
        public byte[] FaceImage31 { get; set; }
        public byte[] FaceImage41 { get; set; }
        public byte[] FaceImage51 { get; set; }
    }
}
