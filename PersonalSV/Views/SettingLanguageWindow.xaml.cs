using System.Windows;
using System.Threading;

using TLCovidTest.Helpers;
using TLCovidTest.Models;

namespace TLCovidTest.Views
{
    /// <summary>
    /// Interaction logic for SettingLanguageWindow.xaml
    /// </summary>
    public partial class SettingLanguageWindow : Window
    {
        public SettingLanguageWindow()
        {
            InitializeComponent();
        }

        private void radEnglish_Click(object sender, RoutedEventArgs e)
        {
            LanguageHelper.SetLanguageDictionary(EnumLanguage.English);
            this.InitializeComponent();
            Thread.Sleep(1500);
            this.Close();
        }

        private void radVietnamese_Click(object sender, RoutedEventArgs e)
        {
            LanguageHelper.SetLanguageDictionary(EnumLanguage.VietNamese);
            this.InitializeComponent();
            Thread.Sleep(1500);
            this.Close();
        }
    }
}
