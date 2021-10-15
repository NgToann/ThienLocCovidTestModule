using System;
using System.Collections.Generic;
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
using LiveCharts;
using LiveCharts.Wpf;

using TLCovidTest.Controllers;
using TLCovidTest.Models;
using TLCovidTest.ViewModels;
using TLCovidTest.Helpers;
using LiveCharts.Defaults;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for EmployeeChartWindow.xaml
    /// </summary>
    public partial class EmployeeChartWindow : Window
    {
        List<EmployeeModel> employeeList;
        List<DepartmentModel> departmentList;
        public EmployeeChartWindow(List<EmployeeModel> employeeList, List<DepartmentModel> departmentList)
        {
            this.employeeList = employeeList;
            this.departmentList = departmentList;
            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            DataContext = this;

            InitializeComponent();

            var sectionList = departmentList.Where(w => String.IsNullOrEmpty(w.ParentID) == true).ToList();
            SeriesCollection sColl = new SeriesCollection();

            List<EmployeeModel> employeeOthers = new List<EmployeeModel>();
            foreach (var section in sectionList)
            {
                var employeeBySection = new List<EmployeeModel>();
                var childDept = departmentList.Where(w => w.ParentID == section.DepartmentID).ToList();
                if (childDept.Count() > 1)
                {
                    var employeeByChildDept = new List<EmployeeModel>();
                    foreach (var child in childDept)
                    {
                        employeeByChildDept = employeeList.Where(w => w.DepartmentName.ToUpper().Trim().ToString() == child.DepartmentFullName.ToUpper().Trim().ToString()).ToList();
                        employeeBySection.AddRange(employeeByChildDept);
                    }
                }
                else
                {
                    employeeBySection = employeeList.Where(w => w.DepartmentName.Trim().ToUpper().ToString() == section.DepartmentName).ToList();
                }
                
                int qty = employeeBySection.Count();
                if (qty == 0)
                    continue;

                if (qty > 100)
                {
                    sColl.Add(new PieSeries
                    {
                        Title = String.Format("{0} ({1})", section.DepartmentName, qty),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(qty) },
                        DataLabels = true,
                        LabelPoint = PointLabel,
                        Tag = employeeBySection
                    });
                }
                else
                {
                    employeeOthers.AddRange(employeeBySection);
                }
            }

            int qtyOthers = employeeOthers.Count();
            sColl.Add(new PieSeries
            {
                Title = String.Format("{0} ({1})", "OTHERS", qtyOthers),
                Values = new ChartValues<ObservableValue> { new ObservableValue(qtyOthers) },
                DataLabels = true,
                LabelPoint = PointLabel,
                Tag = employeeOthers
            });

            pieChart.Series = sColl;
        }

        public Func<double, string> LineFormatter { get; set; }
        public Func<ChartPoint, string> PointLabel { get; set; }
        private void PieChart_DataClick(object sender, ChartPoint chartPoint)
        {
            try
            {
                var chart = (LiveCharts.Wpf.PieChart)chartPoint.ChartView;
                //clear selected slice.
                foreach (PieSeries series in chart.Series)
                    series.PushOut = 0;
                var selectedSeries = (PieSeries)chartPoint.SeriesView;
                selectedSeries.PushOut = 8;

                txtTitleChart.Text = String.Format("Section: {0}", selectedSeries.Title);
                var employeeClickedList = selectedSeries.Tag as List<EmployeeModel>;
                var lineList = employeeClickedList.Select(s => s.DepartmentName).Distinct().OrderBy(o => o).ToList();

                List<double> values = new List<double>();
                List<string> LineLabels = new List<string>();
                foreach (var line in lineList)
                {
                    var employeeByLine = employeeClickedList.Where(w => w.DepartmentName == line).ToList();
                    values.Add((double)employeeByLine.Count());
                    LineLabels.Add(line);
                }

                var TestSeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Workers",
                        Values = new ChartValues<double>(values)
                    }
                };

                chartEmpBySection.AxisX.Clear();
                chartEmpBySection.AxisY.Clear();
                chartEmpBySection.Series.Clear();

                chartEmpBySection.AxisX.Add(new Axis {
                    Labels = LineLabels
                });

                chartEmpBySection.AxisY.Add(new Axis
                {
                    Title = "Volumne",
                    LabelFormatter=  LineFormatter
                });
                LineFormatter = value => value.ToString("N0");
                chartEmpBySection.Series = TestSeriesCollection;

                double widthCustom = 0;
                widthCustom = lineList.Count() * 150;
                if (lineList.Count() > 10)
                    widthCustom = lineList.Count() * 50;
                if (lineList.Count() > 30)
                    widthCustom = lineList.Count() * 16;
                chartEmpBySection.Width = widthCustom;
            }
            catch { }
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
