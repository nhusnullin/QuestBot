namespace Core.Domain
{
    /// <summary>
    /// Данные пользователя для хранения в БД 
    /// </summary>
    public class User
    {
        public const string EntityName = "users";

        public User()
        {

        }

        public User(string channelId, string id)
        {
            ChannelId = channelId;
            Id = id;
            UserId = new UserId(channelId, id);
        }

        public UserId UserId { get; set; }

        public string Id { get; set; }
        public string ChannelId { get; set; }
        public string Name { get; set; }


        public string ChannelData { get; set; }
        public string ConversationData { get; set; }

        public  string GetStorageKey()
        {
            return $"{UserId.ChannelId}/users/{UserId.Id}";
        }

        public static string GetStorageKey(UserId userId)
        {
            return $"{userId.ChannelId}/{EntityName}/{userId.Id}";
        }
    }
    
}