using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TLCovidTest.Models;
using TLCovidTest.Entities;

namespace TLCovidTest.Controllers
{
    public class WorkListController
    {
        public static List<WorkListModel> Get()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<WorkListModel>("EXEC spm_SelectWorkList").ToList();
            };
        }
        //
        public static List<WorkListModel> GetByDate(DateTime date)
        {
            var @TestDate = new SqlParameter("@TestDate", date);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<WorkListModel>("EXEC spm_SelectWorkListByDate @TestDate", @TestDate).ToList();
            };
        }

        public static List<WorkListModel> GetByEmpId(string empId)
        {
            var @EmployeeID = new SqlParameter("@EmployeeID", empId);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<WorkListModel>("EXEC spm_SelectWorkListByEmpId @EmployeeID", @EmployeeID).ToList();
            };
        }

        public static bool UpdateTestStatus(WorkListModel model)
        {
            var @EmployeeID = new SqlParameter("@EmployeeID", model.EmployeeID);
            var @TestDate = new SqlParameter("@TestDate", model.TestDate);
            var @TestStatus= new SqlParameter("@TestStatus", model.TestStatus);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_UpdateWorkListWhenScanOut @EmployeeID, @TestDate, @TestStatus",
                                                                               @EmployeeID, @TestDate, @TestStatus) >= 1)
                    return true;
                return false;
            }
        }
    }
}
