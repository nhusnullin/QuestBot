using Microsoft.Azure.Cosmos.Table;
using System;

namespace CoreBot
{
    public class Answer : TableEntity
    {
        public const string TableName = "answers";

        public Answer()
        {

        }

        public Answer(string userId, string rowkey)
        {
            PartitionKey = userId;
            RowKey = rowkey;
        }

        public string ScenarioId { get; set; }
        public string PuzzleId { get; set; }
        public string ScenarioDetails { get; set; }

        public bool IsLastAnswer { get; set; }
    }


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

        public string TeamId { get; set; }

        public bool IsCaptain { get; set; }

        public string ChannelData { get; set; }
    }
    
}