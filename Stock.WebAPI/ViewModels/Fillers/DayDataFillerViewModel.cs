using Microsoft.EntityFrameworkCore;
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
            string code = e.Stock.Code;

            System.Diagnostics.Debug.WriteLine($"****************  pull daily data : {code} start  ***************************");
            HandleFun hf = new HandleFun();
            if (e.Stock.Type == SecuritiesEnum.Index)
            {
                await hf.Update_PriceAsync(UnitEnum.Unit30m, e.Stock.Code);
                await hf.Update_PriceAsync(UnitEnum.Unit60m, e.Stock.Code);
                await hf.Update_PriceAsync(UnitEnum.Unit120m, e.Stock.Code);
            }
            await hf.Update_PriceAsync(UnitEnum.Unit1d, e.Stock.Code);

            System.Diagnostics.Debug.WriteLine($"****************  pull daily data : {code} end    ***************************");
        }
        protected override List<Securities> GetSecList()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();


                var list = (from i in db.SecuritiesSet
                            where i.Type == SecuritiesEnum.Stock ||
                            i.Type == SecuritiesEnum.Index
                            select i).AsNoTracking().ToList();
                return list;
            }
        }



        /// <summary>
        /// 填充所有的股票日线数据
        /// </summary>
        /// <returns></returns>



        public async Task PullAll()
        {
            await setStartDate();
            System.Diagnostics.Debug.WriteLine("start pull all day data");


            base.DoWork();


            await setFinishedDate();

            System.Diagnostics.Debug.WriteLine("end pull all day data");
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
