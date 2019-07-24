using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Schema;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain;
using Core.Service;

namespace CoreBot
{
    public class NotificationService : INotificationService
    {
        private readonly IAdapterIntegration _adapter;
        private readonly string _botAppId;
        public NotificationService(string botAppId, IAdapterIntegration adapter)
        {
            _botAppId = botAppId ?? throw new ArgumentNullException(nameof(botAppId));
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        public async Task SendMessage(string message, ConversationReference conversationReference, CancellationToken cancellationToken)
        {
            var teamMessage = new TeamMessage()
            {
                Message = message
            };
            await teamMessage.SendMessage(_adapter, _botAppId, conversationReference, cancellationToken);
        }

        public async Task SendMessageInBackground(BackgroundNotifyMsg msg)
        {
            return;
        }


        class TeamMessage
        {
            public string Message { get; set; }

            public async Task SendMessage(IAdapterIntegration adapter, string botAppId, ConversationReference conversationReference, CancellationToken cancellationToken)
            {
                await adapter.ContinueConversationAsync(botAppId, conversationReference, SendMessageAsync, default(CancellationToken));
            }

            private async Task SendMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
            {
                await turnContext.SendActivityAsync(Message);
            }

        }
    }
}
