using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Service
{
    public class CloudStorage: ICloudStorage
    {
        private readonly ILogger<CloudStorage> _logger;
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly string _connectionString;

        public CloudStorage(ILogger<CloudStorage> logger, CloudStorageAccount cloudStorageAccount)
        {
            _logger = logger;
            _cloudStorageAccount = cloudStorageAccount;
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

        public async Task<T> InsertAsync<T>(CloudTable table, T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                TableOperation insert = TableOperation.Insert(entity);
                TableResult result = await table.ExecuteAsync(insert);
                return (T)result.Result;
            }
            catch (StorageException e)
            {
                throw;
            }
        }

        public  CloudTable GetOrCreateTable(string tableName)
        {
            CloudTableClient tableClient = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
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

        public IList<Answer> GetAnswersByUserId(string userId, Func<Answer, bool> whereClause) 
        {
            CloudTableClient tableClient = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(Answer.TableName);

            TableQuery<Answer> query = new TableQuery<Answer>();

            return table.CreateQuery<Answer>().Where(x => x.PartitionKey == userId)
                //.OrderByDescending(x => x.Timestamp)
                .Where(whereClause)
                .ToList();
        }
    }
}
