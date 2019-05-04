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

        public User(string channelId, string userId, string name)
        {
            ChannelId = channelId;
            UserId = userId;
            
            PartitionKey = channelId;
            RowKey = userId;

            Name = name;
            TeamId = userId;
            IsCaptain = false;
        }

        public string UserId { get; set; }
        public string ChannelId { get; set; }

        public string Name { get; set; }

        public string TeamId { get; set; }

        public bool IsCaptain { get; set; }
    }
    
}