using CoreBot.Domain;
using Microsoft.Azure.Cosmos.Table;

namespace CoreBot.Repositories
{
    public class TeamEntity : TableEntity
    {
        public TeamEntity()
        {

        }
        public TeamEntity(string partitionKey, string teamId)
        {
            PartitionKey = partitionKey;
            RowKey = teamId;
        }
        public bool IsSingleUser { get; set; }
    }
}
