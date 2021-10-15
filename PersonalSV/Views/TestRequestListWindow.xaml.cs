using TLCovidTest.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TLCovidTest.Models;
using System.Linq;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for TestRequestListWindow.xaml
    /// </summary>
    public partial class TestRequestListWindow : Window
    {
        List<EmployeeModel> sources;
        List<TestRandomModel> testRequestToday;
        public TestRequestListWindow(List<EmployeeModel> sources, List<TestRandomModel> testRequestToday)
        {
            this.sources = sources;
            this.testRequestToday = testRequestToday;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in sources)
            {
                var testRequestByEmpId = testRequestToday.FirstOrDefault(f => f.EmployeeCode == item.EmployeeCode);
                if (testRequestByEmpId != null)
                    item.TestRandomTimeIn = testRequestByEmpId.TimeIn;
            }
            dgTestRequest.ItemsSource = sources;
            dgTestRequest.Items.Refresh();
        }

        private void dgTestRequest_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
