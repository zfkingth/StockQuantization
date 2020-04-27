﻿using Microsoft.Extensions.DependencyInjection;
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
                               orderby i.Date descending
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


                List<DayData> previousDayDataList = new List<DayData>();

                //需要list升序排列
                foreach (var dateItem in needHandleList)
                {
                    var query2 = from i in db.DayDataSet
                                 where i.Date == dateItem
                                 select i;
                    var currentDayDataList = await query2.AsNoTracking().ToListAsync();

                    if (previousDayDataList.Count > 0)
                    {
                        //进行统计
                        var staItem = new StaPrice()
                        {
                            Date = dateItem,
                            HighlimitNum = 0,
                            LowlimitNum = 0,
                            FailNum = 0,
                            Permanent = true,
                        };

                        foreach (var currentDayData in currentDayDataList)
                        {
                            var previousItem = previousDayDataList.FirstOrDefault(s => s.StockId == currentDayData.StockId);
                            if (previousItem != null)
                            {
                                double zhangtingjia = Math.Round(previousItem.Close * 1.1, 2, MidpointRounding.AwayFromZero);
                                double dietingjia = Math.Round(previousItem.Close * 0.9, 2, MidpointRounding.AwayFromZero);

                                if (currentDayData.Close == zhangtingjia)
                                {
                                    staItem.HighlimitNum++;
                                }
                                else if (currentDayData.Close == dietingjia)
                                {
                                    staItem.LowlimitNum++;
                                }
                            }
                        }

                        db.StaPrice.Add(staItem);
                    }

                    //结束后
                    previousDayDataList = currentDayDataList;

                }

                await db.SaveChangesAsync();


            }
        }

    }
}
