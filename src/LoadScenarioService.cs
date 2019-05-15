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

}