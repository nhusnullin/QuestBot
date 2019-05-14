using CoreBot.Domain;
using CoreBot.Exceptions;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CoreBot
{
    static class UserServiceExtensions
    {
        public static async Task<User> GetOrCreateUser(this IUserService userService, ITurnContext context)
        {
            var user = await userService.GetByAsync(context.Activity.ChannelId, context.Activity.From.Id);
            if (user == null)
            {
                user = new User(context.Activity.ChannelId, context.Activity.From.Id)
                {
                    Name = GetUserName(context.Activity.From),
                    ChannelData = context.Activity.ChannelData != null ? context.Activity.ChannelData.ToString() : string.Empty,
                    ConversationData = GetConversationData(context.Activity.GetConversationReference())
                };
                await userService.InsertOrMergeAsync(user);
            }
            return user;
        }

        public static async Task<User> AddOrUpdateConversation(this IUserService userService, ITurnContext context, ConversationReference conversationReference)
        {
            var user = await userService.GetByAsync(context.Activity.ChannelId, context.Activity.From.Id);
            if (user == null)
            {
                user = new User(context.Activity.ChannelId, context.Activity.From.Id)
                {
                    Name = GetUserName(context.Activity.From),
                    ChannelData = context.Activity.ChannelData != null ? context.Activity.ChannelData.ToString() : string.Empty
                };
            }
            user.ConversationData = GetConversationData(conversationReference);
            await userService.InsertOrMergeAsync(user);
            return user;
        }

        private static string GetConversationData(ConversationReference conversationReference)
        {
            return JsonConvert.SerializeObject(conversationReference);
        }

        public static async Task<User> ValidateUser(this IUserService userService, UserId userId)
        {
            var user = await userService.GetByAsync(userId.ChannelId, userId.Id);
            if (user == null)
                throw new UserNotFoundException(userId);
            return user;
        }

        private static string GetUserName(ChannelAccount account)
        {
            var result = account.Name;
            var firstName = account.Properties["first_name"]?.ToString();
            var lastName = account.Properties["last_name"]?.ToString();
            var fullName = firstName;
            if (String.IsNullOrEmpty(fullName) && !String.IsNullOrEmpty(lastName))
                fullName += " ";
            fullName += lastName;
            if (!String.IsNullOrEmpty(fullName))
                if (String.IsNullOrEmpty(result))
                    result = fullName;
                else
                    result += " (" + fullName + ")";
            return result;

        }
    }
}
