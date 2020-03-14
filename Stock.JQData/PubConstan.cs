using Stock.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stock.JQData
{
    public class PubConstan
    {
        public const string ShortDateFormat = "yyyy-MM-dd";
        public const string LongDateFormat = "yyyy-MM-dd HH:mm";
        public static readonly DateTime PriceStartDate = new DateTime(2015, 1, 1);
        public static readonly Dictionary<UnitEnum, string> UnitParamDic = new Dictionary<UnitEnum, string>
        {{ UnitEnum.Unit1d,"1d"},{ UnitEnum.Unit30m,"30m"},{UnitEnum.Unit60m,"60m" },{UnitEnum.Unit120m,"120" } };

        public static int MaxRecordCntPerFetch = 4992;//能够被8,整除比较好，最小支持30分钟线
        public static readonly Dictionary<UnitEnum, double> RecordCntPerDay = new Dictionary<UnitEnum, double>
        {{ UnitEnum.Unit30m,8f},{UnitEnum.Unit60m,4f },{UnitEnum.Unit120m,2f },{ UnitEnum.Unit1d,1f}, };
    }
}
