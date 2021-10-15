using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCovidTest.Models;
using TLCovidTest.Entities;
using System.Data.SqlClient;

namespace TLCovidTest.Controllers
{
    public class LeaveDayTotalController
    {
        public static List<LeaveDayTotalModel> GetByEmployeeCode(string employeeCode)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", employeeCode);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<LeaveDayTotalModel>("EXEC spm_SelectLeaveDayTotalPerEmployeeCode @EmployeeCode", @EmployeeCode).ToList();
            }
        }

        public static bool Add(LeaveDayTotalModel model)
        {
            var @EmployeeCode    = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @BeginDate       = new SqlParameter("@BeginDate", model.BeginDate);
            var @Paid            = new SqlParameter("@Paid", model.Paid);
            var @Remark          = new SqlParameter("@Remark", model.Remark);
            var @TotalDay        = new SqlParameter("@TotalDay", model.TotalDay);
            var @RandomNo        = new SqlParameter("@RandomNo", model.RandomNo);
            var @EndDate         = new SqlParameter("@EndDate", model.EndDate);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_InsertLeaveDateTotal @EmployeeCode, @BeginDate, @Paid, @Remark, @TotalDay, @RandomNo, @EndDate",
                                                                          @EmployeeCode, @BeginDate, @Paid, @Remark, @TotalDay, @RandomNo, @EndDate) >= 1)
                    return true;
                else return false;
            }
        }

        public static bool Delete(LeaveDayTotalModel model)
        {
            var @EmployeeCode    = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @BeginDate       = new SqlParameter("@BeginDate", model.BeginDate);
            var @RandomNo        = new SqlParameter("@RandomNo", model.RandomNo);
            using (var db = new PersonalDataEntities())
            {
                if (db.ExecuteStoreCommand("EXEC spm_DeleteLeaveDateTotal @EmployeeCode, @BeginDate, @RandomNo", 
                                                                          @EmployeeCode, @BeginDate, @RandomNo) >= 1)
                    return true;
                else return false;
            }
        }
    }
}
