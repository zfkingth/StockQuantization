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
            string code = e.Stock.Code;

            System.Diagnostics.Debug.WriteLine($"****************  pull 30m data : {code} start  ***************************");
            HandleFun hf = new HandleFun();
            await hf.Update_PriceAsync(UnitEnum.Unit30m, e.Stock.Code);

            System.Diagnostics.Debug.WriteLine($"****************  pull 30m data : {code} end    ***************************");
        }
        protected override List<Securities> GetSecList()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();


                var list = (from i in db.SecuritiesSet
                            where i.Type == SecuritiesEnum.Index
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
