using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using TLCovidTest.Entities;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;

namespace TLCovidTest.Controllers
{
    public class EmployeeController
    {
        //static List<DepartmentModel> departmentList = DepartmentController.GetDepartments();

        public static List<EmployeeModel> GetAll()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<EmployeeModel>("EXEC spm_SelectPersonalAll").ToList();
            };
        }

        public static List<EmployeeModel> GetAvailable()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<EmployeeModel>("EXEC spm_SelectPersonalAvailabe").ToList();
            };
        }
        //
        public static List<EmployeeModel> GetAvailableForTestCovid()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<EmployeeModel>("EXEC spm_SelectPersonalAvailabeForTestCovid").ToList();
            };
        }

        public static List<EmployeeModel> GetFromSourceByDate(DateTime dateSearch)
        {
            var @DateSearch = new SqlParameter("@DateSearch", dateSearch);
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<EmployeeModel>("EXEC spm_SelectPersonalFromSourceByDate @DateSearch", @DateSearch).ToList();
            };
        }

        public static List<EmployeeModel> GetEmployeeToExecuteSalaryData()
        {
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<EmployeeModel>("EXEC spm_SelectPersonalFromSourceToExecuteSalaryData").ToList();
            };
        }

        public static EmployeeModel SelectEmployeeByEmployeeID(string employeeID)
        {
            var @EmployeeID = new SqlParameter("@EmployeeID", employeeID.Trim().ToUpper());
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<EmployeeModel>("EXEC spm_SelectPersonalByEmployeeID @EmployeeID", @EmployeeID).FirstOrDefault();
            };
        }

        public static EmployeeModel SelectEmployeeByEmployeeCode(string employeeCode)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", employeeCode.Trim().ToUpper());
            using (var db = new PersonalDataEntities())
            {
                return db.ExecuteStoreQuery<EmployeeModel>("EXEC spm_SelectPersonalByEmployeeCode @EmployeeCode", @EmployeeCode).FirstOrDefault();
            };
        }

        public static bool AddOrUpdate(EmployeeModel model, EnumEditMode editMode)
        {
            bool result = false;

            var @EmployeeCode        = new SqlParameter("@EmployeeCode", model.EmployeeCode.Trim().ToUpper().ToString());
            var @EmployeeID          = new SqlParameter("@EmployeeID", model.EmployeeID.Trim().ToUpper().ToString());
            var @EmployeeName        = new SqlParameter("@EmployeeName", model.EmployeeName.Trim());
            var @Gender              = new SqlParameter("@Gender", model.GenderMan == true ? "MAN" : "WOMAN");
            var @DepartmentName      = new SqlParameter("@DepartmentName", model.DepartmentSelected != null ? model.DepartmentSelected.DepartmentFullName : "");
            var @PositionName        = new SqlParameter("@PositionName", model.PositionSelected != null ? model.PositionSelected.PositionName : "WORKER");
            var @JoinDate            = new SqlParameter("@JoinDate", model.JoinDate);
            var @DayOfBirth          = new SqlParameter("@DayOfBirth", model.DayOfBirth);
            var @NationID            = new SqlParameter("@NationID", model.NationID.Trim().ToUpper().ToString());
            var @Address             = new SqlParameter("@Address", model.Address != null ? model.Address.Trim() : "");
            var @PhoneNumber         = new SqlParameter("@PhoneNumber", model.PhoneNumber != null ? model.PhoneNumber.Trim() : "");
            var @Remark              = new SqlParameter("@Remark", model.Remark != null ? model.Remark.Trim() : "");
            var @AddressId           = new SqlParameter("@AddressId", model.AddressId);
            var @AddressDetail       = new SqlParameter("@AddressDetail", model.AddressDetail != null ? model.AddressDetail : "");
            var @AddressCurrentId    = new SqlParameter("@AddressCurrentId", model.AddressCurrentId);
            var @AddressCurrentDetail = new SqlParameter("@AddressCurrentDetail", model.AddressCurrentDetail != null ? model.AddressCurrentDetail.Trim() : "");
            

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 30;
                if (editMode == EnumEditMode.Add)
                {
                    if (db.ExecuteStoreCommand("EXEC spm_InsertPersonal_3 @EmployeeCode, @EmployeeID, @EmployeeName, @Gender, @DepartmentName, @PositionName, @JoinDate, @NationID, @Address, @PhoneNumber, @Remark, @DayOfBirth, @AddressId, @AddressDetail, @AddressCurrentId, @AddressCurrentDetail",
                                                                          @EmployeeCode, @EmployeeID, @EmployeeName, @Gender, @DepartmentName, @PositionName, @JoinDate, @NationID, @Address, @PhoneNumber, @Remark, @DayOfBirth, @AddressId, @AddressDetail, @AddressCurrentId, @AddressCurrentDetail) >= 1)
                        result = true;
                    else result = false;
                }
                if (editMode == EnumEditMode.Update)
                {
                    if (db.ExecuteStoreCommand("EXEC spm_UpdatePersonal_3 @EmployeeCode, @EmployeeID, @EmployeeName, @Gender, @DepartmentName, @PositionName, @JoinDate, @NationID, @Address, @PhoneNumber, @Remark, @DayOfBirth, @AddressId, @AddressDetail, @AddressCurrentId, @AddressCurrentDetail",
                                                                          @EmployeeCode, @EmployeeID, @EmployeeName, @Gender, @DepartmentName, @PositionName, @JoinDate, @NationID, @Address, @PhoneNumber, @Remark, @DayOfBirth, @AddressId, @AddressDetail, @AddressCurrentId, @AddressCurrentDetail) >= 1)
                        result = true;
                    else result = false;
                }
                return result;
            };
        }

        public static bool UpdateEmployeeCode(string employeeID, string employeeCodeNew, bool isResign)
        {
            var @EmployeeCode = new SqlParameter("@EmployeeCode", employeeCodeNew);
            var @EmployeeID = new SqlParameter("@EmployeeID", employeeID);
            var @IsResign = new SqlParameter("@IsResign", isResign);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 120;
                if (db.ExecuteStoreCommand("EXEC spm_UpdateChangeEmployeeCode_1 @EmployeeCode, @EmployeeID, @IsResign",
                                                                                @EmployeeCode, @EmployeeID, @IsResign) >= 1)
                    return true;
                else return false;
            }
        }

        public static bool ExecuteSalaryData(string employeeCode, DateTime dateExeFrom, DateTime dateExeTo)
        {
            var @DateFrom = new SqlParameter("@DateFrom", dateExeFrom);
            var @DateTo = new SqlParameter("@DateTo", dateExeTo);
            var @EmployeeCode = new SqlParameter("@EmployeeCode", employeeCode);

            using (var db = new PersonalDataEntities())
            {
                db.CommandTimeout = 30;
                if (db.ExecuteStoreCommand("EXEC spm_ExecuteSalaryData @DateFrom, @DateTo, @EmployeeCode",
                                                                  @DateFrom, @DateTo, @EmployeeCode) >= 1)
                    return true;
                else return false;
            }
        }
    }
}
