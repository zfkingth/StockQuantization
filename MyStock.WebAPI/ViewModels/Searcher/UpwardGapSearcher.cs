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
    /// 向上踏空
    /// </summary>
    public class UpwardGapSearcher : BaseDoWorkViewModel
    {



        private readonly ILogger _logger;

        private ArgUpwardGap _arg;

        public UpwardGapSearcher(IServiceScopeFactory serviceScopeFactory,
                string userId, IConfiguration configuration,
            ILogger logger, ArgUpwardGap arg) : base(serviceScopeFactory, userId, configuration)
        {
            _logger = logger;
            _arg = arg;
            this._actionName = "向上跳空";

        }










        private async Task<RealTimeData> Filter(string stockId)
        {
            DayData dayData = null;


            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var helper = new Utils.Utility(db);

                int numcnt = 1 + _arg.NearDaysNum;
                //当天的数据算一个
                var dayDataList = await helper.GetDayData(stockId, numcnt, _arg.BaseDate);

                //使用成交量，需要复权
                //复权，会改变dayDataList中的数据
                await Utils.CalcData.FuQuan(db, stockId, dayDataList);



                if (dayDataList.Count == numcnt)
                {




                    //符合回调条件的点
                    bool match = false;

                    int matchIndex = -1;

                    for (int i = 0; i < _arg.NearDaysNum; i++)
                    {
                        //从最早的数据开始遍历
                        int currentIndex = dayDataList.Count - 1 - i - 1;//要跳过第一个数据
                        var currentListItem = dayDataList[currentIndex];
                        var previousListItem = dayDataList[currentIndex + 1]; //是倒序。
                        if (currentListItem.Low > previousListItem.High &&
                                (currentListItem.Low - previousListItem.Close) / previousListItem.Close > _arg.GapPercent / 100.0 &&
                                currentListItem.Close > currentListItem.Open && //真阳线
                                 currentListItem.ZhangDieFu <= _arg.LimitPercent
                            )
                        {

                            //满足跳空条件

                            //后面的最低价格
                            float suffixMinLow = float.MaxValue;

                            //寻找后续最低点
                            for (int j = i; j < _arg.NearDaysNum; j++)
                            {
                                var temp = dayDataList[dayDataList.Count - 1 - j - 1];
                                float current = temp.Low;
                                if (current < suffixMinLow)
                                {
                                    suffixMinLow = current;
                                }
                            }



                            if (suffixMinLow > previousListItem.High)
                            {
                                //符合回调幅度的高点
                                match = true;
                                matchIndex = currentIndex;
                                break;
                            }


                        }



                    }
                    if (match)
                    {

                        //符合条件
                        dayData = dayDataList[matchIndex];

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
            //必须至少间隔10
            if (_arg.NearDaysNum <= 0)
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
