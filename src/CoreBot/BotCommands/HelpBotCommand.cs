using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Domain;
using Microsoft.Bot.Builder.Dialogs;

namespace CoreBot.BotCommands
{
    public class HelpBotCommand : IBotCommand
    {
        public bool IsApplicable(string message, UserId userId)
        {
            return message.Equals("help", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Validate(UserId userId)
        {
            return true;
        }

        public async Task<DialogTurnResult> ExecuteAsync(DialogContext dialogContext, UserId userId,
            CancellationToken cancellationToken)
        {
            await dialogContext.Context.SendActivityAsync("Чат сапорта квеста https://t.me/AlfaHeisenbug", cancellationToken: cancellationToken);
            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        public IList<ComponentDialog> GetComponentDialogs()
        {
            return new List<ComponentDialog>(0);
        }
    }
}