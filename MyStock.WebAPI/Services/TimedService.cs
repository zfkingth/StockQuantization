using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundTasksSample.Services
{
    #region snippet1
    public class TimedService : IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer1;
        private Timer _timer2;
        BackServiceUtil _util;
        IConfiguration _configuration;
        public TimedService(BackServiceUtil util, ILogger<TimedService> logger, IConfiguration configuration)
        {
            _util = util;
            _logger = logger;
            _configuration = configuration;
        }

        public void Start(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");
            System.Diagnostics.Debug.WriteLine("Timed Background Service is starting.");

            setTimerForLongPeriod();

            setTimerForShortPeriod();

        }

        private void setTimerForLongPeriod()
        {
            int secondsCnt = _configuration.GetValue<int>("LongPeriodCycle");
            int delaySecond = _configuration.GetValue<int>("BackgroundServiceStartDelay");

            _timer1 = new Timer(DoWorkForLongPeriod, null, TimeSpan.FromSeconds(delaySecond),
                TimeSpan.FromSeconds(secondsCnt));
        }

        private void setTimerForShortPeriod()
        {
            int secondsCnt = _configuration.GetValue<int>("ShortPeriodCycle");
            int delaySecond = secondsCnt;// _configuration.GetValue<int>("BackgroundServiceStartDelay");
            _timer2 = new Timer(DoWorkForShortPeriod, null, TimeSpan.FromSeconds(delaySecond),
          TimeSpan.FromSeconds(secondsCnt));
        }

        private static readonly object locker = new object();
        public void ResetTimerForPullRealTimeData()
        {
            if (_timer1 == null) throw new Exception("timed service not start");
            lock (locker)
            {
                _timer2.Change(Timeout.Infinite, Timeout.Infinite);
                _timer2.Dispose();

                setTimerForShortPeriod();
            }
        }

        private void DoWorkForLongPeriod(object state)
        {
            _logger.LogInformation("Timed Background Service is working. for long period");

            execLog(_util.JudgePullStockNames);
            execLog(_util.JudgePullF10);
            execLog(_util.JudgePullDailyData);
            execLog(() => _util.JudgeEraseRealTimeData().Wait());
          

        }

        /// <summary>
        /// 执行函数体，不让抛出异常影响其他方法的执行
        /// </summary>
        /// <param name="action"></param>
        private void execLog(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

        }


        private void DoWorkForShortPeriod(object state)
        {
            _logger.LogInformation("Timed Background Service is working. for short period");


            execLog(() => _util.JudgePullRealTimeDataAsync().Wait());
            execLog(() => _util.JudgepullHuShenTongTAsync());

            //有具体的判断，这个是在开盘前
            execLog(() => _util.JudgePullMarginDataAsync().Wait());

            //这个是在收盘后
            execLog(() => _util.JudgePullMarketDealDataAsync().Wait());



        }


        public void Stop(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer1?.Change(Timeout.Infinite, 0);
            _timer2?.Change(Timeout.Infinite, 0);

        }

        public void Dispose()
        {
            _timer1?.Dispose();
            _timer2?.Dispose();
        }
    }
    #endregion
}
