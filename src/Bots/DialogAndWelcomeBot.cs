using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain;
using CoreBot.Domain;
using CoreBot.Properties;
using CoreBot.Service;
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
        private ConcurrentDictionary<UserId, ConversationReference> _conversationReferences;
        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger, IUserService userService,
            ConcurrentDictionary<UserId, ConversationReference> conversationReferences)
            : base(conversationState, userState, dialog, logger)
        {
            _userService = userService;
            _conversationReferences = conversationReferences;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            //await TurnContextExtensions.SendMessageAsync(turnContext, Resources.WelcomeMessage, cancellationToken);
            //await RunDialog(turnContext, cancellationToken);
        }

        //private async Task AddConversationReference(ITurnContext turnContext)
        //{
        //    var activity = turnContext.Activity;
        //    var conversationReference = activity.GetConversationReference();
        //    await _userService.AddOrUpdateConversation(turnContext, conversationReference);
        //    _conversationReferences.AddOrUpdate(new Id(activity.ChannelId, conversationReference.User.Id), conversationReference, (key, newValue) => conversationReference);

        //}

        //protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    //await AddConversationReference(turnContext);

        //    await base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        //}

        //protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    //await AddConversationReference(turnContext);

        //    await base.OnMessageActivityAsync(turnContext, cancellationToken);
        //}
    }
}