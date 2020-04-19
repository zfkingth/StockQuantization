using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyStock.Data;
using MyStock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyStock.WebAPI.Utils;

namespace MyStock.WebAPI.ViewModels.Fillers
{
    public class CalcLimitNumViewModel : BaseDoWorkViewModel
    {
        private readonly ILogger _logger;
        public CalcLimitNumViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<CalcLimitNumViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
        }



        internal async Task PullAll()
        {
            await setStartDate(SystemEvents.CalcLimitNum);

            StaFun st = new StaFun();

            await st.CalcLimitNum();

            await setFinishedDate(SystemEvents.CalcLimitNum);

        }

    }
}
