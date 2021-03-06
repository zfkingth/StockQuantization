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
    /// 根据流通市值来筛选
    /// </summary>
    public class CirculatedMarketSearcher : BaseDoWorkViewModel
    {



        private readonly ILogger _logger;

        private ArgCirculatedMarket _arg;

        public CirculatedMarketSearcher(IServiceScopeFactory serviceScopeFactory,
                string userId, IConfiguration configuration,
            ILogger logger, ArgCirculatedMarket arg) : base(serviceScopeFactory, userId, configuration)
        {
            _logger = logger;
            _arg = arg;

            this._actionName = "流通市值";

        }










        private async Task<RealTimeData> Filter(string stockId)
        {


            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var helper = new Utils.Utility(db);

                float marketLow = _arg.MarketLow;
                float marketHigh = _arg.MarketHigh;
                RealTimeData ret = null;

                int objIndex = _arg.StockIdList.IndexOf(stockId);

                DateTime date = _arg.BaseDate;
                if (_arg.DateList.Count > 0)
                    date = _arg.DateList[objIndex];

                RealTimeData realData = await helper.GetRealTimeDataWithDate(stockId, date);


                if (realData != null && realData.ZongShiZhi != null)
                {
                    if (marketLow * 1e8 <= realData.LiuTongShiZhi && realData.LiuTongShiZhi <= marketHigh * 1e8)
                    {
                        ret = realData;
                    }

                }



                //如果是null，表示不符合筛选条件

                return ret; ;

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

            base.prepareSearch(_arg);


            list = await DoSearch(_arg.StockIdList, Filter);

            return list;

        }

    }


}
