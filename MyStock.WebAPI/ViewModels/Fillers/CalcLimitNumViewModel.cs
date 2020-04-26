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
            DateTime lastDateTime = await GetLastTradeDayFromWebPage();
            using (var db = new StockContext())
            {

                var querySta = from i in db.StaPrice
                               orderby i.Date
                               select i;
                var lastItem = await querySta.FirstOrDefaultAsync();

                DateTime startDateThisTime = default;

                if (lastItem != null)
                {
                    if (lastItem.Permanent == false)
                    {
                        startDateThisTime = lastItem.Date;
                        //把临时的数据删掉
                        db.StaPrice.Remove(lastItem);
                    }
                    else
                    {
                        startDateThisTime = lastItem.Date.AddDays(1);//增加了一天，但不一定是交易日。
                    }
                }

                var query = from i in db.DayDataSet
                            where i.StockId == Constants.IndexBase
                            && i.Date >= startDateThisTime
                            orderby i.Date ascending
                            select i.Date;

                var needHandleList = await query.ToListAsync();


                foreach (var dateItem in needHandleList)
                {

                    await staByDate(dateItem, db);


                }

                await db.SaveChangesAsync();


            }
        }

        private async Task staByDate(DateTime dateItem, StockContext db)
        {
            //daydata 建立了date 索引
            var query = from i in db.DayDataSet
                        where i.Date == dateItem
                        select i;
            var itemList = await query.AsNoTracking().ToListAsync();
           
        }
    }
}
