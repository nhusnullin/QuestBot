using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Logging;

namespace CoreBot.Bots
{
    public class DialogAndWelcomeBot<T> : DialogBot<T> where T : Dialog
    {
        private readonly IUserService _userService;

        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger, IUserService userService)
            : base(conversationState, userState, dialog, logger)
        {
            _userService = userService;
        }

        protected override Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            //            await _userService.InsertOrMergeAsync(new User(turnContext.Activity.ChannelId, turnContext.Activity.From.Id)
            //            {
            //                ChannelData = turnContext.Activity.ChannelData?.ToString()
            //            });
            return Task.CompletedTask;
        }
    }
}