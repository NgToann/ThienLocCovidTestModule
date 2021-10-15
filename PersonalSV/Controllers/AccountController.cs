using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using TLCovidTest.Models;
using TLCovidTest.Entities;
namespace TLCovidTest.Controllers
{
    public class AccountController
    {
        public static AccountModel GetAccountByUP(string userName, string passWord)
        {
            var @UserName = new SqlParameter("@UserName", userName);
            var @Password = new SqlParameter("@Password", passWord);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<AccountModel>("EXEC spm_SelectAccountByUP @UserName, @Password", @UserName, @Password).FirstOrDefault();
            };
        }
    }
}
