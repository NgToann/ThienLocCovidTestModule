using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCovidTest.Models;
using TLCovidTest.Entities;
using System.Data.SqlClient;

namespace TLCovidTest.Controllers
{
    public class LeaveDayDetailController
    {
        public static List<LeaveDayDetailModel> GetByEmployeeCode(string employeeCode)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", employeeCode);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<LeaveDayDetailModel>("EXEC spm_SelectLeaveDayDetailPerEmployeeCode @EmployeeCode", @EmployeeCode).ToList();
            }
        }
        //
        public static List<LeaveDayDetailModel> GetByYear()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<LeaveDayDetailModel>("EXEC spm_SelectLeaveDayDetailPerYear").ToList();
            }
        }

        public static bool Add(LeaveDayDetailModel model)
        {
            var @EmployeeCode   = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @LeaveDate      = new SqlParameter("@LeaveDate", model.LeaveDate);
            var @Paid           = new SqlParameter("@Paid", model.Paid);
            var @Remark         = new SqlParameter("@Remark", model.Remark);
            var @TotalDay       = new SqlParameter("@TotalDay", model.TotalDay);
            var @RandomNo       = new SqlParameter("@RandomNo", model.RandomNo);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_InsertLeaveDayDetail @EmployeeCode, @LeaveDate, @Paid, @Remark, @TotalDay, @RandomNo", 
                                                                          @EmployeeCode, @LeaveDate, @Paid, @Remark, @TotalDay, @RandomNo) >= 1)
                    return true;
                else return false;
            }
        }

        public static bool Delete(LeaveDayDetailModel model)
        {
            var @EmployeeCode    = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @LeaveDate       = new SqlParameter("@LeaveDate", model.LeaveDate);
            var @RandomNo        = new SqlParameter("@RandomNo", model.RandomNo);
            using (var db = new PersonalDataEntities())
            {
                if (db.ExecuteStoreCommand("spm_DeleteLeaveDayDetail @EmployeeCode, @LeaveDate, @RandomNo", 
                                                                     @EmployeeCode, @LeaveDate, @RandomNo) >= 1)
                    return true;
                else return false;
            }
        }
    }
}
