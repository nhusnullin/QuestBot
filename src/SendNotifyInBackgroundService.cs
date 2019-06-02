using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain;
using CoreBot;
using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    internal class SendNotifyInBackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<UserId, ConversationReference> _conversationReferences;
        private readonly INotificationMessanger _notificationMessanger;
        private readonly ConcurrentBag<BackgroundNotifyMsg> _backgroundNotifyMsgsStore;
        private Timer _timer;

        private bool isWorking = false;
        private object _lockObj = new object();

        public SendNotifyInBackgroundService(ILogger<SendNotifyInBackgroundService> logger,
            ConcurrentDictionary<UserId, ConversationReference> conversationReferences,
            INotificationMessanger notificationMessanger,
            ConcurrentBag<BackgroundNotifyMsg> backgroundNotifyMsgsStore)
        {
            _logger = logger;
            _conversationReferences = conversationReferences;
            _notificationMessanger = notificationMessanger;
            _backgroundNotifyMsgsStore = backgroundNotifyMsgsStore;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            // лочки все потом
            //return;
//
//            if (isWorking) return;
//
//            isWorking = true;
//            foreach (var notifyMsg in _backgroundNotifyMsgsStore.Where(x=>x.WhenByUTC <= DateTime.UtcNow && !x.WasSend))
//            {
//                TeamUtils.SendTeamMessage(_teamService, _notificationMessanger, notifyMsg.TeamId, notifyMsg.Msg,
//                    _conversationReferences,
//                    CancellationToken.None);
//                notifyMsg.WasSend = true;
//            }
//
//            isWorking = false;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}