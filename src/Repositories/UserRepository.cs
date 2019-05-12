using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.Azure.Cosmos.Table;

namespace CoreBot.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ICloudStorage _storage;

        public UserRepository(ICloudStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public Task DeleteUsers()
        {
            _storage.DeleteTableIfExists(User.TableName);
            return Task.CompletedTask;
        }

        public Task<ICollection<User>> GetTeamUsersAsync(string teamId)
        {
            var table = _storage.GetOrCreateTable(User.TableName);

            TableQuery<User> query =
               new TableQuery<User>()
                  .Where(TableQuery.GenerateFilterCondition(nameof(User.TeamId),
                      QueryComparisons.Equal, teamId));
            ICollection<User> result = table.ExecuteQuery(query).ToList();
            return Task.FromResult(result);
        }

        public async Task InsertOrMergeAsync(User user)
        {
            var table = _storage.GetOrCreateTable(User.TableName);
            await _storage.InsertOrMergeEntityAsync(table, user);
        }

        public async Task<User> TryGetUserByIdAsync(UserId userId)
        {
            var table = _storage.GetOrCreateTable(User.TableName);
            return await _storage.RetrieveEntityByAsync<User>(table, userId.ChannelId, userId.Id);
        }
    }
}
