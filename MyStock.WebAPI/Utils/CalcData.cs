using Microsoft.EntityFrameworkCore;
using MyStock.Data;
using MyStock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStock.WebAPI.Utils
{
    public class CalcData
    {
        /// <summary>
        /// 返回复权后的数据
        /// </summary>
        /// <param name="stockId">股票代码</param>
        /// <param name="lastedNum">需要返回最近的日线数据个数</param>
        public static async Task<List<DayData>> GetFuQuanData(StockContext db, string stockId, int lastedNum)
        {
            var list = await (from i in db.DayDataSet
                              where i.StockId == stockId && i.Open != 0
                              orderby i.Date descending
                              select i).Take(lastedNum).AsNoTracking().ToListAsync();

            await FuQuan(db, stockId, list);

            return list;

        }

        public static async Task FuQuan(StockContext db, string stockId, List<DayData> list)
        {
            if (list.Count > 0)
            {
                //除息时间必须在数据列的最小时间之后，不然就不需要运算
                DateTime minDate = list.Last().Date;

                //有可能公告的除息时间还没到，也就是例如今天是3月5号，除息时间为3月9号这种情况

                //除息时间必须在数据列的最大时间之前，不然就不需要计算
                DateTime maxDate = list.First().Date;


                //获取分红派股数据

                var paiguList = await (from i in db.SharingSet
                                       where i.StockId == stockId && i.DateChuXi != null
                                       && minDate < i.DateChuXi
                                       && i.DateChuXi <= maxDate
                                       orderby i.DateGongGao descending
                                       select i).AsNoTracking().ToListAsync();


                for (int i = 0; i < paiguList.Count; i++)
                {
                    Sharing paigu = paiguList[i];
                    //已经在查询表达式中查找了除息时间为空的状况
                    for (int j = 0; j < list.Count; j++)
                    {
                        //list 是逆序
                        DayData item = list[j];
                        if (item.Date < paigu.DateChuXi.Value)
                        {
                            //数据的日期在除息日期之前才需要复权
                            item.Close = (item.Close - paigu.PaiXi / 10.0f) / (1 + paigu.SongGu / 10.0f + paigu.ZhuanZeng / 10.0f);
                            item.Open = ((item.Open - paigu.PaiXi / 10.0f) / (1 + paigu.SongGu / 10.0f + paigu.ZhuanZeng / 10.0f));
                            item.High = ((item.High - paigu.PaiXi / 10.0f) / (1 + paigu.SongGu / 10.0f + paigu.ZhuanZeng / 10.0f));
                            item.Low = ((item.Low - paigu.PaiXi / 10.0f) / (1 + paigu.SongGu / 10.0f + paigu.ZhuanZeng / 10.0f));
                            //只能大致的计算
                            item.Volume = item.Volume * (1 + paigu.SongGu / 10.0f + paigu.ZhuanZeng / 10.0f);
                        }

                    }


                }
            }
        }


        /// <summary>
        /// 获取均线数据
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="daysNum">平均的天数</param>
        /// <returns></returns>
        public static float[] Average(List<float> dataList, int daysNum)
        {
            if (daysNum <= 0)
                throw new Exception("daysNum param error");




            float[] ma = new float[dataList.Count - daysNum];
            int cnt = dataList.Count;

            float tempSum = 0f;

            for (int i = 0; i < daysNum; i++)
            {
                tempSum += dataList[i];
            }

            //逆序排列
            for (int i = 0; i < dataList.Count - daysNum; i++)
            {
                ma[i] = tempSum / daysNum;

                //减去当前，加上最远的
                tempSum -= dataList[i];
                tempSum += dataList[i + daysNum];

            }


            return ma;
        }

        /// <summary>
        /// 计算macd的diff,12天和26天
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public static float?[] DIFF(List<float> dataList)
        {
            int cnt = dataList.Count;

            float?[] diff = new float?[cnt];

            var EMA12 = EXPMA(dataList, 12);
            var EMA26 = EXPMA(dataList, 26);

            for (int i = 26; i < cnt; i++)
            {
                int index = cnt - 1 - i;

                diff[index] = EMA12[index] - EMA26[index];
            }

            return diff;

        }


        /// <summary>
        /// ema(diff,9)
        /// </summary>
        /// <param name="diff"></param>
        /// <returns></returns>
        public static float?[] DEA(float?[] diff)
        {

            int cnt = diff.Length;


            float?[] dea = new float?[cnt];
            for (int i = 0; i < dea.Length; i++)
            {
                //初始化
                dea[i] = 0;
            }

            //从第27个数据 开始取，下标从0开始
            for (int i = 26; i < cnt; i++)
            {
                //DEA（MACD）= 前一日DEA×8/10＋今日DIF×2/10
                //数据时按照时间逆序开始排列的
                int index = cnt - 1 - i;
                dea[index] = dea[index + 1] * 8 / 10f + diff[index] * 2 / 10f;
            }


            return dea;

        }


        public static float?[] EXPMA(List<float> dataList, int daysNum)
        {
            if (daysNum <= 0)
                throw new Exception("daysNum param error");




            float?[] expma = new float?[dataList.Count];
            int cnt = dataList.Count;

            if (cnt >= daysNum)
            {
                //当reverseDataList.Count==daysNum时，有一个ema数据
                for (int i = daysNum - 1; i < dataList.Count; i++)
                {
                    //第一个ema数据为平均数
                    if (i == daysNum - 1)
                    {
                        //求平均数
                        float ma = 0;
                        for (int k = 0; k < daysNum; k++)
                        {
                            ma += dataList[cnt - 1 - k];//逆序排列的
                        }
                        ma = ma / daysNum;

                        expma[cnt - 1 - i] = ma;
                    }
                    else
                    {
                        expma[cnt - 1 - i] = (dataList[cnt - 1 - i] * 2 + expma[cnt - 1 - i + 1] * (daysNum - 1)) / (daysNum + 1);
                    }
                }
            }

            return expma;
        }
    }

}
