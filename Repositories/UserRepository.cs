using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Service;

namespace CoreBot.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ICloudStorage _storage;

        public UserRepository(ICloudStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
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
