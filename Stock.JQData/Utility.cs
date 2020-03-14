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
            string format = PubConstan.ShortDateFormat;
            if (unit < UnitEnum.Unit1d)
                format = PubConstan.LongDateFormat;
            var date = DateTime.ParseExact(s, format, CultureInfo.InvariantCulture);
            return date;
        }

        public static string ToDateString(DateTime date, UnitEnum unit)
        {
            string format = PubConstan.ShortDateFormat;
            if (unit < UnitEnum.Unit1d)
                format = PubConstan.ShortDateFormat;
            string s = date.ToString(format, CultureInfo.InvariantCulture);
            return s;
        }
    }
}
