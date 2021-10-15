using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersonalSV.Models
{
    public class UserModel
    {
        /// <summary>
        /// Name of Account
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Account ID
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Your Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Your Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// When an account was created, a code record was sent to your email. You need to confirm code to completely your registration.
        /// </summary>
        public string ConfirmCode { get; set; }
        /// <summary>
        /// We have pending and active status
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// We have administrator, personel, it mode
        /// </summary>
        public string UserMode { get; set; }
        /// <summary>
        /// List of computer which was used to login your account to system.
        /// </summary>
        public string ComputerNameList { get; set; }
        /// <summary>
        /// Checking your account is running or not.
        /// </summary>
        public bool IsRunningNow { get; set; }
    }
}
