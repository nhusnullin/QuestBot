// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    public class DialogAndWelcomeBot<T> : DialogBot<T> where T : Dialog
    {
        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
            : base(conversationState, userState, dialog, logger)
        {
        }

        //public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        //{
        //    //await CreateImageAttachment(turnContext, cancellationToken);
        //}

        //protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        //{
            
        //    foreach (var member in membersAdded)
        //    {
        //        // Greet anyone that was not the target (recipient) of this message.
        //        // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
        //        if (member.Id != turnContext.Activity.Recipient.Id)
        //        {
        //            var welcomeCard = CreateAdaptiveCardAttachment("welcomeCard.json");
        //            var response = CreateResponse(turnContext.Activity, welcomeCard);
        //            await turnContext.SendActivityAsync(response, cancellationToken);
        //        }
        //    }

        //    var welcomeIntroCard = CreateAdaptiveCardAttachment("welcomeIntroCard.json");
        //    var responseIntro = CreateResponse(turnContext.Activity, welcomeIntroCard);
        //    await turnContext.SendActivityAsync(responseIntro, cancellationToken);
        //}

        // Create an attachment message response.
        private Activity CreateResponse(IActivity activity, Attachment attachment)
        {
            var response = ((Activity)activity).CreateReply();
            response.Attachments = new List<Attachment>() { attachment };
            return response;
        }

        public async Task CreateImageAttachment(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = turnContext.Activity.CreateReply();

            // Create an attachment.
            var attachment = new Attachment
            {
                ContentUrl = "https://images.ctfassets.net/9n3x4rtjlya6/VKpwBdY6CyiM6qowQuEM/a0808abadeec240b989cd7fc8eca285e/_______________________.png?w=240",
                ContentType = "image/png",
                Name = "imageName",
            };

            // Add the attachment to our reply.
            reply.Attachments = new List<Attachment>() { attachment };

            // Send the activity to the user.
            await turnContext.SendActivityAsync(reply, cancellationToken: cancellationToken);
        }

        // Load attachment from file.
        public static Attachment CreateAdaptiveCardAttachment(string cardName)
        {
            // combine path for cross platform support
            string[] paths = { ".", "Cards", cardName };
            string fullPath = Path.Combine(paths);
            var adaptiveCard = File.ReadAllText(fullPath);
            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCard),
            };
        }
    }
}
