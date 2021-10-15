using TLCovidTest.Models;
using TLCovidTest.Controllers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for MissingRecordTimeWindow.xaml
    /// </summary>
    public partial class MissingRecordTimeWindow : Window
    {
        BackgroundWorker bwSearch;
        List<MissingRecordTimeModel> results;
        public MissingRecordTimeWindow()
        {
            bwSearch = new BackgroundWorker();
            bwSearch.DoWork += BwSearch_DoWork;
            bwSearch.RunWorkerCompleted += BwSearch_RunWorkerCompleted;

            results = new List<MissingRecordTimeModel>();

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dpFrom.SelectedDate = DateTime.Now.Date;
            dpTo.SelectedDate = DateTime.Now.Date;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!bwSearch.IsBusy)
            {
                results.Clear();
                dgMissingRecord.ItemsSource = null;
                this.Cursor = Cursors.Wait;
                btnSearch.IsEnabled = false;
                var dtFrom = dpFrom.SelectedDate.Value;
                var dtTo = dpTo.SelectedDate.Value;
                var par = new object[] { dtFrom, dtTo };
                bwSearch.RunWorkerAsync(par);
            }
        }
        private void BwSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            var par = e.Argument as object[];
            var dtFrom = (DateTime)par[0];
            var dtTo = (DateTime)par[1];
            e.Result = false;
            try
            {
                results = ReportController.GetMissingRecordTimesFromTo(dtFrom, dtTo);
                e.Result = true;
            }
            catch
            {
            }
        }
        private void BwSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = null;
            btnSearch.IsEnabled = true;
            if (e.Error != null || e.Cancelled == true || (bool)e.Result == false)
            {
                MessageBox.Show("Cant excute data, please try again !");
                return;
            }
            var displayList = new List<MissingRecordTimeModel>();
            string searchWhat = txtEmployeeIDSearch.Text.Trim().ToUpper().ToString();
            if (!String.IsNullOrEmpty(searchWhat))
            {
                displayList = results.Where(w => w.CardId.ToUpper().Trim().ToString() == searchWhat ||
                                                 w.WorkerId.ToUpper().Trim().ToString() == searchWhat).ToList();
            }
            else
                displayList = results;

            dgMissingRecord.ItemsSource = displayList;
            dgMissingRecord.Items.Refresh();
        }



        private void txtEmployeeIDSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            btnSearch.IsDefault = true;
        }

        private void txtEmployeeIDSearch_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            btnSearch.IsDefault = true;
        }

        private void dgMissingRecord_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
