using CoreBot.Domain;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Service
{
    public class CloudStorage: ICloudStorage
    {
        private readonly ILogger<CloudStorage> _logger;
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly AsyncLocal<CloudTableClient> tableClientHolder = new AsyncLocal<CloudTableClient>();
        public CloudStorage(ILogger<CloudStorage> logger, CloudStorageAccount cloudStorageAccount)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cloudStorageAccount = cloudStorageAccount ?? throw new ArgumentNullException(nameof(cloudStorageAccount));
            tableClientHolder.Value = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
        }

        private CloudTableClient TableClient
        {
            get
            {
                return tableClientHolder.Value;
            }
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
            CloudTable table = TableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
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

        public IList<Answer> GetAnswersByTeamId(string teamId, Func<Answer, bool> whereClause) 
        {
            CloudTable table = TableClient.GetTableReference(Answer.EntityName);

            return table.CreateQuery<Answer>().Where(x => x.PartitionKey == teamId)
                .Where(whereClause)
                .ToList();
        }

        public IList<Answer> GetAllAnswers()
        {
            CloudTable table = TableClient.GetTableReference(Answer.TableName);

            return table.CreateQuery<Answer>()
                .ToList();
        }

        public void DeleteTableIfExists(string tableName)
        {
            CloudTable table = TableClient.GetTableReference(tableName);
            table.DeleteIfExists();
        }

        public Task<ICollection<T>> RetrieveEntitiesAsync<T>(CloudTable table) where T : ITableEntity, new()
        {
            TableContinuationToken token = null;
            var entities = new List<T>();
            do
            {
                var queryResult = table.ExecuteQuerySegmented(new TableQuery<T>(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);
            return Task.FromResult<ICollection<T>>(entities);
        }
    }
}
