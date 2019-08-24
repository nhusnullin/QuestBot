using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using ScenarioBot.Service;

namespace ScenarioBot
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