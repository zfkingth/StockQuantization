using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Stock.Data;
using Stock.JQData;
using Stock.Model;
using Stock.WebAPI.Notifications;
using Stock.WebAPI.Notifications.Models;

namespace Stock.WebAPI.ViewModels.Fillers
{
    public class BaseDoWorkViewModel : ViewModelBase
    {
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly string _userId;
        protected readonly IConfiguration _configuration;
        public BaseDoWorkViewModel(IServiceScopeFactory serviceScopeFactory,
            string userId = null, IConfiguration configuration = null) : base()
        {
            _configuration = configuration;
            _userId = userId;
            _serviceScopeFactory = serviceScopeFactory;
        }





        static int _maxThreadNum = 10;
        /// <summary>
        /// 最大线程数
        /// </summary>
        public static int MaxThreadNum
        {
            get
            {
                return _maxThreadNum;
            }
            set
            {
                if (_maxThreadNum != value)
                {
                    _maxThreadNum = value;
                }
            }
        }

        private static DateTime baseDate = DateTime.MaxValue;


        protected class StockArgs : EventArgs
        {

            public Securities Stock { get; set; }
        }

        protected CancellationTokenSource cts = new CancellationTokenSource();

        public void Cancel()
        {
            cts.Cancel();
            this.IsRunning = false;
        }

        private float _progress;
        /// <summary>
        /// 运行的进度
        /// </summary>
        public float Progress
        {
            get { return _progress; }
            protected set
            {
                if (_progress != value)
                {
                    _progress = value;
                    RaisePropertyChanged("Progress");
                }
            }
        }

        private bool _isRunning = false;

