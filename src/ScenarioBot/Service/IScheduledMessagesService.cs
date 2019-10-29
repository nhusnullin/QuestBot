using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace ScenarioBot.Service
{
    public interface IScheduledMessagesService
    {
        Task SendScheduled(CancellationToken cancellationToken);
        Task Schedule(string messageText, ConversationReference conversationReference, TimeSpan delay);
    }
}