using System;
using System.Collections.Generic;
using System.Text;
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

        public TopCommand(IUserService userService)
        {
            _userService = userService;
        }

        public bool IsApplicable(string message, UserId userId)
        {
            return message.Equals("top", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Validate(UserId userId)
        {
            return true;
        }

        public async Task<DialogTurnResult> ExecuteAsync(DialogContext dialogContext, UserId userId,
            CancellationToken cancellationToken)
        {
            var userWeights = await _userService.CalcUserWeightsAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Top 10");
            
            foreach (var userWeight in userWeights) sb.AppendLine($"{userWeight.Key} - {userWeight.Value}");

            await dialogContext.Context.SendActivityAsync(sb.ToString(), cancellationToken: cancellationToken);

            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        public IList<ComponentDialog> GetComponentDialogs()
        {
            return new List<ComponentDialog>();
        }
    }
}