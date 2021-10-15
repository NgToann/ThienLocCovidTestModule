using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TLCovidTest.Models;

namespace TLCovidTest.ViewModels
{
    public class DepartmentViewModel : BaseViewModel
    {
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string pid { get; set; }
        public string DepartmentFullName { get; set; }
        public string ParentID { get; set; }
        private DepartmentModel _DepartmentSelected;
        public DepartmentModel DepartmentSelected
        {
            get { return _DepartmentSelected; }
            set
            {
                _DepartmentSelected = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DepartmentSelected"));
            }
        }
    }
}
