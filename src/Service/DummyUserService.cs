using System.Collections.Generic;
using System.Threading.Tasks;
using CoreBot.Service;
using CoreBot.Domain;

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

        public Task DeleteUsers()
        {
            throw new System.NotImplementedException();
        }

        public Task SetAnswer(ScenarioDetails scenarioDetails)
        {
            throw new System.NotImplementedException();
        }

        public ScenarioDetails GetLastScenarioDetailsExceptGameOver(string teamId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ICollection<Answer>> GetAnswers()
        {
            throw new System.NotImplementedException();
        }

        public bool IsScenarioIsOverByTeam(string userId, string scenarioId)
        {
            throw new System.NotImplementedException();
        }
    }
}