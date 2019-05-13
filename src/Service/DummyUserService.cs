using System.Collections.Generic;
using System.Threading.Tasks;
using CoreBot.Service;

namespace CoreBot
{
    public class DummyUserService : IUserService
    {
        public Dictionary<string, User> _store = new Dictionary<string, User>();
        public ScenarioDetails GetLastScenarioDetailsExceptGameOver(string channelId, string teamId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> GetByAsync(string channelId, string userId)
        {
            if (_store.ContainsKey(userId))
            {
                return await Task.FromResult(_store[userId]);
            }
            return null;
        }

        public async Task InsertOrMergeAsync(User user)
        {
            _store[user.TeamId] = user;
        }

        public void Remove(string channelId, string userId)
        {
        }

        public bool IsScenarioIsOverByUser(string teamId, string scenarioId)
        {
            throw new System.NotImplementedException();
        }

        public async Task SetAnswer(string channelId, string teamId, string scenarioId, string puzzleId, ScenarioDetails scenarioDetails)
        {
            return;
        }

        public Task DeleteUsers()
        {
            throw new System.NotImplementedException();
        }
    }
}