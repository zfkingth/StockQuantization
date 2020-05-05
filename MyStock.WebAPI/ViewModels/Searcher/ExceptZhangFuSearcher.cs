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
    /// 限定涨跌幅
    /// </summary>
    public class ExceptZhangFuSearcher : BaseDoWorkViewModel
    {



        private readonly ILogger _logger;

        private ArgExceptZhangFu _arg;

        public ExceptZhangFuSearcher(IServiceScopeFactory serviceScopeFactory,
                string userId, IConfiguration configuration,
            ILogger logger, ArgExceptZhangFu arg) : base(serviceScopeFactory, userId, configuration)
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

                float low = _arg.ZhangFuLow;
                float high = _arg.ZhangFuHigh;
                int numcnt = _arg.CircleDaysNum;
                DayData dayData = null;



                //当天的数据算一个
                var dayDataList = await helper.GetDayData(stockId, numcnt, _arg.BaseDate);

                //单独只使用涨跌幅，不需要复权

                if (dayDataList.Count == numcnt)
                {
                    float mul = 1;
                    for (int i = 0; i < dayDataList.Count; i++)
                    {
                        var fudu = dayDataList[i].ZhangDieFu;
                        if (fudu != null)
                            mul *= (1 + fudu.Value / 100f);
                    }


                    if (low / 100f <= mul - 1 && mul - 1 <= high / 100f)
                    {
                        dayData = dayDataList[0];
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
