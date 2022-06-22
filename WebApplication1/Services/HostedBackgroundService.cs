namespace CCare.Shared.Messaging.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Hosting.WindowsServices;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines methods for objects that are managed by the <see cref="HostedBackgroundService"/>.
    /// </summary>
    public interface IHostedAction
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// A <see cref="BackgroundService"/> implementation that manages
    /// the sequential starting and stopping of a list of <see cref="IHostedAction"/> objects.
    /// If any <see cref="IHostedAction"/> fails to start the application will be stopped.
    /// Inspiration drawn from: https://blog.stephencleary.com/2020/05/backgroundservice-gotcha-startup.html
    /// </summary>
    public class HostedBackgroundService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostLifetime;
        private readonly ILogger<HostedBackgroundService> _logger;
        private readonly IServiceProvider _provider;

        private readonly IEnumerable<IHostedAction> _serviceJobs;

        public HostedBackgroundService(
            IEnumerable<IHostedAction> serviceJobs,
            IHostApplicationLifetime hostLifetime,
            ILogger<HostedBackgroundService> logger,
            IServiceProvider provider)
        {
            _serviceJobs = serviceJobs;
            _hostLifetime = hostLifetime;
            _logger = logger;
            _provider = provider;
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(async () =>
        {
            try
            {
                // Delay to allow host and service to start.
                if (WindowsServiceHelpers.IsWindowsService())
                    await Task.Delay(1000, stoppingToken);
                
                _logger.LogWarning("Service is starting.");

                foreach (var task in _serviceJobs)
                {
                    await task.StartAsync(stoppingToken);
                }

                _logger.LogWarning("Service has started.");
            }
            catch (OperationCanceledException)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    //Environment.ExitCode = ExitCodes.AbnormalExit;

                    _hostLifetime.StopApplication();
                }

                _logger.LogWarning("Service was cancelled.");
            }
            catch (Exception ex)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    //Environment.ExitCode = ExitCodes.AbnormalExit;

                    _hostLifetime.StopApplication();
                }

                _logger.LogWarning(ex, "Service startup failed.");
            }
        }, stoppingToken);

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Wait for ExecuteAsync task to complete.
            await base.StopAsync(cancellationToken);
            
            try
            {
                _logger.LogInformation("Service is stopping.");

                foreach (var task in _serviceJobs.Reverse())
                {
                    await task.StopAsync(cancellationToken);
                }

                _logger.LogWarning("Service is stopped.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Service stop was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Service stop failed.");
            }
        }
    }
}