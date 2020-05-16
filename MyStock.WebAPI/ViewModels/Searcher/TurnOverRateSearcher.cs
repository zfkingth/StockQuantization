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
    /// 根据换手率
    /// </summary>
    public class TurnOverRateSearcher : BaseDoWorkViewModel
    {



        private readonly ILogger _logger;

        private ArgTurnOverRate _arg;

        public TurnOverRateSearcher(IServiceScopeFactory serviceScopeFactory,
                string userId, IConfiguration configuration,
            ILogger logger, ArgTurnOverRate arg) : base(serviceScopeFactory, userId, configuration)
        {
            _logger = logger;
            _arg = arg;

        }










        private async Task<RealTimeData> Filter(string stockId)
        {


            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var helper = new Utils.Utility(db);

                float low = _arg.TurnOverRateLow;
                float high = _arg.TurnOverRateHigh;
                RealTimeData ret = null;


                var realData = await helper.GetRealTimeDataWithDate(stockId, _arg.BaseDate);
                if (realData != null)
                {
                    var zhibiao = realData.HuanShouLiu;

                    if (zhibiao != null)
                    {
                        if (low <= zhibiao && zhibiao <= high)
                        {
                            ret = realData;
                        }

                    }
                }



                //如果是null，表示不符合筛选条件

                return ret; ;

            }
        }




        public override async Task<List<RealTimeData>> Search()
        {

            List<RealTimeData> list = null;

          
            base.prepareSearch(_arg, "换手率");

            list = await DoSearch(_arg.StockIdList, Filter);

            return list;

        }

    }


}
