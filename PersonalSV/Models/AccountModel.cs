using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCovidTest.Models
{
    public class AccountModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool IsPersonel { get; set; }
        public bool IsCovidTest { get; set; }
        public string Branch { get; set; }
    }
}
