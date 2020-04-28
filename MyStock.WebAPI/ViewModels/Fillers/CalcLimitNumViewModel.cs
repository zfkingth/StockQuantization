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
    public class CalcLimitNumViewModel : BaseDoWorkViewModel
    {
        private readonly ILogger _logger;
        public CalcLimitNumViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<CalcLimitNumViewModel> logger) : base(serviceScopeFactory)
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

            var stockList = base.GetStockList();
            using (var db = new StockContext())
            {

                var querySta = from i in db.StaPrice
                               orderby i.Date descending
                               select i;
                var list = await querySta.Take(10).ToListAsync(); //更新最后10次的，怕数据不全导致出错

                var item = list.Last();

                await calcStaPrice(stockList, db, item);

                await db.SaveChangesAsync();


            }
        }

        private static async Task<DateTime> calcStaPrice(List<Stock> stockList, StockContext db, StaPrice lastItem)
        {
            DateTime startDateThisTime = default;
            if (lastItem != null)
            {
                startDateThisTime = lastItem.Date;
            }

            var query = from i in db.DayDataSet
                        where i.StockId == Constants.IndexBase
                        && i.Date >= startDateThisTime
                        orderby i.Date ascending
                        select i.Date;

            var needHandleList = await query.ToListAsync();


            List<DayData> previousDayDataList = new List<DayData>();

            //需要list升序排列
            foreach (var dateItem in needHandleList)
            {

                System.Diagnostics.Debug.WriteLine($"calc limit num for {dateItem}");
                var query2 = from i in db.DayDataSet
                             where i.Date == dateItem
                             select i;
                var currentDayDataList = await query2.AsNoTracking().ToListAsync();

                if (previousDayDataList.Count > 0)
                {
                    var staItem = await (
                        from i in db.StaPrice
                        where i.Date == dateItem
                        select i
                               ).FirstOrDefaultAsync();
                    if (staItem == null)
                    {
                        //进行统计
                        staItem = new StaPrice();
                        staItem.Date = dateItem;
                        db.StaPrice.Add(staItem);
                    }

                    staItem.HighlimitNum = 0;
                    staItem.LowlimitNum = 0;
                    staItem.FailNum = 0;
                    staItem.Permanent = true;

                    foreach (var currentDayData in currentDayDataList)
                    {
                        var previousItem = previousDayDataList.FirstOrDefault(s => s.StockId == currentDayData.StockId);
                        if (previousItem != null)
                        {
                            double zhangtingjia = Math.Round(previousItem.Close * 1.1, 2, MidpointRounding.AwayFromZero);
                            double dietingjia = Math.Round(previousItem.Close * 0.9, 2, MidpointRounding.AwayFromZero);

                            var stock = (
                                from i in stockList
                                where i.StockId == currentDayData.StockId
                                select i
                                         ).FirstOrDefault();

                            if (stock != null)
                            {

                                if (currentDayData.Date - stock.MarketStartDate >= TimeSpan.FromDays(30))
                                {
                                    ///只统计上市30天以后的。

                                    if (currentDayData.Close >= zhangtingjia)
                                    {
                                        staItem.HighlimitNum++;
                                    }
                                    else if (currentDayData.Close <= dietingjia)
                                    {
                                        staItem.LowlimitNum++;
                                    }
                                }
                            }
                        }
                    }

                }

                //结束后
                previousDayDataList = currentDayDataList;

            }

            return startDateThisTime;
        }
    }
}
