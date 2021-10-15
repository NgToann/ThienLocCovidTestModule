using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCovidTest.Models;
using TLCovidTest.Entities;
using System.Data.SqlClient;

namespace TLCovidTest.Controllers
{
    public class OverTimeLimitController
    {
        public static List<OverTimeLimitModel> GetByEmployeeByDate(string employeeCode, DateTime date)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", employeeCode);
            var @OverTimeDate = new SqlParameter("@OverTimeDate", date);

            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<OverTimeLimitModel>("EXEC spm_SelectOverTimeLimitByEmployeeByDate @EmployeeCode, @OverTimeDate", @EmployeeCode, @OverTimeDate).ToList();
            };
        }
        public static bool InsertOrUpdate(OverTimeLimitModel model)
        {
            var @EmployeeCode       = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @OverTimeDate       = new SqlParameter("@OverTimeDate", model.OverTimeDate);
            var @DateIn             = new SqlParameter("@DateIn", model.DateIn);
            var @DateOut            = new SqlParameter("@DateOut", model.DateOut);
            var @OverTime           = new SqlParameter("@OverTime", model.OverTime);
            var @TimeOutLimit       = new SqlParameter("@TimeOutLimit", model.TimeOutLimit);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_InsertOrUpdateOverTimeLimit @EmployeeCode, @OverTimeDate, @DateIn, @DateOut, @OverTime, @TimeOutLimit",
                                                                                 @EmployeeCode, @OverTimeDate, @DateIn, @DateOut, @OverTime, @TimeOutLimit) >= 1)
                    return true;
                else return false;
            }
        }
        public static bool DeleteByEmployeeByDate(OverTimeLimitModel model)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @OverTimeDate = new SqlParameter("@OverTimeDate", model.OverTimeDate);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_DeleteOverTimeLimitByEmployeeByDate @EmployeeCode, @OverTimeDate",
                                                                                         @EmployeeCode, @OverTimeDate) >= 1)
                    return true;
                else return false;
            }
        }
    }
}
