using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyStock.Data;
using MyStock.Model;
using MyStock.WebAPI.ViewModels.Fillers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStock.WebAPI.ViewModels.Searcher
{
    /// <summary>
    /// 平台突破
    /// </summary>
    public class MiddleBreakSearcher : BaseDoWorkViewModel
    {



        private readonly ILogger _logger;

        private ArgMiddleBreak _arg;

        public MiddleBreakSearcher(IServiceScopeFactory serviceScopeFactory,
                string userId, IConfiguration configuration,
            ILogger logger, ArgMiddleBreak arg) : base(serviceScopeFactory, userId, configuration)
        {
            _logger = logger;
            _arg = arg;
            this._actionName = "中阶突破";

        }










        private async Task<RealTimeData> Filter(string stockId)
        {
            DayData dayData = null;


            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var helper = new Utils.Utility(db);

                //当天的数据算一个
                var dayDataList = await helper.GetDayData(stockId, _arg.CircleDaysNum + _arg.NearDaysNum, _arg.BaseDate);

                //使用成交量，需要复权
                //复权，会改变dayDataList中的数据
                await Utils.CalcData.FuQuan(db, stockId, dayDataList);



                int breakIndex = 0;
                if (dayDataList.Count == _arg.CircleDaysNum + _arg.NearDaysNum)
                {

                    var query = from item in dayDataList
                                select (item.Open + item.Close) / 2;

                    var midPrices = query.ToArray();

                    //求出最近的最高中阶
                    int i = 0;
                    float maxMiddlePrice = 0;
                    for (; i < _arg.NearDaysNum; i++)
                    {
                        float current = midPrices[i];
                        if (current > maxMiddlePrice)
                        {
                            maxMiddlePrice = current;
                            breakIndex = i;
                        }
                    }

                    //求出之前的最高的盘中最高价格
                    float previousMaxMiddle = 0;

                    for (; i < midPrices.Count() - _arg.NearDaysNum; i++)
                    {
                        float current = midPrices[i];
                        if (current > previousMaxMiddle)
                        {
                            previousMaxMiddle = current;
                        }
                    }

                    //符合回调条件的点
                    bool match = false;
                    if (maxMiddlePrice > previousMaxMiddle)
                    {
                        for (i = 0; i < _arg.CircleDaysNum; i++)
                        {
                            //从最早的数据开始遍历
                            var listItem = midPrices[midPrices.Count() - 1 - i];
                            if (listItem >= previousMaxMiddle)
                            {
                                //寻找到了高点

                                //后面的最低价格
                                float suffixMinLow = float.MaxValue;

                                //寻找后续最低点
                                for (int j = i + 1; j < _arg.CircleDaysNum; j++)
                                {
                                    float current = midPrices[midPrices.Count() - 1 - j];
                                    if (current < suffixMinLow)
                                    {
                                        suffixMinLow = current;
                                    }
                                }

                                float fudu = (listItem - suffixMinLow) / listItem;

                                if (fudu >= _arg.HuiTiaoFuDuLow / 100f && fudu <= _arg.HuiTiaoFuDuHigh / 100f)
                                {
                                    //符合回调幅度的高点
                                    match = true;
                                    break;
                                }


                            }

                        }

                    }
                    if (match)
                    {

                        //符合条件
                        dayData = dayDataList[breakIndex];

                    }
                }



                //如果是null，表示不符合筛选条件

                RealTimeData real = null;
                if (dayData != null)
                {
                    real = await helper.ConvertDayToReal(dayData);
                }

                return real;
            }
        }






        /// <summary>
        /// 价格突破
        /// </summary>
        /// <param name="StockList"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        public override async Task<List<RealTimeData>> Search()
        {
            if (_arg.NearDaysNum > _arg.CircleDaysNum)
            {
                throw new Exception("参数不正确");
            }
            base.prepareSearch(_arg);

            List<RealTimeData> list = null;


            list = await DoSearch(_arg.StockIdList, Filter);

            return list;

        }

    }


}
