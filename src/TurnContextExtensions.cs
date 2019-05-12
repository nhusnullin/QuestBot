using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace CoreBot
{
    static class TurnContextExtensions
    {
        public static async Task<ResourceResponse> SendMessageAsync(ITurnContext turnContext, string message, CancellationToken cancellationToken)
        {
            if (turnContext == null)
            {
                throw new System.ArgumentNullException(nameof(turnContext));
            }

            var activity = MessageFactory.Text(message, null, InputHints.IgnoringInput);
            return await turnContext.SendActivityAsync(activity, cancellationToken);
        }
    }
}
