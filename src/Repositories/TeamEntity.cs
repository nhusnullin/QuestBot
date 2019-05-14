using CoreBot.Domain;
using Microsoft.Azure.Cosmos.Table;

namespace CoreBot.Repositories
{
    public class TeamEntity : TableEntity
    {
        public TeamEntity()
        {

        }
        public TeamEntity(string partitionKey, string teamId, string name)
        {
            PartitionKey = partitionKey;
            RowKey = teamId;
            Name = name;
        }
        public string Name { get; set; }
        public int PinCode { get; set; }
    }
}
