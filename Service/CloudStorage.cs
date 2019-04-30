using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoreBot.Service
{
    public class CloudStorage: ICloudStorage
    {
        private readonly ILogger<CloudStorage> _logger;
        private readonly string _connectionString;

        public CloudStorage(ILogger<CloudStorage> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config["StorageConnectionString"];
        }

        public async Task<T> InsertOrMergeEntityAsync<T>(CloudTable table, T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                return (T)result.Result;
            }
            catch (StorageException e)
            {
                throw;
            }
        }

        public  CloudTable GetOrCreateTable(string tableName)
        {
            CloudStorageAccount storageAccount = Common.CreateStorageAccountFromConnectionString(_connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);
            return table;
        }

        public async Task<T> RetrieveEntityByAsync<T>(CloudTable table, string partitionKey, string rowKey) where T:ITableEntity
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                return (T)result.Result;
            }
            catch (StorageException e)
            {
                throw;
            }
        }
    }
}
