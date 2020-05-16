using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyStock.Data;
using MyStock.Model;
using MyStock.WebAPI.ViewModels.Fillers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStock.WebAPI.ViewModels.Searcher
{
    /// <summary>
    /// 连续上涨，要求收盘价格高于开盘价格
    /// </summary>
    public class NRiseOpenSearcher : BaseDoWorkViewModel
    {



        private readonly ILogger _logger;

        private ArgNRiseOpen _arg;

        public NRiseOpenSearcher(IServiceScopeFactory serviceScopeFactory,
                string userId, IConfiguration configuration,
            ILogger logger, ArgNRiseOpen arg) : base(serviceScopeFactory, userId, configuration)
        {
            _logger = logger;
            _arg = arg;

        }










        private async Task<RealTimeData> Filter(string stockId)
        {
            DayData dayData = null;


            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var helper = new Utils.Utility(db);




                int nearestDayNum = _arg.NearDaysNum;
                int riseNum = _arg.NRiseNum;

                var dayDataList = await helper.GetDayData(stockId, nearestDayNum, _arg.BaseDate);


                if (dayDataList.Count > 0)
                {

                    //对数据进行处理
                    for (int i = 0; i < dayDataList.Count - riseNum + 1; i++)
                    {


                        int j = 0;
                        for (; j < riseNum; j++)
                        {
                            if (dayDataList[dayDataList.Count - 1 - (i + j)].Close <= dayDataList[dayDataList.Count - 1 - (i + j)].Open)
                            {
                                //不符合连涨条件
                                break;
                            }
                        }

                        if (j == riseNum)
                        {
                            //符合条件，不需要再检索
                            //加入
                            dayData = dayDataList[dayDataList.Count - 1 - i];
                            break;
                        }

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


        private async Task<RealTimeData> Filter_Down(string stockId)
        {
            DayData dayData = null;


            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var helper = new Utils.Utility(db);




                int nearestDayNum = _arg.NearDaysNum;
                int riseNum = _arg.NRiseNum;

                //比上面的多一天数据
                var dayDataList = await helper.GetDayData(stockId, nearestDayNum + 1, _arg.BaseDate);


                if (dayDataList.Count > riseNum)
                {


                    //对数据进行处理
                    for (int i = 0; i < dayDataList.Count - riseNum; i++)
                    {

                        //在连涨之前必须是跌的
                        int j = 0;
                        //第一个元素必须是跌的
                        if (dayDataList[dayDataList.Count - 1 - (i + j)].Close < dayDataList[dayDataList.Count - 1 - (i + j)].Open)
                        //if (dayDataList[dayDataList.Count - 1 - (i + j)].ZhangDieFu <= 0)
                        {
                            j++;
                            for (; j < riseNum + 1; j++)
                            {
                                if (dayDataList[dayDataList.Count - 1 - (i + j)].Close <= dayDataList[dayDataList.Count - 1 - (i + j)].Open)
                                //if (dayDataList[dayDataList.Count - 1 - (i + j)].ZhangDieFu <= 0)
                                {
                                    //不符合连涨条件
                                    break;
                                }
                            }
                        }

                        if (j == riseNum + 1)
                        {
                            //符合条件，不需要再检索
                            //加入
                            dayData = dayDataList[i];
                            break;
                        }

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
            if (_arg.NRiseNum > _arg.NearDaysNum)
            {
                throw new Exception("参数不正确");
            }

            base.prepareSearch(_arg, "连续上涨");

            List<RealTimeData> list = null;



            if (_arg.DownTag)
            {
                list = await DoSearch(_arg.StockIdList, Filter_Down);
            }
            else
            {

                list = await DoSearch(_arg.StockIdList, Filter);
            }

            return list;


        }

    }


}
