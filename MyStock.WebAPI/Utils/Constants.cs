using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStock.WebAPI.Utils
{
    public class Constants
    {
        public static readonly TimeSpan IdleTimeStartSpan = new TimeSpan(0);
        public static readonly TimeSpan IdleTimeEndSpan = new TimeSpan(6, 30, 0);

        public static readonly TimeSpan StockStartSpan = new TimeSpan(9, 15, 0);

        /// <summary>
        /// 把交易时间延长了半小时，appsettings.json里的FetchRealTimeDataCycle 不要超过1800秒，不然有会导致取不到最后一次实时数据
        /// </summary>
        public static readonly TimeSpan StockEndSpan = new TimeSpan(15, 25, 0);

        public static readonly int MaxQueueCnt = 5;

        /// <summary>
        /// 深圳成指
        /// </summary>
        public static readonly string IndexBase = "1399001";

    }
}
