﻿using CoreBot.Domain;
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
        Task<ICollection<T>> RetrieveEntitiesAsync<T>(CloudTable table) where T : ITableEntity, new();
        CloudTable GetOrCreateTable(string tableName);
        void DeleteTableIfExists(string tableName);
        Task<T> InsertOrMergeEntityAsync<T>(CloudTable table, T entity) where T : ITableEntity;
        Task<T> InsertAsync<T>(CloudTable table, T entity) where T : ITableEntity;

        IList<Answer> GetAnswersByTeamId(string teamId, Func<Answer, bool> whereClause);
        IList<Answer> GetAllAnswers();
    }
    
}
