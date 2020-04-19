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
    public class PullIndex30mViewModel : BaseDoWorkViewModel
    {


        private readonly ILogger _logger;
        public PullIndex30mViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<PullIndex30mViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            this.stockHandle = handle;

        }


        async Task handle(BaseDoWorkViewModel.StockArgs e)
        {
            string id = e.Stock.StockId;

            System.Diagnostics.Debug.WriteLine($"****************  pull 30m data : {id} start  ***************************");
            HandleFun hf = new HandleFun();
            await hf.Update_Price30mAsync(id);

            System.Diagnostics.Debug.WriteLine($"****************  pull 30m data : {id} end    ***************************");
        }
        protected override List<Stock> GetSecList()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();


                var list = (from i in db.StockSet
                            where i.StockType ==StockTypeEnum.Index
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
            await setStartDate(SystemEvents.PullIndex30m);


            base.DoWork();


            await setFinishedDate(SystemEvents.PullIndex30m);

        }







    }
}
