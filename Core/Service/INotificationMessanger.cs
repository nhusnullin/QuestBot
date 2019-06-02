using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Core.Service
{
    public interface INotificationMessanger
    {
        Task SendMessage(string message, ConversationReference conversationReference, CancellationToken cancellationToken);
    }
}