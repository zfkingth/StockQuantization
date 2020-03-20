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
    public class F10FHPGFillerViewModel : BaseDoWorkViewModel
    {


        private readonly ILogger _logger;
        public F10FHPGFillerViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<DayDataFillerViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            this.stockHandle = F10Filler_stockHandle;

        }


        async Task F10Filler_stockHandle(BaseDoWorkViewModel.StockArgs e)
        {

            System.Diagnostics.Debug.WriteLine($"****************  pull F10 data : {e.Stock} start  ***************************");
            HandleFun hf = new HandleFun();
            await hf.UpdateStockXrXd(e.Stock.Code);
           
            System.Diagnostics.Debug.WriteLine($"****************  pull F10 data : {e.Stock} end    ***************************");
        }


  
        public async Task PullAll()
        {
            await setStartDate();
            System.Diagnostics.Debug.WriteLine("start pull  all F10 data");


            base.DoWork();


            await setFinishedDate();

            System.Diagnostics.Debug.WriteLine("end pull  all F10 data");
        }

        private async Task setStartDate()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var record = db.StockEvents.First(s => s.EventName == Constants.EventPullDailyData);

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

                var record = db.StockEvents.First(s => s.EventName == Constants.EventPullDailyData);

                record.LastAriseEndDate = DateTime.Now;
                record.Status = EventStatusEnum.Idle;
                await db.SaveChangesAsync();

            }
        }







    }
}
