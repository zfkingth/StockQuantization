using Stock.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Stock.JQData
{
    public class Utility
    {
        public static DateTime ParseDateString(string s, UnitEnum unit)
        {
            string format = Constants.ShortDateFormat;
            if (unit < UnitEnum.Unit1d)
                format = Constants.MiddleDateFormat;
            var date = DateTime.ParseExact(s, format, CultureInfo.InvariantCulture);
            return date;
        }

        public static string ToDateString(DateTime date, UnitEnum unit)
        {
            string format = Constants.ShortDateFormat;
            if (unit < UnitEnum.Unit1d)
                format = Constants.ShortDateFormat;
            string s = date.ToString(format, CultureInfo.InvariantCulture);
            return s;
        }

        public static bool IsSameDay(DateTime lastedDate, DateTime lastTradeDay)
        {
            if (lastedDate.Year == lastTradeDay.Year && lastedDate.Month == lastTradeDay.Month
                 && lastedDate.Day == lastTradeDay.Day)
                return true;
            return false;
        }
    }
}
