using Microsoft.Azure.Cosmos.Table;

namespace CoreBot
{

    /// <summary>
    /// Данные пользователя для хранения в БД 
    /// </summary>
    public class User: TableEntity
    {
        public const string TableName = "users";

        public User()
        {

        }

        public User(string channelId, string userId)
        {
            ChannelId = channelId;
            UserId = userId;
            PartitionKey = channelId;
            RowKey = userId;        
            IsCaptain = false;
        }

        public string UserId { get; set; }
        public string ChannelId { get; set; }
        public string Name { get; set; }
        public string TeamId { get; set; }

        public bool IsCaptain { get; set; }

        public string ChannelData { get; set; }
        public string ConversationData { get; set; }
    }
    
}