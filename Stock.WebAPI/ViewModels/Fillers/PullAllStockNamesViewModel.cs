using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyStock.Data;
using MyStock.Model;
using MyStock.WebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStock.WebAPI.ViewModels.Fillers
{
    public class PullAllStockNamesViewModel : BaseDoWorkViewModel
    {
        private readonly ILogger _logger;
        public PullAllStockNamesViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<PullAllStockNamesViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
        }



        internal async Task PullAll()
        {
            await setStartDate(SystemEvents.PullAllStockNames);

            HandleFun hf = new HandleFun();

            //在这里负责刷新token

            await hf.Update_allStock_Names();

            await setFinishedDate(SystemEvents.PullAllStockNames);

        }

    }
}
