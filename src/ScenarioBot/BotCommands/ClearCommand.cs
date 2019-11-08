using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Domain;
using Microsoft.Bot.Builder.Dialogs;
using ScenarioBot.Dialogs;
using ScenarioBot.Service;

namespace ScenarioBot.BotCommands
{
    public class ClearCommand : IBotCommand
    {
        private readonly IUserService _userService;
        private const string CommandPrefix = "clear";

        public ClearCommand(IUserService userService)
        {
            _userService = userService;
        }

        public bool IsApplicable(string message, UserId userId)
        {
            var today = DateTime.UtcNow;
            return message.Equals(CommandPrefix, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Validate(UserId userId)
        {
            return true;
        }

        public async Task<DialogTurnResult> ExecuteAsync(DialogContext dialogContext, UserId userId,
            CancellationToken cancellationToken)
        {
            _userService.RemoveUserAnswers(userId);
            await dialogContext.Context.SendActivityAsync("done", cancellationToken: cancellationToken);
            await dialogContext.CancelAllDialogsAsync(cancellationToken);
            return await dialogContext.BeginDialogAsync(nameof(ScenarioListDialog), userId, cancellationToken);
        }

        public IList<ComponentDialog> GetComponentDialogs()
        {
            return new List<ComponentDialog>();
        }
    }
}