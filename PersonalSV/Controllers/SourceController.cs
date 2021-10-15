using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using TLCovidTest.Models;
using TLCovidTest.Entities;

namespace TLCovidTest.Controllers
{
    public class SourceController
    {

        public static List<SourceModel> SelectSourceByDate(DateTime dateSearch)
        {
            var @DateSearch = new SqlParameter("@DateSearch", dateSearch);
            List<SourceModel> sourceList = new List<SourceModel>();
            using (var db = new PersonalDataEntities())
            {
                var sourceListFromDB = db.ExecuteStoreQuery<SourceModel>("EXEC spm_SelectSourceByDate @DateSearch", @DateSearch).ToList();
                foreach (var source in sourceListFromDB)
                {
                    if (source.SourceTime.Length == 4)
                    {
                        source.SourceTimeView = source.SourceTime[0].ToString() + source.SourceTime[1].ToString() + ":" + source.SourceTime[2].ToString() + source.SourceTime[3].ToString();
                    }
                    else
                    {
                        source.SourceTimeView = source.SourceTime;
                    }
                    sourceList.Add(source);
                }
                return sourceList;
            }
        }

        public static List<SourceModel> SelectSourceByDateFromTo(DateTime dateSearchFrom, DateTime dateSearchTo)
        {
            var @DateSearchFrom = new SqlParameter("@DateSearchFrom", dateSearchFrom);
            var @DateSearchTo   = new SqlParameter("@DateSearchTo", dateSearchTo);
            List<SourceModel> sourceList = new List<SourceModel>();
            using (var db = new PersonalDataEntities())
            {
                var sourceListFromDB = db.ExecuteStoreQuery<SourceModel>("EXEC spm_SelectSourceByDateFromTo @DateSearchFrom, @DateSearchTo", @DateSearchFrom, @DateSearchTo).ToList();
                foreach (var source in sourceListFromDB)
                {
                    if (source.SourceTime.Length == 4)
                    {
                        source.SourceTimeView = source.SourceTime[0].ToString() + source.SourceTime[1].ToString() + ":" + source.SourceTime[2].ToString() + source.SourceTime[3].ToString();
                    }
                    else
                    {
                        source.SourceTimeView = source.SourceTime;
                    }
                    sourceList.Add(source);
                }
                return sourceList;
            }
        }

        public static List<SourceModel> SelectSourceByEmployeeCodeAndDate(string employeeCode, DateTime dateSearch)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", employeeCode);
            var @DateSearch = new SqlParameter("@DateSearch", dateSearch);
            List<SourceModel> sourceList = new List<SourceModel>();
            using (var db = new PersonalDataEntities())
            {
                var sourceListFromDB = db.ExecuteStoreQuery<SourceModel>("EXEC spm_SelectSourceByEmployeeCodeAndDate @EmployeeCode, @DateSearch", @EmployeeCode, @DateSearch).ToList();
                foreach (var source in sourceListFromDB)
                {
                    if (source.SourceTime.Length == 4)
                    {
                        source.SourceTimeView = source.SourceTime[0].ToString() + source.SourceTime[1].ToString() + ":" + source.SourceTime[2].ToString() + source.SourceTime[3].ToString();
                    }
                    else
                    {
                        source.SourceTimeView = source.SourceTime;
                    }
                    sourceList.Add(source);
                }
                return sourceList;
            }
        }

        public static bool Add(SourceModel model)
        {
            var @EmployeeCode    = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @SourceDate      = new SqlParameter("@SourceDate", model.SourceDate);
            var @SourceTime      = new SqlParameter("@SourceTime", model.SourceTime);
            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_InsertSource @EmployeeCode, @SourceDate, @SourceTime", @EmployeeCode, @SourceDate, @SourceTime) >= 1)
                    return true;
                else return false;
            }
        }

        public static bool Delete(SourceModel model)
        {
            var @EmployeeCode    = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @SourceDate      = new SqlParameter("@SourceDate", model.SourceDate);
            var @SourceTime      = new SqlParameter("@SourceTime", model.SourceTime);
            using (var db = new PersonalDataEntities())
            {
                if (db.ExecuteStoreCommand("EXEC spm_DeleteSource @EmployeeCode, @SourceDate, @SourceTime", @EmployeeCode, @SourceDate, @SourceTime) >= 1)
                    return true;
                else return false;
            }
        }
    }
}
