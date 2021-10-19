using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using TLCovidTest.Entities;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;

namespace TLCovidTest.Controllers
{
    public class WorkerPrizeController
    {
        public static List<WorkerPrizeModel> GetByCardId(string cardId)
        {
            var @CardId = new SqlParameter("@CardId", cardId);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<WorkerPrizeModel>("EXEC spm_SelectWorkerPrizeByCardId @CardId", @CardId).ToList();
            };
        }
        public static bool Insert(WorkerPrizeModel model)
        {
            var @WorkerId = new SqlParameter("@WorkerId", model.WorkerId);
            var @CardId = new SqlParameter("@CardId", model.CardId);
            var @FullName = new SqlParameter("@FullName", model.FullName);
            var @DepartmentName = new SqlParameter("@DepartmentName", model.DepartmentName);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_InsertWorkerPrize @WorkerId, @CardId, @FullName, @DepartmentName",
                                                                       @WorkerId, @CardId, @FullName, @DepartmentName) >= 1)
                    return true;
                return false;
            }
        }
        public static bool Delete()
        {
            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_DeleteWorkerPrize") >= 1)
                    return true;
                return false;
            }
        }
        //
        public static bool UpdateTimeScan(WorkerPrizeModel model)
        {
            using (var db = new PersonalDataEntities())
            {
                var @CardId = new SqlParameter("@CardId", model.CardId);
                var @TimeScan = new SqlParameter("@TimeScan", model.TimeScan);
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_UpdateWorkerPrizeTimeScan @CardId, @TimeScan", @CardId, @TimeScan) >= 1)
                    return true;
                return false;
            }
        }
    }
}
