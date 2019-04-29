// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreBot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    public class DialogAndWelcomeBot<T> : DialogBot<T> where T : Dialog
    {
        private readonly IUserService userService;

        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger, IUserService userService)
            : base(conversationState, userState, dialog, logger)
        {
            this.userService = userService;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            userService.InsertOrMerge(new User()
            {
                ChannelId = turnContext.Activity.ChannelId,
                IsCaptain = true,
                Name = turnContext.Activity.From.Name,
                TeamId = turnContext.Activity.From.Id,
                UserId = turnContext.Activity.From.Id
            });
        }
        
    }
}
