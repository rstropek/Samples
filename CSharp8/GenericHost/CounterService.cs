using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GenericHost
{
    public class CounterService : IHostedService, IDisposable
    {
        private Task CounterTask;
        private readonly CancellationTokenSource CancelTokenSource = new CancellationTokenSource();
        private readonly ILogger<CounterService> Logger;
        private readonly IHostApplicationLifetime ApplicationLifetime;
        private readonly int CountUntil;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CancelTokenSource.Dispose();
            }
        }

        public CounterService(ILogger<CounterService> logger, IHostApplicationLifetime applicationLifetime,
            IConfiguration config) =>
            (Logger, ApplicationLifetime, CountUntil) = (logger, applicationLifetime, Int32.Parse(config["CountUntil"]));

        ~CounterService()
        {
            Dispose(false);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting counter task");
            var serviceCancelToken = CancelTokenSource.Token;
            CounterTask = Task.Run(async () =>
            {
                for (int i = 0; i < CountUntil && !serviceCancelToken.IsCancellationRequested; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    Console.WriteLine(i);
                }

                // We are done, request application stop
                ApplicationLifetime.StopApplication();
            }, serviceCancelToken);
            Logger.LogInformation("Counter task started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Sending cancel request to counter task");
            CancelTokenSource.Cancel();

            return CounterTask;
        }
    }
}
