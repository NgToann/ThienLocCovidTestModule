using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using TLCovidTest.Models;
using TLCovidTest.Entities;

namespace TLCovidTest.Controllers
{
    public class LeaveWithSalaryController
    {
        public static bool Insert(LeaveWithReasonModel model)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @AttendanceDate = new SqlParameter("@AttendanceDate", model.AttendanceDate);
            var @SalaryRate = new SqlParameter("@SalaryRate", model.SalaryRate);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_InsertLeaveWithSalary @EmployeeCode, @AttendanceDate, @SalaryRate",
                                                                           @EmployeeCode, @AttendanceDate, @SalaryRate) >= 1)
                    return true;
                else return false;
            }
        }
        public static bool Delete(LeaveWithReasonModel model)
        {
            var @EmployeeCode   = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @AttendanceDate = new SqlParameter("@AttendanceDate", model.AttendanceDate);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_DeleteLeaveWithSalaryByCodeByDate @EmployeeCode, @AttendanceDate",
                                                                                       @EmployeeCode, @AttendanceDate) >= 1)
                    return true;
                else return false;
            }
        }
    }
}
