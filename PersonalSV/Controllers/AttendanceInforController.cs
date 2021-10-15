using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCovidTest.Models;
using TLCovidTest.Entities;
using System.Data.SqlClient;

namespace TLCovidTest.Controllers
{
    public class AttendanceInforController
    {
        public static AttendanceInforModel GetByEmployeeCodeYearMonth(string employeeCode, int year, int month)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", employeeCode);
            var @Year = new SqlParameter("@Year", year);
            var @Month = new SqlParameter("@Month", month);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<AttendanceInforModel>("EXEC spm_SelectAttendanceInforByEmployeeCodeYearMonth @EmployeeCode, @Year, @Month", @EmployeeCode, @Year, @Month).FirstOrDefault();
            }
        }
        public static List<AttendanceInforModel> GetByDepartmentYearMonth(string department, int year, int month)
        {
            var @Department = new SqlParameter("@Department", department);
            var @Year = new SqlParameter("@Year", year);
            var @Month = new SqlParameter("@Month", month);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<AttendanceInforModel>("EXEC spm_SelectAttendanceInforByDepartmentYearMonth @Department, @Year, @Month", @Department, @Year, @Month).ToList();
            }
        }

        public static bool AddOrUpdate(AttendanceInforModel insert)
        {
            var @EmployeeCode    = new SqlParameter("@EmployeeCode", insert.EmployeeCode);
            var @ShiftYear       = new SqlParameter("@ShiftYear", insert.ShiftYear);
            var @ShiftMonth      = new SqlParameter("@ShiftMonth", insert.ShiftMonth);

            var @Shift_1         = new SqlParameter("@Shift_1", insert.Shift_1 != null ? insert.Shift_1 : "");
            var @Shift_2         = new SqlParameter("@Shift_2", insert.Shift_2 != null ? insert.Shift_2 : "");
            var @Shift_3         = new SqlParameter("@Shift_3", insert.Shift_3 != null ? insert.Shift_3 : "");
            var @Shift_4         = new SqlParameter("@Shift_4", insert.Shift_4 != null ? insert.Shift_4 : "");
            var @Shift_5         = new SqlParameter("@Shift_5", insert.Shift_5 != null ? insert.Shift_5 : "");
            var @Shift_6         = new SqlParameter("@Shift_6", insert.Shift_6 != null ? insert.Shift_6 : "");
            var @Shift_7         = new SqlParameter("@Shift_7", insert.Shift_7 != null ? insert.Shift_7 : "");
            var @Shift_8         = new SqlParameter("@Shift_8", insert.Shift_8 != null ? insert.Shift_8 : "");
            var @Shift_9         = new SqlParameter("@Shift_9", insert.Shift_9 != null ? insert.Shift_9 : "");
            var @Shift_10        = new SqlParameter("@Shift_10", insert.Shift_10 != null ? insert.Shift_10 : "");
            var @Shift_11        = new SqlParameter("@Shift_11", insert.Shift_11 != null ? insert.Shift_11 : "");
            var @Shift_12        = new SqlParameter("@Shift_12", insert.Shift_12 != null ? insert.Shift_12 : "");
            var @Shift_13        = new SqlParameter("@Shift_13", insert.Shift_13 != null ? insert.Shift_13 : "");
            var @Shift_14        = new SqlParameter("@Shift_14", insert.Shift_14 != null ? insert.Shift_14 : "");
            var @Shift_15        = new SqlParameter("@Shift_15", insert.Shift_15 != null ? insert.Shift_15 : "");
            var @Shift_16        = new SqlParameter("@Shift_16", insert.Shift_16 != null ? insert.Shift_16 : "");
            var @Shift_17        = new SqlParameter("@Shift_17", insert.Shift_17 != null ? insert.Shift_17 : "");
            var @Shift_18        = new SqlParameter("@Shift_18", insert.Shift_18 != null ? insert.Shift_18 : "");
            var @Shift_19        = new SqlParameter("@Shift_19", insert.Shift_19 != null ? insert.Shift_19 : "");
            var @Shift_20        = new SqlParameter("@Shift_20", insert.Shift_20 != null ? insert.Shift_20 : "");
            var @Shift_21        = new SqlParameter("@Shift_21", insert.Shift_21 != null ? insert.Shift_21 : "");
            var @Shift_22        = new SqlParameter("@Shift_22", insert.Shift_22 != null ? insert.Shift_22 : "");
            var @Shift_23        = new SqlParameter("@Shift_23", insert.Shift_23 != null ? insert.Shift_23 : "");
            var @Shift_24        = new SqlParameter("@Shift_24", insert.Shift_24 != null ? insert.Shift_24 : "");
            var @Shift_25        = new SqlParameter("@Shift_25", insert.Shift_25 != null ? insert.Shift_25 : "");
            var @Shift_26        = new SqlParameter("@Shift_26", insert.Shift_26 != null ? insert.Shift_26 : "");
            var @Shift_27        = new SqlParameter("@Shift_27", insert.Shift_27 != null ? insert.Shift_27 : "");
            var @Shift_28        = new SqlParameter("@Shift_28", insert.Shift_28 != null ? insert.Shift_28 : "");
            var @Shift_29        = new SqlParameter("@Shift_29", insert.Shift_29 != null ? insert.Shift_29 : "");
            var @Shift_30        = new SqlParameter("@Shift_30", insert.Shift_30 != null ? insert.Shift_30 : "");
            var @Shift_31        = new SqlParameter("@Shift_31", insert.Shift_31 != null ? insert.Shift_31 : "");


            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_InsertAttendanceInfor   @EmployeeCode, @ShiftYear, @ShiftMonth, @Shift_1, @Shift_2, @Shift_3, @Shift_4, @Shift_5, @Shift_6, @Shift_7, @Shift_8, @Shift_9, @Shift_10, @Shift_11, @Shift_12, @Shift_13, @Shift_14, @Shift_15, @Shift_16, @Shift_17, @Shift_18, @Shift_19, @Shift_20, @Shift_21, @Shift_22, @Shift_23, @Shift_24, @Shift_25, @Shift_26, @Shift_27, @Shift_28, @Shift_29, @Shift_30, @Shift_31",
                                                                             @EmployeeCode, @ShiftYear, @ShiftMonth, @Shift_1, @Shift_2, @Shift_3, @Shift_4, @Shift_5, @Shift_6, @Shift_7, @Shift_8, @Shift_9, @Shift_10, @Shift_11, @Shift_12, @Shift_13, @Shift_14, @Shift_15, @Shift_16, @Shift_17, @Shift_18, @Shift_19, @Shift_20, @Shift_21, @Shift_22, @Shift_23, @Shift_24, @Shift_25, @Shift_26, @Shift_27, @Shift_28, @Shift_29, @Shift_30, @Shift_31) >= 1)
                    return true;
                else return false;
            }
        }
    }
}
