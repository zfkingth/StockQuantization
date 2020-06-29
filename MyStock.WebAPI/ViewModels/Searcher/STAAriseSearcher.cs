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
    /// 突破MA均线
    /// </summary>
    public class STAAriseSearcher : BaseDoWorkViewModel
    {



        private readonly ILogger _logger;

        private ArgSTA _arg;

        public STAAriseSearcher(IServiceScopeFactory serviceScopeFactory,
                string userId, IConfiguration configuration,
            ILogger logger, ArgSTA arg) : base(serviceScopeFactory, userId, configuration)
        {
            _logger = logger;
            _arg = arg;
            this._actionName = null;//action name是null 就不会用缓存

        }










        private async Task<RealTimeData> Filter(string stockId)
        {
            DayData dayData = null;


            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var helper = new Utils.Utility(db);
                bool fitFlag = false;

                int objIndex = _arg.StockIdList.IndexOf(stockId);

                DateTime startDate = _arg.DateList[objIndex];
                //当天的数据算一个
                var dayDataList = await helper.GetDayData(stockId, startDate, _arg.BaseDate);


                if (dayDataList.Count > 0)
                {

                    //使用成交量，需要复权
                    //复权，会改变dayDataList中的数据
                    await Utils.CalcData.FuQuan(db, stockId, dayDataList);

                    //
                    //获取均线数据
                    var high = (from ii in dayDataList
                                select ii.High).Max();

                    if (high != 0)
                    {
                        dayData = dayDataList.FirstOrDefault();
                        var startItem = dayDataList.LastOrDefault();
                        dayData.ZhangDieFu = (high - startItem.Close) / startItem.Close * 100;

                        fitFlag = true;
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
            if (_arg.StockIdList.Count != _arg.DateList.Count)
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
