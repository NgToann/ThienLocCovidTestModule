using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TLCovidTest.Models;
using TLCovidTest.Entities;

namespace TLCovidTest.Controllers
{
    public class PatientController
    {
        public static List<PatientModel> GetAll()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<PatientModel>("EXEC spm_SelectPatient").ToList();
            };
        }

        public static List<string> GetTotal()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<string>("EXEC spm_SelectPatientTotal").ToList();
            };
        }
        public static List<PatientModel> GetByEmpId(string empId)
        {
            var @EmployeeID = new SqlParameter("@EmployeeID", empId);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<PatientModel>("EXEC spm_SelectPatientByEmployeeID @EmployeeID", @EmployeeID).ToList();
            };
        }
        public static List<PatientModel> GetFromTo(DateTime dateFrom, DateTime dateTo)
        {
            var @DateFrom = new SqlParameter("@DateFrom", dateFrom);
            var @DateTo = new SqlParameter("@DateTo", dateTo);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<PatientModel>("EXEC spm_SelectPatientInFromTo @DateFrom, @DateTo", @DateFrom, @DateTo).ToList();
            };
        }
        public static bool Insert(PatientModel model)
        {
            var @EmployeeID = new SqlParameter("@EmployeeID", model.EmployeeID);
            var @ConfirmDate= new SqlParameter("@ConfirmDate", model.ConfirmDate);
            var @StateIndex = new SqlParameter("@StateIndex", model.StateIndex);
            var @Remarks    = new SqlParameter("@Remarks", model.Remarks);
            var @ConfirmBy  = new SqlParameter("@ConfirmBy", model.ConfirmBy);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_InsertPatient @EmployeeID, @ConfirmDate, @StateIndex, @Remarks, @ConfirmBy",
                                                                   @EmployeeID, @ConfirmDate, @StateIndex, @Remarks, @ConfirmBy) >= 1)
                    return true;
                return false;
            }
        }

        public static bool Delete(PatientModel model)
        {
            var @EmployeeID = new SqlParameter("@EmployeeID", model.EmployeeID);
            var @ConfirmDate = new SqlParameter("@ConfirmDate", model.ConfirmDate);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_DeletePatientEmpIDDate @EmployeeID, @ConfirmDate",
                                                                            @EmployeeID, @ConfirmDate) >= 1)
                    return true;
                return false;
            }
        }
    }
}
