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
    /// 突破MA均线
    /// </summary>
    public class VolumeBreakSearcher : BaseDoWorkViewModel
    {



        private readonly ILogger _logger;

        private ArgVolumeBreak _arg;

        public VolumeBreakSearcher(IServiceScopeFactory serviceScopeFactory,
                string userId, IConfiguration configuration,
            ILogger logger, ArgVolumeBreak arg) : base(serviceScopeFactory, userId, configuration)
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

                int numcnt = _arg.CircleDaysNum + _arg.NearDaysNum;

                //当天的数据算一个
                var dayDataList = await helper.GetDayData(stockId, numcnt, _arg.BaseDate);

                //使用成交量，需要复权
                //复权，会改变dayDataList中的数据
                await Utils.CalcData.FuQuan(db, stockId, dayDataList);

                bool fitFlag = false;

                if (dayDataList.Count == numcnt)
                {


                    for (int j = 0; j < _arg.NearDaysNum; j++)
                    {

                        DayData current = dayDataList[_arg.NearDaysNum - 1 - j];

                        if (current.ZhangDieFu >= _arg.ZhangFu)
                        {
                            float sum = 0;

                            //对数据进行处理,当日的不处理
                            for (int i = 0; i < _arg.CircleDaysNum; i++)
                            {
                                sum += dayDataList[dayDataList.Count - 1 - (i + j)].Volume;


                            }
                            float avrage = sum / _arg.CircleDaysNum;

                            //判断突破的时间

                            if (current.Volume >= avrage * _arg.VolTimesNum)
                            {
                                dayData = current;//满足要求
                                fitFlag = true;

                                break;
                            }

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
            List<RealTimeData> list = null;

            if (_arg.SearchFromAllStocks)
                _arg.StockIdList = GetAllStockIdWithOutIndex();

            list = await DoSearch(_arg.StockIdList, Filter);

            return list;

        }

    }


}
