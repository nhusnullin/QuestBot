using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Domain;
using Microsoft.Bot.Builder.Dialogs;
using ScenarioBot.Service;

namespace ScenarioBot.BotCommands
{
    public class ScenarioBotCommand : IBotCommand
    {
        private readonly IScenarioService _scenarioService;

        public ScenarioBotCommand(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }
        public bool IsApplicable(string message, UserId userId)
        {
            return message.Equals("scenario", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Validate(UserId userId)
        {
            return true;
        }

        public async Task<DialogTurnResult> ExecuteAsync(DialogContext dialogContext, UserId userId, CancellationToken cancellationToken)
        {
            await dialogContext.Context.SendActivityAsync($"Show all scenarios...", cancellationToken: cancellationToken);
            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        public IList<ComponentDialog> GetComponentDialogs()
        {
            return new List<ComponentDialog>(0);
        }
    }
}