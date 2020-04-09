using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stock.Data;
using Stock.JQData;
using Stock.Model;
using Stock.WebAPI.ViewModels.Fillers;

namespace BackgroundTasksSample.Services
{
    #region snippet1
    public class BackServiceUtil
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        IConfiguration _configuration;
        public BackServiceUtil(IBackgroundTaskQueue queue,
            ILogger<BackServiceUtil> logger,
             IServiceScopeFactory serviceScopeFactory,
           IConfiguration configuration)
        {
            _taskQueue = queue;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }




        IBackgroundTaskQueue _taskQueue;

        public bool IsIdleTime(DateTime time)
        {
            var sp = new TimeSpan(time.Hour, time.Minute, time.Second);
            if (sp >= Constants.IdleTimeStartSpan
                && sp <= Constants.IdleTimeEndSpan)
            {
                return true;
            }

            return false;
        }


        internal async Task JudgePullRealTimeDataAsync()
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var se = await db.StockEvents.FirstAsync(s => s.EventName == SystemEvents.PullReadTimeData);
                if (se.LastAriseEndDate == null)
                {
                    EnquepullRealTimeDataTask();
                }
                else
                {
                    //交易时间执行频率由appsettings.json文件里的ShortPeriodCycle来，定义。
                    //每次执行成功后，会更新LastAriseStartDate，所以不会有太多的执行次数
                    if (Utility.IsTradingTime(DateTime.Now))
                    {

                        EnquepullRealTimeDataTask();
                    }
                }
            }
        }

        internal void EnquePullMarketDealData()
        {
            if (_taskQueue.Count >= Constants.MaxQueueCnt) return;
            _logger.LogInformation("enque pull market deal task.");
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var puller = scopedServices.GetRequiredService<PullMarketDealDataViewModel>();

                    await puller.PullAll();
                }

            });


        }

        internal void EnqueCalcLimitNum()
        {
            if (_taskQueue.Count >= Constants.MaxQueueCnt) return;
            _logger.LogInformation("enque calc limit numtask.");
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var puller = scopedServices.GetRequiredService<CalcLimitNumViewModel>();

                    await puller.PullAll();
                }

            });


        }

        internal void EnquePullMarginData()
        {
            if (_taskQueue.Count >= Constants.MaxQueueCnt) return;
            _logger.LogInformation("enque pull margin data task.");
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var puller = scopedServices.GetRequiredService<PullMarginDataViewModel>();

                    await puller.PullAll();
                }

            });


        }

        /// <summary>
        /// The time between two operations meets the requirements
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        //private bool IsMeet(DateTime date)
        //{
        //    int seconds = _configuration.GetValue<int>("ShortPeriodCycle");

        //    TimeSpan ts = DateTime.Now - date;
        //    TimeSpan tsRequire = new TimeSpan(0, 0, seconds);
        //    if (ts > tsRequire)
        //        return true;

        //    return false;
        //}

        internal void EnqueEraseRealTimeDataTask()
        {
            if (_taskQueue.Count >= Constants.MaxQueueCnt) return;

            _logger.LogInformation("enque erase realTime data task.");
            _taskQueue.QueueBackgroundWorkItem(async token =>
{

    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<StockContext>();



        await db.TruncateRealTimeAndCacheTable();

    }

});



        }

        internal void JudgePullDailyData()
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var se = db.StockEvents.First(s => s.EventName == SystemEvents.PullDailyData);
                if (se.LastAriseEndDate == null)
                {
                    EnquePullDayDataTask();

                }
                else
                {
                    if (IsIdleTime(DateTime.Now))
                        EnquePullDayDataTask();
                }
            }
        }

        internal async Task JudgePullMarginDataAsync()
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var se = await db.StockEvents.FirstOrDefaultAsync(s => s.EventName == SystemEvents.PulMarginData);
                if (se.LastAriseEndDate == null)
                {
                    EnquePullMarginData();

                }
                else
                {
                    if (Utility.IsAfterMarketEnd(DateTime.Now))
                        EnquePullMarginData();
                }
            }
        }


        internal void JudgePullF10()
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var se = db.StockEvents.First(s => s.EventName == SystemEvents.PullF10);
                if (se.LastAriseEndDate == null)
                {
                    EnquePullF10Task();

                }
                else
                {
                    //只有在空闲时间才更新
                    if (IsIdleTime(DateTime.Now))
                    {
                        //检查标志位，如果上一次没有成果
                        //或者上一次的开始时间和今天不是同一天
                        if (se.Status != EventStatusEnum.Idle ||
                            !Utility.IsSameDay(DateTime.Now, se.LastAriseStartDate))
                        {
                            EnquePullF10Task();
                        }
                    }
                }
            }

        }

        internal async Task JudgeEraseRealTimeData()
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                if (IsIdleTime(DateTime.Now) && await RealTimeDataNewestDateIsInHistoryData(db))
                    EnqueEraseRealTimeDataTask();

            }

        }

        private async Task<bool> RealTimeDataNewestDateIsInHistoryData(StockContext db)
        {

            DateTime newestDate;
            var newestItem = await (from i in db.PriceSet
                                    where i.Unit == UnitEnum.Unit1d
                                    orderby i.Date descending
                                    select i).FirstOrDefaultAsync();
            if (newestItem == null) return false; //空表，不用truncate操作

            newestDate = newestItem.Date;


            //也就是说历史数据表（日线）中有的话，实时数据就可以不要了对应的数据，
            //truncate操作要在非交易时间执行，因为real time 数据在交易时间很重要，不能删除，这个时候还没有相应的日线数据
            //从网易取得的历史数据只有日期，时间都是00:00:00。
            var handledDate = new DateTime(newestDate.Year, newestDate.Month, newestDate.Day);

            bool existInHistory = await db.PriceSet.AnyAsync(s => s.Unit == UnitEnum.Unit1d &&
                                             s.Date == handledDate);

            return existInHistory;



        }

        internal void JudgePullStockNames()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var se = db.StockEvents.First(s => s.EventName == SystemEvents.PullStockNames);
                if (se.LastAriseEndDate == null)
                {
                    EnquePullAllStockNamesTask();

                }
                else
                {
                    //只有在空闲时间才更新
                    if (IsIdleTime(DateTime.Now))
                    {
                        //检查标志位，如果上一次没有成果
                        //或者上一次的开始时间和今天不是同一天
                        if (se.Status != EventStatusEnum.Idle ||
                            !Utility.IsSameDay(DateTime.Now, se.LastAriseStartDate))
                        {
                            EnquePullAllStockNamesTask();
                        }
                    }
                }
            }
        }

        public void EnquePullAllStockNamesTask()
        {
            if (_taskQueue.Count >= Constants.MaxQueueCnt) return;

            _logger.LogInformation("enque pull all stock names task.");
            _taskQueue.QueueBackgroundWorkItem(async token =>
{

    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var scopedServices = scope.ServiceProvider;
        var puller = scopedServices.GetRequiredService<PullAllStockNamesViewModel>();

        await puller.PullAll();
    }

});


        }

        public void EnquePullF10Task()
        {
            if (_taskQueue.Count >= Constants.MaxQueueCnt) return;
            _logger.LogInformation("enque pull all stock F10 task.");
            _taskQueue.QueueBackgroundWorkItem(async token =>
{

    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var scopedServices = scope.ServiceProvider;
        var puller = scopedServices.GetRequiredService<F10FHPGFillerViewModel>();

        await puller.PullAll();
    }

});


        }

        public void EnquePullDayDataTask()
        {
            if (_taskQueue.Count >= Constants.MaxQueueCnt) return;
            _logger.LogInformation("enque pull all stock daily data task.");
            _taskQueue.QueueBackgroundWorkItem(async token =>
{

    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var scopedServices = scope.ServiceProvider;
        var puller = scopedServices.GetRequiredService<DayDataFillerViewModel>();

        await puller.PullAll();
    }

});


        }


        public void EnquepullRealTimeDataTask()
        {
            if (_taskQueue.Count >= Constants.MaxQueueCnt) return;
            _logger.LogInformation("enque pull all stock realtime data task.");
            _taskQueue.QueueBackgroundWorkItem(async token =>
{

    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var scopedServices = scope.ServiceProvider;
        var puller = scopedServices.GetRequiredService<RealTimeDataFillerViewModel>();

        await puller.PullAll();
    }

});


        }


    }
    #endregion
}
