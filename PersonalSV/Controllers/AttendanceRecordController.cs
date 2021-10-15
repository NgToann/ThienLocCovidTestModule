using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using TLCovidTest.Models;
using TLCovidTest.Entities;
using TLCovidTest.ViewModels;

namespace TLCovidTest.Controllers
{
    public class AttendanceRecordController
    {
        public static List<AttendanceRecordModel> GetAttendanceRecordByEmployeeCodeFromTo(string employeeCode, DateTime dateFrom, DateTime dateTo)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", employeeCode);

            var @DateFrom    = new SqlParameter("@DateFrom", dateFrom.Date);
            var @DateTo      = new SqlParameter("@DateTo", dateTo.Date);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<AttendanceRecordModel>("EXEC spm_SelectAttendanceRecordByEmployeeFromTo @EmployeeCode, @DateFrom, @DateTo", 
                                                                                                                    @EmployeeCode, @DateFrom, @DateTo).ToList();
            };
        }

        public static List<AttendanceRecordModel> GetAttendanceRecordByDepartmentNameFromTo(string departmentName, DateTime dateFrom, DateTime dateTo)
        {
            var @DepartmentName  = new SqlParameter("@DepartmentName", departmentName);
            var @DateFrom        = new SqlParameter("@DateFrom", dateFrom.Date);
            var @DateTo          = new SqlParameter("@DateTo", dateTo.Date);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<AttendanceRecordModel>("EXEC spm_SelectAttendanceRecordByDepartmentNameFromTo @DepartmentName, @DateFrom, @DateTo", 
                                                                                                                          @DepartmentName, @DateFrom, @DateTo).ToList();
            };
        }
    }
}
