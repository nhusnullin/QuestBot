using System.Threading;
using System.Threading.Tasks;
using Core.Domain;
using Microsoft.Bot.Schema;

namespace Core.Service
{
    public interface INotificationService
    {
        Task SendMessage(string message, ConversationReference conversationReference,
            CancellationToken cancellationToken);

        Task SendMessageInBackground(BackgroundNotifyMsg msg);
    }
}