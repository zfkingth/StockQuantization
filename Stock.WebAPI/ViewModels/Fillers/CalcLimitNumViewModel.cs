using Stock.CalcOnServer;
using Microsoft.Extensions.DependencyInjection;
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

            await st.CalcLimitNum(UnitEnum.Unit1d);

            await setFinishedDate(SystemEvents.CalcLimitNum);

        }

    }
}
