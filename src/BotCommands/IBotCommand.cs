using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Domain;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;

namespace CoreBot.BotCommands
{
    public interface IBotCommand
    {
        bool IsApplicable(string message, UserId userId);
        bool Validate(UserId userId);
        Task<DialogTurnResult> ExecuteAsync(DialogContext dialogContext, UserId userId, CancellationToken cancellationToken);

        IList<ComponentDialog> GetComponentDialogs();

    }
}