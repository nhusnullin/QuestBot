using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScenarioBot.Service;

namespace ScenarioBot.HostedService
{
    public class BackgroundNotificationsService : BackgroundService
    {
        private readonly ILogger<BackgroundNotificationsService> _logger;
        private readonly IScheduledMessagesService _scheduledMessagesService;
        private static readonly TimeSpan NotificationInterval = TimeSpan.FromSeconds(90);

        public BackgroundNotificationsService(IScheduledMessagesService scheduledMessagesService,
            ILogger<BackgroundNotificationsService> logger)
        {
            _logger = logger;
            _scheduledMessagesService = scheduledMessagesService;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting {nameof(BackgroundNotificationsService)}...");
            try
            {
                while (true)
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    await Task.Delay(NotificationInterval, stoppingToken);
                    try
                    {
                        await _scheduledMessagesService.Send(stoppingToken);
                    }
                    catch (Exception ex) when (ex.GetType() != typeof(OperationCanceledException))
                    {
                        // Если произошла какая-то ошибка, лучше продолжить работу, иначе придется рестартовать приложение.
                        _logger.LogError(ex, "Send notifications error");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Stopping {nameof(BackgroundNotificationsService)}...");
            }
        }
    }
}