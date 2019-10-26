using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string TopCommandPrefix = "top";
        private const int DefaultUserCount = 10;
        private const int MaxAllowedUserCountLength = 6;

        public TopCommand(IUserService userService)
        {
            _userService = userService;
        }

        public bool IsApplicable(string message, UserId userId)
        {
            message = message.Trim();
            return message.StartsWith(TopCommandPrefix, StringComparison.InvariantCultureIgnoreCase) &&
                   message.Length < TopCommandPrefix.Length + MaxAllowedUserCountLength &&
                   message.Substring(TopCommandPrefix.Length).All(Char.IsDigit);
        }

        public bool Validate(UserId userId)
        {
            return true;
        }

        public async Task<DialogTurnResult> ExecuteAsync(DialogContext dialogContext, UserId userId,
            CancellationToken cancellationToken)
        {
            var userCount = GetUserCount();
            var userWeights = await _userService.CalcUserWeightsAsync(userCount);

            var sb = new StringBuilder();
            sb.Append($"Top {userCount} \r\n");
            
            foreach (var userWeight in userWeights) sb.Append($"{userWeight.Key} - {userWeight.Value} \r\n");

            await dialogContext.Context.SendActivityAsync(sb.ToString(), cancellationToken: cancellationToken);

            return new DialogTurnResult(DialogTurnStatus.Waiting);

            int GetUserCount()
            {
                var userCountString = dialogContext.Context
                    .Activity
                    .Text?.Substring(TopCommandPrefix.Length);
            
                return string.IsNullOrWhiteSpace(userCountString) 
                    ? DefaultUserCount
                    : Convert.ToInt32(userCountString);
            }
        }

        public IList<ComponentDialog> GetComponentDialogs()
        {
            return new List<ComponentDialog>();
        }
    }
}