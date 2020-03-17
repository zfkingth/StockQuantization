using System;
using System.Threading;
using System.Threading.Tasks;
using Blog.API.Utils;
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

            setTimerForPullBasicData();

            setTimerForPullRealTimeData();

        }

        private void setTimerForPullBasicData()
        {
            int secondsCnt = _configuration.GetValue<int>("FetchBasicInfoCycle");
            int delaySecond = _configuration.GetValue<int>("BackgroundServiceStartDelay");

            _timer1 = new Timer(DoWork1, null, TimeSpan.FromSeconds(delaySecond),
                TimeSpan.FromSeconds(secondsCnt));
        }

        private void setTimerForPullRealTimeData()
        {
            int secondsCnt = _configuration.GetValue<int>("FetchRealTimeDataCycle");
            int delaySecond = secondsCnt;// _configuration.GetValue<int>("BackgroundServiceStartDelay");
            _timer2 = new Timer(DoWork2, null, TimeSpan.FromSeconds(delaySecond),
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

                setTimerForPullRealTimeData();
            }
        }

        private void DoWork1(object state)
        {
            _logger.LogInformation("Timed Background Service is working. for timer1");

            _util.JudgePullStockNames();
            _util.JudgePullF10();
            _util.JudgePullDailyData();
            Task.Run(async () => await _util.JudgeEraseRealTimeData());


        }


        private void DoWork2(object state)
        {
            _logger.LogInformation("Timed Background Service is working. for timer2");

            Task.Run(async () => await _util.JudgePullRealTimeDataAsync());
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
