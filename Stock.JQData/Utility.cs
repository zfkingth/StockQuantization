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

        public static string ToDateString(DateTime date)
        {
            string format = Constants.ShortDateFormat;
            //if (unit < UnitEnum.Unit1d)
            //    format = Constants.ShortDateFormat;
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

        public static bool IsTradingTime(DateTime time)
        {


            if (time.DayOfWeek == DayOfWeek.Saturday || time.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
            else
            {

                var sp = new TimeSpan(time.Hour, time.Minute, time.Second);
                if (sp >= Constants.StockStartSpan && sp <= Constants.StockEndSpan)
                {
                    return true;
                }

                return false;

            }
        }

        public static bool IsAfterMarketEnd(DateTime time)
        {

            var sp = new TimeSpan(time.Hour, time.Minute, time.Second);
            if (sp >= Constants.StockEndSpan)
            {
                return true;
            }

            return false;
        }
    }
}
