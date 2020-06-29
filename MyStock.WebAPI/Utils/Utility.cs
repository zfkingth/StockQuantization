using Microsoft.EntityFrameworkCore;
using MyStock.Data;
using MyStock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStock.WebAPI.Utils
{
    public class Utility
    {
        public Utility(StockContext db)
        {
            this._db = db;
        }

        private StockContext _db;


        public async Task<RealTimeData> ConvertDayToReal(DayData item)
        {
            RealTimeData ret = new RealTimeData();

            ret.StockId = item.StockId;

            ret.StockName = await GetStockNameById(item.StockId);
            ret.Date = item.Date;
            ret.Open = item.Open;
            ret.High = item.High;
            ret.Low = item.Low;
            ret.Close = item.Close;
            ret.Amount = item.Amount;
            ret.Volume = item.Volume;
            ret.ZhangDieFu = item.ZhangDieFu;
            ret.HuanShouLiu = item.HuanShouLiu;
            ret.LiuTongShiZhi = item.LiuTongShiZhi;
            ret.ZongShiZhi = item.ZongShiZhi;


            return ret; ;
        }


        public async Task<string> GetStockNameById(string stockId)
        {


            string name = await (from i in _db.StockSet
                                 where i.StockId == stockId
                                 select i.StockName).FirstOrDefaultAsync();

            if (name == null)
            {
                return "--";
            }
            else
            {
                return name;
            }

        }


        public DayData ConvertRealToDay(RealTimeData item)
        {
            DayData ret = new DayData();

            ret.StockId = item.StockId;
            ret.Date = item.Date;
            ret.Open = item.Open;
            ret.High = item.High;
            ret.Low = item.Low;
            ret.Close = item.Close;
            ret.Amount = item.Amount;
            ret.Volume = item.Volume;
            ret.ZhangDieFu = item.ZhangDieFu;
            ret.HuanShouLiu = item.HuanShouLiu;
            ret.LiuTongShiZhi = item.LiuTongShiZhi;
            ret.ZongShiZhi = item.ZongShiZhi;


            return ret;
        }


        /// <summary>
        /// 获取包括筛选时间的股票市值,换手率的的 real time data，如果没有会查询历史数据进行转换。
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="baseDate"></param>
        /// <returns></returns>
        public async Task<RealTimeData> GetRealTimeDataWithDate(string stockId, DateTime baseDate)
        {
            RealTimeData rt = null;

            var realData = await (from i in _db.RealTimeDataSet
                                  where i.StockId == stockId && i.Date <= baseDate && i.Open != 0
                                  orderby i.Date descending
                                  select i).AsNoTracking().FirstOrDefaultAsync();
            if (realData != null)
            {
                rt = realData;
            }
            else
            {
                var dayDataItem = await (from i in _db.DayDataSet
                                         where i.StockId == stockId && i.Date <= baseDate && i.Open != 0
                                         orderby i.Date descending
                                         select i
                                         ).AsNoTracking().FirstOrDefaultAsync();
                if (dayDataItem != null)
                {
                    rt = await ConvertDayToReal(dayDataItem);
                }


            }

            return rt;
        }


        internal async Task<List<DayData>> GetDayData(string stockId, DateTime startDate, DateTime endDate)
        {
            var dayDataList = await (from i in _db.DayDataSet
                                     where i.StockId == stockId
                                     && i.Date >= startDate
                                     && i.Date <= endDate
                                     orderby i.Date descending
                                     select i
                             ).AsNoTracking().ToListAsync();
            var realData = await (from i in _db.RealTimeDataSet
                                  where i.StockId == stockId && i.Date <= endDate
                                  orderby i.Date descending
                                  select i).AsNoTracking().FirstOrDefaultAsync();


            if (dayDataList.Count > 0)
            {
                DayData lastData = dayDataList[0];//逆序排列的

                DateTime tempDate = DateTime.MinValue;
                if (realData != null)
                    tempDate = new DateTime(realData.Date.Year, realData.Date.Month, realData.Date.Day);

                //dayData只有日期，没有时间
                if (tempDate > lastData.Date)
                {

                    //要将数据附加到日线数据中
                    DayData temp = ConvertRealToDay(realData);
                    dayDataList.Insert(0, temp);

                }

            }
            return dayDataList;

        }


        /// <summary>
        /// 获取最近的日线数据(综合了实时数据),返回值为逆序列表
        /// </summary>
        /// <param name="item"></param>
        /// <param name="nearestDayNum"></param>
        /// <param name="baseDate">基准日期，所有获取的数据在基准日期之前</param>
        /// <returns></returns>
        public async Task<List<DayData>> GetDayData(string stockId, int nearestDayNum, DateTime baseDate)
        {
            var dayDataList = await (from i in _db.DayDataSet
                                     where i.StockId == stockId && i.Date <= baseDate
                                     orderby i.Date descending
                                     select i
                             ).Take(nearestDayNum).AsNoTracking().ToListAsync();
            var realData = await (from i in _db.RealTimeDataSet
                                  where i.StockId == stockId && i.Date <= baseDate
                                  orderby i.Date descending
                                  select i).AsNoTracking().FirstOrDefaultAsync();


            if (dayDataList.Count > 0)
            {
                DayData lastData = dayDataList[0];//逆序排列的

                DateTime tempDate = DateTime.MinValue;
                if (realData != null)
                    tempDate = new DateTime(realData.Date.Year, realData.Date.Month, realData.Date.Day);

                //dayData只有日期，没有时间
                if (tempDate > lastData.Date)
                {

                    //要将数据附加到日线数据中
                    DayData temp = ConvertRealToDay(realData);
                    dayDataList.RemoveAt(dayDataList.Count - 1);
                    dayDataList.Insert(0, temp);
                    //保证总数目不变

                }

            }
            return dayDataList;

        }



        public static double Round(double value, int decimals)
        {
            if (value < 0)
            {
                return Math.Round(value + 5 / Math.Pow(10, decimals + 1), decimals, MidpointRounding.AwayFromZero);
            }
            else
            {
                return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
            }
        }

        internal static bool IsSameDay(DateTime lastedDate, DateTime lastTradeDay)
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

        /// <summary>
        /// 在收盘之后
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool IsAfterMarketEnd(DateTime time)
        {

            var sp = new TimeSpan(time.Hour, time.Minute, time.Second);
            if (sp >= Constants.StockEndSpan)
            {
                return true;
            }

            return false;
        }

        ///// <summary>
        ///// 在开盘之前
        ///// </summary>
        ///// <param name="time"></param>
        ///// <returns></returns>
        //public static bool IsBeforeStart(DateTime time)
        //{

        //    var sp = new TimeSpan(time.Hour, time.Minute, time.Second);
        //    if (sp <= Constants.StockStartSpan)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        public static bool IsBetween(TimeSpan start, TimeSpan end)
        {
            DateTime time = DateTime.Now;

            var sp = new TimeSpan(time.Hour, time.Minute, time.Second);
            if (sp >= start && sp <= end)
            {
                return true;
            }

            return false;
        }


        public static float? convertToFloat(string numberStr)
        {
            float number;

            // The numberStr is the string you want to parse
            if (float.TryParse(numberStr, out number))
            {
                return number;
            }

            return null;
        }

    }
}
