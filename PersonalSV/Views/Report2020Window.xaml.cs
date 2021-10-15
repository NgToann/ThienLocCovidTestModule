using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for Report2020Window.xaml
    /// </summary>
    public partial class Report2020Window : Window
    {
        BackgroundWorker bwLoad;
        List<EmployeeModel> employeeList;
        List<Report3T> reportList;
        List<LeaveDayDetailModel> leaveDayDetail2019;
        public Report2020Window()
        {
            bwLoad = new BackgroundWorker();
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;

            employeeList = new List<EmployeeModel>();
            reportList = new List<Report3T>();
            leaveDayDetail2019 = new List<LeaveDayDetailModel>();
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bwLoad.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                bwLoad.RunWorkerAsync();
            }
        }
        private void BwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            employeeList = EmployeeController.GetAvailable();
            leaveDayDetail2019 = LeaveDayDetailController.GetByYear();
            var months = new Int32[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var monthsName = new String[] {"Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"};

            // Create Column
            DataTable dt = new DataTable();
            Dispatcher.Invoke(new Action(() =>
            {
                // Column Name
                dt.Columns.Add("EmployeeName", typeof(String));
                DataGridTemplateColumn colEmpName = new DataGridTemplateColumn();
                colEmpName.Header = String.Format("EmployeeName");
                DataTemplate templateEmpName = new DataTemplate();
                FrameworkElementFactory tblEmpName = new FrameworkElementFactory(typeof(TextBlock));
                templateEmpName.VisualTree = tblEmpName;
                tblEmpName.SetBinding(TextBlock.TextProperty, new Binding(String.Format("EmployeeName")));
                tblEmpName.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);
                tblEmpName.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                tblEmpName.SetValue(TextBlock.PaddingProperty, new Thickness(3, 0, 0, 0));
                colEmpName.CellTemplate = templateEmpName;
                colEmpName.SortMemberPath = "EmployeeName";
                colEmpName.ClipboardContentBinding = new Binding(String.Format("EmployeeName"));
                dgReport.Columns.Add(colEmpName);

                // Column Code
                dt.Columns.Add("EmployeeCode", typeof(String));
                DataGridTemplateColumn colEmpCode= new DataGridTemplateColumn();
                colEmpCode.Header = String.Format("EmployeeCode");
                DataTemplate templateEmpCode = new DataTemplate();
                FrameworkElementFactory tblEmpCode = new FrameworkElementFactory(typeof(TextBlock));
                templateEmpCode.VisualTree = tblEmpCode;
                tblEmpCode.SetBinding(TextBlock.TextProperty, new Binding(String.Format("EmployeeCode")));
                tblEmpCode.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                tblEmpCode.SetValue(TextBlock.PaddingProperty, new Thickness(3, 0, 0, 0));
                colEmpCode.CellTemplate = templateEmpCode;
                colEmpCode.SortMemberPath = "EmployeeCode";
                colEmpCode.ClipboardContentBinding = new Binding(String.Format("EmployeeCode"));
                dgReport.Columns.Add(colEmpCode);

                // Column ID
                dt.Columns.Add("EmployeeID", typeof(String));
                DataGridTemplateColumn colEmpID = new DataGridTemplateColumn();
                colEmpID.Header = String.Format("EmployeeID");
                DataTemplate templateEmpID = new DataTemplate();
                FrameworkElementFactory tblEmpID = new FrameworkElementFactory(typeof(TextBlock));
                templateEmpID.VisualTree = tblEmpID;
                tblEmpID.SetBinding(TextBlock.TextProperty, new Binding(String.Format("EmployeeID")));
                tblEmpID.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                tblEmpID.SetValue(TextBlock.PaddingProperty, new Thickness(3, 0, 0, 0));
                colEmpID.CellTemplate = templateEmpID;
                colEmpID.SortMemberPath = "EmployeeID";
                colEmpID.ClipboardContentBinding = new Binding(String.Format("EmployeeID"));
                dgReport.Columns.Add(colEmpID);

                // Column Department
                dt.Columns.Add("Department", typeof(String));
                DataGridTemplateColumn colDept = new DataGridTemplateColumn();
                colDept.Header = String.Format("Department");
                DataTemplate templateDept = new DataTemplate();
                FrameworkElementFactory tblDept = new FrameworkElementFactory(typeof(TextBlock));
                templateDept.VisualTree = tblDept;
                tblDept.SetBinding(TextBlock.TextProperty, new Binding(String.Format("Department")));
                tblDept.SetValue(TextBlock.PaddingProperty, new Thickness(3, 0, 0, 0));
                tblDept.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                colDept.CellTemplate = templateDept;
                colDept.SortMemberPath = "Department";
                colDept.ClipboardContentBinding = new Binding(String.Format("Department"));
                dgReport.Columns.Add(colDept);

                

                // Column Absent Per Month
                for(int i = 0; i < months.Count(); i++)
                { 
                    dt.Columns.Add(String.Format("Column{0}", months[i]), typeof(Int32));
                    //dt.Columns.Add(String.Format("ColumnSupplierID{0}", months[i]), typeof(Int32));
                    DataGridTemplateColumn colMonth= new DataGridTemplateColumn();
                    colMonth.Header = String.Format("{0}", monthsName[i]);
                    DataTemplate templateMonth = new DataTemplate();
                    FrameworkElementFactory tblMonth = new FrameworkElementFactory(typeof(TextBlock));
                    templateMonth.VisualTree = tblMonth;
                    tblMonth.SetBinding(TextBlock.TextProperty, new Binding(String.Format("Column{0}", months[i])));
                    tblMonth.SetValue(TextBlock.PaddingProperty, new Thickness(3, 3, 3, 3));
                    tblMonth.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                    tblMonth.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);

                    colMonth.CellTemplate = templateMonth;
                    colMonth.SortMemberPath = String.Format("Column{0}", months[i]);
                    colMonth.ClipboardContentBinding = new Binding(String.Format("Column{0}", months[i]));
                    dgReport.Columns.Add(colMonth);
                }
                // Column Total Absent
                dt.Columns.Add("TotalAbsent", typeof(Int32));
                DataGridTemplateColumn colTotal = new DataGridTemplateColumn();
                colTotal.Header = String.Format("TotalAbsent");
                DataTemplate templateTotal = new DataTemplate();
                FrameworkElementFactory tblTotal = new FrameworkElementFactory(typeof(TextBlock));
                templateTotal.VisualTree = tblTotal;
                tblTotal.SetBinding(TextBlock.TextProperty, new Binding(String.Format("TotalAbsent")));
                tblTotal.SetValue(TextBlock.PaddingProperty, new Thickness(3, 0, 0, 0));
                tblTotal.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                colTotal.CellTemplate = templateTotal;
                colTotal.SortMemberPath = "TotalAbsent";
                colTotal.ClipboardContentBinding = new Binding(String.Format("TotalAbsent"));
                dgReport.Columns.Add(colTotal);
            }));

            if (employeeList.Count() > 0)
                employeeList = employeeList.OrderBy(o => o.DepartmentName).ThenBy(th => th.EmployeeID).ToList();

            foreach (var employee in employeeList)
            {
                var absentListPerEmployee = leaveDayDetail2019.Where(w => w.EmployeeCode == employee.EmployeeCode).ToList();
                DataRow dr = dt.NewRow();
                dr["EmployeeName"] = employee.EmployeeName;
                dr["EmployeeCode"] = employee.EmployeeCode;
                dr["EmployeeID"] = employee.EmployeeID;
                dr["Department"] = employee.DepartmentName;
                for (int i = 0; i < months.Count(); i++)
                {
                    var absentPerMonth = absentListPerEmployee.Where(w => w.LeaveDate.Month == months[i]).ToList();
                    dr[String.Format("Column{0}", months[i])] = absentPerMonth.Count();
                }
                dr["TotalAbsent"] = absentListPerEmployee.Count();
                dt.Rows.Add(dr);
            }
            e.Result = dt;
        }

        private void BwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var dt = e.Result as DataTable;
            this.Cursor = null;
            dgReport.ItemsSource = dt.AsDataView();
            dgReport.Items.Refresh();
        }
    }
    public class Report3T
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string CardNo { get; set; }
        public string Dept { get; set; }
        public int AbsentNo { get; set; }
        public DateTime AbsentDate { get; set; }
    }
}
