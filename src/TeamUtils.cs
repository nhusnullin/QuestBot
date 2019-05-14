using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.Azure.Documents.Client;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot
{
    static class TeamUtils
    {
        public static async Task SendTeamMessage(ITeamService teamService,
            ITurnContext turnContext, string teamId, string message, IDictionary<UserId, ConversationReference> conversationReferences)
        {
            var users = await teamService.GetTeamMembers(teamId);
            var activities = conversationReferences.Where(i => users.Contains(i.Key)).Select(i =>
            {
                var activity = turnContext.Activity.CreateReply(message);
                activity.ApplyConversationReference(i.Value);
                return activity;
            }).ToArray();
            foreach(var activity in activities)
                await turnContext.SendActivityAsync(activity);
        }
    }
}
