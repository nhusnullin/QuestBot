using System;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Domain
{
    /// <summary>
    /// Данные пользователя для хранения в БД 
    /// </summary>
    public class User
    {
        public User()
        {

        }

        public User(Activity activity)
        {
            UserId = new UserId(activity);
            Name = GetUserName(activity.From, activity.ChannelData);
            ChannelData = activity.ChannelData != null ? activity.ChannelData.ToString() : string.Empty;
            ConversationData = GetConversationData(activity.GetConversationReference());
        }
        
        public User(string channelId, string id)
        {
            UserId = new UserId(channelId, id);
        }

        
        public UserId UserId { get; set; }
        public string Id => UserId.Id;
        public string ChannelId => UserId.ChannelId;
        public string Name { get; set; }
        public string ChannelData { get; set; }
        public string ConversationData { get; set; }
        
        private static string GetUserName(ChannelAccount account, object channelData)
        {
            var result = account.Name;
            var (firstName, lastName) = GetPersonInfo(channelData);
            var fullName = firstName;
            if (!string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(lastName))
            {
                fullName += " ";
            }
            
            fullName += lastName;
            
            if (string.IsNullOrEmpty(fullName))
            {
                return result;
            }
            
            if (string.IsNullOrEmpty(result))
            {
                result = fullName;
            }
            else
            {
                result += " (" + fullName + ")";
            }
            return result;
        }
        
        private static (string firstName, string lastName) GetPersonInfo(object channelData)
        {
            (string firstName, string lastName) result = (null, null);
            try
            {
                if (channelData == null)
                {
                    return result;
                }
                
                var parseChannelData = JObject.Parse(channelData.ToString());
                result.firstName = parseChannelData?.SelectToken("message.from.first_name")?.Value<string>();
                result.lastName = parseChannelData?.SelectToken("message.from.last_name")?.Value<string>();
                return result;
            }
            catch(Exception)
            {
                return result;
            }
        }
        
        private static string GetConversationData(ConversationReference conversationReference)
        {
            return JsonConvert.SerializeObject(conversationReference);
        }
    }
    
}