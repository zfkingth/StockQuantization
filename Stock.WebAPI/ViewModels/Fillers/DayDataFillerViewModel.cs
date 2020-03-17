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
    public class DayDataFillerViewModel : BaseDoWorkViewModel
    {


        private readonly ILogger _logger;
        public DayDataFillerViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<DayDataFillerViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            this.stockHandle = DayDataFiller_stockHandle;

        }


        async Task DayDataFiller_stockHandle(BaseDoWorkViewModel.StockArgs e)
        {

            System.Diagnostics.Debug.WriteLine($"******************pull daily data : {e.StockId}***************************");
            await FillStockData(e.StockId);


        }

        /// <summary>
        /// 填充所有的股票日线数据
        /// </summary>
        /// <returns></returns>



        public async Task PullAll()
        {
            await setStartDate();
            System.Diagnostics.Debug.WriteLine("Filling all day data");


            base.DoWork();


            await setFinishedDate();

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


        public async Task FillStockData(string stockId)
        {
            HandleFun hf = new HandleFun();
           await hf.FillStockData(UnitEnum.Unit1d, stockId);
        }








    }
}
