using Stock.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stock.JQData
{
    public class PubConstan
    {
        public const string DateFormatString = "yyyy-MM-dd";
        public static readonly DateTime PriceStartDate = new DateTime(2014, 1, 1);
        public static readonly Dictionary<UnitEnum, string> UnitParamDic = new Dictionary<UnitEnum, string>
        {{ UnitEnum.Unit1d,"1d"},{ UnitEnum.Unit30m,"30m"},{UnitEnum.Unit60m,"60m" },{UnitEnum.Unit120m,"120" } };
    }
}
