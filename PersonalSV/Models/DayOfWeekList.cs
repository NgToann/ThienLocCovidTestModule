using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TLCovidTest.Helpers;

namespace TLCovidTest.Models
{
    public class DayOfWeekList
    {
        public static List<string> DayOfWeekListInit()
        {
            List<string> dayOfWeekList = new List<string>();

            string monday = LanguageHelper.GetStringFromResource("dayOfWeekMonday");
            string tuesday = LanguageHelper.GetStringFromResource("dayOfWeekTuesday");
            string wednesday = LanguageHelper.GetStringFromResource("dayOfWeekWednesday");
            string thursday = LanguageHelper.GetStringFromResource("dayOfWeekThursday");
            string friday = LanguageHelper.GetStringFromResource("dayOfWeekFriday");
            string saturday = LanguageHelper.GetStringFromResource("dayOfWeekSaturday");
            string sunday = LanguageHelper.GetStringFromResource("dayOfWeekSunday");

            dayOfWeekList.Add("Default");
            dayOfWeekList.Add(monday);
            dayOfWeekList.Add(tuesday);
            dayOfWeekList.Add(wednesday);
            dayOfWeekList.Add(thursday);
            dayOfWeekList.Add(friday);
            dayOfWeekList.Add(saturday);
            dayOfWeekList.Add(sunday);

            return dayOfWeekList;
        }
    }
}
