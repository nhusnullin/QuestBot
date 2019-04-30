using CoreBot.Service;
using System.Threading.Tasks;

namespace CoreBot
{
    public class UserService : IUserService
    {
        private readonly ICloudStorage _storage;

        public UserService(ICloudStorage storage)
        {
            _storage = storage;
        }

        public async Task<User> GetByAsync(string channelId, string userId)
        {
            var table = _storage.GetOrCreateTable(User.TableName);
            return await _storage.RetrieveEntityByAsync<User>(table, channelId, userId);
        }

        public async Task InsertOrMergeAsync(User user)
        {
            var table = _storage.GetOrCreateTable(User.TableName);
            await _storage.InsertOrMergeEntityAsync(table, user);
        }

        public void Remove(string channelId, string userId)
        {
        }

        public void SetAnswer(string userOrTeamId, string scenarioId, string puzzleId, string actualAnswer)
        {
        }
    }
}