        /// <summary>
        /// 表明是否在运行
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
            protected set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    RaisePropertyChanged("IsRunning");
                }
            }
        }


        /// <summary>
        /// 获取需要处理的股票或者指数在代码,默认只处理股票
        /// </summary>
        /// <returns></returns>
        protected virtual List<Securities> GetSecList()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();


                var list = (from i in db.SecuritiesSet
                            where i.Type == SecuritiesEnum.Stock
                            select i).AsNoTracking().ToList();
                return list;
            }
        }



        protected Func<StockArgs, Task> stockHandle;


        /// <summary>
        /// 相当于整个Fill 过程的架构,通过stockHandle回调来做具体的事
        /// </summary>
        /// <param name="task"></param>
        protected void DoWork()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<StockContext>();

                    var list = GetSecList();
                    //定义线程取消的一个对象

                    int progressCnt = 0;

                    int cnt = list.Count;
                    Progress = 0;

                    IsRunning = true;


                    var po = new ParallelOptions()
                    {
                        CancellationToken = cts.Token,
                        MaxDegreeOfParallelism = MaxThreadNum,
                    };


                    Parallel.ForEach(list, po, (stock) =>
                   {
                       po.CancellationToken.ThrowIfCancellationRequested();
                       //using (var asyncBridge = AsyncHelper.Wait)
                       //{
                       //    asyncBridge.Run(stockHandle(new StockArgs() { StockId = stockId }));
                       //}

                       stockHandle(new StockArgs() { Stock = stock }).Wait();

                       Interlocked.Increment(ref progressCnt);
                       //增加1%才更新
                       float progress = (float)progressCnt / cnt;
                       if (progress - 0.025f >= Progress || progress == 1)
                       {
                           Progress = progress;
                       }

                   });
                }
            }
            finally
            {
                IsRunning = false;
            }


        }


        protected async Task setStartDate(string eventName)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var record = await db.StockEvents.FirstOrDefaultAsync(s => s.EventName == eventName);

                record.LastAriseStartDate = DateTime.Now;
                record.Status = EventStatusEnum.Running;
                await db.SaveChangesAsync();

            }
        }

        protected async Task setFinishedDate(string eventName)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var record = await db.StockEvents.FirstOrDefaultAsync(s => s.EventName == eventName);

                record.LastAriseEndDate = DateTime.Now;
                record.Status = EventStatusEnum.Idle;
                await db.SaveChangesAsync();

            }
        }


        #region search clause

        /// <summary>
        /// 全局唯一，存储任务状态。
        /// </summary>
        protected static ConcurrentDictionary<string, StockTaskStatus> _stockTaskStatusDic =
            new ConcurrentDictionary<string, StockTaskStatus>();

        public static int GetTaskNum()
        {
            return _stockTaskStatusDic.Count();
        }
        /// <summary>
        /// 只有任务成功开启，才能修改task status.
        /// </summary>
        StockTaskStatus _taskStatus;

        protected async Task<bool> isFetchingRealTimeData()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                var stockEvent = await db.StockEvents.FindAsync(SystemEvents.EventPullReadTimeData);
                if (stockEvent.Status == EventStatusEnum.Running)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        protected void StockTaskStart()
        {
            string userId = _userId;

            //if (await this.isFetchingRealTimeData())
            //{
            //    throw new Exception("服务器后台正在获取实时数据，请等会再尝试当前操作");
            //}

            StockTaskStatus oldStatus;
            if (_stockTaskStatusDic.TryGetValue(userId, out oldStatus))
            {
                int duration = _configuration.GetValue<int>("MaximumTaskDuration");
                if (DateTime.Now.Subtract(oldStatus.StartTime).TotalSeconds > duration)
                {
                    //有任务超时了，但是没有被清除
                    _stockTaskStatusDic.TryRemove(userId, out oldStatus);
                }
            }

            var tempSts = new StockTaskStatus()
            {
                Id = Guid.NewGuid(),
                StartTime = DateTime.Now,
            };

            if (_stockTaskStatusDic.TryAdd(userId, tempSts) == false)
            {
                //throw new Exception("has another task running");
                throw new Exception("任务失败，因为当前用户已经有一个任务在服务端上执行");
            }
            else
            {
                //成功了才能更新_taskStatus

                _taskStatus = tempSts;
                notifyTaskStart(userId, _taskStatus);
            }


            System.Diagnostics.Debug.WriteLine($"_stock task is starting , add status {_taskStatus} ");

        }

        private void notifyTaskStart(string userId, StockTaskStatus sts)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var hubContext = scopedServices.GetRequiredService<IHubContext<NotificationsHub>>();



                hubContext.Clients.User(userId).SendAsync(
                      "notification",
                      new Notification<TaskStartPayload>
                      {
                          NotificationType = NotificationType.TaskStart,
                          Payload = new TaskStartPayload
                          {
                              Status = sts,
                          }
                      }
                  );
            }
        }

        protected virtual void StockTaskReportProgress(object state)
        {
            //如果找不到对应的item ，表示任务已经完成。
            //但是有另外 一种情况，本任务已完成，其它http request又开启了新的task,这种情况要比较taskId

            _maxTimerDoCnt--;
            if (_maxTimerDoCnt <= 0)
            {
                //退出并发执行
                cts.Cancel();
            }
            //执行通知操作。
            string userId = _userId;
            StockTaskStatus sts;



            if (_stockTaskStatusDic.TryGetValue(userId, out sts))
            {
                //有可能任务已经完成，并从列表中删除
                //然后用户的新请求，又加入了另外一个task，id不同
                if (_taskStatus?.Id == sts?.Id)
                {

                    System.Diagnostics.Debug.WriteLine($"_stock task is reporting  ");
                    notifyTaskProgress(userId, sts);


                }
            }

        }

        private void notifyTaskProgress(string userId, StockTaskStatus sts)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var hubContext = scopedServices.GetRequiredService<IHubContext<NotificationsHub>>();

                int progress = 100 * _progressedCnt / _allItemsCnt;

                hubContext.Clients.User(userId).SendAsync(
                      "notification",
                      new Notification<TaskProgressPayload>
                      {
                          NotificationType = NotificationType.TaskProgress,
                          Payload = new TaskProgressPayload
                          {
                              Progress = progress,
                          }
                      }
                  );
            }
        }

        protected virtual void StockTaskSuccess(List<TempPrice> retList)
        {

            string userId = _userId;
            StockTaskStatus sts;
            _stockTaskStatusDic.TryGetValue(userId, out sts);
            notifyTaskSuccess(userId, sts, retList);

            //只能删除本任务对应的status
            if (_taskStatus?.Id == sts?.Id)
            {
                _stockTaskStatusDic.TryRemove(userId, out sts);
            }
            System.Diagnostics.Debug.WriteLine($"_stock task is completed , add status {sts} ");


        }

        private void notifyTaskSuccess(string userId, StockTaskStatus sts, List<TempPrice> retList)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var hubContext = scopedServices.GetRequiredService<IHubContext<NotificationsHub>>();


                hubContext.Clients.User(userId).SendAsync(
                      "notification",
                      new Notification<TaskSuccessPayload>
                      {
                          NotificationType = NotificationType.TaskSuccess,
                          Payload = new TaskSuccessPayload
                          {
                              Message = "task success",
                              ResultList = retList,

                          }
                      }
                  );
            }
        }

        protected virtual void StockTaskFail(Exception ae, List<TempPrice> retList)
        {
            string userId = _userId;
            StockTaskStatus sts;
            if (_stockTaskStatusDic.TryGetValue(userId, out sts))
            {


                System.Diagnostics.Debug.WriteLine($"_stock task is fail ,  {ae.Message} ");

                //只能删除本任务对应的status
                if (_taskStatus?.Id == sts?.Id)
                {
                    _stockTaskStatusDic.TryRemove(userId, out sts);
                }
            }
            //有可能还没有分配sts就失败了。
            notifyTaskFail(userId, sts, ae.Message, retList);

        }

        private void notifyTaskFail(string userId, StockTaskStatus sts, string message, List<TempPrice> retList)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var hubContext = scopedServices.GetRequiredService<IHubContext<NotificationsHub>>();


                hubContext.Clients.User(userId).SendAsync(
                      "notification",
                      new Notification<TaskFailPayload>
                      {
                          NotificationType = NotificationType.TaskFail,
                          Payload = new TaskFailPayload
                          {
                              Message = message,
                              ResultList = retList,

                          }
                      }
                  );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list">需要处理的股票代码列表</param>
        /// <param name="handle">处理函数</param>
        /// <param name="retList">返回结果</param>
        private void ProcessDataInParallel(List<string> list,
            Func<string, Task<TempPrice>> handle,
              List<TempPrice> retList)
        {
            // Use ConcurrentQueue to enable safe enqueueing from multiple threads.
            var exceptions = new ConcurrentQueue<Exception>();
            object locker = new object();
            var po = new ParallelOptions()
            {
                CancellationToken = cts.Token,
                MaxDegreeOfParallelism = MaxThreadNum,
            };


            // Execute the complete loop and capture all exceptions.
            Parallel.ForEach(list, po, (item) =>
        {
            try
            {
                po.CancellationToken.ThrowIfCancellationRequested();

                TempPrice real = null;
                //using (var asyncBridge = AsyncHelper.Wait)
                //{
                //    asyncBridge.Run(handle(item), (callArg) => real = callArg);
                //}

                real = handle(item).Result;

                Interlocked.Increment(ref _progressedCnt);

                if (real != null)
                {
                    lock (locker)
                    {
                        retList.Add(real);
                    }
                }
            }
            // Store the exception and continue with the loop.                    
            catch (Exception e)
            {
                exceptions.Enqueue(e);
                if (exceptions.Count >= 3)
                {
                    var nex = new Exception("too many exception in Parallel.ForEach clause");
                    exceptions.Enqueue(nex);
                    throw new AggregateException(exceptions); //未捕获的异常会导致Parallel.ForEach退出。

                }
            }
        });

            // Throw the exceptions here after the loop completes.
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }


        #region result cache


        protected DateTime _taskStartTime;
        protected string _argString;
        protected string _actionName;
        protected bool _useCahce = false;
        protected bool _needStoreInCache = false;
        protected virtual async Task<List<TempPrice>> GetResultFromCache()
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();
                DateTime time = await getIndexTradeDateInRealData(db);
                if (time == default)
                {
                    time = await getLastTradeDateInHistoryData(db);
                }

                string item = await (
                    from s in db.SearchResultSet
                    where s.ActionName == _actionName &&
                    s.ActionParams == _argString &&
                    s.ActionDate > time
                    orderby s.ActionDate descending
                    select s.ActionReslut).AsNoTracking().FirstOrDefaultAsync();
                if (item == default)
                {
                    _useCahce = false;
                    return null;
                }
                else
                {
                    var list = JsonConvert.DeserializeObject<List<TempPrice>>(item);
                    _useCahce = true;
                    return list;
                }
            }
        }

        private async Task<DateTime> getLastTradeDateInHistoryData(StockContext db)
        {
            var date = await (from i in db.PriceSet
                              where i.Unit == UnitEnum.Unit1d
                              orderby i.Date descending
                              select i.Date
                        ).FirstOrDefaultAsync();

            return date;
        }

        private async Task<DateTime> getIndexTradeDateInRealData(StockContext db)
        {
            DateTime date = await (from i in db.TempPrice
                                   where i.Unit == UnitEnum.Unit1d &&
                                   i.Code == Constants.IndexShangHai
                                   orderby i.Date descending
                                   select i.Date).FirstOrDefaultAsync();

            return date;
        }


        protected async Task SaveResultToCache(List<TempPrice> list)
        {

            if (_useCahce == false && _needStoreInCache && _actionName != null)
            {//在缓存中找不到才会插入这个,
                //只有从所有股票中的操作结果才会缓存
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<StockContext>();


                    SearchResult item = new SearchResult()
                    {
                        ActionName = _actionName,
                        ActionParams = _argString,
                        ActionDate = _taskStartTime,
                    };

                    db.SearchResultSet.Add(item);


                    item.ActionReslut = JsonConvert.SerializeObject(list);

                    await db.SaveChangesAsync();
                }
            }

        }

        #endregion

        /// <summary>
        /// 总个数
        /// </summary>
        private int _allItemsCnt = 0;
        /// <summary>
        /// 已经处理的个数
        /// </summary>
        private int _progressedCnt = 0;
        protected async Task<List<TempPrice>> DoSearch(List<string> list, Func<string, Task<TempPrice>> handle)
        {
            List<TempPrice> retList = new List<TempPrice>();


            _progressedCnt = 0;
            _allItemsCnt = list.Count;

            IsRunning = true;

            Timer timer = null;

            bool taskIsSuccess = false;
            try
            {
                var cacheResult = await GetResultFromCache();
                if (cacheResult == null)
                {
                    //缓存中没有的话，执行筛选操作，有的话直接利用结果。
                    StockTaskStart(); //启动失败会抛出异常，这样被捕捉后，可以进行下一步的处理。

                    timer = setTimer();

                    ProcessDataInParallel(list, handle, retList); //可能在处理过程中抛出一些异常。
                }
                else
                {
                    retList = cacheResult;
                }

                taskIsSuccess = true;
            }
            catch (Exception ae)
            {
                StockTaskFail(ae, retList);   //失败情况下的处理
            }
            finally
            {
                IsRunning = false;
                timer?.Change(Timeout.Infinite, 0);
                if (taskIsSuccess)
                {
                    StockTaskSuccess(retList); //成功情况下的处理。

                    await SaveResultToCache(retList);
                }
            }



            return retList;
        }

        int _maxTimerDoCnt = 0;
        private Timer setTimer()
        {

            int duration = _configuration.GetValue<int>("MaximumTaskDuration");
            int interval = _configuration.GetValue<int>("TaskProgressReportInterval");

            _maxTimerDoCnt = duration / interval;

            return new Timer(StockTaskReportProgress, null, TimeSpan.FromSeconds(interval),
            TimeSpan.FromSeconds(interval));
        }

        public virtual async Task<List<TempPrice>> Search()
        {
            throw new Exception("Base Class not impletation");
        }


        #endregion

    }
}
