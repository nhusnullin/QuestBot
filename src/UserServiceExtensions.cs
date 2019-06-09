using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Core.Domain;
using ScenarioBot.Service;

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
                    Name = GetUserName(context.Activity.From, context.Activity.ChannelData),
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
                    Name = GetUserName(context.Activity.From, context.Activity.ChannelData),
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

//        public static async Task<User> ValidateUser(this IUserService userService, UserId userId)
//        {
//            var user = await userService.GetByAsync(userId.ChannelId, userId.Id);
//            if (user == null)
//                throw new UserNotFoundException(userId);
//            return user;
//        }

        private static string GetUserName(ChannelAccount account, object channelData)
        {
            var result = account.Name;
            var personInfo = GetPersonInfo(channelData);
            var fullName = personInfo.firstName;
            if (!String.IsNullOrEmpty(fullName) && !String.IsNullOrEmpty(personInfo.lastName))
                fullName += " ";
            fullName += personInfo.lastName;
            if (!String.IsNullOrEmpty(fullName))
                if (String.IsNullOrEmpty(result))
                    result = fullName;
                else
                    result += " (" + fullName + ")";
            return result;

        }

        private static (string firstName, string lastName) GetPersonInfo(object channelData)
        {
            (string firstName, string lastName) result = (null, null);
            try
            {
                if (channelData == null)
                    return result;
                var parseChannelData = JObject.Parse(channelData.ToString());
                result.firstName = parseChannelData != null ? parseChannelData.SelectToken("message.from.first_name")?.Value<string>() : null;
                result.lastName = parseChannelData != null ? parseChannelData.SelectToken("message.from.last_name")?.Value<string>() : null;
                return result;
            }
            catch(Exception)
            {
                return result;
            }
        }
    }
}
