using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Domain;
using Microsoft.Bot.Builder.Dialogs;
using ScenarioBot.Service;

namespace ScenarioBot.BotCommands
{
    public class TopCommand : IBotCommand
    {
        private readonly IUserService _userService;
        private readonly ScenarioService _scenarioService;

        public TopCommand(IUserService userService, ScenarioService scenarioService)
        {
            _userService = userService;
            _scenarioService = scenarioService;
        }
        
        public bool IsApplicable(string message, UserId userId)
        {
            return true;
        }

        public bool Validate(UserId userId)
        {
            return true;
        }

        public async Task<DialogTurnResult> ExecuteAsync(DialogContext dialogContext, UserId userId, CancellationToken cancellationToken)
        {
            await dialogContext.Context.SendActivityAsync($"Top 10", cancellationToken: cancellationToken);

            
            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        public IList<ComponentDialog> GetComponentDialogs()
        {
            return new List<ComponentDialog>();
        }
    }
}