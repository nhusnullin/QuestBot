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
    }
    
}