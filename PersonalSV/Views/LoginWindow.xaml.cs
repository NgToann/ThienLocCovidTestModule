using System;
using System.Windows;

using TLCovidTest.Models;
using TLCovidTest.Controllers;
using TLCovidTest.Helpers;
using TLCovidTest.Views;
using System.ComponentModel;
using System.Windows.Input;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        BackgroundWorker bwLogin;
        public LoginWindow()
        {
            bwLogin = new BackgroundWorker();
            bwLogin.DoWork += BwLogin_DoWork;
            bwLogin.RunWorkerCompleted += BwLogin_RunWorkerCompleted;

            InitializeComponent();
            txtUserName.Focus();
        }

        private void BwLogin_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var accountLogin = e.Result as AccountModel;
            if (accountLogin != null)
            {
                MessageBox.Show(string.Format("{0}{1} !", LanguageHelper.GetStringFromResource("logInWindowMsgWelcome"), accountLogin.FullName), this.Title, MessageBoxButton.OK, MessageBoxImage.Information);

                MainWindow window = new MainWindow(accountLogin);
                this.Hide();
                this.txtPassword.Clear();
                window.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show(string.Format("{0}", LanguageHelper.GetStringFromResource("logInWindowMsgLoginFailed")), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Cursor = null;
        }

        private void BwLogin_DoWork(object sender, DoWorkEventArgs e)
        {
            var par = e.Argument as object[];
            var username = par[0] as string;
            var password = par[1] as string;

            var accountLogin = new AccountModel();
            accountLogin = AccountController.GetAccountByUP(username, password);
            e.Result = accountLogin;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string userName = txtUserName.Text;
            string password = txtPassword.Password;

            if (bwLogin.IsBusy == false)
            {
                this.Cursor = Cursors.Wait;
                var par = new object[] { userName, password };
                bwLogin.RunWorkerAsync(par);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
