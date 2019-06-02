using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;
using CoreBot.Domain;

namespace CoreBot.Repository.Impl.InMemory
{
    public class UserRepository : IUserRepository
    {
        private readonly IStorage _storage;

        public UserRepository(IStorage storage)
        {
            _storage = storage;
        }

        public async Task<User> GetUserByIdAsync(UserId userId)
        {
            var key = User.GetStorageKey(userId);
            var resDic = await _storage.ReadAsync<User>(new[] {key});
            return resDic.Values.FirstOrDefault();
        }

        public async Task InsertOrUpdateAsync(User user)
        {
            var key = user.GetStorageKey();
            await _storage.WriteAsync(new Dictionary<string, object> {{key, user}});
        }

        public Task DeleteUsers()
        {
            throw new NotSupportedException();
        }

        public Task<ICollection<User>> GetTeamUsersAsync(string teamId)
        {
            throw new NotSupportedException();
        }

        public Task<ICollection<User>> GetAllUsersAsync()
        {
            throw new NotSupportedException();
        }
    }
}