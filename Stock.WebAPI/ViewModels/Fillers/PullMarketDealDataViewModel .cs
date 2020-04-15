﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stock.Data;
using Stock.JQData;
using Stock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.WebAPI.ViewModels.Fillers
{
    public class PullMarketDealDataViewModel : BaseDoWorkViewModel
    {
        private readonly ILogger _logger;
        public PullMarketDealDataViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<PullMarketDealDataViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
        }



        internal async Task PullAll()
        {
            await setStartDate(SystemEvents.PullMarketDealData);

            HandleFun hf = new HandleFun();
            await hf.Update_MarketDeal_data();

            await setFinishedDate(SystemEvents.PullMarketDealData);

        }

    }
}
