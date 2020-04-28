using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyStock.Data;
using MyStock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyStock.WebAPI.Utils;
using Microsoft.EntityFrameworkCore;

namespace MyStock.WebAPI.ViewModels.Fillers
{
    public class CalcRealTimeLimitNumViewModel : BaseDoWorkViewModel
    {
        private readonly ILogger _logger;
        public CalcRealTimeLimitNumViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<CalcRealTimeLimitNumViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
        }



        internal async Task PullAll()
        {
            await setStartDate(SystemEvents.CalcLimitNum);


            await staLimitNum();

            await setFinishedDate(SystemEvents.CalcLimitNum);

        }

        private async Task staLimitNum()
        {
            DateTime lastDateTime = await GetLastTradeDayFromWebPage();

            lastDateTime = new DateTime(lastDateTime.Year, lastDateTime.Month, lastDateTime.Day);

            var stockList = base.GetStockList();
            using (var db = new StockContext())
            {

                var querySta = from i in db.StaPrice
                               where i.Date == lastDateTime
                               select i;
                var staItem = await querySta.FirstOrDefaultAsync();


                if (staItem != null)
                {
                    if (staItem.Permanent)
                    {
                        return;
                    }
                }
                else
                {
                    staItem = new StaPrice()
                    {
                        Date = lastDateTime,

                    };

                    db.StaPrice.Add(staItem);
                }
                staItem.HighlimitNum = 0;
                staItem.LowlimitNum = 0;
                staItem.FailNum = 0;
                staItem.Permanent = false;


                DateTime previousDate = await (
                    from i in db.DayDataSet
                    where i.StockId == Constants.IndexBase && i.Date < lastDateTime
                    orderby i.Date descending
                    select i.Date
                    ).FirstOrDefaultAsync();


                System.Diagnostics.Debug.WriteLine($"calc realtime  limit num for {lastDateTime}");
                var query2 = from i in db.DayDataSet
                             where i.Date == previousDate
                             select i;
                var previousDayDataList = await query2.AsNoTracking().ToListAsync();


                foreach (var previousDayData in previousDayDataList)
                {
                    var realTimeItem = await (
                        from i in db.RealTimeDataSet
                        where i.StockId == previousDayData.StockId
                        orderby i.Date descending
                        select i
                        ).FirstOrDefaultAsync();

                    if (realTimeItem != null)
                    {
                        double zhangtingjia = Math.Round(previousDayData.Close * 1.1, 2, MidpointRounding.AwayFromZero);
                        double dietingjia = Math.Round(previousDayData.Close * 0.9, 2, MidpointRounding.AwayFromZero);

                        var stock = (
                            from i in stockList
                            where i.StockId == previousDayData.StockId
                            select i
                                     ).FirstOrDefault();

                        if (stock != null)
                        {

                            if (realTimeItem.Date - stock.MarketStartDate >= TimeSpan.FromDays(30))
                            {
                                ///只统计上市30天以后的。

                                if (realTimeItem.Close >= zhangtingjia)
                                {
                                    staItem.HighlimitNum++;
                                }
                                else if (realTimeItem.Close <= dietingjia)
                                {
                                    staItem.LowlimitNum++;
                                }
                            }
                        }
                    }
                }






                await db.SaveChangesAsync();


            }

        }
    }

}
