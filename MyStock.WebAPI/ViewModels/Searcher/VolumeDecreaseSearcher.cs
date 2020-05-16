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
    /// 连续缩量
    /// </summary>
    public class VolumeDecreaseSearcher : BaseDoWorkViewModel
    {



        private readonly ILogger _logger;

        private ArgVolumeDecrease _arg;

        public VolumeDecreaseSearcher(IServiceScopeFactory serviceScopeFactory,
                string userId, IConfiguration configuration,
            ILogger logger, ArgVolumeDecrease arg) : base(serviceScopeFactory, userId, configuration)
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
                float volumePercent = _arg.VolumePercent;

                var dayDataList = await helper.GetDayData(stockId, nearestDayNum, _arg.BaseDate);

                if (dayDataList.Count > 0)
                {

                    //对数据进行处理
                    for (int i = 0; i < dayDataList.Count - riseNum + 1; i++)
                    {


                        int j = 0;
                        for (; j < riseNum - 1; j++)
                        {
                            int index = dayDataList.Count - 1 - (i + j);
                            if (dayDataList[index].Volume < dayDataList[index - 1].Volume)
                            {
                                //不符合连续缩量
                                break;
                            }
                        }

                        if (j == riseNum - 1)
                        {
                            //符合条件
                            //再判断成交量
                            int index = dayDataList.Count - 1 - (i + j);

                            if (dayDataList[index].Volume <= dayDataList[index + riseNum - 1].Volume * volumePercent / 100f)
                            {
                                //加入
                                dayData = dayDataList[dayDataList.Count - 1 - i];
                                break;
                            }
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


            base.prepareSearch(_arg, "成交缩量");
            List<RealTimeData> list = null;

            list = await DoSearch(_arg.StockIdList, Filter);


            return list;


        }

    }


}
