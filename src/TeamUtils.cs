using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot
{
    static class TeamUtils
    {
        public static async Task SendTeamMessage(ITeamService teamService,
            ITurnContext turnContext,
            INotificationMessanger messenger, 
            string teamId,
            string message, 
            ConcurrentDictionary<UserId, ConversationReference> conversationReferences,
            CancellationToken cancellationToken,
            bool sendMe = true)
        {
            var users = await teamService.GetTeamMembers(teamId);
            var teamConversations = conversationReferences.ToArray();
            if (sendMe)
                await TurnContextExtensions.SendMessageAsync(turnContext, message, cancellationToken);
            var excludeUserId = new UserId(turnContext.Activity.ChannelId, turnContext.Activity.From.Id);
            foreach (var reference in teamConversations.Where(i => users.Contains(i.Key)))
            {
                if (reference.Key.Equals(excludeUserId))
                    continue;
                await messenger.SendMessage(message, reference.Value, cancellationToken);
            }
        }

        public static void SendTeamMessage(ITeamService teamService,
            INotificationMessanger messenger,
            string teamId,
            string message,
            ConcurrentDictionary<UserId, ConversationReference> conversationReferences,
            CancellationToken cancellationToken
            )
        {
            var users = teamService.GetTeamMembers(teamId).GetAwaiter().GetResult();
            var teamConversations = conversationReferences.ToArray();
            foreach (var reference in teamConversations.Where(i => users.Contains(i.Key)))
            {
                messenger.SendMessage(message, reference.Value, cancellationToken).GetAwaiter().GetResult();
            }
        }
    }
}
