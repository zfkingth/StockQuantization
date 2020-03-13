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
        {{ UnitEnum.Unit_1d,"1d"},{ UnitEnum.Unit_30m,"30m"},{UnitEnum.Unit_60m,"60m" },{UnitEnum.Unit_120m,"120" } };
    }
}
