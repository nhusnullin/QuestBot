// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreBot;
using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class LoadScenarioService : IHostedService
    {
        private readonly IScenarioService _scenarioService;

        public LoadScenarioService(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _scenarioService.LoadAll();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    internal class SendNotifyInBackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ITeamService _teamService;
        private readonly ConcurrentDictionary<UserId, ConversationReference> _conversationReferences;
        private readonly INotificationMessanger _notificationMessanger;
        private readonly ConcurrentBag<BackgroundNotifyMsg> _backgroundNotifyMsgsStore;
        private Timer _timer;

        private bool isWorking = false;
        private object _lockObj = new object();

        public SendNotifyInBackgroundService(ILogger<SendNotifyInBackgroundService> logger,
            ITeamService teamService,
            ConcurrentDictionary<UserId, ConversationReference> conversationReferences,
            INotificationMessanger notificationMessanger,
            ConcurrentBag<BackgroundNotifyMsg> backgroundNotifyMsgsStore)
        {
            _logger = logger;
            _teamService = teamService;
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

            if (isWorking) return;

            isWorking = true;
            foreach (var notifyMsg in _backgroundNotifyMsgsStore.Where(x=>x.WhenByUTC <= DateTime.UtcNow && !x.WasSend))
            {
                TeamUtils.SendTeamMessage(_teamService, _notificationMessanger, notifyMsg.TeamId, notifyMsg.Msg,
                    _conversationReferences,
                    CancellationToken.None);
                notifyMsg.WasSend = true;
            }

            isWorking = false;
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