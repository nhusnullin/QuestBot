using System.Collections.Generic;

namespace CoreBot
{
    public class DummyUserService : IUserService
    {
        public Dictionary<string, User> _store = new Dictionary<string, User>();
        public User GetBy(string channelId, string userId)
        {
            if (_store.ContainsKey(userId))
            {
                return _store[userId];
            }
            return null;
        }

        public void InsertOrMerge(User user)
        {
            _store[user.TeamId] = user;
        }

        public void Remove(string channelId, string userId)
        {
        }

        public void SetAnswer(string userOrTeamId, string scenarioId, string puzzleId, string actualAnswer)
        {
        }
    }
}