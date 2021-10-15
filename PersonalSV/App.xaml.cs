using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Reflection;
using System.Runtime.Remoting;

using TLCovidTest.Views;

namespace TLCovidTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {        
        private static Mutex _mutex = null;
        public App()
        {
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "PersonalSV";
            bool createdNew;
            _mutex = new Mutex(true, appName, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show(String.Format("{0} is already running !", appName), appName, MessageBoxButton.OK, MessageBoxImage.Stop);
                Application.Current.Shutdown();
            }
            // Endcode connectionString
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            // Get the connectionStrings section.
            ConfigurationSection configSection = config.GetSection("connectionStrings");
            //Ensures that the section is not already protected.
            if (configSection.SectionInformation.IsProtected == false)
            {
                //Uses the Windows Data Protection API (DPAPI) to encrypt the configuration section using a machine-specific secret key.
                configSection.SectionInformation.ProtectSection(
                    "DataProtectionConfigurationProvider");
                config.Save();
            }
            base.OnStartup(e);
        }
        public void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "SV_HRS", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }
}
