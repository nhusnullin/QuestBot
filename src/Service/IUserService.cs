using System.Threading.Tasks;

namespace CoreBot.Service
{
    public interface IUserService
    {
        Task SetAnswer(string channelId, string teamId, string scenarioId, string puzzleId, ScenarioDetails scenarioDetails);
        ScenarioDetails GetLastScenarioDetailsExceptGameOver(string channelId, string teamId);
        Task<User> GetByAsync(string channelId, string userId);
        Task InsertOrMergeAsync(User user);
        Task DeleteUsers();
        bool IsScenarioIsOverByUser(string teamId, string scenarioId);
        
    }
}