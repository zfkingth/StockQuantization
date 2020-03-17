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
    internal class WrappeHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        TimedService _timedService;
        public WrappeHostedService(ILogger<WrappeHostedService> logger,TimedService timedService)
        {
            _logger = logger;
            _timedService = timedService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timedService.Start(cancellationToken);

            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timedService.Stop(cancellationToken);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timedService.Dispose();
        }
    }
    #endregion
}
