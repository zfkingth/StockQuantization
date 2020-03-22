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
            await setStartDate(Constants.EventPullF10);
            System.Diagnostics.Debug.WriteLine("start pull  all F10 data");


            base.DoWork();


            await setFinishedDate(Constants.EventPullF10);

            System.Diagnostics.Debug.WriteLine("end pull  all F10 data");
        }

     





    }
}
