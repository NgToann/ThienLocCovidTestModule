using TLCovidTest.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for TestRandomViewEmployeeListWindow.xaml
    /// </summary>
    public partial class TestRandomViewEmployeeListWindow : Window
    {
        List<EmployeeModel> employeeList;
        public TestRandomViewEmployeeListWindow(List<EmployeeModel> employeeList)
        {
            this.employeeList = employeeList;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgEmployeeList.ItemsSource = employeeList;
            dgEmployeeList.Items.Refresh();
        }

        private void dgEmployeeList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
