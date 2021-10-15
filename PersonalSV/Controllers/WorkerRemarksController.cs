using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using TLCovidTest.Entities;
using TLCovidTest.Models;

namespace TLCovidTest.Controllers
{
    public class WorkerRemarksController
    {
        //
        public static List<WorkerRemarkModel> GetFromTo(DateTime from, DateTime to)
        {
            var @From = new SqlParameter("@From", from);
            var @To = new SqlParameter("@To", to);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<WorkerRemarkModel>("EXEC spm_SelectWorkerRemarksFromTo @From, @To", @From, @To).ToList();
            };
        }
        public static List<WorkerRemarkModel> GetAll()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<WorkerRemarkModel>("EXEC spm_SelectWorkerRemarks").ToList();
            };
        }

        public static bool AddRecord(WorkerRemarkModel model)
        {         
            var @Date = new SqlParameter("@Date", model.Date);
            var @EmployeeID = new SqlParameter("@EmployeeID", model.EmployeeID);
            var @Remarks = new SqlParameter("@Remarks", model.Remarks);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 30;

                if (db.ExecuteStoreCommand("EXEC spm_InsertWorkerRemarks @Date, @EmployeeID, @Remarks",
                                                                         @Date, @EmployeeID, @Remarks) >= 1)
                    return true;
                else
                    return false;
                    
            };
        }
    }
}
