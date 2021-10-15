using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using TLCovidTest.Entities;
using TLCovidTest.Models;

namespace TLCovidTest.Controllers
{
    public class CommonController
    {
        //
        public static PrivateDefineModel GetDefineProps()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<PrivateDefineModel>("EXEC spm_SelectPrivateDefine").FirstOrDefault();
            };
        }

        public static List<WorkerPriorityModel> GetWorkerPriorityList()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<WorkerPriorityModel>("EXEC spm_SelectWorkerPriority").ToList();
            };
        }
        public static List<WorkerF0Model> GetWorkerF0()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<WorkerF0Model>("EXEC spm_SelectWorkerF0").ToList();
            };
        }
    }
}
