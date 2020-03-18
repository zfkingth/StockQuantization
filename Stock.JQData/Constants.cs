using Stock.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stock.JQData
{
    public class Constants
    {
        public const string ShortDateFormat = "yyyy-MM-dd";
        public const string MiddleDateFormat = "yyyy-MM-dd HH:mm";
        public const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
        public static readonly DateTime PriceStartDate = new DateTime(2015, 1, 1);
        public static readonly Dictionary<UnitEnum, string> UnitParamDic = new Dictionary<UnitEnum, string>
        {{ UnitEnum.Unit1d,"1d"},{ UnitEnum.Unit30m,"30m"},{UnitEnum.Unit60m,"60m" },{UnitEnum.Unit120m,"120m" } };

        public static int MaxRecordCntPerFetch = 4992;//能够被8,整除比较好，最小支持30分钟线
        public static readonly Dictionary<UnitEnum, double> RecordCntPerDay = new Dictionary<UnitEnum, double>
        {{ UnitEnum.Unit30m,8f},{UnitEnum.Unit60m,4f },{UnitEnum.Unit120m,2f },{ UnitEnum.Unit1d,1f}, };


        public static readonly Dictionary<SecuritiesEnum, string> TypeParamDic = new Dictionary<SecuritiesEnum, string>
        {{ SecuritiesEnum.Stock,"stock"},{ SecuritiesEnum.Index,"index"}};


        public static readonly string EventPullStockNames = "pullStockNames";
        public static readonly string EventPullF10 = "pullF10";
        public static readonly string EventPullDailyData = "pullDaily";
        public static readonly string EventPullReadTimeData = "pullRealTime";

        public static readonly TimeSpan IdleTimeStartSpan = new TimeSpan(0);
        public static readonly TimeSpan IdleTimeEndSpan = new TimeSpan(6, 30, 0);

        public static readonly TimeSpan StockStartSpan = new TimeSpan(9, 15, 0);

        /// <summary>
        /// 把交易时间延长了半小时，appsettings.json里的FetchRealTimeDataCycle 不要超过1800秒，不然有会导致取不到最后一次实时数据
        /// </summary>
        public static readonly TimeSpan StockEndSpan = new TimeSpan(15, 25, 0);

        public static List<DateTime> AllTradeDays = null;

        public static readonly int MaxQueueCnt = 5;



        //收市时间
        public static readonly TimeSpan MarketEndTime = new TimeSpan(15, 0, 0);

        public static DateTime LastTradeEndDateTime { get; internal set; }


        public  const string ShangHaiIndex= "000001.XSHG";

        /// <summary>
        /// 需要处理的指数Code
        /// </summary>
        public static readonly string[] IndexsCode = new string[] {ShangHaiIndex ,
        "399001.XSHE","399006.XSHE"};
    }
}
