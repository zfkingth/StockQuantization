using Microsoft.EntityFrameworkCore;
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
    public class PullStockIndex1dViewModel : BaseDoWorkViewModel
    {


        private readonly ILogger _logger;
        public PullStockIndex1dViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<PullStockIndex1dViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            this.stockHandle = DayDataFiller_stockHandle;

        }


        async Task DayDataFiller_stockHandle(BaseDoWorkViewModel.StockArgs e)
        {
            string code = e.Stock.StockId;

            System.Diagnostics.Debug.WriteLine($"****************  pull daily data : {code} start  ***************************");
            HandleFun hf = new HandleFun();
            await hf.Update_Price1dAsync(e.Stock.StockId);

            System.Diagnostics.Debug.WriteLine($"****************  pull daily data : {code} end    ***************************");
        }
        protected override List<Stock> GetSecList()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();


                var list = (from i in db.StockSet

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
            await setStartDate(SystemEvents.PullStockIndex1d);


            base.DoWork();


            await setFinishedDate(SystemEvents.PullStockIndex1d);

        }







    }
}
