using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Service
{
    public interface ICloudStorage
    {
        Task<T> RetrieveEntityByAsync<T>(CloudTable table, string partitionKey, string rowKey) where T : ITableEntity;
        CloudTable GetOrCreateTable(string tableName);
        Task<T> InsertOrMergeEntityAsync<T>(CloudTable table, T entity) where T : ITableEntity;
        Task<T> InsertAsync<T>(CloudTable table, T entity) where T : ITableEntity;

        IList<Answer> GetAnswersByUserId(string userId, Func<Answer, bool> whereClause);
    }
    
}
