using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using TLCovidTest.Models;
using TLCovidTest.Entities;

namespace TLCovidTest.Controllers
{
    public class ReportController
    {
        public static List<MissingRecordTimeModel> GetMissingRecordTimesFromTo(DateTime dtFrom, DateTime dtTo)
        {
            var @DateFrom = new SqlParameter("@DateFrom", dtFrom);
            var @DateTo = new SqlParameter("@DateTo", dtTo);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<MissingRecordTimeModel>("EXEC spm_ReportMissingRecordTimeFromTo @DateFrom, @DateTo", @DateFrom, @DateTo).ToList();
            };
        }
    }
}
