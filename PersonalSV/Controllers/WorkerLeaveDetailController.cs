using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using TLCovidTest.Entities;
using TLCovidTest.Models;

namespace TLCovidTest.Controllers
{
    public class WorkerLeaveDetailController
    {
        public static List<WorkerLeaveDetailModel> GetAll()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<WorkerLeaveDetailModel>("EXEC spm_SelectWorkerLeaveDetail").ToList();
            };
        }
        //spm_InsertWorkerLeaveDetail
        public static bool AddRecord(WorkerLeaveDetailModel model)
        {
            var @EmployeeId = new SqlParameter("@EmployeeId", model.EmployeeID);
            var @EmployeeCode = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @LeaveDate = new SqlParameter("@LeaveDate", model.LeaveDate);
            var @Reason = new SqlParameter("@Reason", model.Reason);
            var @Remark = new SqlParameter("@Remark", model.Remark);
            var @DateDisplay = new SqlParameter("@DateDisplay", model.DateDisplay);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 120;

                if (db.ExecuteStoreCommand("EXEC spm_InsertWorkerLeaveDetail @EmployeeId, @EmployeeCode, @LeaveDate, @Reason, @Remark, @DateDisplay",
                                                                             @EmployeeId, @EmployeeCode, @LeaveDate, @Reason, @Remark, @DateDisplay) >= 1)
                    return true;
                else
                    return false;

            };
        }

        //
        public static bool Delete(WorkerLeaveDetailModel model)
        {
            var @EmployeeId = new SqlParameter("@EmployeeId", model.EmployeeID);
            var @LeaveDate = new SqlParameter("@LeaveDate", model.LeaveDate);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 120;
                if (db.ExecuteStoreCommand("EXEC spm_DeleteWorkerLeaveDetailByIdByDate @EmployeeId, @LeaveDate",
                                                                                       @EmployeeId, @LeaveDate) >= 1)
                    return true;
                else return false;
            }
        }
    }
}
