using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TLCovidTest.Models;
using System.Windows;

namespace TLCovidTest.Helpers
{
    public class LanguageHelper
    {
        public static void SetLanguageDictionary(EnumLanguage lang)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (lang)
            {
                case EnumLanguage.English:
                    dict.Source = new Uri(@"..\Resources\LanguageEnglish.xaml", UriKind.Relative);
                    break;
                case EnumLanguage.VietNamese:
                    dict.Source = new Uri(@"..\Resources\LanguageVietnamese.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri(@"..\Resources\LanguageEnglish.xaml", UriKind.Relative);
                    break;
            }

            Cons.CurrentLanguage = lang;
            // Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        public static string GetStringFromResource(string resourceName)
        {
            try
            {
                return (string)Application.Current.FindResource(resourceName);
            }
            catch
            {
                return String.Format("Can't Find Resource Name: {0}", resourceName);
            }
        }
    }

    public static class Cons
    {
        public static EnumLanguage CurrentLanguage = EnumLanguage.English;
    }
}
