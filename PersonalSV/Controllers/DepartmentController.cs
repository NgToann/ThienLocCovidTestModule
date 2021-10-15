using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCovidTest.Models;
using TLCovidTest.Entities;

namespace TLCovidTest.Controllers
{
    public class DepartmentController
    {
        public static List<DepartmentModel> GetDepartments()
        {
            List<DepartmentModel> departmentList = new List<DepartmentModel>();
            using (var db = new PersonalDataEntities())
            {
                //return db.ExecuteStoreQuery<DepartmentModel>("spm_SelectDepartmentFromSource").ToList();
                var departmentSourceList = db.ExecuteStoreQuery<DepartmentModel>("EXEC spm_SelectDepartmentFromSource").ToList();
                foreach (var depart in departmentSourceList)
                {
                    if (!string.IsNullOrEmpty(depart.pid))
                    {
                        var parentDepart = departmentSourceList.Where(w => w.DepartmentID == depart.pid).FirstOrDefault();
                        depart.DepartmentFullName = parentDepart.DepartmentName + " " + depart.DepartmentName;
                    }
                    else
                    {
                        depart.DepartmentFullName = depart.DepartmentName;
                    }
                    departmentList.Add(depart);
                }
            };
            return departmentList;
        }
    }
}
