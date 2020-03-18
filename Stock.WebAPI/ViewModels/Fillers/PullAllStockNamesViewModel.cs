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
            await setStartDate();

            JQData.HandleFun hf = new JQData.HandleFun();

            //在这里负责刷新token
            JQData.QueryFun qf = new QueryFun();
            await qf.RefreshTokenAsync();
            await qf.RefreshAllTradeDays();

            var res = hf.Update_allStock_Names();

            await setFinishedDate();

        }

        private async Task setStartDate()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var record = db.StockEvents.First(s => s.EventName == Constants.EventPullStockNames);

                record.LastAriseStartDate = DateTime.Now;
                record.Status = EventStatusEnum.Running;
                await db.SaveChangesAsync();

            }
        }

        private async Task setFinishedDate()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var record = db.StockEvents.First(s => s.EventName == Constants.EventPullStockNames);

                record.LastAriseEndDate = DateTime.Now;
                record.Status = EventStatusEnum.Idle;
                await db.SaveChangesAsync();

            }
        }

    }
}
