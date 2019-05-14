// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using CoreBot;
using Microsoft.Extensions.Hosting;

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