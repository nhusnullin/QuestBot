using System.Threading.Tasks;

namespace CoreBot
{
    public interface IUserService
    {
        Task SetAnswer(string channelId, string userId, string scenarioId, string puzzleId, ScenarioDetails scenarioDetails);

        Task<User> GetByAsync(string channelId, string userId);
        Task InsertOrMergeAsync(User user);
        void Remove(string channelId, string userId);
    }
}