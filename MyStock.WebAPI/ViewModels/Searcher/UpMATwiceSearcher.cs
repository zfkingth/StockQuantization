﻿using Microsoft.Extensions.Configuration;
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
    /// 突破MA均线
    /// </summary>
    public class UpMATwiceSearcher : BaseDoWorkViewModel
    {



        private readonly ILogger _logger;

        private ArgUpMATwice _arg;

        public UpMATwiceSearcher(IServiceScopeFactory serviceScopeFactory,
                string userId, IConfiguration configuration,
            ILogger logger, ArgUpMATwice arg) : base(serviceScopeFactory, userId, configuration)
        {
            _logger = logger;
            _arg = arg;
            this._actionName = "二次上穿均线";

        }










        private async Task<RealTimeData> Filter(string stockId)
        {
            DayData dayData = null;


            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var helper = new Utils.Utility(db);

                int numcnt = _arg.RecentDaysNum
                    + _arg.MaxDaysNumDownAvgBeforeTwice +
                    +_arg.MaxDaysNumUpAvgAfterFirst
                    + _arg.MinDaysNumDownAvgBeforeFirst
                    + _arg.AvgDays;

                //当天的数据算一个
                var dayDataList = await helper.GetDayData(stockId, numcnt, _arg.BaseDate);

                //使用成交量，需要复权
                //复权，会改变dayDataList中的数据
                await Utils.CalcData.FuQuan(db, stockId, dayDataList);

                bool fitFlag = false;

                if (dayDataList.Count == numcnt)
                {
                    //
                    //获取均线数据
                    var closeList = (from ii in dayDataList
                                     select ii.Close).ToList();

                    var maArray = Utils.CalcData.Average(closeList, _arg.AvgDays);
                    int gobleIndex = 0;
                    //满足条件的flag
                    for (; gobleIndex < _arg.RecentDaysNum; gobleIndex++)
                    {
                        var current = dayDataList[gobleIndex];

                        if (current.Close >= maArray[gobleIndex] &&
                             dayDataList[gobleIndex + 1].Close < maArray[gobleIndex + 1])
                        {

                            //满足本阶段条件
                            fitFlag = true;
                            dayData = current;
                            break;
                        }
                    }

                    if (fitFlag)
                    {
                        //重新设置标记
                        //检查在均线之下是否超过最大天数
                        fitFlag = false;
                        int startIndex = gobleIndex + 1;  //之前的处理前一个数据时，并没有移动索引
                        int bianjie = startIndex + _arg.MaxDaysNumDownAvgBeforeTwice + 1;//往前再弄一天
                        for (gobleIndex = startIndex; gobleIndex < bianjie; gobleIndex++)
                        {
                            var current = dayDataList[gobleIndex];
                            if (current.Close >= maArray[gobleIndex])
                            {


                                fitFlag = true;
                                break;
                            }
                        }

                    }


                    if (fitFlag)
                    {
                        //重新设置标记
                        //检查在均线之上是否超过最大天数
                        fitFlag = false;
                        int startIndex = gobleIndex;  // //之前的处理前一个数据时，移动了索引
                        int bianjie = startIndex + _arg.MaxDaysNumUpAvgAfterFirst + 1;//刚好全部都在均线上，然后再往前一天在均线下了
                        for (gobleIndex = startIndex; gobleIndex < bianjie; gobleIndex++)
                        {
                            var current = dayDataList[gobleIndex];
                            if (current.Close < maArray[gobleIndex])
                            {
                                //在指定数量内逆序后，第一次出现低于均线

                                //满足本阶段条件
                                fitFlag = true;
                                break;
                            }

                        }

                        if (gobleIndex - startIndex < _arg.MinDaysNumUpAvgAfterFirst)
                        {
                            //在均线之上的天数少于要求。
                            fitFlag = false;
                        }

                    }

                    if (fitFlag)
                    {
                        int exceptCnt = 0;
                        fitFlag = false;
                        int startIndex = gobleIndex;//之前的处理前一个数据时，移动了索引
                        int bianjie = startIndex + _arg.MinDaysNumDownAvgBeforeFirst - 1; //这里是满足性检查，不是数据变化检查，上一个检查必定有一个处于均线之下
                        for (gobleIndex = startIndex; gobleIndex < bianjie; gobleIndex++)
                        {

                            //进一步判读之前的是否是在均线之下
                            var current = dayDataList[gobleIndex];
                            if (current.Close >= maArray[gobleIndex])
                            {
                                //不满足条件
                                exceptCnt++;
                                if (exceptCnt > _arg.MaxDaysNumUpAvgBeforeFirst)
                                {
                                    fitFlag = false;
                                    break;
                                }
                            }
                        }
                        if (exceptCnt <= _arg.MaxDaysNumUpAvgBeforeFirst)
                        {
                            fitFlag = true;
                        }


                    }
                }



                //如果是null，表示不符合筛选条件

                RealTimeData real = null;
                if (fitFlag)
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
            if (_arg.MinDaysNumDownAvgBeforeFirst < 1 || _arg.AvgDays < 1 || _arg.AvgDays > 240)
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
