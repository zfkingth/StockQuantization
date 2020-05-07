using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyStock.Data;
using MyStock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyStock.WebAPI.ViewModels.Fillers
{
    public class PullRealTimeViewModel : BaseDoWorkViewModel
    {


        private readonly ILogger _logger;
        public PullRealTimeViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<PullRealTimeViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            _eventName = SystemEvents.PullRealTime;

        }

        private async Task UpdateDateFlag(string stockId, DateTime date)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();


                var item = await (from i in db.StockSet
                                  where i.StockId == stockId
                                  select i).FirstOrDefaultAsync();
                item.RealDataUpdated = date;
                await db.SaveChangesAsync();
            }


        }


        /// <summary>
        /// 以并发的形式获取实时数据，并且每个请求获取100支股票的实时数据
        /// </summary>
        public async Task PullAll()
        {
            await setStartDate(_eventName);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();


                var list = await (from i in db.StockSet
                                  select i.StockId).ToListAsync();//不会超过300000

                List<List<string>> listofIdList = new List<List<string>>();

                int perRequest = 100;

                List<string> tempList = null;
                for (int i = 0; i < list.Count; i++)
                {
                    if (i % perRequest == 0)
                    {
                        tempList = new List<string>();
                        listofIdList.Add(tempList);
                    }

                    tempList.Add(list[i]);

                }

                //定义线程取消的一个对象

                int progressCnt = 0;

                int cnt = listofIdList.Count;
                Progress = 0;

                IsRunning = true;


                var po = new ParallelOptions()
                {
                    CancellationToken = cts.Token,
                    MaxDegreeOfParallelism = MaxThreadNum,
                };

                try
                {
                    Parallel.ForEach(listofIdList, po, (item) =>
                    {
                        progressCnt = processList(item, progressCnt, cnt, po);

                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Parallel.ForEach throw exception :{ ex.Message}");
                }
                finally
                {
                    IsRunning = false;
                }

            }

            await setFinishedDate(_eventName);
        }

        private int processList(List<string> item, int progressCnt, int cnt, ParallelOptions po)
        {
            po.CancellationToken.ThrowIfCancellationRequested();
            try
            {

                IEnumerable<RealTimeData> realItems = null; ;
                realItems = GetStockRealTimeFormNetEase(item).Result;

                using (StockContext db = new StockContext())
                {
                    foreach (var realItem in realItems)
                    {
                        WriteRealTdDb(db, realItem).Wait();
                    }
                }



                Interlocked.Increment(ref progressCnt);
                //增加1%才更新
                float progress = (float)progressCnt / cnt;
                if (progress - 0.05f >= Progress || progress == 1)
                {
                    Progress = progress;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return progressCnt;
        }

        private async Task WriteRealTdDb(StockContext db, RealTimeData realItem)
        {


            var stockId = realItem.StockId;
            var date = realItem.Date;


            //数据库中只保留多个时间点的实时数据，


            if (await db.RealTimeDataSet.AnyAsync(s => s.StockId == stockId && s.Date == date))
            {
                //如果有这个数据则处理下一个股票的数据
                System.Diagnostics.Debug.WriteLine($"{stockId} {date} 的实时数据已存在");
                return;
            }

            var previous = await (from i in db.DayDataSet
                                  where i.StockId == stockId   //没有换手就是停牌,确保数据库中没有停牌的数据
                                  orderby i.Date descending
                                  select i).FirstOrDefaultAsync();



            db.RealTimeDataSet.Add(realItem);

            if (previous != null)
            {
                if (previous.LiuTongShiZhi != null && previous.Close != 0)
                {
                    float liutongNum = (previous.LiuTongShiZhi.Value / previous.Close);
                    realItem.HuanShouLiu = realItem.Volume / liutongNum * 100f;

                    realItem.LiuTongShiZhi = realItem.Close * liutongNum;

                }

                if (previous.ZongShiZhi != null && previous.Close != 0)
                {
                    float allNum = (previous.ZongShiZhi.Value / previous.Close);

                    realItem.ZongShiZhi = realItem.Close * allNum;

                }


            }



            await db.SaveChangesAsync();
            await UpdateDateFlag(stockId, date);


        }





    }
}
