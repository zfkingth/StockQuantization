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
    public class PullMarginDataViewModel : BaseDoWorkViewModel
    {
        private readonly ILogger _logger;
        public PullMarginDataViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<PullMarginDataViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
        }



        internal async Task PullAll()
        {
            await setStartDate(SystemEvents.PullMarginData);

            HandleFun hf = new HandleFun();
            await hf.Update_margin_data();

            await setFinishedDate(SystemEvents.PullMarginData);

        }

    }
}
