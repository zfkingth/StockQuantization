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
    public class PullStockF10ViewModel : BaseDoWorkViewModel
    {


        private readonly ILogger _logger;
        public PullStockF10ViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<PullStockIndex1dViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            this.stockHandle = F10Filler_stockHandle;

        }


        async Task F10Filler_stockHandle(BaseDoWorkViewModel.StockArgs e)
        {

            HandleFun hf = new HandleFun();
            await hf.UpdateStockF10(e.Stock.StockId);
           
        }


  
        public async Task PullAll()
        {
            await setStartDate(SystemEvents.PullStockF10);


            base.DoWork();


            await setFinishedDate(SystemEvents.PullStockF10);

        }

     





    }
}
