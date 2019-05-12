using Microsoft.Azure.Cosmos.Table;

namespace CoreBot.Domain
{
    public class Answer : TableEntity
    {
        public const string TableName = "answers";

        public Answer()
        {

        }

        public Answer(string teamId, string rowkey)
        {
            PartitionKey = teamId;
            RowKey = rowkey;
        }

        public string ScenarioId { get; set; }
        public string PuzzleId { get; set; }
        public string ScenarioDetails { get; set; }

        public bool IsLastAnswer { get; set; }
    }
    
}