using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using TLCovidTest.Entities;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;

namespace TLCovidTest.Controllers
{
    public class TestRandomController
    {
        //
        public static List<TestRandomModel> GetAll()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<TestRandomModel>("EXEC spm_SelectTestRandom").ToList();
            };
        }
        public static List<TestRandomModel> GetByDate(DateTime dateSearch)
        {
            var @TestDate = new SqlParameter("@TestDate", dateSearch);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<TestRandomModel>("EXEC spm_SelectTestRandomByDate @TestDate", @TestDate).ToList();
            };
        }
        public static List<TestRandomModel> GetByEmpCode(string empCode)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", empCode);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<TestRandomModel>("EXEC spm_SelectTestRandomByEmpCode @EmployeeCode", @EmployeeCode).ToList();
            };
        }
        public static bool Insert(TestRandomModel model)
        {
            var @Id = new SqlParameter("@Id", model.Id);
            var @EmployeeCode = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @TestDate = new SqlParameter("@TestDate", model.TestDate);
            var @Term = new SqlParameter("@Term", model.Term);
            var @Round = new SqlParameter("@Round", model.Round);
            var @Result = new SqlParameter("@Result", model.Result);
            var @PersonConfirm = new SqlParameter("@PersonConfirm", model.PersonConfirm);
            var @Remark = new SqlParameter("@Remark", model.Remark);
            var @TimeIn = new SqlParameter("@TimeIn", model.TimeIn);
            var @TimeOut = new SqlParameter("@TimeOut", model.TimeOut);
            var @Status = new SqlParameter("@Status", model.Status);
            var @AddByManual = new SqlParameter("@AddByManual", model.AddByManual);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (db.ExecuteStoreCommand("EXEC spm_InsertTestRandom_1 @Id, @EmployeeCode, @TestDate, @Term, @Round, @Result, @PersonConfirm, @Remark, @TimeIn, @TimeOut, @Status, @AddByManual",
                                                                        @Id, @EmployeeCode, @TestDate, @Term, @Round, @Result, @PersonConfirm, @Remark, @TimeIn, @TimeOut, @Status, @AddByManual) >= 1)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Update Test Random Record
        /// </summary>
        /// <param name="model"></param>
        /// <param name="option">1: update TimeIn, 2: update TimeOut, 3: Update Result</param>
        /// <returns></returns>
        
        public static bool Update(TestRandomModel model, int option)
        {
            var @Id = new SqlParameter("@Id", model.Id);
            var @Result = new SqlParameter("@Result", model.Result);
            var @PersonConfirm = new SqlParameter("@PersonConfirm", model.PersonConfirm);
            var @Remark = new SqlParameter("@Remark", model.Remark);
            var @TimeIn = new SqlParameter("@TimeIn", model.TimeIn);
            var @TimeOut = new SqlParameter("@TimeOut", model.TimeOut);
            var @Status = new SqlParameter("@Status", model.Status);
            var @UpdateResultTime = new SqlParameter("@UpdateResultTime", model.UpdateResultTime);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;
                if (option == 1)
                {
                    if (db.ExecuteStoreCommand("EXEC spm_UpdateTestRandomTimeInById @Id, @TimeIn, @Status", @Id, @TimeIn, @Status) >= 1)
                        return true;
                }
                else if (option == 2)
                {
                    if (db.ExecuteStoreCommand("EXEC spm_UpdateTestRandomTimeOutById @Id, @TimeOut, @Status", @Id, @TimeOut, @Status) >= 1)
                        return true;
                }
                else if (option == 3)
                {
                    if (db.ExecuteStoreCommand("EXEC spm_UpdateTestRandomResultById @Id, @Result, @PersonConfirm, @Remark, @Status, @UpdateResultTime",
                                                                                    @Id, @Result, @PersonConfirm, @Remark, @Status, @UpdateResultTime) >= 1)
                        return true;
                }
                return false;

            }
        }
        
        public static bool DeleteRecord(TestRandomModel model)
        {
            var @Id = new SqlParameter("@Id", model.Id);
            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;

                if (db.ExecuteStoreCommand("EXEC spm_DeleteTestRandomById @Id",
                                                                          @Id) >= 1)
                    return true;
                return false;
            }
        }
        //
        public static bool DeleteByEmpCodeByDate(TestRandomModel model)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", model.EmployeeCode);
            var @TestDate = new SqlParameter("@TestDate", model.TestDate);
            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;

                if (db.ExecuteStoreCommand("EXEC spm_DeleteTestRandomByEmpCodeByDate @EmployeeCode, @TestDate",
                                                                                     @EmployeeCode, @TestDate) >= 1)
                    return true;
                return false;
            }
        }
        public static bool DeleteByDate(DateTime dateDelete)
        {
            var @TestDate = new SqlParameter("@TestDate", dateDelete);
            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 45;

                if (db.ExecuteStoreCommand("EXEC spm_DeleteTestRandomByDate @TestDate",
                                                                            @TestDate) >= 1)
                    return true;
                return false;
            }
        }
    }
}
