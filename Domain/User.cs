using Microsoft.Azure.Cosmos.Table;

namespace CoreBot
{
    /// <summary>
    /// Данные пользователя для хранения в БД 
    /// </summary>
    public class User: TableEntity
    {
        public static string TableName = "users";

        public User()
        {

        }

        public User(string channelId, string userId)
        {
            ChannelId = channelId;
            UserId = userId;
            
            PartitionKey = channelId;
            RowKey = userId;

            TeamId = userId;
            IsCaptain = false;
        }

        public string UserId { get;  }
        public string ChannelId { get; }

        public string TeamId { get; set; }

        public bool IsCaptain { get; set; }

        public string ChannelData { get; set; }
    }
    
}