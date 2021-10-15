using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class AddressModel
    {
        public int AddressId { get; set; }
        public string Province { get; set; }
        public string ProvinceId { get; set; }
        public string District { get; set; }
        public string DistrictId { get; set; }
        public string Commune { get; set; }
        public string CommuneId { get; set; }
        public int OrderNo { get; set; }
    }
}
